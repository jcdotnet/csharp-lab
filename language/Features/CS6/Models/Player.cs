using System.ComponentModel;
using static System.Math;               // C#6: importing static members
using static CS6.Models.PlayerType;     // C#6: importing static members

namespace CS6.Models;

public class Player : INotifyPropertyChanged
{

    private string name = string.Empty;

    public int MyProperty { get; set; } // automatic property

    // C#6: immutable (getter only) properties 
    public int Armor { get; } = 100; // C#6: auto-property initializers

    public PlayerType Type { get; }

    public int Wear { get; private set; } = 15;

    public int Health { get; private set; } = 100;

    // C#6: expression-bodied members (methods and read-only properties)
    public int Defense => Wear >= Armor ? 0 : Armor - Wear;
    /*
    public int Defense
    {
        get { return Wear >= Armor ? 0 : Armor - Wear }
    }
    */
    public Weapon? Weapon { get; }

    public string Name
    {
        get { return name; }
        set
        {
            name = value;
            // OnPropertyChanged("Name");
            OnPropertyChanged(nameof(Name)); // C#6: nameOf keyword
        }
    }

    public Player(PlayerType type)
    {
        Type = type;
        Armor = 90; // ctor has preference in property initialization
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void Damage(int damage) => Health -= damage - Defense; // C#6: expression-bodied members
                                                                  //public void Damage(int damage)
                                                                  //{
                                                                  //    Health -= damage - Defense;
                                                                  //}

    public static bool ArmedByKnife(Player? c)
    {
        //return c != null && c.Weapon != null && c.Weapon.Name == "Knife";
        return c?.Weapon?.Name == "Knife"; // C#6: safe navigation operator
    }

    public void StaticImportExample()
    {
        switch (Type)
        {
            /*
            case Type.Elf: Console.WriteLine("Elf"); break; // before C#6
            case Type.Ork: Console.WriteLine("Elf"); break;
            */
            case Elf: Console.WriteLine("Elf"); break; // C#6: importing static members
            case Ork: Console.WriteLine("Elf"); break;
        }
        Console.WriteLine(Pow(10, 10)); // C#6: importing static members
    }

    public override string ToString()
    {
        // string oldWay = string.Format("Name: {0}, Health: {1}", Name, Health);
        return $"Name: {Name}, Health: {Health}"; // C#6: string interpolation
    }
}
