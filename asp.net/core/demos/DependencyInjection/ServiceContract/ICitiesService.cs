namespace ServiceContract // can also be called interfaces, etc
{
    // Dependency Inversion Principle
    // higher-level modules (clients) SHOULD NOT depend on low-level modules (dependencies)
    // both should depend on abstractions (interfaces / abstract classes)
    public interface ICitiesService
    {
        List<string> GetCities();
    }
}
