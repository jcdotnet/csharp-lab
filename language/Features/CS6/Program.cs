// https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-version-history#c-version-60
// https://learn.microsoft.com/en-us/shows/connecton-demand/211
// https://learn.microsoft.com/en-us/archive/msdn-magazine/2014/october/csharp-the-new-and-improved-csharp-6-0

using ClassLibrary;
using CS6.ExceptionHandling;
using CS6.Models;
using CS6.NullConditionalOperators;

Console.WriteLine("C#6 features");

Student student = new Student();
Console.WriteLine(student.Location); // Málaga // auto-property initializer

Player player = new Player(CS6.Models.PlayerType.Elf); // static imports
player.Name = "My Player"; // nameof operator
Console.WriteLine(player.Armor); // immutable (read-only) properties
Console.WriteLine(player.Defense); // expression bodied members
Console.WriteLine(player); // string interpolation 

player.StaticImportExample(); // static imports

// Null-conditional operators
Console.WriteLine("Null-Conditional Operators");
NullConditionalOperators.RunExamples();

// expression bodied members (functions with single body statements or return value)
// C#7 expands this syntax to cover constructors, finalizers, and property accessors
// https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/expression-bodied-members
Console.WriteLine(student.GetNameLength());

// Exception filters
await ExceptionFilters.RunExamples();