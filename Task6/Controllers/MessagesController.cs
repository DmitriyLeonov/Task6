using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using ASP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using Task6.Data;
using Task6.Hubs;
using Task6.Models;

namespace Task6.Controllers
{
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<MessageHub> _hubContext;
        private readonly IToastNotification _toastNotification;
        private DateTime lastMessageNotification = DateTime.MinValue;

        public MessagesController(ApplicationDbContext context,
            IHubContext<MessageHub> hubContext,
            IToastNotification toastNotification)
        {
            _context = context;
            _hubContext = hubContext;
            _toastNotification = toastNotification;
        }

        public async Task<IActionResult> Index()
        {
            var user = HttpContext.User.Identity.Name;
            List<ViewMessage> messages = new List<ViewMessage>();
            var res = _context.Message.ToList();
            foreach (var message in res)
            {
                if (message.Reciever.Equals(user))
                {
                    
                    messages.Add(new ViewMessage()
                    {
                        Id = message.Id,
                        Body = message.Body,
                        Created = message.Created.ToString("g"),
                        IsRead = message.IsRead,
                        Sender = message.Sender,
                        Title = message.Title
                    });
                }
            }
            var result = messages.OrderByDescending(m => m.Id);
            return View(result);
        }

        [HttpGet]
        public IActionResult GetMessages()
        {
            var user = HttpContext.User.Identity.Name;
            List<ViewMessage> messages = new List<ViewMessage>();
            var res = _context.Message.ToList();
            foreach (var message in res)
            {
                if (message.Reciever.Equals(user))
                {
                    if (!message.IsRead)
                    {
                        _toastNotification.AddInfoToastMessage($"{message.Title}<br />{message.Body}", new ToastrOptions()
                        {
                            Title = message.Sender
                        });
                        if (message.Created > lastMessageNotification)
                        {
                            lastMessageNotification = message.Created;
                        }
                    }
                    messages.Add(new ViewMessage()
                    {
                        Id = message.Id,
                        Body = message.Body,
                        Created = message.Created.ToString("g"),
                        IsRead = message.IsRead,
                        Sender = message.Sender,
                        Title = message.Title
                    });
                }
            }

            var result = messages.OrderByDescending(m => m.Id);
            return Ok(result);
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Message == null)
            {
                return NotFound();
            }

            var message = await _context.Message
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }
            ViewMessage viewMessage = new ViewMessage()
            {
                Id = message.Id,
                Body = message.Body,
                Created = message.Created.ToString("g"),
                IsRead = message.IsRead,
                Sender = message.Sender,
                Title = message.Title
            };
            message.IsRead = true;
            await _context.SaveChangesAsync();
            return View(viewMessage);
        }

        // GET: Messages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Body,Sender,Created,Reciever,IsRead")] Message message)
        {
            var user = HttpContext.User.Identity.Name;
            message.Sender = user;
            message.IsRead = false;
            if (!ModelState.IsValid)
            {
                _context.Add(message);
                await _context.SaveChangesAsync();
                await _hubContext.Clients.All.SendAsync("LoadMessages");
                return RedirectToAction(nameof(Index));
            }
            return View(message);
        }

        [HttpPost]
        public JsonResult GetUsers(string prefix)
        {
            var users = (from u in _context.Users
                where u.UserName.StartsWith(prefix)
                select new { u.UserName });
            return Json(users);
        }

        private bool MessageExists(int id)
        {
          return (_context.Message?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
