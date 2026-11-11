using CS15.UnionTypes;

namespace CS15.ClosedHierarchies;

/// <summary>
/// Closed hierarchies
/// https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-15#closed-hierarchies
/// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/closed
/// </summary>
public class ClosedHierarchies
{
    public static void RunExamples()
    {
        // After adding the closed C# 15 keyword to the Person class...

        // ...Person becomes a closed hierarchy with its derived classes declared in the same assembly
        Person john = new Student { Age = 15 };
        Person jane = new Teacher { Age = "20" };
        Person mary = new Visitor { Age = DateTime.Today };

        // ...Person is implicitly abstract, so we must instantiate one of its derived classes
        // Person person = new Person()

        // ... The compiler knows all the descendants of Person,
        // so we don't need a default case in the switch expression
        foreach (var person in new[] { john, jane, mary })
        {
            var johnPersonType = person switch
            {
                Student => "Student",
                Teacher => "Teacher",
                Visitor => "Visitor"
                // _ => // No compiler error
            };

            Console.WriteLine(johnPersonType);
        }
    }
}
