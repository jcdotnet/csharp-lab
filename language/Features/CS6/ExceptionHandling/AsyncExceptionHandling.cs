namespace CS6.ExceptionHandling;

/// <summary>
/// Await in catch/finally blocks
/// </summary>
public class AsyncExceptionHandling
{
    private static async Task HandleHttpRequestException()
    {
        using var client = new HttpClient();

        try
        {
            var textResponse = await client.GetStringAsync("https://example.net");
            Console.WriteLine(textResponse);
        }
        catch (HttpRequestException ex)
        {
            await LogErrorAsync(ex);
        }
        finally
        {
            await CleanupAsync();
        }
    }

    public static async Task RunExamples()
    {
        await HandleHttpRequestException();
    }

    #region private

    private static async Task LogErrorAsync(HttpRequestException ex)
    {
        //await File.AppendAllTextAsync("errors.log", $"{DateTime.Now}: {ex.Message}\n");
        //Console.WriteLine("Error logged.");

        await Console.Error.WriteLineAsync($"HTTP error: {ex.Message}");
    }

    private static async Task CleanupAsync()
    {
        await Task.Delay(100);
        Console.WriteLine("Cleanup completed.");
    }

    #endregion
}
