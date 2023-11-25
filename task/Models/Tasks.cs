namespace task.Models
{
    public class Tasks{
        public int TasksId { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public int? PriorityId { get; set; }
        public Priority? Priority { get; set; }
        public List<Comment>? Comments { get; set; }
        public AccountUser? Owner { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateEdited { get; set; }
    }
}