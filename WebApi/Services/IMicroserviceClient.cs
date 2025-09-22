namespace BackendExample.Services;

public interface IMicroserviceClient
{
    Task<HttpResponseMessage> ForwardToInventoryAsync(string path, HttpMethod method, HttpContent? content = null);
}