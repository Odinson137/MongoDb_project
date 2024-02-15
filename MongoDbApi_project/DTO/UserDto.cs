using MongoDB.Bson.Serialization.Attributes;
using MongoDb_project.Models;

namespace MongoDbApi_project.DTO
{
    public class UserDto
    {
        public string Name { get; set; }
        public string LastName { get; set; }
    }
}
