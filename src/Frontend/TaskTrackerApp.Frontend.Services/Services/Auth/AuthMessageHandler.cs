using System.Net;
using System.Net.Http.Headers;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

public sealed class AuthMessageHandler : DelegatingHandler
{
    private readonly ITokenStorage _tokenStorage;
    private readonly IAuthService _auth;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);

    public AuthMessageHandler(
        ITokenStorage tokenStorage,
        IAuthService auth)
    {
        _tokenStorage = tokenStorage;
        _auth = auth;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (IsAuthEndpoint(request))
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var retryRequest = CloneRequest(request);

        var accessToken = _tokenStorage.GetAccessToken();
        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.Unauthorized)
        {
            return response;
        }

        await _refreshLock.WaitAsync(cancellationToken);

        try
        {
            var newToken = _tokenStorage.GetAccessToken();
            if (!string.Equals(accessToken, newToken, StringComparison.Ordinal))
            {
                retryRequest.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", newToken);

                return await base.SendAsync(retryRequest, cancellationToken);
            }

            var refreshed = await _auth.RefreshAsync();

            if (!refreshed.IsSuccess)
            {
                _tokenStorage.ClearAccessToken();
                return response;
            }

            newToken = _tokenStorage.GetAccessToken();

            retryRequest.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", newToken);

            return await base.SendAsync(retryRequest, cancellationToken);
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    private static bool IsAuthEndpoint(HttpRequestMessage request)
        => request.RequestUri?.AbsolutePath.Contains("/auth", StringComparison.OrdinalIgnoreCase) == true;

    private static HttpRequestMessage CloneRequest(HttpRequestMessage request)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri);

        if (request.Content != null)
        {
            var ms = new MemoryStream();
            request.Content.CopyToAsync(ms).GetAwaiter().GetResult();
            ms.Position = 0;

            clone.Content = new StreamContent(ms);

            foreach (var header in request.Content.Headers)
            {
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        foreach (var header in request.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        return clone;
    }
}