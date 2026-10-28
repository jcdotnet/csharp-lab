using ContactsManager.Core.Entities;
using ContactsManager.Core.IdentityEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Entities
{
    // Inheriting from IdentityDbContext instead of DbContext
    // DbSets for identity entities are automatically created
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {

        public ApplicationDbContext(DbContextOptions options): base(options) { }     

        // DbSet properties: virtual to allow mocking classes to override them
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Person> People { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // optional but recommended
            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("People");

            // seeding the DB with initial data (countries)
            // https://learn.microsoft.com/en-us/ef/core/modeling/data-seeding
            //modelBuilder.Entity<Country>().HasData(
            //    new Country() { Id = Guid.Parse("6B820C12-FF44-4412-9E14-7941A869FB03"), Name = "USA" },
            //);
            string jsonCountries = File.ReadAllText("countries.json");
            List<Country>? countries = JsonSerializer.Deserialize<List<Country>>(jsonCountries)!;
            foreach (var country in countries)
            {
                modelBuilder.Entity<Country>().HasData(country);
            }

            // seeding the DB with initial data (people)
            // https://learn.microsoft.com/en-us/ef/core/modeling/data-seeding
            string jsonPeople = File.ReadAllText("people.json");
            List<Person>? people = JsonSerializer.Deserialize<List<Person>>(jsonPeople)!;
            foreach (var person in people)
            {
                modelBuilder.Entity<Person>().HasData(person);
            }
        }

        public List<Person> sp_GetPeople()
        {
            return People.FromSqlRaw("EXEC [dbo].[GetPeople]")
                //.Include(p => p.Country)
                .ToList();
        }

        public int sp_InsertPerson(Person person)
        {
            SqlParameter[] parameters =
            [
                new SqlParameter("@Id", person.Id),
                new SqlParameter("@Name", person.Name),
                new SqlParameter("@Email", (object?)person.Email ?? DBNull.Value),
                new SqlParameter("@Address", (object?)person.Address ?? DBNull.Value),
                new SqlParameter("@BirthDate", (object?)person.BirthDate ?? DBNull.Value),
                new SqlParameter("@Gender", (object?)person.Gender ?? DBNull.Value),
                new SqlParameter("@CountryId", (object?)person.CountryId ?? DBNull.Value),
                new SqlParameter("@ReceiveNewsletters", person.ReceiveNewsletters),
            ]; 
            return Database.ExecuteSqlRaw(
                "EXECUTE [dbo].[InsertPerson] @Id, @Name, @Email, @Address, @BirthDate, @Gender, @CountryId, @ReceiveNewsletters", 
                parameters
            );
        }
    }
}
