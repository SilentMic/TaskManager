using System.Collections.Generic;

namespace TaskManager.Models
{
    public class Category
    {
        public Category()
        {
            Tasks = new List<Task>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Task> Tasks { get; set; }
    }
}