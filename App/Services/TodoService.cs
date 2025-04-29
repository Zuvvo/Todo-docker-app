using TodoApp.Data;
using TodoApp.DTO;
using TodoApp.Models;

namespace TodoApp.Services
{
    public class TodoService
    {
        private readonly AppDbContext _context;

        public TodoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TodoDTO> AddTodo(AddTodoDTO addTodoDTO)
        {
            var todo = new Todo(addTodoDTO);
            _context.Add(todo);
            await _context.SaveChangesAsync();

            return new TodoDTO(todo);
        }

    }
}
