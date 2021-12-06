using Newtonsoft.Json;
using System;

namespace Zork
{
    public class Player
    {
        private Room location;
        private Room _location;

        private int _moves, _score;

        public event EventHandler<Room> LocationChanged;
        public event EventHandler<int> MovesChanged;
        public event EventHandler<int> ScoreChanged;

        public World World { get; }

        [JsonIgnore]
        public Room Location
        {
            get
            {
                return _location;
            }

            private set
            {
                if (_location != value)
                {
                    _location = value;
                    LocationChanged?.Invoke(this, _location);
                }
            }
        }

        public int Moves 
        {
            get
            {
                return _moves;
            }
            private set
            {
                if (_moves != value)
                {
                    _moves = value;
                    MovesChanged?.Invoke(this, _moves);
                }
            }
        }

        public int Score 
        {
            get
            {
                return _score;
            }
            set
            {
                if (_score != value)
                {
                    _score = value;
                    ScoreChanged?.Invoke(this, _score);
                }
            }
        }

        public Player(World world, string startingLocation)
        {
            Assert.IsTrue(world != null);
            Assert.IsTrue(world.RoomsByName.ContainsKey(startingLocation));

            World = world;
            Location = world.RoomsByName[startingLocation];
        }

        public bool Move(Directions direction)
        {
            bool isValidMove = Location.Neighbors.TryGetValue(direction, out Room destination);
            if (isValidMove)
            {
                Location = destination;
            }

            return isValidMove;
        }

        public void IncreaseMoves()
        {
            Moves++;
        }
    }
}
