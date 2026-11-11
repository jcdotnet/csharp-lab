namespace CS15.UnionTypes;

/// <summary>
/// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/union
/// </summary>
public partial class UnionTypes
{
    public static void Primitives()
    {
        var john = new Student() { Age = 15 };
        var jane = new Student() { Age = "20" };
        var mary = new Student() { Age = DateTime.Today };

        Console.WriteLine(john.Age.Value.GetType());        // System.Int32
        Console.WriteLine(jane.Age.Value.GetType());        // System.String
        Console.WriteLine(mary.Age.Value.GetType());        // System.DateTime

        Console.WriteLine(IsAdult(john));   // False
        Console.WriteLine(IsAdult(jane));   // True
        Console.WriteLine(IsAdult(mary));   // False
    }

    public static void Records()
    {
        Pet cat = new Cat("Whiskers");
        Pet dog = new Dog("Buddy");
        Pet bird = new Bird("Tweety");

        Console.WriteLine(dog.Value.GetType());  // CS15.UnionTypes.Dog

        Console.WriteLine(GetPetName(cat));
        Console.WriteLine(GetPetName(dog));
        Console.WriteLine(GetPetName(bird));
    }

    private static void Classes()
    {
        SchoolMember student = new Student { Age = 20 };
        SchoolMember teacher = new Teacher { Age = 35 };

        // Compile time error:
        //SchoolMember visitor = new Visitor { Age = 25 };

        Console.WriteLine(student.Value.GetType()); // CS15.UnionTypes.Student
        Console.WriteLine(teacher.Value.GetType()); // CS15.UnionTypes.Teacher
    }

    public static void RunExamples()
    {
        Primitives();
        Records();
        Classes();
    }

    #region private
    private static bool IsAdult(Person person)
    {
        return person.Age.Value switch
        {
            int value => value >= 18,
            string value => int.TryParse(value, out int parsedAge) && parsedAge >= 18,
            DateTime value => value.Date <= DateTime.Today.AddYears(-18),

            // No default case is needed: the compiler knows all the union cases and they are handled above
            _ => false, 
        };
    }

    private static string GetPetName(Pet pet)
    {
        return pet switch
        {
            Dog d => d.Name,
            Cat c => c.Name,
            Bird b => b.Name
        };
    }

    #endregion
}