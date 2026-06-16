namespace GymManager.Api.DTOs.MembershipPayments;

public class CreateMembershipPaymentRequest
{
    public long MemberId { get; set; }

    public DateOnly PaymentDate { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public decimal Amount { get; set; }

    public string? Note { get; set; }
}