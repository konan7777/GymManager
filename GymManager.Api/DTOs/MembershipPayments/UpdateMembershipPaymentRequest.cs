namespace GymManager.Api.DTOs.MembershipPayments;

public class UpdateMembershipPaymentRequest
{
    public DateOnly PaymentDate { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public decimal Amount { get; set; }

    public string? Note { get; set; }
}