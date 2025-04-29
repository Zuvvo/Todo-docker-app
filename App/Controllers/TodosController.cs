using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.DTO;
using TodoApp.Enums;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TodoService _todoService;

        public TodosController(AppDbContext context, TodoService todoService)
        {
            _context = context;
            _todoService = todoService;
        }

        // GET: api/Todos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Todo>>> GetTodos()
        {
            return await _context.Todos.ToListAsync();
        }

        // GET: api/Todos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoDTO>> GetTodo(int id)
        {
            var todo = await _todoService.GetTodoWithId(id);
            if (todo == null)
            {
                return NotFound(); // Return 404 if the Todo is not found
            }

            return Ok(todo); // Return 200 with the TodoDTO
        }

        // GET: api/Todos/incoming/{range}
        [HttpGet("incoming/{range}")]
        public async Task<ActionResult<List<TodoDTO>>> GetIncomingTodos([FromRoute] TodoRange range)
        {
            var todos = await _todoService.GetIncomingTodos(range);
            var validValues = Enum.GetNames(typeof(TodoRange));

            // Return the response with todos and valid values
            return Ok(new
            {
                Message = "Here are the incoming todos based on the specified range.",
                ValidRanges = validValues,
                SelectedRange = range.ToString(),
                Todos = todos
            });
        }

        // PUT: api/Todos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodo(int id, UpdateTodoDTO updateTodoDTO)
        {
            var updatedTodo = await _todoService.UpdateTodo(id, updateTodoDTO);
            if (updatedTodo == null)
            {
                return NotFound();
            }

            return Ok(updatedTodo);
        }

        [HttpPost]
        public async Task<ActionResult<Todo>> PostTodo(AddTodoDTO todo)
        {
            TodoDTO result = await _todoService.AddTodo(todo);
            return Ok(result);
        }

        // DELETE: api/Todos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            var isDeleted = await _todoService.DeleteTodoAsync(id);
            if (!isDeleted)
            {
                return NotFound(); // Return 404 if the Todo is not found
            }

            return NoContent(); // Return 204 if the deletion is successful
        }

        [HttpPatch("{id}/done")]
        public async Task<IActionResult> MarkTodoAsDone(int id)
        {
            var updatedTodo = await _todoService.MarkTodoAsDone(id);
            if (updatedTodo == null)
            {
                return NotFound(); // Return 404 if the Todo is not found
            }

            return Ok(updatedTodo); // Return 200 with the updated TodoDTO
        }
    }
}
