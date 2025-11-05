using FinanceZakatManager.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceZakatManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/me")]
public class MeController : ControllerBase
{
    private readonly ProfileService _profileService;

    public MeController(ProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var profile = await _profileService.GetOrCreateAsync(cancellationToken);
        var etag = Api.Utils.EtagGenerator.FromStrings(profile.Id.ToString(), profile.CreatedUtc.ToString("O"));
        Response.Headers.ETag = etag;
        return Ok(new
        {
            profile.Id,
            profile.Email,
            profile.DisplayName,
            profile.CreatedUtc
        });
    }
}
