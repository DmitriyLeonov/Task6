namespace Task6.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Sender { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public string Reciever { get; set; }
    }
}
