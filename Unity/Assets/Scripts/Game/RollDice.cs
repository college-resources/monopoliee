using UnityEngine;

public class RollDice : MonoBehaviour
{
    public void OnClick()
    {
        APIWrapper.Instance.PlayerRollDice((response, error) =>
        {
            if (error != null)
            {
                Debug.Log(error); // TODO: Show error to player
            }
        });
    }
}