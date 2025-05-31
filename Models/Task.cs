using System;

namespace TaskManager.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime? DueDate { get; set; }
        public int? Priority { get; set; } // 1-High, 2-Medium, 3-Low
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}