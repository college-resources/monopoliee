using System;
using Newtonsoft.Json.Linq;
using Schema;
using UniRx;

public class Session
{
   // public static Session Instance => _session;
   public static readonly BehaviorSubject<Session> Instance = new BehaviorSubject<Session>(null);
   
   private User _user;
   private bool _inGame;
   private bool _wasDisconnected;

   public User User => _user;
   public bool InGame => _inGame;
   public bool WasDisconnected => _wasDisconnected;

   private Session(JToken session)
   {
      _user = User.GetUser(session["user"]);
      _inGame = (bool) session["in_game"];
      _wasDisconnected = (bool) session["was_disconnected"];
   }

   public static void Login(JToken session)
   {
      Instance.OnNext(new Session(session));
   }

   public void Logout()
   {
      Instance.OnNext(null);
   }
}
