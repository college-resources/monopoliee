using UnityEngine;

public class Logout : MonoBehaviour
{
    public void OnClick()
    {
        Session.Logout();
    }
}
