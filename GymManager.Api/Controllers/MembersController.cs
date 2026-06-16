using GymManager.Api.Data;
using GymManager.Api.DTOs.Members;
using GymManager.Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManager.Api.DTOs.Common;

namespace GymManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MembersController : BaseApiController
{
    private readonly GymDbContext _context;

    public MembersController(GymDbContext context)
    {
        _context = context;
    }

    private static MemberResponse Map(Member member)
    {
        return new MemberResponse
        {
            Id = member.Id,
            FirstName = member.FirstName,
            LastName = member.LastName,
            Phone = member.Phone,
            Email = member.Email,
            BirthDate = member.BirthDate,
            Gender = member.Gender,
            MemberType = member.MemberType,
            Notes = member.Notes,
            IsActive = member.IsActive,
            CreatedAt = member.CreatedAt
        };
    }

    [HttpGet]
    public async Task<IActionResult> GetMembers(
    int page = 1,
    int pageSize = 20,
    string? search = null)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        if (page < 1)
            page = 1;

        if (pageSize < 1)
            pageSize = 20;

        if (pageSize > 100)
            pageSize = 100;

        var query = _context.Members
            .Where(x => x.GymId == GymId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim().ToLower();

            query = query.Where(x =>
                x.FirstName.ToLower().Contains(search) ||
                x.LastName.ToLower().Contains(search) ||
                (x.FirstName + " " + x.LastName)
                    .ToLower()
                    .Contains(search) ||
                x.Phone.Contains(search) ||
                (x.Email != null &&
                 x.Email.ToLower().Contains(search)));
        }

        var totalCount = await query.CountAsync();

        var members = await query
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new MemberResponse
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Phone = x.Phone,
                Email = x.Email,
                BirthDate = x.BirthDate,
                Gender = x.Gender,
                MemberType = x.MemberType,
                Notes = x.Notes,
                IsActive = x.IsActive,

                MembershipEndDate = x.MembershipPayments
                    .Where(p => !p.IsDeleted)
                    .OrderByDescending(p => p.EndDate)
                    .Select(p => (DateOnly?)p.EndDate)
                    .FirstOrDefault(),

                HasActiveMembership = x.MembershipPayments
                    .Where(p => !p.IsDeleted)
                    .Any(p => p.EndDate >= today),

                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return Ok(new PagedResponse<MemberResponse>
        {
            Items = members,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMember(long id)
    {
        var member = await _context.Members
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.GymId == GymId);

        if (member == null)
        {
            return NotFound();
        }

        return Ok(Map(member));
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveMembers()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var members = await _context.Members
            .Where(x => x.GymId == GymId)
            .Select(x => new MemberStatusResponse
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Phone = x.Phone,

                MembershipEndDate = x.MembershipPayments
                    .Where(p => !p.IsDeleted)
                    .OrderByDescending(p => p.EndDate)
                    .Select(p => (DateOnly?)p.EndDate)
                    .FirstOrDefault(),

                IsMembershipActive = x.MembershipPayments
                    .Where(p => !p.IsDeleted)
                    .Any(p => p.EndDate >= today)
            })
            .Where(x => x.IsMembershipActive)
            .ToListAsync();

        return Ok(members);
    }

    [HttpGet("expired")]
    public async Task<IActionResult> GetExpiredMembers()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var members = await _context.Members
            .Where(x => x.GymId == GymId)
            .Where(x =>
                !x.MembershipPayments
                    .Where(p => !p.IsDeleted)
                    .Any(p => p.EndDate >= today)
            )
            .Select(x => new MemberStatusResponse
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Phone = x.Phone,

                MembershipEndDate = x.MembershipPayments
                    .Where(p => !p.IsDeleted)
                    .OrderByDescending(p => p.EndDate)
                    .Select(p => (DateOnly?)p.EndDate)
                    .FirstOrDefault(),

                IsMembershipActive = x.MembershipPayments
                    .Where(p => !p.IsDeleted)
                    .Any(p => p.EndDate >= today)
            })
            .ToListAsync();

        return Ok(members);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchMembers(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return Ok(new List<MemberSearchResponse>());
        }

        term = term.Trim().ToLower();

        var members = await _context.Members
            .Where(x =>
                x.GymId == GymId &&
                (
                    x.FirstName.ToLower().Contains(term) ||
                    x.LastName.ToLower().Contains(term) ||
                    x.Phone.Contains(term) ||
                    (x.Email != null &&
                     x.Email.ToLower().Contains(term))
                ))
            .Select(x => new MemberSearchResponse
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Phone = x.Phone,
                Email = x.Email
            })
            .Take(20)
            .ToListAsync();

        return Ok(members);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMember(CreateMemberRequest request)
    {
        var member = new Member
        {
            GymId = GymId,

            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            Email = request.Email,
            BirthDate = request.BirthDate,

            Gender = request.Gender,
            MemberType = request.MemberType,

            Notes = request.Notes,

            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Members.Add(member);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetMember),
            new { id = member.Id },
            Map(member));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMember(
        long id,
        UpdateMemberRequest request)
    {
        var member = await _context.Members
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.GymId == GymId);

        if (member == null)
        {
            return NotFound();
        }

        member.FirstName = request.FirstName;
        member.LastName = request.LastName;
        member.Phone = request.Phone;
        member.Email = request.Email;
        member.BirthDate = request.BirthDate;

        member.Gender = request.Gender;
        member.MemberType = request.MemberType;

        member.Notes = request.Notes;
        member.IsActive = request.IsActive;

        await _context.SaveChangesAsync();

        return Ok(Map(member));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMember(long id)
    {
        var member = await _context.Members
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.GymId == GymId);

        if (member == null)
        {
            return NotFound();
        }

        _context.Members.Remove(member);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}