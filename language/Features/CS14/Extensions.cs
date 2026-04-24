using ClassLibrary;
using System.Numerics;

namespace CS14
{
    public static class Extensions
    {
        // extension members for IEnumerable<int>
        //extension (IEnumerable<int> source)

        // extension membersfor IEnumerable with generic constraints
        extension<T>(IEnumerable<T> source) where T : INumber<T>

        {
            // extension property
            public bool IsEmpty => !source.Any();

            // extension methods
            //public IEnumerable<int> WhereGreaterThan(int value) => source.Where(x => x > value);
            public IEnumerable<T> WhereGreaterThan(T value) => source.Where(x => x > value);

            public IEnumerable<T> ValuesEqualTo(T value) => source.Where(x => x.Equals(value));

            public IEnumerable<T> ValuesLessThan(T value) => source.Where(x => x < value);
        }

        extension(Employee employee)
        {
            public bool IsManager()
            {
                if (string.IsNullOrEmpty(employee.Job)) return false;

                string[] managerRoles = ["Manager", "Director", "Lead", "Chief"];
                return managerRoles.Any(role => employee.Job.Contains(role, StringComparison.OrdinalIgnoreCase));
            }
        }
            

        extension(Student student)
        {
            public double GetAverage(params int[] grades) => grades is { Length: > 0 } ? grades.Average() : 0.0;

            public string GetLocation() => 
                string.IsNullOrWhiteSpace(student.Location) ? "Remote" : $"Located in {student.Location}";
        }




  
    }
}
