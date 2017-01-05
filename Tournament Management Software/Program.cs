using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tournament_Management_Software.DataObjects;
using Tournament_Management_Software.Data_Access_Layer;

namespace Tournament_Management_Software
{
    class Program
    {
        //Manual Entering
        public static void AddAgeClasses(MatchContext context)
        {
            Console.WriteLine("How many Age Classes? ");
            int amount = Int32.Parse(Console.ReadLine());
            List<AgeClass> ageClasses = new List<AgeClass>();

            for (int i = 0; i < amount; i++)
            {
                Console.WriteLine("Give the years FROM ... TO ... followed by ENTER");
                int minYear = Int32.Parse(Console.ReadLine());
                int maxYear = Int32.Parse(Console.ReadLine());                
                ageClasses.Add(new AgeClass() {MaxYear = maxYear, MinYear = minYear});
            }

            context.AgeClasses.AddRange(ageClasses);
            context.SaveChanges();
        }
        public static void AddWeightClasses(MatchContext context)
        {
            Console.WriteLine("Please give the weight limits separated by a ',' exit by entering");
            string input = Console.ReadLine();
            Console.WriteLine("{0} - Are you sure?", input);
            var weightLimits = input.Split(',');
            List<WeightClass> classes = new List<WeightClass>();
            foreach (var r in weightLimits)
            {
                Console.WriteLine(double.Parse(r));
                classes.Add(new WeightClass(double.Parse(r))); 
            }
            context.WeightClasses.AddRange(classes);
            context.SaveChanges();

            foreach (var c in classes)
            {
                Console.WriteLine("MIN = {0}  MAX = {1},  _p = {2}", c.MinimumWeight, c.MaximumWeight, c.getPrevious());
            }


        }

        public static int GetAgeId(MatchContext context, int birthYear)
        {
            var ages = context.AgeClasses.ToList();
            int index = -1;
            foreach (var ac in ages)
            {
                if (ac.MinYear <= birthYear && ac.MaxYear >= birthYear)
                {
                    index = ac.Id;
                    return index;
                }
            }
           // int index = ages.FindIndex(ac => (ac.MinYear <= birthYear && ac.MaxYear >= birthYear));
            return index;
        }
        public static void GroupContestantsByAge(MatchContext context)
        {
            var contestants = context.Contestants.ToList();
            foreach (var contestant in contestants)
            {
                if(contestant != null)
                    contestant.AgeId = GetAgeId(context,contestant.DateOfBirth.Year);
            }
        }
        public static void GroupContestantsByWeight(MatchContext context)
        {
            var contestants = context.Contestants.ToList();
            var weightClasses = context.WeightClasses.ToList();
            foreach (var c in contestants)
            {
                WeightClass myClass = weightClasses.Find(r => r.MaximumWeight >= c.Weight);
                Console.WriteLine("Contestant {0} Weight: {1} \n CLASS: {2}", c.FirstName + c.LastName, c.Weight, myClass.ClassName);
                c.WeightId = myClass.Id;
            }

        }
        public static void CreateMatchesAllToAllForWeightClass(MatchContext context, int id)
        {
            var matches = context.Matches.ToList();
            Random rand = new Random();
            var contestants = context.Contestants.ToList();
            List<Contestant> contestantsInWeightClass = (contestants.Where(c => c.WeightId == id)).ToList();
            if (contestantsInWeightClass.Count()%2 != 0)
            {
                Console.WriteLine("Nie parzysta ilość uczestników. Brak możliwości turnieju 'Każdy z każdym'");
                return;
            }
            while(contestantsInWeightClass.Any())
            {
                //bierze pierwszy element z listy
                var cont = contestantsInWeightClass.FirstOrDefault();
                //wyszukuje randomowo przeciwnika z pozostałych na liscie
                var r = rand.Next(contestantsInWeightClass.Count());
                //dopóki wylosował się ten sam element losuj ponownie
                while (contestantsInWeightClass.ElementAt(r).Equals(cont))
                {
                    r = rand.Next(contestantsInWeightClass.Count());
                }
                // tworzy nowy match i laczy w nim przeciwnikow
                Match match = new Match(cont,contestantsInWeightClass.ElementAt(r));
                matches.Add(match);
                //sciagasz z listy do przydzielenia
                contestantsInWeightClass.RemoveAt(r);
                contestantsInWeightClass.Remove(cont);
            }
            Console.WriteLine("TEST MATCH ALL2ALL: contestants not in match: " + contestantsInWeightClass.Count);
            context.Matches.AddRange(matches);
            context.SaveChanges();
        }

        public static void ShowAgeClasses(MatchContext context)
        {
            var ageClasses = context.AgeClasses.ToList();
            Console.WriteLine("You have {0} age classes...", ageClasses.Count);
            foreach (var ac in ageClasses)
            {
                Console.WriteLine("AgeClass ID = {0}, {1}", ac.Id, ac.MinYear, ac.ClassName);
            }
        }
        public static void ShowWeightClasses(MatchContext context)
        {
            var weightClasses = context.WeightClasses.ToList();
            Console.WriteLine("You have {0} age classes...", weightClasses.Count);
            foreach (var ac in weightClasses)
            {
                Console.WriteLine("WeightClass ID = {0}, {1}", ac.Id, ac.ClassName);
            }
        }
        public static void ShowContestants(MatchContext context)
        {
            var contestants = context.Contestants.ToList();
            foreach (var c in contestants)
            {
                if(c.AgeId == -1) Console.WriteLine("Contestant nr {0}, Name: {1}, AgeClass = {2}, Year of Birth = {3}, WeightClass = {4}", c.Id, c.FirstName + " " + c.LastName, "Not valid!!!", c.DateOfBirth.Year, c.WeightId);
                else Console.WriteLine("Contestant nr {0}, Name: {1}, AgeClass = {2}, Year of Birth = {3}, WeightClass = {4}", c.Id, c.FirstName+" "+c.LastName, c.AgeId, c.DateOfBirth.Year, c.WeightId);
            }
        }

        public static void ShowMatches(MatchContext context)
        {
            var matches = context.Matches.ToList();
            foreach (var match in matches)
            {
                if (match.Contestants.First() != null)
                {
                    string name = match.Contestants.FirstOrDefault().FirstName +
                                  match.Contestants.FirstOrDefault().LastName;
                    Console.WriteLine("Match nr {0}   :   {1} VS {2}", match.Id, name, match.Contestants.ElementAt(1));
                }
                else
                {
                    Console.WriteLine("ERROR EXCEPTION");
                }
                
            }
        }

        static int MainMenuView(MatchContext context)
        {
            Console.Clear();
            Console.WriteLine("1. Add Data To Database");
            Console.WriteLine("2. Display Data From Database");
            Console.WriteLine("3. Group Contestants");
            Console.WriteLine("4. Make Matches");
            Console.WriteLine("-----------------\n5. Exit");
            int input = Int32.Parse(Console.ReadLine());
            return input;
        }
        static int DisplayMenuView(MatchContext context)
        {
            Console.Clear();
            Console.WriteLine("1. Display Contestant Data");
            Console.WriteLine("2. Display Age Groups");
            Console.WriteLine("3. Display Weight Groups");
            Console.WriteLine("4. Display ALL Data");
            Console.WriteLine("5. Display Matches");
            Console.WriteLine("-----------------\n6. Exit");
            int input = Int32.Parse(Console.ReadLine());
            return input;
        }
        static int GroupByMenuView(MatchContext context)
        {
            Console.Clear();
            Console.WriteLine("Group By Menu\n-----------------");
            Console.WriteLine("1. Group contestants by AGE");
            Console.WriteLine("2. Group contestants by WEIGHT");
            Console.WriteLine("5. Back to main menu");
            int input = Int32.Parse(Console.ReadLine());
            return input;
        }
        static void AddContestantView(MatchContext context)
        {
            Console.Clear();
            Console.WriteLine("NEW CONTESTANT\n-------------------------");
            Console.WriteLine("First Name = ");
            string firstName = Console.ReadLine();
            Console.WriteLine("Family Name = ");
            string lastName = Console.ReadLine();
            Console.WriteLine("Date of birth = ");
            string date = Console.ReadLine();
            DateTime dateTime = DateTime.Parse(date);
            Console.WriteLine("Current weight = ");
            //string sWeight = Console.ReadLine();
            double weight = Double.Parse(Console.ReadLine());
            
            Contestant newContestant = new Contestant(dateTime);
            newContestant.Weight = weight;
            newContestant.FirstName = firstName;
            newContestant.LastName = lastName;
            context.Contestants.Add(newContestant);
            context.SaveChanges();
            Console.WriteLine("New Contestant {0}, Aged {1}, Weighing {2} kg has been added to the database!", newContestant.FirstName+" "+lastName, newContestant.Age, newContestant.Weight);
        }
        static void AddAgeClassView(MatchContext context)
        {
            Console.Clear();
            Console.WriteLine("NEW AGE CLASS\n-------------------------");
            Console.WriteLine("Minimum Year = ");
            int min = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Maximum Year = ");
            int max = Int32.Parse(Console.ReadLine());

            AgeClass newAgeClass = new AgeClass(min, max);
            context.AgeClasses.Add(newAgeClass);
            context.SaveChanges();
            Console.WriteLine("New Age Class {0} added to database!", newAgeClass.ClassName);
        }
        static void AddWeightClassView(MatchContext context)
        {
            Console.Clear();
            Console.WriteLine("NEW WEIGHT CLASS\n-------------------------");
            Console.WriteLine("Maximum Weight = ");
            int max = Int32.Parse(Console.ReadLine());

            WeightClass newWeightClass = new WeightClass(max);
            context.WeightClasses.Add(newWeightClass);
            context.SaveChanges();
            Console.WriteLine("New Weight Class {0} added to database!", newWeightClass.ClassName);
        }
        static int AddMenuView(MatchContext context)
        {
            Console.Clear();
            Console.WriteLine("Adding Data Menu\n---------------------");
            Console.WriteLine("1. Add Contestants");
            Console.WriteLine("2. Add Age Classes");
            Console.WriteLine("3. Add Weight Classes");
            Console.WriteLine("5. Exit");
            int input = Int32.Parse(Console.ReadLine());
            return input;
        }
        static int MatchMenuView(MatchContext context)
        {
            Console.Clear();
            Console.WriteLine("MATCH MENU\n---------------------\n");
            Console.WriteLine("1. Set Match ALL TO ALL");
            Console.WriteLine("2. Set match LADDER");
            Console.WriteLine("\n-----------------\n5. Exit\n----------------");
            int input = Int32.Parse(Console.ReadLine());
            return input;
        }

        static int AddMenuController(MatchContext context)
        {
            int choice = AddMenuView(context);
            switch (choice)
            {
                case 1:
                {
                    Console.Clear();
                    AddContestantView(context);
                        Console.ReadKey();
                        return 0;
                }
                case 2:
                {
                    Console.Clear();
                    AddAgeClassView(context);
                        Console.ReadKey();
                        return 0;
                }
                case 3:
                {
                    Console.Clear();
                    AddWeightClassView(context);
                        Console.ReadKey();
                        return 0;
                }
                case 5:
                    return 5;
                default:
                    return 0;
            }
        }
        static int GroupByMenuController(MatchContext context)
        {
            int choice = GroupByMenuView(context);
            switch (choice)
            {
                case 1:
                {
                    Console.WriteLine("Grouping contestants by their age...");
                    GroupContestantsByAge(context);
                    Console.WriteLine("Grouping successfully finished!");
                    Console.ReadKey();
                    return 0;
                }
                case 2:
                {
                    Console.Clear();
                    Console.WriteLine("Grouping contestants by their weight...");
                    GroupContestantsByWeight(context);
                    Console.WriteLine("Grouping successfully finished!");
                        Console.ReadKey();
                        return 0;
                }
                case 5:
                    return 5;
                default:
                    return 0;
            }
        }
        static int DisplayMenuController(MatchContext context)
        {
            int choice = DisplayMenuView(context);
            switch (choice)
            {
                //return 0 jak ma wrocic do main menu, 5 exit
                case 1:
                {
                    Console.Clear();
                    ShowContestants(context);
                    Console.ReadKey();
                    return 0;
                }
                case 2:
                {
                    Console.Clear();
                    ShowAgeClasses(context);
                        Console.ReadKey();
                        return 0;
                }
                case 3:
                {
                    Console.Clear();
                    ShowWeightClasses(context);
                        Console.ReadKey();
                        return 0;
                }
                case 4:
                {
                    Console.Clear();
                    Console.WriteLine("\n Contestants:\n");
                    ShowContestants(context);
                    Console.WriteLine("\n Age Classes:\n");
                    ShowAgeClasses(context);
                    Console.WriteLine("\n Weight Classes:\n");
                    ShowWeightClasses(context);
                    Console.WriteLine("\n Upcoming matches: \n");
                    ShowMatches(context);
                    Console.ReadKey();
                        return 0;
                }
                case 5:
                {
                    Console.WriteLine("\n Upcoming matches: \n");
                    ShowMatches(context);
                    Console.ReadKey();
                    return 0;
                }
                case 6:
                {
                    Console.Clear();
                    Console.WriteLine("Exiting....");
                        Console.ReadKey();
                        return 5;
                }
                default:
                {
                    Console.Clear();
                    Console.WriteLine("Enter a different value! \n Returning to main menu....");
                        Console.ReadKey();
                        return 0;
                }
            }
        }
        static int MainMenuController(MatchContext context, int choice)
        {       
            switch (choice)
            {
                case 1:
                {
                    int myChoice = AddMenuController(context);
                    //Return to main menu
                    return myChoice;
                }
                case 2:
                {
                    int myChoice = DisplayMenuController(context);
                    return myChoice;
                }
                case 3:
                {
                    int myChoice = GroupByMenuController(context);
                    return myChoice;
                }
                case 4:
                {
                    int myChoice = MatchMenuController(context);
                    return myChoice;
                }
                case 5:
                {
                    Console.WriteLine("Exiting!!!!");
                    return 5;
                }
                default:
                {
                    return MainMenuView(context);
                }
            }
        }
        static int MatchMenuController(MatchContext context)
        {
            int choice = MatchMenuView(context);
            switch (choice)
            {
                case 1:
                {
                    Console.Clear();
                    Console.WriteLine("TODO: Implement making match ALL TO ALL by Weightclass.\nPlease submit the Weightclass you want to match contestants for.  ");
                    int weightClassId = Int32.Parse(Console.ReadLine());
                    var classes = context.WeightClasses.Find(weightClassId);
                    if (classes == null)
                    {
                        Console.WriteLine("No such weight class found. Returning to main menu.");
                    }
                    CreateMatchesAllToAllForWeightClass(context,weightClassId);
                    Console.WriteLine("Matching for weightclass {0} succeeded!", weightClassId);
                    Console.ReadKey();
                    return 0;
                }
                case 2:
                {
                    Console.Clear();
                    Console.WriteLine("TODO: Implement making match LADDER");
                        return 0;
                }
                case 5:
                    return 5;
                default:
                    return 0;
            }
        }

        static void Main(string[] args)
        {
            MatchContext context = new MatchContext();
            int choice = 0;
            while (choice != 5)
            {
                choice = MainMenuController(context, choice);
            }
            context.SaveChanges();
            



            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
