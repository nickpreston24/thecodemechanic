namespace thecodemechanic;

public static class HttpClientSetup
{
    /*
     * todo: keep experimenting with this so you can inject httpclient into console apps [like this](https://stackoverflow.com/questions/52622586/can-i-use-httpclientfactory-in-a-net-core-app-which-is-not-asp-net-core)_
     */
    public static IServiceCollection UseHttpClients(this ServiceCollection services)
    {
        return services.AddScoped<HttpClient>(x =>
            x.GetService<IHttpClientFactory>()
                .CreateClient("yourClientName"));
    }
}


internal class DummyJsonHttpClient : IDummyJsonHttpClient
{
}

internal interface IDummyJsonHttpClient
{
}
