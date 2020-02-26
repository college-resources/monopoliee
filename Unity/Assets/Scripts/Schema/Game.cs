using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UniRx;

namespace Schema
{
    public class Game
    {
        #region Caching

        private static Dictionary<string, Game> _games;

        public static Game GetGame(JToken game)
        {
            if (_games == null)
            {
                _games = new Dictionary<string, Game>();
            }
            
            string gameId = (string) game["_id"];

            if (!_games.ContainsKey(gameId))
            {
                Game newGame = new Game(game);
                _games.Add(gameId, newGame);
            }
            
            return _games[gameId];
        }

        #endregion

        private string _id;
        private int _seats;

        public string Id => _id;
        public List<Player> Players { get; }
        public List<Property> Properties { get; }
        public int Seats => _seats;
        public BehaviorSubject<string> Status { get; }
        public string CurrentPlayerId { get; private set; }

        private Game(JToken game)
        {
            _id = (string) game["_id"];
            Players = ((JArray) game["players"]).Select(Player.GetPlayer).ToList();
            Properties = ((JArray) game["properties"]).Select(Property.GetProperty).ToList();
            _seats = (int) game["seats"];
            Status = new BehaviorSubject<string>((string) game["status"]);
            CurrentPlayerId = (string) game["currentPlayer"];
            
            SocketIo.Instance.GameStarted += SocketIoOnGameStarted;
        }

        public void AddPlayer(Player player)
        {
            Players.Add(player);
        }

        public static void ClearCache()
        {
            _games?.Clear();
            Player.ClearCache();
            Property.ClearCache();
        }

        public void UpdateCurrentPlayer(Player player)
        {
            CurrentPlayerId = player.UserId;
        }
        
        public Property GetPropertyByIndex(int index)
        {
            return Properties[index];
        }

        public string SeatsToString() => $"{Players.Count}/{_seats}";
        
        private void SocketIoOnGameStarted()
        {
            Status.OnNext("running");
        }
    }
}
