namespace Task6.Models
{
    public class ViewMessage
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Sender { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public bool IsRead { get; set; }
    }
}
