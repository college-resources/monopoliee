using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UniRx;

namespace Schema
{
    public class Player
    {
        #region Caching

        private static Dictionary<string, Player> _players;

        public static Player GetPlayer(JToken player)
        {
            if (_players == null)
            {
                _players = new Dictionary<string, Player>();
            }

            string userId = (string) player["user"];

            if (!_players.ContainsKey(userId))
            {
                Player newPlayer = new Player(player);
                _players.Add(userId, newPlayer);
            }

            return _players[userId];
        }
        
        public static Player GetPlayerById(string userId)
        {
            try
            {
                return _players[userId];
            }
            catch (ArgumentNullException)
            {
                return null;
            }
        }

        #endregion

        public string UserId { get; }
        public BehaviorSubject<int> Balance { get; }
        public int Position { get; set; }
        public int DuplicateRolls { get; }
        public bool Jailed { get; }
        public int Index { get; }
        public string Name { get; }

        private Player(JToken player)
        {
            UserId = (string) player["user"];
            Balance = new BehaviorSubject<int>((int) player["balance"]);
            Position = (int) player["position"];
            DuplicateRolls = (int) player["duplicateRolls"];
            Jailed = (bool) player["jailed"];
            Index = (int) player["index"];
            Name = (string) player["name"];

            SocketIo.Instance.PlayerBalanceChanged += SocketIoOnPlayerBalanceChanged;
        }

        ~Player()
        {
            Balance?.Dispose();
            
            SocketIo.Instance.PlayerBalanceChanged -= SocketIoOnPlayerBalanceChanged;
        }

        public void SetPosition(int position)
        {
            Position = position;
        }
        
        public static void ClearCache()
        {
            _players?.Clear();
        }

        public void Delete()
        {
            _players.Remove(UserId);
        }
        
        private void SocketIoOnPlayerBalanceChanged(Player player, int balance)
        {
            player.Balance.OnNext(balance);
        }
    }
}
