namespace Task6.Models
{
    public class ViewMessage
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Sender { get; set; }
        public string Created { get; set; }
        public bool IsRead { get; set; }
    }
}
