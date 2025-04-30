using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.Data;
using TodoApp.Services;

public class TestBase
{
    protected ServiceProvider ServiceProvider { get; private set; }

    public TestBase()
    {
        var services = new ServiceCollection();

        // Add DbContext with InMemory provider
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("TestDatabase"));

        // Add services
        services.AddScoped<TodoService>();

        ServiceProvider = services.BuildServiceProvider();
    }
}