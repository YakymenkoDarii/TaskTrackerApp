using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace TaskTrackerApp.Frontend.Services.Services.Auth;

public class CookieHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
    {
        // THIS IS THE MISSING PIECE
        // It forces the browser to attach cookies to this cross-origin request
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

        return await base.SendAsync(request, cancellationToken);
    }
}