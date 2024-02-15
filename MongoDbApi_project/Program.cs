using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDb_project.Models;
using MongoDbApi_project.DTO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(new MongoClient("mongodb://root:example@localhost:27017").GetDatabase("test"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/users", async (IMongoDatabase db) =>
{
    var collections = db.GetCollection<User>("users");
    var users = await collections
            .Find(new BsonDocument())
            .Project(u => new { u.Name, u.LastName })
            .ToListAsync();
    return users;
})
.WithOpenApi();

app.MapGet("/usersWithCompany", async (IMongoDatabase db) =>
{
    var collections = db.GetCollection<User>("users");
    var users = collections.Find("{}").SortByDescending(u => u.Id);
    return await users.ToListAsync();
})
.WithOpenApi();

app.MapGet("/companyGroup", async (IMongoDatabase db) =>
{
    var collections = db.GetCollection<User>("users");
    var users = await collections.Aggregate()
            .Group(u => u.Company.Name, g => new
            {
                Key = g.Key,
                Sum = g.Sum(e => e.Id),
                Min = g.Min(e => e.Id),
                Max = g.Max(e => e.Id),
                Count = g.Count()
            }).ToListAsync();
    return users;
})
.WithOpenApi();

app.MapGet("/users/searchById", async (IMongoDatabase db, int userId) =>
{
    var collections = db.GetCollection<User>("users");
    var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
    var user = await collections.Find(filter).SingleOrDefaultAsync();
    return user;
})
.WithOpenApi();
app.MapGet("/users/searchByName", async (IMongoDatabase db, string userName) =>
{
    var collections = db.GetCollection<User>("users");
    var filter = Builders<User>.Filter.Eq(u => u.Name, userName);
    var users = await collections.Find(filter).ToListAsync();
    return users;
})
.WithOpenApi();
app.MapGet("/users/searchByCompanyName", async (IMongoDatabase db, string companyName) =>
{
    var collections = db.GetCollection<User>("users");
    var filter = Builders<User>.Filter.Eq(u => u.Company.Name, companyName);
    var users = await collections.Find(filter).ToListAsync();
    return users;
})
.WithOpenApi();
app.MapGet("/{yourDocument}", async (IMongoDatabase db, string yourDocument) =>
{
    var collections = db.GetCollection<object>(yourDocument);
    var objects = collections.FindAsync(new BsonDocument());
    return objects;
})
.WithOpenApi();

app.MapPost("/CreateCollection/{listName}", async (IMongoDatabase db, string listName) =>
{
    await db.CreateCollectionAsync(listName);
    return "create";
})
.WithOpenApi();

app.MapPost("/AddUser", async (IMongoDatabase db, [FromBody] UserDto userDto) =>
{
    var collection = db.GetCollection<User>("users");
    var randomNumber = new Random().Next();
    var user = new User()
    {
        Id = randomNumber,
        Name = userDto.Name,
        LastName = userDto.LastName
    };
    await collection.InsertOneAsync(user);
    return "user added";
})
.WithOpenApi();

app.MapPatch("/ChangeUserCompany", async (IMongoDatabase db, [FromQuery] int userId, [FromBody] CompanyDto companyDto) =>
{
    var collection = db.GetCollection<User>("users");
    var randomNumber = new Random().Next();
    var searchUserFilter = Builders<User>.Filter.Eq(u => u.Id, userId);
    var changeCompanyFilter = Builders<User>.Update.Set(u => u.Company, new Company(randomNumber, companyDto.Name));
    await collection.UpdateOneAsync(searchUserFilter, changeCompanyFilter);
    return "company changed";
})
.WithOpenApi();

app.MapDelete("/DeleteUser", async (IMongoDatabase db, [FromQuery] int userId) =>
{
    var collection = db.GetCollection<User>("users");
    var searchUserFilter = Builders<User>.Filter.Eq(u => u.Id, userId);
    await collection.DeleteOneAsync(searchUserFilter);
    return "user deleted";
})
.WithOpenApi();

app.Run();
