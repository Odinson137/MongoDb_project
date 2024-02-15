using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDb_project.Models;

var db = new MongoClient("mongodb://root:example@localhost:27017").GetDatabase("test");

User user = new User
{
    Id = 3,
    Name = "Yura",
    LastName = "Bury",
    Company = new Company()
    {
        Id = 1,
        Name = "Microsoft",
    }
};

var collection = db.GetCollection<User>("users");

var users = await collection.FindAsync(new BsonDocument { { "_id", 3 } });


foreach (var selected in await users.ToListAsync())
{
    Console.WriteLine(selected.Name);
}

