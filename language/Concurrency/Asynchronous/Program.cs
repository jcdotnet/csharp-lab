// https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using Asyncrhonous;

Console.WriteLine("Displaying file content");

await FileIO.Demo();
Console.WriteLine();

Console.WriteLine("\nDisplaying website contents (with cancellation support)");

using var cts1 = new CancellationTokenSource();

cts1.CancelAfter(TimeSpan.FromSeconds(5));

var httpServerAsync = new HttpServerAsync("https://example.com");
Console.WriteLine($"Downloading content from {httpServerAsync.Url}");

try
{
    // Waits for the URL content without blocking the thread
    string urlContent = await httpServerAsync.GetUrlContentAsync(cts1.Token, 10);

    Console.WriteLine(urlContent);

}
catch (OperationCanceledException)
{
    Console.WriteLine($"The request to {httpServerAsync.Url} was cancelled.");
}

using var cts2 = new CancellationTokenSource();

var completedUrl = string.Empty;
var content = string.Empty;
var tasks = new Dictionary<Task<string>, string>();

string[] urls =
[
    "https://example.com",
    "https://www.microsoft.com",
    "https://www.github.com"
];

try
{
    foreach (var url in urls)
    {
        httpServerAsync = new HttpServerAsync(url);

        Console.WriteLine($"Downloading content from {url}");
        tasks.Add(httpServerAsync.GetUrlContentAsync(cts2.Token), url);
    }
    Task<string> completedTask = await Task.WhenAny(tasks.Keys);
    
    completedUrl = tasks[completedTask];
    content = await completedTask;
    Console.WriteLine(content);

    cts2.Cancel();

    // Wait for the remaining tasks (not to finish in this case, but to respond to the cancellation request)
    await Task.WhenAll(tasks.Keys);
}
catch (OperationCanceledException)
{
    foreach (var task in tasks)
    {
        if (task.Key.IsCanceled)
        {
            Console.Write($"Request to {task.Value} cancelled");
            if (!string.IsNullOrEmpty(completedUrl))
            {
                Console.WriteLine($": {completedUrl} has already been downloaded.");
            }
        }
    }
}