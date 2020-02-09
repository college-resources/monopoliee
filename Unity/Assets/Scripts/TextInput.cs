using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextInput : MonoBehaviour
{
    private TMP_InputField _inputField;
    private Outline _outline;
    
    public string Value => _inputField.text;

    public bool Highlight
    {
        set => _outline.enabled = value;
    }
    
    private void Awake()
    {
        _inputField = GetComponentInChildren<TMP_InputField>();
        _outline = GetComponentInChildren<Outline>();
    }
}
