using System;
using Newtonsoft.Json.Linq;
using Schema;
using UniRx;
using UniRx.Async;
using UnityEngine;

public class Session
{
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

   public static async void Load()
   {
      try
      {
         var response = await ApiWrapper.AuthSession();            
         Login(response);
      }
      catch (Exception e)
      {
         Instance.OnNext(null);
         Debug.Log(e); // TODO: Show error to player
      }
   }

   public static void Login(JToken session)
   {
      Instance.OnNext(new Session(session));
   }

   public static async void Logout()
   {
      try
      {
         await ApiWrapper.AuthLogout();
         Instance.OnNext(null);
      }
      catch (Exception e)
      {
         Debug.Log(e); // TODO: Show error to player
      }
   }
   
   public static async UniTask LoginFormSubmit(string username, string password)
   {
      try
      {
         var response = await ApiWrapper.AuthLogin(username, password);
         Login(response);
      }
      catch (Exception e)
      {
         Debug.Log(e); // TODO: Show error to player
         throw;
      }
   }

   public static async UniTask RegisterFormSubmit(string username, string password)
   {
      try
      {
         var response = await ApiWrapper.AuthRegister(username, password);
         Login(response);
      }
      catch (Exception e)
      {
         Debug.Log(e); // TODO: Show error to player
         throw;
      }
   }
}
