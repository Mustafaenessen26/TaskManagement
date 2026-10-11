namespace TaskManagement.Application.DTOs.Tasks;

public class TaskReportDto
{
    public int TotalCount { get; set; }
    public int CompletedCount { get; set; }
    public int InProgressCount { get; set; }
    public int PendingCount { get; set; }

    /// <summary>Tamamlanan görevlerin toplama oranı (yüzde, 0-100).</summary>
    public double CompletionRate { get; set; }
}