using TaskManagement.Domain.Common;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Entities;

public class TaskItem : BaseEntity
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Period Period { get; set; }
    public Status Status { get; set; } = Status.Pending;
    public DateTime Date { get; set; }

    public User? User { get; set; }
}