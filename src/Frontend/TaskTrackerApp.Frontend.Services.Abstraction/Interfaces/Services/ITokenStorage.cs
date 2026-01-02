namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

public interface ITokenStorage
{
    public string? GetAccessToken();

    public void SetAccessToken(string token);

    public void ClearAccessToken();
}