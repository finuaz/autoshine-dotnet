using Microsoft.AspNetCore.Mvc;
using AutoShine.Web.ViewModels;
using AutoShine.Web.ApiClients;


namespace AutoShine.Web.Controllers;

public class ApiStatusController : Controller
{
    private readonly IAutoShineApiClient _apiClient;

    private readonly ILogger<ApiStatusController> _logger;

    public ApiStatusController(IAutoShineApiClient apiClient, ILogger<ApiStatusController> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _apiClient.GetHealthAsync(cancellationToken);

            if (response is null)
            {
                return View(new ApiStatusViewModel
                {
                    IsAvailable = false,
                    ErrorMessage = "The API returned an empty response."
                });
            }
            return View(new ApiStatusViewModel
            {
                IsAvailable = true,
                Status = response.Status,
            });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Unable to reach AutoShine API.");

            return View(new ApiStatusViewModel
            {
                IsAvailable = false,
                ErrorMessage = "The API could not be reached. Check whether it is running."
            });
        }
        catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(ex, "AutoShine API request timed out.");

            return View(new ApiStatusViewModel
            {
                IsAvailable = false,
                ErrorMessage = "The API request timed out."
            });
        }
    }
}
