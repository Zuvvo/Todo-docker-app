using TodoApp.Models;

namespace TodoApp.DTO
{
    public class TodoDTO
    {
        public int Id { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public float Progress { get; set; } = 0;

        public TodoDTO() { }

        public TodoDTO(Todo todo)
        {
            Id = todo.Id;
            ExpiresAt = todo.ExpiresAt;
            Title = todo.Title;
            Description = todo.Description;
            Progress = todo.Progress;
        }
    }
}
