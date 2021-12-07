using System.IO;
using Newtonsoft.Json;
using Zork.Common;

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

            output.WriteLine(string.IsNullOrWhiteSpace(game.WelcomeMessage) ? "Welcome to Zork!\n" : ($"{game.WelcomeMessage}\n"));
            game.Player.Inventory.Clear();
            game.Start((IInputService)input, (IOutputService)output);

            Room previousRoom = null;

            while (game.IsRunning)
            {
                output.WriteLine($"Score: {game.Player.Score}\n{game.Player.Location}\n");
                if (previousRoom != game.Player.Location)
                {
                    Game.Look(game);
                    previousRoom = game.Player.Location;
                }
                

                output.Write("> ");
                input.ProcessInput();
            }

            output.WriteLine(string.IsNullOrWhiteSpace(game.ExitMessage) ? "Thanks for playing!" : game.ExitMessage);


        }

        private enum CommandLineArguments
        {
            GameFilename = 0
        }
    }
}