using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using TournamentCalendar.Models.AccountViewModels;
using TournamentCalendar.Views;

namespace TournamentCalendar.Controllers;

public class Auth : ControllerBase
{
    private readonly IConfiguration _configuration;

    public Auth(IConfiguration configuration)
    {
        _configuration = configuration;
    }
        
    [HttpGet("/sign-in")]
    public IActionResult SignIn([FromQuery] string? returnUrl)
    {
        if (!ModelState.IsValid)
            return View(ViewName.Auth.SignIn);
        
        if (returnUrl != null) ViewData["ReturnUrl"] = returnUrl;
        return View(ViewName.Auth.SignIn);
    }
        
    [HttpPost("/sign-in"), ValidateAntiForgeryToken]
    public async Task<IActionResult> SignIn([FromForm] SignInViewModel model, [FromQuery] string? returnUrl)
    {
        if(!ModelState.IsValid)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(ViewName.Auth.SignIn);
        }

        Library.Authentication.User? foundUser = null;
        var hashedPassword =
            Convert.ToBase64String(
                System.Security.Cryptography.SHA1.HashData(
                    new UTF8Encoding().GetBytes(model.Password ?? string.Empty)));

        var users = new List<Library.Authentication.User>();
        _configuration.Bind("Authentication", users); // Load users from configuration

        foreach (var user in users) // NOSONAR - users not always empty
        {
            if ((user.UserName == model.EmailOrUsername || user.Email == model.EmailOrUsername) && user.PasswordHash== hashedPassword)
            {
                foundUser = user; 
            }
        }

        if (foundUser == null) // NOSONAR - not always null
        {
            ViewData["ReturnUrl"] = returnUrl;
            ModelState.AddModelError("", "Benutzer nicht gefunden");
            return View(ViewName.Auth.SignIn);
        }
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, foundUser.UserName, ClaimValueTypes.String, "https://volleyball-turnier.de"),
            new(ClaimTypes.Email, foundUser.Email, ClaimValueTypes.String, "https://volleyball-turnier.de"),
        };
        foreach (var roleName in foundUser.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, roleName));
        }

        var userIdentity = new ClaimsIdentity(claims, "SecureLogin");
        var userPrincipal = new ClaimsPrincipal(userIdentity);
            
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            userPrincipal,
            new AuthenticationProperties
            {
                ExpiresUtc = DateTime.UtcNow.AddMonths(3),
                IsPersistent = false,
                AllowRefresh = true
            });

        return RedirectToLocal(returnUrl ?? "/");
    }

    [HttpGet("/sign-out")]
    public new async Task<IActionResult> SignOut()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction(nameof(SignIn));
    }

    [HttpGet]
    public IActionResult Denied()
    {
        return View(ViewName.Auth.Denied);
    }

    private IActionResult RedirectToLocal(string returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(Calendar.All), nameof(Controllers.Calendar));
    }
}
