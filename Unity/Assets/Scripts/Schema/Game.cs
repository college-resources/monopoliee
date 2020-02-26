using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UniRx;
using UnityEngine;

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
        
        public static readonly BehaviorSubject<Game> Current = new BehaviorSubject<Game>(null);

        public string Id { get; }
        public List<Player> Players { get; }
        public List<Property> Properties { get; }
        public int Seats { get; }
        public BehaviorSubject<string> Status { get; }
        public string CurrentPlayerId { get; private set; }

        private Game(JToken game)
        {
            Id = (string) game["_id"];
            Players = ((JArray) game["players"]).Select(Player.GetPlayer).ToList();
            Properties = ((JArray) game["properties"]).Select(Property.GetProperty).ToList();
            Seats = (int) game["seats"];
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
        
        private static void ClearCacheExceptCurrentGame()
        {
            foreach (var game in _games.Values.Where(game => game.Id != Current.Value.Id))
            {
                foreach (var player in game.Players)
                {
                    player.Delete();
                }
                
                foreach (var property in game.Properties)
                {
                    property.Delete();
                }
                
                _games.Remove(game.Id);
            }
        }

        public void UpdateCurrentPlayer(Player player)
        {
            CurrentPlayerId = player.UserId;
        }
        
        public Property GetPropertyByIndex(int index)
        {
            return Properties[index];
        }

        public string SeatsToString() => $"{Players.Count}/{Seats}";

        public static void Join(JToken game)
        {
            Current.OnNext(GetGame(game));
            ClearCacheExceptCurrentGame();
        }
        
        public async void Leave()
        {
            try
            {
                await ApiWrapper.GameLeave();
            }
            catch (Exception e)
            {
                Debug.Log(e); // TODO: Show error to player
            }
            
            Current.OnNext(null);
            _games.Remove(Id);
        }
        
        private void SocketIoOnGameStarted()
        {
            Status.OnNext("running");
        }
    }
}
