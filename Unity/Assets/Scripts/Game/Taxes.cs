using System.Collections.Generic;
using UnityEngine;

public class Taxes : MonoBehaviour
{
    private TextMesh[] _prices;
    public List<TextMesh> pricesList = new List<TextMesh>();

    private void Start()
    {
        pricesList.Clear();

        _prices = GetComponentsInChildren<TextMesh>();
        foreach (var textMesh in _prices)
        {
            pricesList.Add(textMesh);
        }
    }
}
