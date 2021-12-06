using System.IO;
using Newtonsoft.Json;
using Zork.Common;
using System;

namespace Zork
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const string defaultGameFilename = "Zork.json";
            string gameFilename = (args.Length > 0 ? args[(int)CommandLineArguments.GameFilename] : defaultGameFilename);

            Game game = JsonConvert.DeserializeObject<Game>(File.ReadAllText(gameFilename));

            ConsoleInputService input = new ConsoleInputService();
            ConsoleOutputService output = new ConsoleOutputService();

            game.Player.LocationChanged += Player_LocationChanged;

            output.WriteLine(string.IsNullOrWhiteSpace(game.WelcomeMessage) ? "Welcome to Zork!" : game.WelcomeMessage);
            game.Start((IInputService)input, (IOutputService)output);
            output.WriteLine(string.IsNullOrWhiteSpace(game.ExitMessage) ? "Thanks for playing!" : game.ExitMessage);

            while (game.IsRunning) //loop
            {
                Room previousRoom = null;
                output.WriteLine(game.Player.Location);
                if (previousRoom != game.Player.Location)
                {
                    Game.Look(game);
                    previousRoom = game.Player.Location;
                }
                

                output.Write("\n> ");
                input.ProcessInput();
            }

            EventHandler<Room> handler = MyHandler;


        }

        private static void Player_LocationChanged(object sender, Room e)
        {
            Console.WriteLine($"You moved to {e.Name}");
        }

        private static void MyHandler(object sender, Room args)
        {

        }

        private enum CommandLineArguments
        {
            GameFilename = 0
        }
    }
}