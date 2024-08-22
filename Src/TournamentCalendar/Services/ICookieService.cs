namespace TournamentCalendar.Services;

public interface ICookieService
{
    string? GetCookieValue(string cookieName);
    bool SetCookieValue(string cookieName, string cookieValue, TimeSpan? expireTime);
    void RemoveCookie(string cookieName);
}
