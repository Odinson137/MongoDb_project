
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDb_project.Models;

public class User
{
    [BsonId]
    public int Id { get; set; }
    public string Name {  get; set; }
    public string LastName {  get; set; }
    public Company Company {  get; set; }
}

