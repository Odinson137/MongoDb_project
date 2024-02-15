using MongoDB.Bson.Serialization.Attributes;

namespace MongoDb_project.Models;

public class Company
{
    public Company()
    {
        
    }
    public Company(int id, string name)
    {
        Id = id;
        Name = name;
    }

    [BsonId]
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime FoundedTime { get; set; } = DateTime.Now;
}
