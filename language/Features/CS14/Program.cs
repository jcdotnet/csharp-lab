using ClassLibrary;
using CS14;

// https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-14
Console.WriteLine("C#14 features");

// feat :: extension members
var student = new Student("John Doe", 20);

int[] grades = [80, 20, 50, 40, 70];

if (!grades.IsEmpty) // Extension property
{
    // Extension methods
    Console.WriteLine($"Average grade {student.GetAverage(grades)}");
    Console.WriteLine($"Passing grades: {string.Join(", ", grades.WhereGreaterThan(40))}");
}

var employee = new Employee(1, "Alice Doe") { Job = "Senior Developer" };
if (employee.IsManager()) // Extension method
{
    Console.WriteLine($"{employee.Name} is a manager.");
}

// feat :: null-conditional assignment
var alice = new Employee(1, "Alice Doe");
Employee? noEmployee = null;

alice?.Job = "Senior Developer";    // This is valid in C# 14
noEmployee?.Job = "Developer";      // This is valid in C# 14

Console.WriteLine(noEmployee?.Job ?? "No employee");

// feat :: field backed properties
var product = new Product(Guid.NewGuid())
{
    ProductName = " Macbook Pro  ",
    QuantityInStock = null
};
Console.WriteLine($"---> {product.ProductName} <---");
Console.WriteLine($"Quantity in stock: {product.QuantityInStock}");