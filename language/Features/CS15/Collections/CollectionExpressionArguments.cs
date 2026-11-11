namespace CS15.Collections;

/// <summary>
/// Collection expression arguments
/// https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-15#collection-expression-arguments
/// </summary>
public class CollectionExpressionArguments
{
    public static void RunExamples()
    {
        // This feature allows us to pass constructor arguments inside collection expressions

        // C# 14: We had to use the traditional collection initializer syntax to pass them
        {
            List<int> numbers = new(capacity: 10) { 1, 2, 3 };
            
            Console.WriteLine($"C# 14");
            Console.WriteLine($"Count: {numbers.Count}");
            Console.WriteLine($"Capacity: {numbers.Capacity}");
        }

        // C# 15: Now, we can using the with(...) syntax
        { 
            List<int> numbers = [with(capacity: 10), 1, 2, 3];

            Console.WriteLine($"C# 15");
            Console.WriteLine($"Count: {numbers.Count}");
            Console.WriteLine($"Capacity: {numbers.Capacity}");
        }
    }
}
