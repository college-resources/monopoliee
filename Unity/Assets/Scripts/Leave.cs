using Schema;
using UnityEngine;

public class Leave : MonoBehaviour
{
    public void OnClick()
    {
        Game.Current.Value.Leave();
    }
}
