using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toralux.Open.IdentityServer.Admin.UI.Services.AntiForgeryProtection;

namespace Toralux.Open.IdentityServer.Admin.Controllers;

public class CsrfController(IAntiforgery antiforgery) : Controller
{
    [HttpPost]
    [AntiForgeryProtection]
    [AllowAnonymous]
    public ActionResult<AntiForgeryProtectionToken> GetToken()
    {
        var tokens = antiforgery.GetAndStoreTokens(HttpContext);

        if (tokens.RequestToken == null)
        {
            return BadRequest("Could not generate anti-forgery token");
        }

        return Ok(new AntiForgeryProtectionToken(tokens.RequestToken, tokens.FormFieldName));
    }
}