namespace CS15.UnionTypes;

public record class Cat(string Name);
public record class Dog(string Name);
public record class Bird(string Name);

public union Pet(Cat, Dog, Bird);
public union SchoolMember(Student, Teacher);
public union AgeValue(int, string, DateTime);