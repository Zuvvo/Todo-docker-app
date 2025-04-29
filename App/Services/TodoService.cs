using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.DTO;
using TodoApp.Enums;
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

        public async Task<TodoDTO?> GetTodoWithId(int id)
        {
            var todo = await _context.Todos.FindAsync(id);
            return todo == null ? null : new TodoDTO(todo);
        }

        public async Task<List<TodoDTO>> GetIncomingTodos(TodoRange range)
        {
            DateTime startDate = DateTime.Today; // today at 0:00:00
            DateTime endDate;

            switch (range)
            {
                case TodoRange.Today:
                    endDate = startDate.Date.AddDays(1).AddTicks(-1); // End of today
                    break;
                case TodoRange.NextDay:
                    startDate = startDate.Date.AddDays(1); // Start of next day
                    endDate = startDate.AddDays(1).AddTicks(-1); // End of next day
                    break;
                case TodoRange.CurrentWeek: // From today 0:00:00 to end of the current week
                    int daysUntilEndOfWeek = DayOfWeek.Saturday - startDate.DayOfWeek;
                    endDate = startDate.Date.AddDays(daysUntilEndOfWeek + 2).AddTicks(-1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(range), "Invalid range specified.");
            }

            var todos = await _context.Todos
                .Where(todo => todo.ExpiresAt >= startDate && todo.ExpiresAt <= endDate)
                .ToListAsync();

            return todos.Select(todo => new TodoDTO(todo)).ToList();
        }

        public async Task<TodoDTO?> UpdateTodo(int id, UpdateTodoDTO updateTodoDTO)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null)
            {
                return null;
            }

            // Update fields only if they are provided in the DTO
            if (!string.IsNullOrEmpty(updateTodoDTO.Title))
                todo.Title = updateTodoDTO.Title;

            if (!string.IsNullOrEmpty(updateTodoDTO.Description))
                todo.Description = updateTodoDTO.Description;

            if (updateTodoDTO.ExpiresAt.HasValue)
                todo.ExpiresAt = updateTodoDTO.ExpiresAt.Value;

            if (updateTodoDTO.Progress.HasValue)
                todo.Progress = updateTodoDTO.Progress.Value;

            await _context.SaveChangesAsync();

            return new TodoDTO(todo);
        }

        public async Task<bool> DeleteTodoAsync(int id)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null)
            {
                return false; // Return false if the Todo is not found
            }

            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();

            return true; // Return true if the deletion is successful
        }

        public async Task<TodoDTO?> MarkTodoAsDone(int id)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null)
            {
                return null; // Return null if the Todo is not found
            }

            todo.Progress = 1; // Mark the Todo as done (100% progress)
            await _context.SaveChangesAsync();

            return new TodoDTO(todo); // Return the updated Todo as a DTO
        }
    }
}
