using System;
using UnityEngine;

public class EndTurn : MonoBehaviour
{
    public async void OnClick()
    {
        try
        {
            await APIWrapper.Instance.PlayerEndTurn();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
}
