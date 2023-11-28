namespace task.Models{
    public class Comment
{
    public int CommentId { get; set; }

    public int? TasksId { get; set; }
    public Tasks? Tasks { get; set; }

    public string Text { get; set; }
    public string? OwnerId { get; set; }
    public AccountUser? Owner { get; set; }
    public DateTime? DateCreated { get; set; }
    public DateTime? DateEdited { get; set; }
}
}