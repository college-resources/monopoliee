using System;
using UnityEngine;

public class RollDice : MonoBehaviour
{
    public async void OnClick()
    {
        try
        {
            await APIWrapper.Instance.PlayerRollDice();
        }
        catch (Exception e)
        {
            Debug.Log(e); // TODO: Show error to player
        }
    }
}