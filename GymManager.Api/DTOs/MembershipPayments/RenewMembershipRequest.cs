namespace GymManager.Api.DTOs.MembershipPayments;

public class RenewMembershipRequest
{
    public long MemberId { get; set; }

    public int NumberOfDays { get; set; }

    public decimal Amount { get; set; }

    public string? Note { get; set; }
}