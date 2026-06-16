namespace GymManager.Api.DTOs.Dashboard;

public class DashboardResponse
{
    public int TotalMembers { get; set; }

    public int ActiveMembers { get; set; }

    public int ExpiredMembers { get; set; }

    public decimal MonthlyRevenue { get; set; }
}