namespace AutoShine.Web.ApiClients
{
    public interface IAutoShineApiClient
    {
        Task<HealthResponse?> GetHealthAsync(
            CancellationToken cancellationToken = default);
    }
}
