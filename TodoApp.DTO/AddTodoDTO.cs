namespace TodoApp.DTO
{
    public class AddTodoDTO
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
