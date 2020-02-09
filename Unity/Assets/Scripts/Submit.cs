using System;
using UnityEngine;
using UnityEngine.UI;

public class Submit : MonoBehaviour
{
    private Button _button;

    public event EventHandler Click;
    
    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(delegate {
            var handler = Click;
            handler?.Invoke(_button, null);
        });
    }
}
