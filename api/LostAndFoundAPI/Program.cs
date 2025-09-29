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

// Serve static files from wwwroot directory
app.UseDefaultFiles();
app.UseStaticFiles();

// Add fallback to serve index.html for SPA routing
app.MapFallbackToFile("index.html");

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
            new LostAndFoundAPI.Models.User 
            { 
                Id = Guid.NewGuid().ToString(),
                Name = "John Doe", 
                Email = "john@example.com", 
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                UserType = LostAndFoundAPI.Models.UserType.Student
            },
            new LostAndFoundAPI.Models.User 
            { 
                Id = Guid.NewGuid().ToString(),
                Name = "Jane Smith", 
                Email = "jane@example.com", 
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                UserType = LostAndFoundAPI.Models.UserType.Student
            },
            new LostAndFoundAPI.Models.User 
            { 
                Id = Guid.NewGuid().ToString(),
                Name = "Admin User", 
                Email = "admin@example.com", 
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                UserType = LostAndFoundAPI.Models.UserType.Admin
            }
        };
        context.Users.AddRange(users);
        context.SaveChanges();
        
        var foundItems = new[]
        {
            new LostAndFoundAPI.Models.FoundItem 
            { 
                Id = Guid.NewGuid().ToString(),
                Name = "Lost iPhone", 
                Description = "Black iPhone 13 found in library", 
                Building = "Gorgas Library", 
                Room = "Study Room 205",
                DateFound = DateTime.Now.AddDays(-2),
                AddedBy = users[0].Id,
                IsVisible = true
            },
            new LostAndFoundAPI.Models.FoundItem 
            { 
                Id = Guid.NewGuid().ToString(),
                Name = "Red Backpack", 
                Description = "Red North Face backpack with laptop", 
                Building = "Student Center", 
                Room = "Food Court",
                DateFound = DateTime.Now.AddDays(-1),
                AddedBy = users[1].Id,
                IsVisible = true
            },
            new LostAndFoundAPI.Models.FoundItem 
            { 
                Id = Guid.NewGuid().ToString(),
                Name = "Car Keys", 
                Description = "Toyota key fob with house keys", 
                Building = "Parking Deck B", 
                Room = "Level 2",
                DateFound = DateTime.Now.AddHours(-6),
                AddedBy = users[0].Id,
                IsVisible = true
            }
        };
        context.FoundItems.AddRange(foundItems);
        
        context.SaveChanges();
    }
}

app.Run();
