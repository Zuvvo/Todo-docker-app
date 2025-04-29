namespace TodoApp.DTO
{
    public class UpdateTodoDTO
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public float? Progress { get; set; }
    }
}
