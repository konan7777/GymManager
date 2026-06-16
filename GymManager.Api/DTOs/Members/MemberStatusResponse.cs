namespace GymManager.Api.DTOs.Members;

public class MemberStatusResponse
{
    public long Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public DateOnly? MembershipEndDate { get; set; }

    public bool IsMembershipActive { get; set; }
}