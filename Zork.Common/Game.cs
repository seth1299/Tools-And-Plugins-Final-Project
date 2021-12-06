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
                { "REWARD", new Command("REWARD", new string[] { "REWARD", "R", "r", "reward" }, game => Reward()) }
            };
        }

        public void Start(IInputService input, IOutputService output)
        {
            Assert.IsNotNull(output);
            Output = output;

            Assert.IsNotNull(input);
            IInput = input;
            IInput.InputReceived += InputReceivedHandler;

            DisplayWelcomeMessage();

            IsRunning = true;            
        }

        public void DisplayWelcomeMessage()
        {
            Output.WriteLine(string.IsNullOrWhiteSpace(WelcomeMessage) ? "Welcome to Zork!" : WelcomeMessage);
        }

        public static void Start(string defaultGameFilename, IInputService input, IOutputService output)
        {
            Instance = Load(defaultGameFilename);
            IInput = input;
            Output = output;
            //Instance.LoadCommands();
            Instance.DisplayWelcomeMessage();
            Instance.IsRunning = true;
            //IInput.InputReceived += Instance.InputReceived;
        }


        private void InputReceivedHandler(object sender, string commandString)
        {
            if (commandString.ToUpper().Trim().Equals("QUIT") || commandString.ToUpper().Trim().Equals("Q") commandString.ToUpper().Trim().Equals("quit") || commandString.ToUpper().Trim().Equals("q") || commandString.ToUpper().Trim().Equals("bye") || commandString.ToUpper().Trim().Equals("BYE"))
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
                Output.WriteLine("Unknown command.");
            }
        }

        private void Reward()
        {
            Player.Score += 5;
        }

        private static void Move(Game game, Directions direction)
        {
            if (game.Player.Move(direction) == false)
            {
                Output.WriteLine("The way is shut!");
            }
            else
            {
                Output.WriteLine($"{game.Player.Location.Name}\n{game.Player.Location.Description}");
            }
        }

        public static IInputService GetIInput()
        {
            return IInput;
        }

        public static void Look(Game game) => Output.WriteLine(game.Player.Location.Description);

        //Refer to 24:15 in Part 1 video
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
   
    public static Game Load(string defaultGameFilename) //11/12/21
        {
            Game game = JsonConvert.DeserializeObject<Game>(defaultGameFilename);
            //game.Player = game.World.SpawnPlayer();

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