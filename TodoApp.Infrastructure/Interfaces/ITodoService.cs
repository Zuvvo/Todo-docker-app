using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.DTO;
using TodoApp.Infrastructure.Enums;

namespace TodoApp.Infrastructure.Interfaces
{
    public interface ITodoService
    {
        Task<List<TodoDTO>> GetAllTodos();
        Task<TodoDTO> AddTodo(AddTodoDTO addTodoDTO);
        Task<TodoDTO?> GetTodoWithId(int id);
        Task<List<TodoDTO>> GetIncomingTodos(TodoRange range);
        Task<TodoDTO?> UpdateTodo(int id, UpdateTodoDTO updateTodoDTO);
        Task<bool> DeleteTodoAsync(int id);
        Task<TodoDTO?> MarkTodoAsDone(int id);
    }
}
