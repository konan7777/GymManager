using GymManager.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GymManager.Api.DTOs.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace GymManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : BaseApiController
{
    private readonly GymDbContext _context;

    public DashboardController(GymDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var totalMembers = await _context.Members
            .CountAsync(x => x.GymId == GymId);

        var activeMembers = await _context.Members
            .Where(x =>
                x.GymId == GymId &&
                x.IsActive &&
                x.MembershipPayments.Any(p =>
                    !p.IsDeleted &&
                    p.EndDate >= today))
            .CountAsync();

        var inactiveMembers = await _context.Members
            .CountAsync(x =>
                x.GymId == GymId &&
                !x.IsActive);

        var expiredMembers = await _context.Members
            .CountAsync(x =>
                x.GymId == GymId &&
                x.IsActive &&
                !x.MembershipPayments.Any(p =>
                    !p.IsDeleted &&
                    p.EndDate >= today));

        var firstDayOfMonth = new DateOnly(
            DateTime.Today.Year,
            DateTime.Today.Month,
            1);

        var monthlyRevenue = await _context.MembershipPayments
            .Where(x =>
                !x.IsDeleted &&
                x.Member.GymId == GymId &&
                x.PaymentDate >= firstDayOfMonth)
            .SumAsync(x => x.Amount);

        var response = new DashboardResponse
        {
            TotalMembers = totalMembers,
            ActiveMembers = activeMembers,
            ExpiredMembers = expiredMembers,
            MonthlyRevenue = monthlyRevenue
        };

        return Ok(response);
    }
}