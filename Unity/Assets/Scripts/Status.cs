using TMPro;
using UnityEngine;
public class Status : MonoBehaviour
{
    private TextMeshProUGUI _text;
    
    public string Value
    {
        set => _text.text = value;
    }
    
    private void Awake()
    {
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }
}
