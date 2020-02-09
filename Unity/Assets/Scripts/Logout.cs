using UnityEngine;

public class Logout : MonoBehaviour
{
    public void OnClick()
    {
        AuthenticationManager.Instance.Logout();
    }
}
