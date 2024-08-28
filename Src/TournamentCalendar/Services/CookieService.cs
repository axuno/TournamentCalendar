namespace TournamentCalendar.Services;

using Microsoft.AspNetCore.Http;

/// <summary>
/// This service provides methods to get, set and remove cookies.
/// </summary>
public class CookieService : ICookieService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CookieService> _logger;

    public const string LocationCookieName = ".tc.loc";

    /// <summary>
    /// Initializes a new instance of the <see cref="CookieService"/> class.
    /// Register as a scoped service.
    /// </summary>
    /// <param name="httpContextAccessor"></param>
    /// <param name="logger"></param>
    public CookieService(IHttpContextAccessor httpContextAccessor, ILogger<CookieService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    /// <summary>
    /// Gets the value of a cookie.
    /// </summary>
    /// <param name="cookieName"></param>
    /// <returns>The value of the cookie or <see langword="null"/> if not found.</returns>
    public string? GetCookieValue(string cookieName)
    {
        if (!EnsureHttpContext())
        {
            return null;
        }

        _httpContextAccessor.HttpContext!.Request.Cookies.TryGetValue(cookieName, out var cookieValue);
        return cookieValue;
    }

    /// <summary>
    /// Sets the value of a cookie.
    /// </summary>
    /// <param name="cookieName"></param>
    /// <param name="cookieValue"></param>
    /// <param name="expireTime">if <see langword="null"/>, 1 year is used as default</param>
    /// <returns><see langword="true"/> if the cookie could be set.</returns>
    public bool SetCookieValue(string cookieName, string cookieValue, TimeSpan? expireTime)
    {
        if (!EnsureHttpContext())
        {
            return false;
        }

        _httpContextAccessor.HttpContext!.Response.Cookies.Append(cookieName, cookieValue, new CookieOptions
        {
            Expires = expireTime.HasValue ? DateTime.Now.Add(expireTime.Value) : DateTime.Now.AddYears(1),
            HttpOnly = true, IsEssential = true, Secure = true, SameSite = SameSiteMode.Strict
        });

        return true;
    }

    /// <summary>
    /// Removes a cookie.
    /// </summary>
    /// <param name="cookieName"></param>
    public void RemoveCookie(string cookieName)
    {
        if (!EnsureHttpContext())
        {
            return;
        }

        _httpContextAccessor.HttpContext!.Response.Cookies.Delete(cookieName);
    }

    private bool EnsureHttpContext()
    {
        if (_httpContextAccessor.HttpContext == null)
        {
            _logger.LogCritical("HttpContext is null");
            return false;
        }
        return true;
    }
}
