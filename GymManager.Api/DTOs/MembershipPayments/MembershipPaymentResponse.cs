namespace GymManager.Api.DTOs.MembershipPayments;

public class MembershipPaymentResponse
{
    public long Id { get; set; }

    public long MemberId { get; set; }

    public string MemberName { get; set; } = string.Empty;

    public string MemberPhone { get; set; } = string.Empty;

    public DateOnly PaymentDate { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public decimal Amount { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }
}