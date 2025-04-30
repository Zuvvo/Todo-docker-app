using Microsoft.Extensions.DependencyInjection;
using System;
using TodoApp.Data;
using Microsoft.EntityFrameworkCore;
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