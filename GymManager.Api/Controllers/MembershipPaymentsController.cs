using GymManager.Api.Data;
using GymManager.Api.DTOs.Common;
using GymManager.Api.DTOs.MembershipPayments;
using GymManager.Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MembershipPaymentsController : BaseApiController
{
    private readonly GymDbContext _context;

    public MembershipPaymentsController(
        GymDbContext context)
    {
        _context = context;
    }

    private static MembershipPaymentResponse Map(MembershipPayment p)
    {
        return new MembershipPaymentResponse
        {
            Id = p.Id,
            MemberId = p.MemberId,
            PaymentDate = p.PaymentDate,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            Amount = p.Amount,
            Note = p.Note,
            CreatedAt = p.CreatedAt
        };
    }

    [HttpGet]
    public async Task<IActionResult> GetPayments(
    int page = 1,
    int pageSize = 20)
    {
        if (page < 1)
            page = 1;

        if (pageSize < 1)
            pageSize = 20;

        if (pageSize > 100)
            pageSize = 100;

        var query = _context.MembershipPayments
            .Include(x => x.Member)
            .Where(x =>
                !x.IsDeleted &&
                x.Member.GymId == GymId);

        var totalCount = await query.CountAsync();

        var payments = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new MembershipPaymentResponse
            {
                Id = x.Id,
                MemberId = x.MemberId,
                MemberName = x.Member.FirstName + " " + x.Member.LastName,
                MemberPhone = x.Member.Phone,
                PaymentDate = x.PaymentDate,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Amount = x.Amount,
                Note = x.Note,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return Ok(new PagedResponse<MembershipPaymentResponse>
        {
            Items = payments,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPayment(long id)
    {
        var payment = await _context.MembershipPayments
            .Include(x => x.Member)
            /*.Where(x =>
                x.Id == id &&
                x.Member.GymId == GymId)*/
            .Where(x =>
                x.Id == id &&
                !x.IsDeleted &&
                x.Member.GymId == GymId)
            .Select(x => new MembershipPaymentResponse
            {
                Id = x.Id,
                MemberId = x.MemberId,
                MemberName = x.Member.FirstName + " " + x.Member.LastName,
                MemberPhone = x.Member.Phone,
                PaymentDate = x.PaymentDate,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Amount = x.Amount,
                Note = x.Note,
                CreatedAt = x.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (payment == null)
        {
            return NotFound();
        }

        return Ok(payment);
    }

    [HttpGet("member/{memberId}")]
    public async Task<IActionResult> GetByMember(long memberId)
    {
        var payments = await _context.MembershipPayments
            .Where(x =>
                x.MemberId == memberId &&
                !x.IsDeleted &&
                x.Member.GymId == GymId)
            .OrderByDescending(x => x.EndDate)
            .Select(x => new MembershipPaymentResponse
            {
                Id = x.Id,
                MemberId = x.MemberId,
                MemberName = x.Member.FirstName + " " + x.Member.LastName,
                MemberPhone = x.Member.Phone,
                PaymentDate = x.PaymentDate,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Amount = x.Amount,
                Note = x.Note,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return Ok(payments);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePayment(
    CreateMembershipPaymentRequest request)
    {
        var member = await _context.Members
            .FirstOrDefaultAsync(x =>
                x.Id == request.MemberId &&
                x.GymId == GymId);

        if (member == null)
        {
            return NotFound("Member not found.");
        }

        

        var payment = new MembershipPayment
        {
            MemberId = request.MemberId,

            CreatedByUserId = UserId,

            PaymentDate = request.PaymentDate,
            StartDate = request.StartDate,
            EndDate = request.EndDate,

            Amount = request.Amount,
            Note = request.Note,

            CreatedAt = DateTime.UtcNow
        };

        _context.MembershipPayments.Add(payment);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetPayment),
            new { id = payment.Id },
            Map(payment));
    }

    [HttpPost("renew")]
    public async Task<IActionResult> RenewMembership(RenewMembershipRequest request)
    {
        var member = await _context.Members
            .FirstOrDefaultAsync(x =>
                x.Id == request.MemberId &&
                x.GymId == GymId);

        if (member == null)
            return NotFound("Member not found");

        member.IsActive = true;

        var lastPayment = await _context.MembershipPayments
            .Where(x =>
                x.MemberId == request.MemberId &&
                !x.IsDeleted)
            .OrderByDescending(x => x.EndDate)
            .FirstOrDefaultAsync();

        DateOnly startDate;

        if (lastPayment == null)
        {
            startDate = DateOnly.FromDateTime(DateTime.Today);
        }
        else if (lastPayment.EndDate >= DateOnly.FromDateTime(DateTime.Today))
        {
            startDate = lastPayment.EndDate.AddDays(1);
        }
        else
        {
            startDate = DateOnly.FromDateTime(DateTime.Today);
        }

        var endDate = startDate.AddMonths(1).AddDays(-1);

        var payment = new MembershipPayment
        {
            MemberId = request.MemberId,
            CreatedByUserId = UserId,

            PaymentDate = DateOnly.FromDateTime(DateTime.Today),
            StartDate = startDate,
            EndDate = endDate,

            Amount = request.Amount,
            Note = request.Note,

            CreatedAt = DateTime.UtcNow
        };

        _context.MembershipPayments.Add(payment);
        await _context.SaveChangesAsync();

        var response = new MembershipPaymentResponse
        {
            Id = payment.Id,
            MemberId = payment.MemberId,
            PaymentDate = payment.PaymentDate,
            StartDate = payment.StartDate,
            EndDate = payment.EndDate,
            Amount = payment.Amount,
            Note = payment.Note,
            CreatedAt = payment.CreatedAt
        };

        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePayment(long id, UpdateMembershipPaymentRequest request)
    {
        var payment = await _context.MembershipPayments
            .Include(x => x.Member)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                !x.IsDeleted &&
                x.Member.GymId == GymId);

        if (payment == null)
            return NotFound();

        payment.PaymentDate = request.PaymentDate;
        payment.StartDate = request.StartDate;
        payment.EndDate = request.EndDate;
        payment.Amount = request.Amount;
        payment.Note = request.Note;

        await _context.SaveChangesAsync();

        /*return Ok(new MembershipPaymentResponse
        {
            Id = payment.Id,
            MemberId = payment.MemberId,
            PaymentDate = payment.PaymentDate,
            StartDate = payment.StartDate,
            EndDate = payment.EndDate,
            Amount = payment.Amount,
            Note = payment.Note,
            CreatedAt = payment.CreatedAt
        });*/
        return Ok(Map(payment));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePayment(long id)
    {
        var payment = await _context.MembershipPayments
            .Include(x => x.Member)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                !x.IsDeleted &&
                x.Member.GymId == GymId);

        if (payment == null)
            return NotFound();

        payment.IsDeleted = true;

        await _context.SaveChangesAsync();

        return NoContent();
    }

}