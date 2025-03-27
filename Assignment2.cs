using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

// Code by Maverick I. N.



public class Race
{
    private int Strength;
    private int Agility;
    private int Intelligence;
    private int InitialHP;
    private int MaxHP;
    public readonly string RaceName;
    private List<string> VictoryPhrase;

    protected Race(string name, int strength, int agility, int intelligence, int initialHP, int maxHP, List<string> victoryPhrase)   // Constructor
    {
        RaceName = name;
        Strength = strength;
        Agility = agility;
        Intelligence = intelligence;
        InitialHP = initialHP;
        VictoryPhrase = victoryPhrase;
        MaxHP = maxHP;
    }
    public int GetStrength() => Strength;
    public int GetAgility() => Agility;
    public int GetIntelligence() => Intelligence;
    public int GetInitialHP() => InitialHP;
    public int GetMaxHP() => MaxHP;
    public string VictoryPhraseGenerator()  // Picks a random victory phrase
    {
        Random rand = new Random();
        if(VictoryPhrase.Count == 0)
        {
            throw new Exception("No VictoryPhrase");
        }
        return VictoryPhrase[rand.Next(VictoryPhrase.Count)];
    }

    public static readonly Race Fairy = new Race("Fairy", 2, 5, 9, 20, 20, new List<string> { "The fairy does backflips on it", "The fairy laughs horrendously" });
    public static readonly Race Orc = new Race("Orc", 10, 3, 2, 40, 40, new List<string> { "The orc hits the griddy", "The orc releases a powerful roar" });
    public static readonly Race Elf = new Race("Elf", 4, 7, 6, 30, 30, new List<string> { "The elf snickers", "The elf bows to the audience" });
    public static readonly Race Ooze = new Race("Ooze", 4, 8, 1, 45, 45, new List<string> { "The ooze shapes itself into an L", "The ooze bounces up and down in excitement" });

}

public abstract class Classes
{
    static Random rand = new Random();
    private string name;
    private int hp;
    private double xp;
    private Race Race;
    protected Classes(string nameIn, Race race, double startXp)    // Constructor
    {
        this.name = nameIn;
        this.Race = race;
        xp = startXp;
        hp = Race.GetInitialHP();
    }

    public string GetName() => name;
    public int GetHP() => hp;
    protected int Attack()
    {
        int damage = (int)(AtkFormula(Race.GetStrength(), Race.GetAgility(), Race.GetIntelligence()) * RandomMultiplier());
        return damage;
    }
    protected int Defense()
    {
        int defense = (int)(DefFormula(Race.GetStrength(), Race.GetAgility(), Race.GetIntelligence()) * RandomMultiplier());
        return defense;
    }
    private double RandomMultiplier()
    {
        return rand.NextDouble() * xp;
    }
    public abstract int OnAttack(); // Is as abstract function as is needs will be changed by the children and doesnt need a base case
    public abstract int OnDefense();

    protected abstract double AtkFormula(int Str, int Agility, int Int);    

    protected abstract double DefFormula(int Str, int Agility, int Int);
    
    public string ToStrings()
    {
        return $"Name: {name}, Race: {Race.RaceName}, Class: {GetType().Name}, XP Level - {xp}/10, HP - {hp}/{Race.GetInitialHP()}";
    }
    public void AddXP(double addedXP)
    {
        xp = Math.Min(10, xp + addedXP);
    }

   

    public void Heal(int healedAmount)
    {
        int preHealHP = hp;
        hp = Math.Min(Race.GetInitialHP(), hp + healedAmount);
        Console.ForegroundColor = ConsoleColor.Green;
        if (healedAmount == 0)
        {
            Console.WriteLine($"{this.name} didnt heal after the fight");
        }
        else if(hp != Race.GetInitialHP() || hp == preHealHP + healedAmount)    
        {
            Console.WriteLine($"{this.name} healed {healedAmount} HP");
        }
        else // For edge cases where the healing would result in the combatant having more hp than their initial hp
        {
            Console.WriteLine($"{this.name} healed {Race.GetInitialHP() - preHealHP} HP");
        }
        Console.ForegroundColor = ConsoleColor.Gray;
    }
    public void TakeDamage(int damageAmount, Classes attacker)
    {
        if (damageAmount < 0) { damageAmount = 0; }
        hp = Math.Max(0, hp - damageAmount);
        Console.WriteLine($"{attacker.name} dealt {damageAmount} damage, {this.name} has {hp} hp left");
        if(hp == 0)
        {
            Console.WriteLine(attacker.Race.VictoryPhraseGenerator());
        }

    }
}

class Warrior : Classes
{
    public override int OnAttack()
    {
        Console.WriteLine("{0} swings a gigantic sword", GetName());
        return Attack();
    }
    public override int OnDefense()
    {
        Console.WriteLine("{0} blocks with the broad side of their massive sword ", GetName());
        return Defense();
    }
    public Warrior(string nameIn, Race race) : base(nameIn, race, 3.7) { }
    protected override double AtkFormula(int s, int a, int i)   // Overrides the abstract function found in "Classes"
    {
        return 0.6 * s + 0.3 * a + 0.1 * i;
    }
    protected override double DefFormula(int s, int a, int i)
    {
        return 0.3 * s + 0.3 * a + 0.2 * i;
    }
}
class Mage : Classes
{
    public override int OnAttack()
    {
        Console.WriteLine("{0} casts Fireball at third level", GetName());
        return Attack();
    }
    public override int OnDefense()
    {
        Console.WriteLine("{0} creates a magical barrier", GetName());
        return Defense();
    }
    public Mage(string nameIn, Race race) : base(nameIn, race, 2.75) { }
    protected override double AtkFormula(int s, int a, int i)
    {
        return 0.2 * s + 0.2 * a + i;
    }
    protected override double DefFormula(int s, int a, int i)
    {
        return 0.1 * s + 0.4 * a + 0.8 * i;
    }
}
class Archer : Classes
{
    public override int OnAttack()
    {
        Console.WriteLine("{0} flings an arrow", GetName());
        return Attack();
    }
    public override int OnDefense()
    {
        Console.WriteLine("{0} dodges swiftly", GetName());
        return Defense();
    }
    public Archer(string nameIn, Race race) : base(nameIn, race, 3.15) { }
    protected override double AtkFormula(int s, int a, int i)
    {
        return 0.3 * s + 0.7 * a + 0.2 * i;
    }
    protected override double DefFormula(int s, int a, int i)
    {
        return 0.2 * s + 0.7 * a + 0.4 * i;
    }


}
class Monk : Classes
{
    public override int OnAttack()
    {
        Console.WriteLine("{0} strikes with their palm", GetName());
        return Attack();
    }
    public override int OnDefense()
    {
        Console.WriteLine("{0} deflects the strike", GetName());
        return Defense();
    }
    public Monk(string nameIn, Race race) : base(nameIn, race, 3.0) { }
    protected override double AtkFormula(int s, int a, int i)
    {
        return 0.2 * s + 0.8 * a + 0.1 * i;
    }
    protected override double DefFormula(int s, int a, int i)
    {
        return 0.1 * s + 0.8 * a + 0.3 * i;
    }
}

public class Combat
{
    public Combat(List<Classes> combatants, double healingMult)
    {
        Random bracketMaker = new Random();
        int index1, index2;
        while (combatants.Count > 1)
        {
            index1 = bracketMaker.Next(combatants.Count);
            index2 = bracketMaker.Next(combatants.Count);
            while (index1 == index2)    // Makes sure that the fighters are not the same
            {
                index2 = bracketMaker.Next(combatants.Count);
            }
            Classes fighter1 = combatants[index1];
            Classes fighter2 = combatants[index2];
            Fight(fighter1, fighter2, combatants, healingMult);
        }
    }
    private void Fight(Classes fighter1, Classes fighter2, List<Classes> combatants, double healingMultiplier)
    {
        int preFightHP1 = fighter1.GetHP(), preFightHP2 = fighter2.GetHP();     // Hp at the start of the fight used to calculate the healing at the end
        int damage;
        Classes attacker, defender;


        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(fighter1.ToStrings());
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\t\tVERSUS!!!!!!!!!!");
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(fighter2.ToStrings());
        Console.ForegroundColor = ConsoleColor.Gray;


        for (int i = 1; i < 11; i++)
        {
            
            if(i % 2 == 1)
            {
                attacker = fighter1;
                defender = fighter2;
            }
            else
            {
                attacker = fighter2;
                defender = fighter1;
            }
            damage = attacker.OnAttack() - defender.OnDefense();
            defender.TakeDamage(damage, attacker);
            
            if(defender.GetHP() == 0)
            {
                combatants.Remove(defender);
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("The winner is...\n" + attacker.ToStrings());
                Console.ForegroundColor = ConsoleColor.Gray;

                attacker.AddXP(0.5);


                if(i % 2 == 1)
                {
                    attacker.Heal((int)(healingMultiplier * (preFightHP1 - attacker.GetHP())));
                }
                else
                {
                    attacker.Heal((int)(healingMultiplier * (preFightHP2 - attacker.GetHP())));
                }
                Console.WriteLine("\n");
                return;
            }
            Console.WriteLine();
        }
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("The fight enden in a draw");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(fighter1.ToStrings());
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(fighter2.ToStrings());

        fighter1.AddXP(0.25);
        fighter1.Heal((int)(healingMultiplier * (preFightHP1 - fighter1.GetHP())));

        fighter2.AddXP(0.25);
        fighter2.Heal((int)(healingMultiplier * (preFightHP2 - fighter2.GetHP())));

        Console.WriteLine("\n");
    }
}
public class Client()
{
    public static void Main()
    {
        List<Classes> combatants = new List<Classes>
        {
        new Warrior("Big Guy",  Race.Orc),
        new Warrior("Small Guy", Race.Elf),

        new Mage("The Wise", Race.Fairy),
        new Mage("The Enlghtened", Race.Orc),

        new Archer("Ranger Guy", Race.Elf),
        new Archer("The Pin Cushion", Race.Ooze),

        new Monk("Slimeuh", Race.Ooze),
        new Monk("The Mosquito", Race.Fairy)
        };

        double healingMultiplier = 0.4;     // The multiplier of the lost health that is restored at the end of a battle

        Combat gamerFight = new Combat(combatants, healingMultiplier);

        
    }
}

    