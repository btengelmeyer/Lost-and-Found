using Microsoft.EntityFrameworkCore;
using LostAndFoundAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Entity Framework with SQLite
builder.Services.AddDbContext<LostAndFoundContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin() // Allow all origins for development
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LostAndFoundContext>();
    context.Database.EnsureCreated();
    
    // Seed sample data if database is empty
    if (!context.Users.Any())
    {
        var users = new[]
        {
            new LostAndFoundAPI.Models.User { Name = "John Doe", Email = "john@example.com", Phone = "555-0101" },
            new LostAndFoundAPI.Models.User { Name = "Jane Smith", Email = "jane@example.com", Phone = "555-0102" },
            new LostAndFoundAPI.Models.User { Name = "Bob Johnson", Email = "bob@example.com", Phone = "555-0103" }
        };
        context.Users.AddRange(users);
        
        var foundItems = new[]
        {
            new LostAndFoundAPI.Models.FoundItem 
            { 
                Title = "Lost iPhone", 
                Description = "Black iPhone 13 found in library", 
                Location = "Gorgas Library", 
                DateFound = DateTime.Now.AddDays(-2),
                ContactInfo = "555-0101",
                IsVisible = true
            },
            new LostAndFoundAPI.Models.FoundItem 
            { 
                Title = "Red Backpack", 
                Description = "Red North Face backpack with laptop", 
                Location = "Student Center", 
                DateFound = DateTime.Now.AddDays(-1),
                ContactInfo = "555-0102",
                IsVisible = true
            },
            new LostAndFoundAPI.Models.FoundItem 
            { 
                Title = "Car Keys", 
                Description = "Toyota key fob with house keys", 
                Location = "Parking Deck B", 
                DateFound = DateTime.Now.AddHours(-6),
                ContactInfo = "555-0103",
                IsVisible = true
            }
        };
        context.FoundItems.AddRange(foundItems);
        
        context.SaveChanges();
    }
}

app.Run();
