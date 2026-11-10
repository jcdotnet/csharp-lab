using System.Net;

namespace CS6.ExceptionHandling;

/// <summary>
/// Exception filters
/// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/when
/// </summary>
public class ExceptionFilters
{
    /// <summary>
    /// Demonstrates using an exception filter (when) with FormatException.
    /// </summary>
    public static void HandleFormatException()
    {
        Console.Write("Enter an integer: ");
        var userInput = Console.ReadLine() ?? string.Empty;

        try
        {
            int number = int.Parse(userInput);
            Console.WriteLine($"Integer: {number}");
        }
        catch (FormatException) when (userInput.Length == 0)
        {
            Console.WriteLine("Input cannot be empty. Please enter an integer.");
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"Format error: {ex.Message}");
        }
    }

    /// <summary>
    /// Demonstrates using exception filters with HttpRequestException.
    /// </summary>
    public static async Task HandleHttpRequestException()
    {
        // C# 6 uses traditional using statements instead of the C# 8 using declaration used here for readability
        using var client = new HttpClient();
        try
        {
            var textResponse = await client.GetStringAsync("https://example.net");
            // Uncomment to trigger an HttpRequestException with a 404 status code
            // textResponse = await client.GetStringAsync("https://example.net/1");
            Console.WriteLine(textResponse);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            Console.WriteLine("Resource not found (404).");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.InternalServerError)
        {
            Console.WriteLine("Internal server error (500).");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP error: {ex.Message}");
        }
    }

    public static async Task RunExamples()
    {
        HandleFormatException();
        await HandleHttpRequestException();
    }

}
