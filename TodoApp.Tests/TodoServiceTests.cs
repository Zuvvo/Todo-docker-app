using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.Data;
using TodoApp.DTO;
using TodoApp.Enums;
using TodoApp.Models;
using TodoApp.Services;

//tests written with help of Copilot but carefully reviewed, checked and improved for correctness and reliability

namespace TodoApp.Tests
{
    public class TodoServiceTests : TestBase
    {
        #region AddTodo

        [Fact]
        public async Task AddTodo_ShouldAddTodoSuccessfully()
        {
            // Arrange
            var todoService = ServiceProvider.GetRequiredService<TodoService>();
            var newTodo = new AddTodoDTO
            {
                Title = "Buy Groceries",
                Description = "Purchase milk, eggs, and bread from the store.",
                ExpiresAt = DateTime.UtcNow.AddDays(1),
            };

            // Act
            await todoService.AddTodo(newTodo);

            // Assert
            using var scope = ServiceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var todoInDb = await dbContext.Todos.FirstOrDefaultAsync(t => t.Title == "Buy Groceries");

            Assert.NotNull(todoInDb);
            Assert.Equal("Buy Groceries", todoInDb.Title);
            Assert.Equal("Purchase milk, eggs, and bread from the store.", todoInDb.Description);
            Assert.Equal(0, todoInDb.Progress);
            Assert.True(todoInDb.ExpiresAt > DateTime.UtcNow);
        }

        [Fact]
        public async Task AddTodo_ShouldFailWhenTitleIsEmpty()
        {
            // Arrange
            var todoService = ServiceProvider.GetRequiredService<TodoService>();
            var newTodo = new AddTodoDTO
            {
                Title = string.Empty,
                Description = string.Empty,
                ExpiresAt = DateTime.UtcNow.AddDays(1),
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await todoService.AddTodo(newTodo);
            });
        }

        [Fact]
        public async Task AddTodo_ShouldFailWhenExpiresAtIsInThePast()
        {
            // Arrange
            var todoService = ServiceProvider.GetRequiredService<TodoService>();
            var newTodo = new AddTodoDTO
            {
                Title = "Clean house",
                Description = "Clean the living room and kitchen.",
                ExpiresAt = DateTime.UtcNow.AddDays(-5),
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await todoService.AddTodo(newTodo);
            });
        }

        #endregion

        #region GetAllTodos

        [Fact]
        public async Task GetAllTodos_ShouldReturnAllTodos()
        {
            // Arrange
            var todoService = ServiceProvider.GetRequiredService<TodoService>();
            using (var scope = ServiceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Todos.RemoveRange(dbContext.Todos);
                dbContext.Todos.Add(new Todo { Title = "Plan Vacation", Description = "Research destinations and book flights.", ExpiresAt = DateTime.UtcNow.AddDays(10) });
                dbContext.Todos.Add(new Todo { Title = "Prepare Presentation", Description = "Create slides for the quarterly meeting.", ExpiresAt = DateTime.UtcNow.AddDays(5) });
                await dbContext.SaveChangesAsync();
            }

            // Act
            var todos = await todoService.GetAllTodos();

            // Assert
            Assert.NotNull(todos);
            Assert.Equal(2, todos.Count);
        }

        [Fact]
        public async Task GetAllTodos_ShouldReturnTodosWithCorrectProperties()
        {
            // Arrange
            using (var scope = ServiceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Clear any existing data
                dbContext.Todos.RemoveRange(dbContext.Todos);
                await dbContext.SaveChangesAsync();

                // Add test data
                dbContext.Todos.Add(new Todo
                {
                    Title = "Complete Homework",
                    Description = "Finish math and science assignments.",
                    ExpiresAt = DateTime.UtcNow.AddDays(2)
                });
                dbContext.Todos.Add(new Todo
                {
                    Title = "Grocery Shopping",
                    Description = "Buy vegetables, fruits, and snacks.",
                    ExpiresAt = DateTime.UtcNow.AddDays(1)
                });
                await dbContext.SaveChangesAsync();
            }

            // Act
            using (var scope = ServiceProvider.CreateScope())
            {
                var todoService = scope.ServiceProvider.GetRequiredService<TodoService>();
                var todos = await todoService.GetAllTodos();

                // Assert
                Assert.NotNull(todos);
                Assert.Equal(2, todos.Count);

                // Verify the first todo
                var firstTodo = todos.FirstOrDefault(t => t.Title == "Complete Homework");
                Assert.NotNull(firstTodo);
                Assert.Equal("Complete Homework", firstTodo.Title);
                Assert.Equal("Finish math and science assignments.", firstTodo.Description);

                // Verify the second todo
                var secondTodo = todos.FirstOrDefault(t => t.Title == "Grocery Shopping");
                Assert.NotNull(secondTodo);
                Assert.Equal("Grocery Shopping", secondTodo.Title);
                Assert.Equal("Buy vegetables, fruits, and snacks.", secondTodo.Description);
            }
        }


        [Fact]
        public async Task GetAllTodos_ShouldReturnTodosAsDTOs()
        {
            // Arrange
            var todoService = ServiceProvider.GetRequiredService<TodoService>();
            using (var scope = ServiceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                // Clear any existing data
                dbContext.Todos.RemoveRange(dbContext.Todos);
                dbContext.Todos.Add(new Todo { Title = "Organize Files", Description = "Sort and archive old documents.", ExpiresAt = DateTime.UtcNow.AddDays(3) });
                await dbContext.SaveChangesAsync();
            }

            // Act
            var todos = await todoService.GetAllTodos();

            // Assert
            Assert.NotNull(todos);
            Assert.Single(todos);
            Assert.IsType<TodoDTO>(todos.First());
        }

        #endregion

        #region GetTodoWithId
        [Fact]
        public async Task GetTodoWithId_ShouldReturnTodoWhenExists()
        {
            // Arrange
            var todoService = ServiceProvider.GetRequiredService<TodoService>();
            int todoId;
            using (var scope = ServiceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var todo = new Todo { Title = "Renew Passport", Description = "Submit application for passport renewal.", ExpiresAt = DateTime.UtcNow.AddDays(15) };
                dbContext.Todos.Add(todo);
                await dbContext.SaveChangesAsync();
                todoId = todo.Id;
            }

            // Act
            var todoDTO = await todoService.GetTodoWithId(todoId);

            // Assert
            Assert.NotNull(todoDTO);
            Assert.Equal("Renew Passport", todoDTO.Title);
        }

        [Fact]
        public async Task GetTodoWithId_ShouldReturnNullWhenTodoDoesNotExist()
        {
            // Arrange
            var todoService = ServiceProvider.GetRequiredService<TodoService>();

            // Act
            var todoDTO = await todoService.GetTodoWithId(999);

            // Assert
            Assert.Null(todoDTO);
        }

        [Fact]
        public async Task GetTodoWithId_ShouldReturnTodoAsDTO()
        {
            // Arrange
            var todoService = ServiceProvider.GetRequiredService<TodoService>();
            int todoId;
            using (var scope = ServiceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var todo = new Todo { Title = "Plan Team Outing", Description = "Coordinate with team members for a weekend outing.", ExpiresAt = DateTime.UtcNow.AddDays(7) };
                dbContext.Todos.Add(todo);
                await dbContext.SaveChangesAsync();
                todoId = todo.Id;
            }

            // Act
            var todoDTO = await todoService.GetTodoWithId(todoId);

            // Assert
            Assert.NotNull(todoDTO);
            Assert.IsType<TodoDTO>(todoDTO);
        }

        #endregion

        #region GetIncomingTodos

        [Fact]
        public async Task GetIncomingTodos_ShouldReturnTodosForToday()
        {
            // Arrange
            var todoService = ServiceProvider.GetRequiredService<TodoService>();
            using (var scope = ServiceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Todos.Add(new Todo { Title = "Morning Jog", ExpiresAt = DateTime.Today.AddHours(6) });
                dbContext.Todos.Add(new Todo { Title = "Evening Yoga", ExpiresAt = DateTime.Today.AddDays(1) });
                await dbContext.SaveChangesAsync();
            }

            // Act
            var todos = await todoService.GetIncomingTodos(TodoRange.Today);

            // Assert
            Assert.NotNull(todos);
            Assert.Single(todos);
            Assert.Equal("Morning Jog", todos.First().Title);
        }

        [Fact]
        public async Task GetIncomingTodos_ShouldReturnEmptyListWhenNoTodosInRange()
        {
            // Arrange
            var todoService = ServiceProvider.GetRequiredService<TodoService>();

            // Act
            var todos = await todoService.GetIncomingTodos(TodoRange.Today);

            // Assert
            Assert.NotNull(todos);
            Assert.Empty(todos);
        }

        [Fact]
        public async Task GetIncomingTodos_ShouldThrowExceptionForInvalidRange()
        {
            // Arrange
            var todoService = ServiceProvider.GetRequiredService<TodoService>();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            {
                await todoService.GetIncomingTodos((TodoRange)999);
            });
        }

        #endregion

        #region UpdateTodo

        [Fact]
        public async Task UpdateTodo_ShouldUpdateFieldsWhenValid()
        {
            // Arrange
            var todoService = ServiceProvider.GetRequiredService<TodoService>();
            int todoId;
            using (var scope = ServiceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var todo = new Todo { Title = "Fix Leaky Faucet", Description = "Repair the faucet in the kitchen sink.", ExpiresAt = DateTime.UtcNow.AddDays(2) };
                dbContext.Todos.Add(todo);
                await dbContext.SaveChangesAsync();
                todoId = todo.Id;
            }

            var updateDTO = new UpdateTodoDTO { Title = "Fix Faucet", Progress = 0.5f };

            // Act
            var updatedTodo = await todoService.UpdateTodo(todoId, updateDTO);

            // Assert
            Assert.NotNull(updatedTodo);
            Assert.Equal("Fix Faucet", updatedTodo.Title);
            Assert.Equal(0.5f, updatedTodo.Progress);
        }

        [Fact]
        public async Task UpdateTodo_ShouldReturnNullWhenTodoDoesNotExist()
        {
            // Arrange
            var todoService = ServiceProvider.GetRequiredService<TodoService>();
            var updateDTO = new UpdateTodoDTO { Title = "Fix Faucet" };

            // Act
            var updatedTodo = await todoService.UpdateTodo(999, updateDTO);

            // Assert
            Assert.Null(updatedTodo);
        }

        [Fact]
        public async Task UpdateTodo_ShouldNotUpdateFieldsWhenDTOIsEmpty()
        {
            // Arrange
            var todoService = ServiceProvider.GetRequiredService<TodoService>();
            int todoId;
            using (var scope = ServiceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var todo = new Todo { Title = "Organize Closet", Description = "Sort clothes and donate unused items.", ExpiresAt = DateTime.UtcNow.AddDays(3) };
                dbContext.Todos.Add(todo);
                await dbContext.SaveChangesAsync();
                todoId = todo.Id;
            }

            var updateDTO = new UpdateTodoDTO();

            // Act
            var updatedTodo = await todoService.UpdateTodo(todoId, updateDTO);

            // Assert
            Assert.NotNull(updatedTodo);
            Assert.Equal("Organize Closet", updatedTodo.Title);
            Assert.Equal("Sort clothes and donate unused items.", updatedTodo.Description);
        }

        #endregion

        #region DeleteTodo

        [Fact]
        public async Task DeleteTodoAsync_ShouldDeleteTodoWhenExists()
        {
            // Arrange
            var todoService = ServiceProvider.GetRequiredService<TodoService>();
            int todoId;
            using (var scope = ServiceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var todo = new Todo { Title = "Pay Electricity Bill", Description = "Pay the monthly electricity bill online.", ExpiresAt = DateTime.UtcNow.AddDays(1) };
                dbContext.Todos.Add(todo);
                await dbContext.SaveChangesAsync();
                todoId = todo.Id;
            }

            // Act
            var result = await todoService.DeleteTodoAsync(todoId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteTodoAsync_ShouldReturnFalseWhenTodoDoesNotExist()
        {
            // Arrange
            var todoService = ServiceProvider.GetRequiredService<TodoService>();

            // Act
            var result = await todoService.DeleteTodoAsync(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteTodoAsync_ShouldRemoveTodoFromDatabase()
        {
            // Arrange
            var todoService = ServiceProvider.GetRequiredService<TodoService>();
            int todoId;
            using (var scope = ServiceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var todo = new Todo { Title = "Buy Groceries", Description = "Purchase milk, eggs, and bread from the store.", ExpiresAt = DateTime.UtcNow.AddDays(1) };
                dbContext.Todos.Add(todo);
                await dbContext.SaveChangesAsync();
                todoId = todo.Id;
            }

            // Act
            await todoService.DeleteTodoAsync(todoId);

            // Assert
            using (var scope = ServiceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var todoInDb = await dbContext.Todos.FindAsync(todoId);
                Assert.Null(todoInDb);
            }
        }

        #endregion

        #region MarkTodoAsDone

        [Fact]
        public async Task MarkTodoAsDone_ShouldMarkTodoAsCompleted()
        {
            // Arrange
            var todoService = ServiceProvider.GetRequiredService<TodoService>();
            int todoId;
            using (var scope = ServiceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var todo = new Todo { Title = "Submit Tax Documents", Progress = 0, ExpiresAt = DateTime.UtcNow.AddDays(1) };
                dbContext.Todos.Add(todo);
                await dbContext.SaveChangesAsync();
                todoId = todo.Id;
            }

            // Act
            var updatedTodo = await todoService.MarkTodoAsDone(todoId);

            // Assert
            Assert.NotNull(updatedTodo);
            Assert.Equal(1, updatedTodo.Progress);
        }

        [Fact]
        public async Task MarkTodoAsDone_ShouldReturnNullWhenTodoDoesNotExist()
        {
            // Arrange
            var todoService = ServiceProvider.GetRequiredService<TodoService>();

            // Act
            var updatedTodo = await todoService.MarkTodoAsDone(999);

            // Assert
            Assert.Null(updatedTodo);
        }

        [Fact]
        public async Task MarkTodoAsDone_ShouldNotChangeOtherFields()
        {
            // Arrange
            var todoService = ServiceProvider.GetRequiredService<TodoService>();
            int todoId;
            using (var scope = ServiceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var todo = new Todo { Title = "Clean Garage", Description = "Organize tools and clean the garage.", Progress = 0, ExpiresAt = DateTime.UtcNow.AddDays(1) };
                dbContext.Todos.Add(todo);
                await dbContext.SaveChangesAsync();
                todoId = todo.Id;
            }

            // Act
            var updatedTodo = await todoService.MarkTodoAsDone(todoId);

            // Assert
            Assert.NotNull(updatedTodo);
            Assert.Equal("Clean Garage", updatedTodo.Title);
            Assert.Equal("Organize tools and clean the garage.", updatedTodo.Description);
            Assert.Equal(1, updatedTodo.Progress);
        }
        #endregion
    }

}
