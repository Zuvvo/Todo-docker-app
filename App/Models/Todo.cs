using TodoApp.DTO;

namespace TodoApp.Models
{
    public class Todo
    {
        public int Id { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public float Progress { get; set; } = 0;

        public Todo() { }

        public Todo(AddTodoDTO addTodoDTO)
        {
            ExpiresAt = DateTime.Now;
            Title = addTodoDTO.Title ?? string.Empty;
            Description = addTodoDTO.Description ?? string.Empty;
            Progress = 0;
        }
    }
}
