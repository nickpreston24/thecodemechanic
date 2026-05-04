
public static class HateoasExtensions
{
    public static IServiceCollection UseHateoas(this IServiceCollection services)
    {
        Hateoas.ConfigureServices(services);
        return services;
    }
}

public static class Hateoas
{

    public static void ConfigureServices(IServiceCollection services)
    {
        // services
        //     .AddMvc()
        //     .AddLinks(policy =>
        //     {
        //         policy
        //             .AddPolicy<BookViewModel>(model =>
        //             {
        //                 model
        //                     .AddSelf(m => m.Id, "This is a GET self link.")
        //                     .AddRoute(m => m.Id, BookController.UpdateBookById)
        //                     .AddRoute(m => m.Id, BookController.DeleteBookById)
        //                     .AddCustomPath(m => m.Id, "Edit", method: HttpMethods.Post, message: "Edits resource")
        //                     .AddCustomPath(m => $"/change/resource/state/?id={m.Id}", "ChangeResourceState", method: HttpMethods.Post, message: "Any operation in your resource.")
        //                     .AddExternalUri(m => m.Id, "https://my-domain.com/api/books/", "Custom Domain External Link")
        //                     .AddExternalUri(m => $"/search?q={m.Title}", "https://google.com", "Google Search External Links", message: "This will do a search on Google engine.");
        //             });
        //     });
    }

}
