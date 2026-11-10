using CS6.Models;

namespace CS6.NullConditionalOperators;

/// <summary>
/// Null-conditional operators
/// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/member-access-operators#null-conditional-operators--and-
/// 
/// Note: Nullable Reference Types (C# 8.0) are used here to avoid compiler warnings.
/// </summary>
public class NullConditionalOperators
{
    /// <summary>
    /// Demonstrates null-conditional member access (?.).
    /// AKA null propagation operator
    /// </summary>
    public static void NullConditionalMemberAccess()
    {
        // Example with Weapon = null
        Player? player = new Player(PlayerType.Elf);

        Console.WriteLine(Player.ArmedByKnife(player));     // False

        // Example with Player = null
        Player? nullPlayer = null;
        Console.WriteLine(Player.ArmedByKnife(nullPlayer)); // False
    }

    /// <summary>
    /// Demonstrates null-conditional element access (?[]).
    /// </summary>
    public static void NullConditionalElementAccess()
    {
        int[]? numbers = { 10, 20, 30 };

        Console.WriteLine(numbers?[0]);     // 10

        int[]? nullNumbers = null;

        Console.WriteLine(nullNumbers?[0]); // null
    }

    public static void RunExamples()
    {
        // Used nullable reference types to ignore C #8+ warnings 
        NullConditionalMemberAccess();
        NullConditionalElementAccess();
    }
   
}
