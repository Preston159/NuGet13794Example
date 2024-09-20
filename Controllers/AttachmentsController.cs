using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace NuGet13794Example.Controllers;

[ApiController]
[Route("[controller]/v0")]
[Authorize]
[RequiredScope(Scopes.ATTACHMENTS_READ)]
public class AttachmentsController : ControllerBase
{
    private readonly ILogger<AttachmentsController> _logger;

    public AttachmentsController(ILogger<AttachmentsController> logger)
    {
        _logger = logger;
    }
}
