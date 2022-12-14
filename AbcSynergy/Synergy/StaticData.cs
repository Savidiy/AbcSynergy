using static AbcSynergy.Synergy.Class;
using static AbcSynergy.Synergy.Race;

namespace AbcSynergy.Synergy
{
    internal static class StaticData
    {
        internal static readonly List<HeroData> Heroes = new List<HeroData>()
        {
            new("Диего", Plant, Warrior),
            new("Шарисса", Reptile, Slayer),
            new("Конрадд", Undead, Warrior),
            new("Максимус", Tribe, Slayer),
            new("Рататоск", Beast, Thief),
            new("Тодд", Reptile, Guardian),
            new("Рагнар", Gnome, Guardian),
            new("Империон", Empire, Soaring),
            new("Нобель", Elemental, Soaring),
            new("Геката", Elf, Warlock),
            new("Хеллсинг", Drifter, Shooter),
            new("Минерва", Beast, Mage),
            new("Мэрилин", Empire, Mage),
            new("Титания", Empire, Warrior),
            new("До-Цо", Beast, Warrior),
            new("Норрис", Beast, Slayer),
            new("Юкки", Drifter, Thief),
            new("Ланселот", Empire, Guardian),
            new("Джакси", Tribe, Guardian),
            new("Эдвин", Drifter, Warlock),
            new("Рашмор", Elemental, Warlock),
            new("Ярра", Tribe, Shaman),
            new("Бруно", Plant, Shaman),
            new("Клаус", Gnome, Shooter),
            new("Вара", Demon, Shooter),
            new("Рамзес", Undead, Healer),
            new("Вивьен", Elf, Healer),
            new("Садако", Undead, Mage),
            new("Али", Tribe, Warrior),
            // new("Ангелия", Empire, Slayer),
            // new("Винчи", Gnome, Warrior),
            // new("Азраил", Undead, Slayer),
            // new("Альтаир", Reptile, Thief),
            // new("Гаргулли", Demon, Guardian),
            // // new("Гискара", Beast, Soaring),
            // new("Хазред", Undead, Warlock),
            // new("Ингрид", Empire, Shaman),
            // new("Бурбон", Beast, Shaman),
            // new("Гомер", Plant, Shooter),
            // new("Лариэль", Elf, Shooter),
            // new("Блэр", Drifter, Healer),
            // new("Имар", Elemental, Mage),
            // new("Мордред", Demon, Warrior),
            // new("Сильвия", Elf, Thief),
            // new("Лихо", Plant, Guardian),
            // new("Таурус", Beast, Guardian),
            // new("Смауг", Undead, Soaring),
            // new("Кормак", Gnome, Shaman),
            // new("Артемида", Empire, Shooter),
            // new("Тортулус", Reptile, Healer),
            // new("Нерон", Drifter, Mage),
            // new("Клио", Demon, Mage),
            // new("Ханзо", Drifter, Warrior),
            // new("Танатос", Demon, Slayer),
            // new("Мясник", Undead, Guardian),
            // new("Шен-Лунь", Reptile, Soaring),
            // new("Тенгу", Beast, Warlock),
            // new("Мария", Empire, Healer),
            // new("Зевс", Gnome, Mage),
        };

        internal static readonly List<ClassRule> ClassRules = new List<ClassRule>()
        {
            new(Warrior, 2, 10f, BuffType.OnlyMyType),
            new(Warrior, 4, 23f, BuffType.OnlyMyType),
            new(Slayer, 2, 10f, BuffType.OnlyMyType),
            new(Slayer, 4, 23f, BuffType.OnlyMyType),
            new(Thief, 2, 25f, BuffType.OnlyMyType),
            new(Thief, 4, 40f, BuffType.All),
            new(Guardian, 2, 10f, BuffType.All),
            new(Guardian, 4, 25f, BuffType.All),
            new(Soaring, 2, 20f, BuffType.All),
            new(Soaring, 3, 27f, BuffType.All),
            new(Soaring, 4, 40f, BuffType.All),
            new(Warlock, 2, 5f, BuffType.All),
            new(Warlock, 3, 7.5f, BuffType.All),
            new(Warlock, 4, 12f, BuffType.All),
            new(Shaman, 2, 12f, BuffType.All),
            new(Shaman, 3, 26f, BuffType.All),
            new(Shaman, 4, 52f, BuffType.All),
            new(Shooter, 2, 10f, BuffType.OnlyMyType),
            new(Shooter, 4, 23f, BuffType.OnlyMyType),
            new(Healer, 2, 11f, BuffType.All),
            new(Healer, 3, 16f, BuffType.All),
            new(Healer, 4, 22f, BuffType.All),
            new(Mage, 2, 9f, BuffType.CanHaveMana),
            new(Mage, 4, 23f, BuffType.CanHaveMana),
        };

        internal static readonly List<ClassRule> WithoutClassRules = new List<ClassRule>()
        {
            new(Class.Any, 0, 0, BuffType.All),
            new(Class.Any, 1, 0, BuffType.All),
            new(Class.Any, 2, 0, BuffType.All),
            new(Class.Any, 3, 0, BuffType.All),
            new(Class.Any, 4, 0, BuffType.All),
            new(Class.Any, 5, 0, BuffType.All),
            new(Class.Any, 6, 0, BuffType.All),
            new(Class.Any, 7, 0, BuffType.All),
            new(Class.Any, 8, 0, BuffType.All),
        };

        internal static readonly List<RaceRule> RaceRules = new List<RaceRule>()
        {
            new(Empire, 2, 8.8f, BuffType.All),
            new(Empire, 4, 22f, BuffType.All),
            new(Undead, 2, 12.5f, BuffType.All),
            new(Undead, 4, 30f, BuffType.All),
            new(Beast, 2, 7f, BuffType.All),
            new(Beast, 3, 14f, BuffType.All),
            new(Beast, 4, 26f, BuffType.All),
            new(Drifter, 2, 10f, BuffType.All),
            new(Drifter, 3, 15f, BuffType.All),
            new(Drifter, 4, 25f, BuffType.All),
            new(Tribe, 2, 40f, BuffType.MostDanger),
            new(Tribe, 3, 70f, BuffType.MostDanger),
            new(Plant, 2, 12f, BuffType.All),
            new(Plant, 3, 26f, BuffType.All),
            new(Reptile, 2, 20f, BuffType.OnlyMyType),
            new(Reptile, 4, 80f, BuffType.All),
            new(Gnome, 2, 14f, BuffType.OnlyMyType),
            new(Gnome, 4, 28f, BuffType.All),
            new(Elemental, 3, 25f, BuffType.OnlyMyType),
            new(Elf, 2, 16f, BuffType.OnlyMyType),
            new(Elf, 3, 26f, BuffType.All),
            new(Demon, 2, 11f, BuffType.All),
            new(Demon, 4, 22f, BuffType.All),
        };

        internal static readonly List<RaceRule> WithoutRaceRules = new List<RaceRule>()
        {
            new(Race.Any, 0, 0f, BuffType.All),
            new(Race.Any, 1, 0f, BuffType.All),
            new(Race.Any, 2, 0f, BuffType.All),
            new(Race.Any, 3, 0f, BuffType.All),
            new(Race.Any, 4, 0f, BuffType.All),
            new(Race.Any, 5, 0f, BuffType.All),
            new(Race.Any, 6, 0f, BuffType.All),
            new(Race.Any, 7, 0f, BuffType.All),
            new(Race.Any, 8, 0f, BuffType.All),
        };

        public static void UpdateHeroesMight(int seed)
        {
            var random = new Random(seed);
            foreach (HeroData heroData in StaticData.Heroes)
            {
                int might = random.Next(10, 1000);
                bool canHaveMana = random.Next(0, 2) == 0;
                int damagePerSecond = might * random.Next(7, 13);
                heroData.SetRandom(might, canHaveMana, damagePerSecond);
            }

            UpdateSortedLists();
        }

        private static void UpdateSortedLists()
        {
            MightyHeroes = new List<HeroData>(Heroes);
            MightyHeroes.Sort(MightComparison());

            foreach (Class type in Enum.GetValues(typeof(Class)))
            {
                List<HeroData> heroes = MightyHeroes.FindAll(a => a.Class == type);
                if (heroes.Count > 0)
                    MightyHeroesByClass[type] = heroes;
            }

            foreach (Race type in Enum.GetValues(typeof(Race)))
            {
                List<HeroData> heroes = MightyHeroes.FindAll(a => a.Race == type);
                if (heroes.Count > 0)
                    MightyHeroesByRace[type] = heroes;
            }
        }

        public static Dictionary<Class, List<HeroData>> MightyHeroesByClass { get; } = new();
        public static Dictionary<Race, List<HeroData>> MightyHeroesByRace { get; } = new();

        private static Comparison<HeroData> MightComparison()
        {
            return (a, b) => b.Might.CompareTo(a.Might);
        }

        public static List<HeroData> MightyHeroes { get; private set; }

        public static void PrintHeroes()
        {
            Console.WriteLine("\nHeroes:");
            foreach (HeroData heroData in Heroes)
            {
                Console.WriteLine(heroData);
            }
        }

        public static void PrintRules()
        {
            Console.WriteLine("\nClass rules:");
            foreach (ClassRule classRule in ClassRules)
                if (classRule.IsAvailableRule)
                    Console.WriteLine(classRule.ToLongString());

            Console.WriteLine("\nRace rules:");
            foreach (RaceRule raceRule in RaceRules)
                if (raceRule.IsAvailableRule)
                    Console.WriteLine(raceRule.ToLongString());
        }
    }
}