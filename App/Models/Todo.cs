namespace TodoApp.Models
{
    public class Todo
    {
        public int Id { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Description { get; set; } = string.Empty;
        public float Progress { get; set; } = 0;
    }
}
