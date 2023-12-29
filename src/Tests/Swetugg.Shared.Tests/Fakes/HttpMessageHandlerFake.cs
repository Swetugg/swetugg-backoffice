namespace Swetugg.Shared.Tests.Fakes;
internal class HttpMessageHandlerFake : HttpMessageHandler
{
    Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _requestAction;

    public HttpMessageHandlerFake(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> requestAction)
    {
        _requestAction = requestAction;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if( _requestAction is null)
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);

        return await _requestAction.Invoke(request, cancellationToken);
    }
}
