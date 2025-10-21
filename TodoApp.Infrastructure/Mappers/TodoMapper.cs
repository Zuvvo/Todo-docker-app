using System;
using TodoApp.DAL.Models;
using TodoApp.DTO;

namespace TodoApp.Infrastructure.Mappers
{
    public static class TodoMapper
    {
        public static TodoDTO ToDto(this Todo todo)
        {
            return new TodoDTO
            {
                Id = todo.Id,
                ExpiresAt = todo.ExpiresAt,
                Title = todo.Title,
                Description = todo.Description,
                Progress = todo.Progress
            };
        }

        public static Todo ToModel(this TodoDTO todoDto)
        {
            return new Todo
            {
                Id = todoDto.Id,
                ExpiresAt = todoDto.ExpiresAt,
                Title = todoDto.Title,
                Description = todoDto.Description,
                Progress = todoDto.Progress
            };
        }

        public static Todo ToModel(this AddTodoDTO addTodoDTO)
        {
            return new Todo
            {
                ExpiresAt = addTodoDTO.ExpiresAt,
                Title = addTodoDTO.Title ?? string.Empty,
                Description = addTodoDTO.Description ?? string.Empty,
                Progress = 0
            };
        }
    }
}
