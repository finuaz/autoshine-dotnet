namespace AutoShine.Web.Models;

public class MembershipPlan
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal DiscountPercentage { get; set; }
    public decimal RequiredDepositPercentage { get; set; }
    public bool IsVip { get; set; }
    public ICollection<CustomerMembership> CustomerMemberships { get; set; }
        = new List<CustomerMembership>();
}