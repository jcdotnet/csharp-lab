namespace CS15.UnionTypes;

public closed class Person
{
    public AgeValue Age { get; set; }
}

public class Student : Person
{
}

public class Teacher : Person
{
}

public class Visitor : Person
{
}