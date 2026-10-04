namespace AutoShine.Web.Models;

public class CustomerMembership
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int MembershipPlanId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public Customer Customer { get; set; } = null!;
    public MembershipPlan MembershipPlan { get; set; } = null!;
}