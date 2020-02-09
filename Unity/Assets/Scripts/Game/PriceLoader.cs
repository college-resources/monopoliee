using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class PriceLoader : MonoBehaviour
{
    public Properties properties;
    public Taxes taxes;

    private async void Start()
    {
        try
        {
            var response = await APIWrapper.Instance.GamePrices();
            
            var propertyPricesArray = (JArray) response["properties"];
            var propertyPrices = new List<int>(propertyPricesArray.Count);
            foreach (var price in propertyPricesArray)
            {
                var p = (int) price["price"];
                propertyPrices.Add(p);
            }
                
            var taxPricesArray  = (JArray) response["taxes"];
            var taxPrices = new List<int>(taxPricesArray.Count);
            foreach (var price in taxPricesArray)
            {
                var p = (int) price["price"];
                taxPrices.Add(p);
            }

            LoadPrices(propertyPrices, taxPrices);
        }
        catch (Exception e)
        {
            Debug.Log(e); // TODO: Show error to player
        }
    }

   private void LoadPrices(IReadOnlyList<int> propertyPrices, IReadOnlyList<int> taxPrices)
    {
        for (var i = 0; i < propertyPrices.Count; i++)
        {
            properties.pricesList[i].text = propertyPrices[i] + "ΔΜ";
        }
        
        for (var i = 0; i < taxPrices.Count; i++)
        {
            taxes.pricesList[i].text = taxPrices[i] + "ΔΜ";
        }
    }
}
