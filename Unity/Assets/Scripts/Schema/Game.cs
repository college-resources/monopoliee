using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UniRx;
using UnityEngine;

namespace Schema
{
    public class Game : IDisposable
    {
        #region Caching

        private static Dictionary<string, Game> _games;

        public static Game GetGame(JToken game)
        {
            if (_games == null)
            {
                _games = new Dictionary<string, Game>();
            }
            
            var gameId = (string) game["_id"];

            if (!_games.ContainsKey(gameId))
            {
                var newGame = new Game(game);
                _games.Add(gameId, newGame);
            }
            
            return _games[gameId];
        }

        #endregion
        
        public static readonly BehaviorSubject<Game> Current = new BehaviorSubject<Game>(null);
        public readonly Subject<Player> PlayerAdded = new Subject<Player>();
        public readonly Subject<Player> PlayerRemoved = new Subject<Player>();

        public string Id { get; }
        public List<Player> Players { get; }
        public List<Property> Properties { get; }
        public int Seats { get; }
        public BehaviorSubject<string> Status { get; }
        public BehaviorSubject<string> CurrentPlayerId { get; }
        public BehaviorSubject<int> LobbyTime { get; }

        private Game(JToken game)
        {
            Id = (string) game["_id"];
            Players = ((JArray) game["players"]).Select(Player.GetPlayer).ToList();
            Properties = ((JArray) game["properties"]).Select(Property.GetProperty).ToList();
            Seats = (int) game["seats"];
            Status = new BehaviorSubject<string>((string) game["status"]);
            CurrentPlayerId = new BehaviorSubject<string>((string) game["currentPlayer"]);

            LobbyTime = new BehaviorSubject<int>(0);

            SocketIo.Instance.GameIsStarting += SocketIoOnGameIsStarting;
            SocketIo.Instance.GameLobbyTimer += SocketIoOnGameLobbyTimer;
            SocketIo.Instance.GameStarted += SocketIoOnGameStarted;
            SocketIo.Instance.PlayerJoined += SocketIoOnPlayerJoined;
            SocketIo.Instance.PlayerLeft += SocketIoOnPlayerLeft;
            SocketIo.Instance.PlayerTurnChanged += SocketIoOnPlayerTurnChanged;
            SocketIo.Instance.PropertyOwnerChanged += SocketIoOnPropertyOwnerChanged;
        }

        public static void ClearCache()
        {
            if (_games == null) return;
            
            foreach (var game in _games)
            {
                game.Value.Dispose(false);
            }
            
            _games.Clear();
        }

        public string SeatsToString() => $"{Players.Count}/{Seats}";

        public static void Join(JToken game)
        {
            ClearCache();
            Current.OnNext(GetGame(game));
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
            
            Dispose();
            Current.OnNext(null);
        }
        
        private void SocketIoOnGameIsStarting()
        {
            Status.OnNext("starting");
        }
        
        private void SocketIoOnGameLobbyTimer(int remainingSeconds)
        {
            LobbyTime.OnNext(remainingSeconds);
        }
        
        private void SocketIoOnGameStarted(Player firstPlayer)
        {
            CurrentPlayerId.OnNext(firstPlayer.UserId);
            Status.OnNext("running");
        }
        
        private void SocketIoOnPlayerJoined(Player player)
        {
            Players.Add(player);
            PlayerAdded.OnNext(player);
        }
        
        private void SocketIoOnPlayerLeft(Player player)
        {
            Players.Remove(player);
            PlayerRemoved.OnNext(player);
        }

        private void SocketIoOnPlayerTurnChanged(Player player)
        {
            CurrentPlayerId.OnNext(player.UserId);
        }

        private void SocketIoOnPropertyOwnerChanged(int propertyIndex, string ownerId)
        {
            Properties[propertyIndex].OwnerId.OnNext(ownerId);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool removeFromCache)
        {
            PlayerAdded?.Dispose();
            PlayerRemoved?.Dispose();
            Status?.Dispose();
            CurrentPlayerId?.Dispose();
            LobbyTime?.Dispose();
        
            SocketIo.Instance.GameIsStarting -= SocketIoOnGameIsStarting;
            SocketIo.Instance.GameLobbyTimer -= SocketIoOnGameLobbyTimer;
            SocketIo.Instance.GameStarted -= SocketIoOnGameStarted;
            SocketIo.Instance.PlayerJoined -= SocketIoOnPlayerJoined;
            SocketIo.Instance.PlayerLeft -= SocketIoOnPlayerLeft;
            SocketIo.Instance.PlayerTurnChanged -= SocketIoOnPlayerTurnChanged;
            SocketIo.Instance.PropertyOwnerChanged -= SocketIoOnPropertyOwnerChanged;
        
            foreach (var player in Players)
            {
                player?.Dispose();
            }
        
            foreach (var property in Properties)
            {
                property?.Dispose();
            }
        
            if (removeFromCache) _games.Remove(Id);
            
            GC.SuppressFinalize(this);
        }
    }
}
