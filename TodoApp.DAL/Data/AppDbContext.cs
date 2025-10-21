using Microsoft.EntityFrameworkCore;
using TodoApp.DAL.Models;

namespace TodoApp.DAL.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Todo>? Todos { get; set; }
    }
}
