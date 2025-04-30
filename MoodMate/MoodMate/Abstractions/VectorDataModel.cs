using System.Text.Json;

namespace MoodMate.Abstractions
{
    public class VectorDataModel
    {
        public required Dictionary<string, object> MetaData { get; set; } 
        public required string Data { get; set; } 
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
