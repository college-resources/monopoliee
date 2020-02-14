using System;
using UnityEngine;

public class EndTurn : MonoBehaviour
{
    public async void OnClick()
    {
        try
        {
            await ApiWrapper.PlayerEndTurn();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
}
