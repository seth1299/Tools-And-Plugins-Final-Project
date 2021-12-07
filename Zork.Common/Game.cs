using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Zork.Common;
using Zork;


namespace Zork
{
    public class Game : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public World World { get; private set; }

        public string StartingLocation { get; set; }
        
        public string WelcomeMessage { get; set; }
        
        public string ExitMessage { get; set; }

        private Item itemToTake;

        [JsonIgnore]
        public Player Player { get; private set; }
        [JsonIgnore]
        public static IInputService IInput { get; private set; }

        [JsonIgnore]
        public bool IsRunning { get; set; }
        
        [JsonIgnore]
        public static IOutputService Output { get; private set; } //added JSONIgnore and get/set 11/17/21

        [JsonIgnore]
        public Dictionary<string, Command> Commands { get; private set; }
        public static Game Instance { get; private set; }

        public Game(World world, Player player)
        {
            World = world;
            Player = player;

            Commands = new Dictionary<string, Command>()
            {
                { "QUIT", new Command("QUIT", new string[] { "QUIT", "Q", "BYE", "bye", "q", "quit" }, Quit) },
                { "LOOK", new Command("LOOK", new string[] { "LOOK", "L", "l", "look" }, Look) },
                { "NORTH", new Command("NORTH", new string[] { "NORTH", "N", "n", "north" }, game => Move(game, Directions.North)) },
                { "SOUTH", new Command("SOUTH", new string[] { "SOUTH", "S", "s", "south" }, game => Move(game, Directions.South)) },
                { "EAST", new Command("EAST", new string[] { "EAST", "E", "e", "east"}, game => Move(game, Directions.East)) },
                { "WEST", new Command("WEST", new string[] { "WEST", "W", "w", "west" }, game => Move(game, Directions.West)) },
                { "INVENTORY", new Command("INVENTORY", new string[] { "INVENTORY", "I", "i", "inventory" }, game => DisplayInventory()) },
                { "TAKE", new Command("TAKE", new string[] { "TAKE", "T", "t", "take" }, game => Take()) },
                { "DROP", new Command("DROP", new string[] { "DROP", "D", "d", "drop" }, game => Drop()) },
                { "CLEAR", new Command("CLEAR", new string[] { "CLEAR", "C", "c", "clear" }, game => Clear()) }
            };
        }

        public void Start(IInputService input, IOutputService output)
        {
            Assert.IsNotNull(output);
            Output = output;

            Assert.IsNotNull(input);
            IInput = input;
            IInput.InputReceived += InputReceivedHandler;

            IsRunning = true;            
        }

        public void DisplayWelcomeMessage()
        {
            Output.WriteLine(string.IsNullOrWhiteSpace(WelcomeMessage) ? "Welcome to Zork!" : WelcomeMessage);
        }

        private void Clear()
        {
            Console.Clear();
        }

        public static void Start(string defaultGameFilename, IInputService input, IOutputService output)
        {
            Instance = Load(defaultGameFilename);
            IInput = input;
            Output = output;
            Instance.IsRunning = true;
        }


        private void InputReceivedHandler(object sender, string commandString)
        {
            if (commandString.ToUpper().Trim().Equals("QUIT") || commandString.ToUpper().Trim().Equals("Q") || commandString.ToUpper().Trim().Equals("quit") || commandString.ToUpper().Trim().Equals("q") || commandString.ToUpper().Trim().Equals("bye") || commandString.ToUpper().Trim().Equals("BYE"))
            {
                IsRunning = false;
            }

            Command foundCommand = null;
            foreach (Command command in Commands.Values)
            {
                if (command.Verbs.Contains(commandString.ToUpper().Trim()))
                {
                    foundCommand = command;
                    break;
                }
            }

            if (foundCommand != null)
            {
                foundCommand.Action(this);
                Player.IncreaseMoves();
            }
            else
            {
                Output.WriteLine("Unknown command.\n");
            }
        }

        private void Take()
        {
            itemToTake = Player.Location.RoomItem;
            if (itemToTake != null && itemToTake.IsTakeable)
            {
                Random rand = new Random();
                switch (rand.Next(1, 4))
                {
                    case 1:
                    Output.WriteLine($"Added {itemToTake} to your inventory.\n");
                    break;

                    case 2:
                    Output.WriteLine($"Picked up the {itemToTake}.\n");
                    break;

                    default:
                    Output.WriteLine($"Got the {itemToTake}.\n");
                    break;
                }
                
                Player.Score += itemToTake.ScoreValue;
                itemToTake.AlreadyTaken = true;
                Player.Inventory.Add(itemToTake);
                Player.Location.RemoveRoomItem();
            }
            else if (itemToTake != null && !itemToTake.IsTakeable)
            {
                Random rand = new Random();
                switch (rand.Next(1, 4))
                {
                    case 1:
                        Output.WriteLine($"Nothing doing, there's no way that I can fit that {itemToTake} in my inventory.\n");
                        break;

                    case 2:
                        Output.WriteLine($"Nope, I can't lift that {itemToTake} no matter how hard I try.\n");
                        break;

                    default:
                        Output.WriteLine($"I may be good at carrying my teammates, but there's no way that I can carry that {itemToTake}.\n");
                        break;
                }
            }
            else if (itemToTake == null)
            {
                Output.WriteLine("There's nothing to take.");
            }
        }

        private void Drop()
        {
            if ( Player.Inventory == null || Player.Inventory.Count == 0 )
            {
                Output.WriteLine("You don't have anything to drop.\n");
            }
            else
            {
                Item itemToDrop = Player.Inventory[0];
                if (Player.Location.RoomItem == null)
                {
                    Random rand = new Random();
                    switch (rand.Next(1, 4))
                    {
                        case 1:
                            Output.WriteLine($"All right, you dropped the {itemToDrop}.\n");
                            break;

                        case 2:
                            Output.WriteLine($"Threw the {itemToDrop} onto the ground.\n");
                            break;

                        default:
                            Output.WriteLine($"Phew, I thought I'd never get rid of that {itemToDrop}.\n");
                            break;
                    }
                    
                    Player.Score -= itemToDrop.ScoreValue;
                    Player.Location.RoomItem = itemToDrop;
                    Player.Inventory.RemoveAt(0);
                }
                else
                {
                    Random rand = new Random();
                    switch (rand.Next(1, 4))
                    {
                        case 1:
                            Output.WriteLine($"There's already too many things in this room to be going around dropping a whole {itemToDrop} in it.\n");
                            break;

                        case 2:
                            Output.WriteLine($"I remember what my mom always said: \"Player, you should never throw a {itemToDrop} onto the ground, especially if there's already a {Player.Location.RoomItem} in the room.\n");
                            break;

                        default:
                            Output.WriteLine($"Nope, I shouldn't put too many items in this room.\n");
                            break;
                    }
                    
                }
            }
        }
        public void DisplayInventory()
        {
            if ( Player.Inventory == null || Player.Inventory.Count == 0)
            {
                Output.WriteLine("You are carrying nothing except the clothes on your back.\n");
            }
            if (Player.Inventory != null && Player.Inventory.Count != 0)
            {
                Output.WriteLine("\nYour inventory consists of the following: \n\n=============\n");
                foreach (Item item in Player.Inventory)
                {
                    string firstLetter = item.Name[0].ToString().ToUpper(), restOfWord = item.Name.Substring(1), wholeWord = firstLetter + restOfWord;
                    Output.WriteLine(($"{wholeWord}: {item.Description}\n"));
                }
                Output.WriteLine("=============\n");
            }
                
        }

        private static void Move(Game game, Directions direction)
        {
            if (game.Player.Move(direction) == false)
            {
                Output.WriteLine("The way is shut!");
            }
        }

        public static IInputService GetIInput()
        {
            return IInput;
        }

        public static void Look(Game game)
        {
            Item tempItem = game.Player.Location.RoomItem;

            if (tempItem != null)
            {
                Output.WriteLine(($"{game.Player.Location.Description} Additionally, there is a {tempItem} in here as well.\n"));
            }
            else
            {
                Output.WriteLine(($"{game.Player.Location.Description}\n"));
            }
        }

        public static void StartFromFile(string gamefilename, IOutputService outputService)
        {
            if (!File.Exists(gamefilename))
            {
                throw new FileNotFoundException("Expected File.", gamefilename);
            }

            Start(File.ReadAllText(gamefilename), IInput, Output);

        }

        private static void Quit(Game game) => game.IsRunning = false;

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) => Player = new Player(World, StartingLocation);
   
    public static Game Load(string defaultGameFilename)
        {
            Game game = JsonConvert.DeserializeObject<Game>(defaultGameFilename);

            return game;
        }

    public int GetPlayerScore()
        {
            return Player.Score;
        }

        public int GetPlayerMoves()
        {
            return Player.Moves;
        }
    
    }

}