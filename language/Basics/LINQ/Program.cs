// LINQ (Language INtegrated Query)
// Queries against any type of data source:
// Objects (LINQ to Collections), SQL databases (LINQ to SQL), XML files (LINQ to XML)
// Entity Framework DbSet (LINQ to Entities), ADO.NET DataSet (LINQ to DataSet) 
// https://learn.microsoft.com/en-us/dotnet/csharp/linq/
using Classes;

Console.WriteLine("LINQ");

/**
 * Numbers
 */
List<int> list = [23, 75, 30, 34, 98, 27, 0];

var filteredNumbers = list.Where(number => number > 50); // IEnumerable<int>
Console.WriteLine("Numbers greater than 50:");
foreach (int number in filteredNumbers) Console.WriteLine(number);

/**
 * Employess
 */
List<Employee> employees =
[
    new Employee() { Id = 100, Name = "John", Position = "Developer", Location = "NYC" },
    new Employee() { Id = 101, Name = "Jane", Position = "Designer", Location = "LA" },
    new Employee() { Id = 102, Name = "Mike", Position = "Developer", Location = "NYC" },
    new Employee() { Id = 103, Name = "Alice", Position = "Designer", Location = "NYC" },
    new Employee() { Id = 104, Name = "David", Position = "Developer", Location = "LA" },
    new Employee() { Id = 200, Name = "Antonio", Position = "Developer", Location = "Málaga" },
    new Employee() { Id = 201, Name = "Ana", Position = "Designer", Location = "Madrid" },
    new Employee() { Id = 202, Name = "Laura", Position = "Designer", Location = "Madrid" },
    new Employee() { Id = 400, Name = "Peter", Position = "Developer", Location = "London" },
];

var developers = employees.Where(emp => emp.Position == "Developer"); // IEnumerable<Employee>
Console.WriteLine("Developers:");
foreach (var dev in developers) Console.WriteLine(dev);

var orderBy = employees.OrderBy(emp => emp.Name); // returns IOrderedEnumerable<Employee>
var orderByDescending = employees.OrderByDescending(emp => emp.Name);
var orderByThenBy = employees.OrderBy(emp => emp.Name).ThenBy(emp => emp.Id);
Console.WriteLine("Ordered by Name. Then By Id:");
foreach (var employee in orderByThenBy) Console.WriteLine(employee);

employees.Add(new Employee() { Id = 110, Name = "Laura", Position = "Designer", Location = "LA" });
orderByThenBy = employees.OrderBy(emp => emp.Name).ThenBy(emp => emp.Id);
Console.WriteLine("Ordered by Name. Then By Id:");
foreach (var emp in orderByThenBy) Console.WriteLine(emp); // first Laura LA then Laura Madrid

var firstMadrid = employees.First(emp => emp.Location == "Madrid"); // Employee // 201 Ana
// Same with where:
// var madrid = employees.Where(emp => emp.Location == "Madrid").ToList();
// var firstMadrid = employees[0];

//var firstBcn = employees.First(emp => emp.Location == "Barcelona"); // Employee // InvalidOperationException
var firstBcn = employees.FirstOrDefault(emp => emp.Location == "Barcelona"); // Employee // null
// to handle null here (firstBcn)
var lastMadrid = employees.Last(emp => emp.Location == "Madrid"); // Employee // 202 Laura
// LastOrDefault

var developer = employees.Where(emp => emp.Position == "Developer").ElementAt(2); // Employee // 102 Mike
// ElementAtOrDefault

// var singleDev = employees.Where(emp => emp.Position == "Developer").Single(); // System.InvalidOperationException
var singleDev = employees.Where(emp => emp.Location == "Málaga").Single(); // Employee // 200 Antonio 
// SingleOrDefault

// projection (select)
var employesIds = employees.Select(emp => emp.Id); // IEnumerable<Int>

/**
 * Students
 */
List<Student> students =
[
    new Student("John", 20),
    new Student("Jane", 17),
    new Student("Daniel", 21),
];

// returns students name
var studentNames = students.Select(student => student.Name);
// returns an anonymous  object with name and age 
var studentProfiles = students.Select(student => new
{
    student.Name,
    IsAdult = student.Age >= 18,
    FormatLabel = $"Student: {student.Name} ({student.Age} year-old)"
});
foreach (var profile in studentProfiles)
{
    Console.WriteLine(profile.FormatLabel);
    Console.WriteLine();
}

//var adultNames = students.Where(student => student.Age > 18).Select(student => student.Name);
var adultNames = studentProfiles.Where(p => p.IsAdult).Select(p => p.Name);

// More on LINQ (aggregation methods): Min, Max, Count, Sum, Average
int ageSum = students.Sum(sum => sum.Age);
double ageMean = students.Average(sum => sum.Age);
int count = students.Count();