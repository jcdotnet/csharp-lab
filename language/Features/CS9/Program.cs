// https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-version-history#c-version-9
// https://learn.microsoft.com/en-us/shows/on-dotnet/c-9-language-features

// https://devblogs.microsoft.com/dotnet/c-9-0-on-the-record/
using ClassLibrary;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

Console.WriteLine("C#9 features");

// feat: top level statemens
// https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/program-structure/top-level-statements
// See https://aka.ms/new-console-template for more information
// we are use them in this project
// command-line arguments with top-level statements
if (args.Length == 1)
{
    Console.WriteLine();
}

// feat: module initializers
// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/attributes/general#moduleinitializer-attribute
// class ClassName {
// [ModuleInitializer]
// internal static void MethodName() {
// // this method will run before Main()
// }
// }

// feat: target-typed new expressions
// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-9.0/target-typed-new
// ClassName variableName = new(); // equivalent to new ClassName()
Employee employee = new();
//var employee = new(); // cannot be used with the var keyword (neither with foreach loops nor using blocks) 

// improv: Pattern matching enhancement (relational and logical patterns)
// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/patterns#relational-patterns
// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/patterns#logical-patterns
Person person = new("John", "Doe", 20);
string result = person switch
{
    Person p when p.Age is < 13 => $"{p} is a child",
    Person p when p.Age is >= 13 and < 18 => $"{p} is a teenager",
    Person p when p.Age is > 18 => $"{p} is an adult",
    _ => $"{person} is a person"
};
Console.WriteLine(result);

// feat: init-only setters
// DataType PropertyName { get; init; } // will be initialize inline, in obj initializer or in the ctor only

// feat: records (allows to create a immutable reference types in a shortcut syntax) 
// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record

PersonRecord myPersonRecord = new("John", 40, new AddressRecord("Málaga"));
//myPersonRecord.Name = "Jane"; // records are immutable by default (Name is a init-only setter/property)
myPersonRecord.Address.Country = "Spain";

PersonRecord anotherPersonRecord = new("John", 40, new AddressRecord("Málaga") { Country = "Spain" });
Console.WriteLine(myPersonRecord == anotherPersonRecord); // record = value-bassed equality

anotherPersonRecord = myPersonRecord; // reference copy 
myPersonRecord.Address.Country = "USA";
Console.WriteLine(anotherPersonRecord);
// output: PersonRecord { Name = John, Age = 40, Address = AddressRecord { City = Málaga, Country = USA } }

anotherPersonRecord = myPersonRecord with { }; // shallow copy
// myPersonRecord.Age = 50; // in case age was mutable this change wouldn't be reflected on anotherPersonRecord
myPersonRecord.Address.Country = "UK";
Console.WriteLine(anotherPersonRecord);
// output: PersonRecord { Name = John, Age = 40, Address = AddressRecord { City = Málaga, Country = UK }

// deep copy
anotherPersonRecord = myPersonRecord with { Age = 30, Address = new AddressRecord("LA") { Country = "USA" } };
myPersonRecord.Address.Country = "Spain";
Console.WriteLine(anotherPersonRecord);
// output: PersonRecord { Name = John, Age = 40, Address = AddressRecord { City = LA, Country = USA }

// record deconstruct
var (variable1, variable2, variable3) = myPersonRecord;
Console.WriteLine(variable1); // John
Console.WriteLine(variable2); // 40
Console.WriteLine(variable3); // AddressRecord { City = Málaga, Country = Spain }

// improv: partial methods return type
// C# 9: a partial method can have any return type (not only void)
// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-9.0/extending-partial-methods

// improv: static anonymous functions
// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-9.0/static-anonymous-functions
#region type declarations

public record PersonRecord(string? Name, int? Age, AddressRecord? Address);

public record AddressRecord(string City) 
{
    public string? Country {  get; set; } // mutable property // we don't want to use them in records
};

public record EmployeeRecord(string? Name, int? Age, AddressRecord? Address, double Salary) :
    PersonRecord(Name, Age, Address);
#endregion
