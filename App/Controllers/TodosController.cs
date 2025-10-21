using Microsoft.AspNetCore.Mvc;
using TodoApp.DTO;
using TodoApp.Infrastructure.Enums;
using TodoApp.Infrastructure.Interfaces;

namespace TodoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodosController : ControllerBase
    {
        private readonly ITodoService _todoService;

        public TodosController(ITodoService todoService)
        {
            _todoService = todoService;
        }

        // GET: api/Todos
        [HttpGet]
        public async Task<ActionResult<List<TodoDTO>>> GetTodos()
        {
            var todos = await _todoService.GetAllTodos();

            if (todos.Count == 0)
            {
                return NotFound("No todos found.");
            }

            return Ok(todos); // Return 200 with the list of TodoDTOs
        }

        // GET: api/Todos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoDTO>> GetTodo(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID. ID must be greater than 0.");
            }

            var todo = await _todoService.GetTodoWithId(id);
            if (todo == null)
            {
                return NotFound($"Todo with ID {id} not found."); // Return 404 if the Todo is not found
            }

            return Ok(todo); // Return 200 with the TodoDTO
        }

        // GET: api/Todos/incoming/{range}
        [HttpGet("incoming/{range}")]
        public async Task<ActionResult<List<TodoDTO>>> GetIncomingTodos([FromRoute] TodoRange range)
        {
            var validRanges = Enum.GetNames(typeof(TodoRange));

            if (!Enum.IsDefined(typeof(TodoRange), range))
            {
                var validValues = string.Join(", ", validRanges);
                return BadRequest($"Invalid range. Valid values are: {validValues}");
            }

            var todos = await _todoService.GetIncomingTodos(range);

            if (todos.Count == 0)
            {
                return NotFound("No incoming todos found for the specified range.");
            }

            // Return the response with todos and valid values
            return Ok(new
            {
                Message = "Here are the incoming todos based on the specified range.",
                ValidRanges = validRanges,
                SelectedRange = range.ToString(),
                Todos = todos
            });
        }

        // PUT: api/Todos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodo(int id, UpdateTodoDTO updateTodoDTO)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID. ID must be greater than 0.");
            }

            if (updateTodoDTO == null)
            {
                return BadRequest("Update data cannot be null.");
            }


            var updatedTodo = await _todoService.UpdateTodo(id, updateTodoDTO);
            if (updatedTodo == null)
            {
                return NotFound($"Todo with ID {id} not found.");
            }

            return Ok(updatedTodo);
        }

        [HttpPost]
        public async Task<ActionResult<TodoDTO>> PostTodo(AddTodoDTO todo)
        {
            if (todo == null)
            {
                return BadRequest("Todo data cannot be null.");
            }

            if (string.IsNullOrEmpty(todo.Title))
            {
                return BadRequest("Title is required.");
            }

            if (todo.ExpiresAt <= DateTime.Now)
            {
                return BadRequest("Expiration date must be in the future.");
            }

            TodoDTO result = await _todoService.AddTodo(todo);
            return Ok(result);
        }

        // DELETE: api/Todos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID. ID must be greater than 0.");
            }

            var isDeleted = await _todoService.DeleteTodoAsync(id);
            if (!isDeleted)
            {
                return NotFound($"Todo with ID {id} not found."); // Return 404 if the Todo is not found
            }

            return NoContent(); // Return 204 if the deletion is successful
        }

        [HttpPatch("{id}/done")]
        public async Task<IActionResult> MarkTodoAsDone(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID. ID must be greater than 0.");
            }

            var updatedTodo = await _todoService.MarkTodoAsDone(id);
            if (updatedTodo == null)
            {
                return NotFound($"Todo with ID {id} not found."); // Return 404 if the Todo is not found
            }

            return Ok(updatedTodo); // Return 200 with the updated TodoDTO
        }
    }
}
