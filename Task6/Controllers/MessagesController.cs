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
using Task6.Data;
using Task6.Hubs;
using Task6.Models;

namespace Task6.Controllers
{
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<MessageHub> _hubContext;

        public MessagesController(ApplicationDbContext context, IHubContext<MessageHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            List<ViewMessage> messages = new List<ViewMessage>();
            var res = _context.Message.ToList();
            foreach (var message in res)
            {
                messages.Add(new ViewMessage()
                {
                    Body = message.Body,
                    Created = message.Created,
                    IsRead = message.IsRead,
                    Sender = message.Sender,
                    Title = message.Title
                });
            }
            return View(messages);
        }

        [HttpGet]
        public IActionResult GetMessages()
        {
            List<ViewMessage> messages = new List<ViewMessage>();
            var res = _context.Message.ToList();
            foreach (var message in res)
            {
                messages.Add(new ViewMessage()
                {
                    Body = message.Body,
                    Created = message.Created,
                    IsRead = message.IsRead,
                    Sender = message.Sender,
                    Title = message.Title
                });
            }
            return Ok(messages);
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

            message.IsRead = true;
            return View(message);
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
            message.IsRead = false;
            if (ModelState.IsValid)
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
