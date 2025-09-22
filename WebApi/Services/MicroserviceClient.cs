namespace BackendExample.Services;

public class MicroserviceClient : IMicroserviceClient
{
    private readonly IHttpClientFactory _factory;

    public MicroserviceClient(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public async Task<HttpResponseMessage> ForwardToInventoryAsync(string path, HttpMethod method, HttpContent? content = null)
    {
        var client = _factory.CreateClient("inventory");
        var req = new HttpRequestMessage(method, path);
        if (content != null) req.Content = content;

        // Simple retry logic
        for (int i = 0; i < 3; i++)
        {
            var resp = await client.SendAsync(req);
            if (resp.IsSuccessStatusCode) return resp;
            await Task.Delay(200 * (i + 1));
        }
        // final attempt
        return await client.SendAsync(req);
    }
}