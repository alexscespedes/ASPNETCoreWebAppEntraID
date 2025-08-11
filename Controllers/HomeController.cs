using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Abstractions;
using webmvc.Models;

namespace webmvc.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    readonly IAuthorizationHeaderProvider authorizationHeaderProvider;

    public HomeController(ILogger<HomeController> logger, IAuthorizationHeaderProvider authorizationHeaderProvider)
    {
        _logger = logger;
        this.authorizationHeaderProvider = authorizationHeaderProvider;
    }

    public async Task<IActionResult> Index()
    {
        // Acquire the access token.
        string[] scopes = new string[] { "https://storage.azure.com/user_impersonation" };
        string accessToken = await authorizationHeaderProvider.CreateAuthorizationHeaderForUserAsync(scopes);

        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("x-ms-version", "2024-05-04");
        client.DefaultRequestHeaders.Add("Authorization", accessToken);

        string containerName = "data";
        string fileName = "universe.sql";

        string storageAccountName = "appstorage90090030";
        string blobUri = $"https://{storageAccountName}.blob.core.windows.net/{containerName}/{fileName}";

        HttpResponseMessage message = await client.GetAsync(blobUri);
        string blobContent = await message.Content.ReadAsStringAsync();

        ViewData["blob_content"] = blobContent;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
