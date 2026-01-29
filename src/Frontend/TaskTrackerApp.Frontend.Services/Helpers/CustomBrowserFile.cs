using Microsoft.AspNetCore.Components.Forms;

namespace TaskTrackerApp.Frontend.Services.Helpers;

public class CustomBrowserFile : IBrowserFile
{
    private readonly byte[] _data;

    public CustomBrowserFile(byte[] data, string name, string contentType)
    {
        _data = data;
        Name = name;
        ContentType = contentType;
        LastModified = DateTimeOffset.Now;
    }

    public string Name { get; }

    public DateTimeOffset LastModified { get; }

    public long Size => _data.Length;

    public string ContentType { get; }

    public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = default)
    {
        return new MemoryStream(_data);
    }
}