﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Schema;
using TMPro;
using Debug = UnityEngine.Debug;

public class LoadGames : MonoBehaviour
{
    public List<Game> gameList;
    public TextMeshProUGUI noGamesFoundText;
    public GameObject mainScrollContentView;
    public GameObject contentDataPanel;
    public GameObject errorText;
    
    private void Start()
    {
        ClearErrorText();
        LoadGameList();
    }

    private void Initialize()
    {
        if (gameList.Count > 0) {
            HideNoGamesFoundText();
            
            foreach (Transform child in mainScrollContentView.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (var game in gameList)
            {
                var playerTextPanel = Instantiate(contentDataPanel, mainScrollContentView.transform, true);
                playerTextPanel.transform.localScale = new Vector3(1,1,1);
                playerTextPanel.transform.localPosition = new Vector3(0,0,0);
                
                if (game.Status == "waitingPlayers")
                {
                    playerTextPanel.transform.Find("WaitingText").GetComponent<TextMeshProUGUI>().text = "Waiting for players " + game.SeatsToString();

                    playerTextPanel.transform.Find("Join").GetComponent<Submit>().Click += JoinClick(game);
                }
                else
                {
                    playerTextPanel.transform.Find("WaitingText").GetComponent<TextMeshProUGUI>().text = game.Players.Count + " players in-game";
                    playerTextPanel.transform.Find("Join").gameObject.SetActive(false);
                }
            }
        }
        else
        {
            ShowNoGamesFoundText();
        }
    }
    
    public async void LoadGameList()
    {
        ClearErrorText();
        
        try
        {
            var response = await ApiWrapper.GameList();
            
            Game.ClearCache();
            
            var games = (JArray) response;
            gameList = new List<Game>(games.Count);
            foreach (var game in games)
            {
                gameList.Add(Game.GetGame(game));
            }
            
            Initialize();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
    
    private EventHandler JoinClick(Game game)
    {
        return async (sender, args) =>
        {
            try
            {
                var response = await ApiWrapper.GameJoin(game.Id);
                
                Game.ClearCache();
                var gameToJoin = Game.GetGame(response);
                GameManager.Instance.GoToLobby(gameToJoin);
            }
            catch (BadResponseException e)
            {
                var errorMessage = (string) e.Response["error"]["message"];

                Debug.Log(errorMessage);
                errorText.transform.GetComponent<TextMeshProUGUI>().text = errorMessage;
            }
            catch (Exception e)
            {
                Debug.Log(e); // TODO: Show error to player
            }
        };
    }

    private void ClearErrorText()
    {
        errorText.transform.GetComponent<TextMeshProUGUI>().text = "";
    }

    private void ShowNoGamesFoundText()
    {
        noGamesFoundText.gameObject.SetActive(true);
    }
    
    private void HideNoGamesFoundText()
    {
        noGamesFoundText.gameObject.SetActive(false);
    }
}
