using System;
using UnityEngine;

public class RollDice : MonoBehaviour
{
    public async void OnClick()
    {
        try
        {
            await ApiWrapper.PlayerRollDice();
        }
        catch (Exception e)
        {
            Debug.Log(e); // TODO: Show error to player
        }
    }
}