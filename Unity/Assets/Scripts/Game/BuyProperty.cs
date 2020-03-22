using System;
using System.Collections;
using System.Collections.Generic;
using Schema;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyProperty : MonoBehaviour
{
    private readonly Session _session = Session.Instance.Value;
    
    // Buttons
    private GameObject _abandon;
    private GameObject _buy;
    
    // UI Cards
    private GameObject _uiCards;
    private List<GameObject> _uiCardsList;
    
    // Card Data
    public GameObject cardLoader;
    private CardLoader _cardLoader;

    private void Start()
    {
        _abandon = transform.Find("Abandon").gameObject;
        _buy = transform.Find("Buy").gameObject;
        _uiCards = transform.Find("Cards").gameObject;
        _uiCardsList = new List<GameObject>();
        foreach (Transform child in _uiCards.transform)
        {
            _uiCardsList.Add(child.gameObject);
        }
        _cardLoader = cardLoader.GetComponent<CardLoader>();
    }

    public IEnumerator DisplayCard(int location)
    {
        if (Game.Current.Value.CurrentPlayerId.Value == _session.User.Id &&
            string.IsNullOrEmpty(Property.GetPropertyByLocation(location).OwnerId.Value))
        {
            _buy.SetActive(true);
            _abandon.SetActive(true);
        }
        
        switch (location)
        {
            // Stations
            case 5:
            case 15:
            case 25:
            case 35:
            {
                // Get the right data
                var stationCards = _cardLoader.stationsList;
                var stationCard = stationCards[0];
                foreach (var card in stationCards)
                {
                    if (card.location == location)
                    {
                        stationCard = card;
                    }
                }

                // Display the data on the card
                _uiCards.SetActive(true);
                var uiStationCard = _uiCardsList[1];
                uiStationCard.SetActive(true);
                uiStationCard.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = stationCard.name;
                for (var i = 0; i < stationCard.rents.Length; i++)
                {
                    uiStationCard.transform.Find("Rent" + i).gameObject.GetComponent<TextMeshProUGUI>().text =
                        stationCard.rents[i].ToString();
                }
                uiStationCard.transform.Find("MortgageText").gameObject.GetComponent<TextMeshProUGUI>().text = stationCard.mortgage.ToString();
                break;
            }
            // First Utility
            case 12:
            {
                // Get the right data
                var utilityCards = _cardLoader.utilitiesList;
                var utilityCard = utilityCards[0];
                
                // Display the data on the card
                _uiCards.SetActive(true);
                var uiUtilityCard1 = _uiCardsList[2];
                uiUtilityCard1.SetActive(true);
                uiUtilityCard1.transform.Find("MortgageText").gameObject.GetComponent<TextMeshProUGUI>().text = utilityCard.mortgage.ToString();
                break;
            }
            // Second Utility
            case 28:
            {
                // Get the right data
                var utilityCards = _cardLoader.utilitiesList;
                var utilityCard = utilityCards[0];
                
                // Display the data on the card
                _uiCards.SetActive(true);
                var uiUtilityCard2 = _uiCardsList[3];
                uiUtilityCard2.SetActive(true);
                uiUtilityCard2.transform.Find("MortgageText").gameObject.GetComponent<TextMeshProUGUI>().text = utilityCard.mortgage.ToString();
                break;
            }
            // Generic Property
            default:
            {
                // Get the right data
                var propertyCards = _cardLoader.propertiesList;
                var propertyCard = propertyCards[0];
                foreach (var card in propertyCards)
                {
                    if (card.location == location)
                    { 
                        propertyCard = card;
                    }
                }
                
                // Display the data on the card
                _uiCards.SetActive(true);
                foreach (var card in _uiCardsList)
                {
                    if (card.name != "PropertyCard")
                    {
                        card.SetActive(false);
                    }
                    else
                    {
                        card.SetActive(true);
                        Color color;
                        ColorUtility.TryParseHtmlString("#" + propertyCard.color, out color);
                        card.transform.Find("Colour").gameObject.GetComponent<Image>().color = color;
                        card.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = propertyCard.name;
                        for (var i = 0; i < propertyCard.rents.Length; i++)
                        {
                            card.transform.Find("Rent" + i).gameObject.GetComponent<TextMeshProUGUI>().text =
                                propertyCard.rents[i].ToString();
                        }
                        card.transform.Find("BookCostText").gameObject.GetComponent<TextMeshProUGUI>().text = propertyCard.houseCost.ToString();
                        card.transform.Find("DegreeCostText").gameObject.GetComponent<TextMeshProUGUI>().text =
                            propertyCard.houseCost.ToString();
                        card.transform.Find("MortgageText").gameObject.GetComponent<TextMeshProUGUI>().text = propertyCard.mortgage.ToString();
                    }
                }
                break;
            }
        }
        yield return new WaitForSeconds(5f);
        Abandon();
    }

    private void Abandon()
    {
        StopCoroutine(nameof(DisplayCard));
        _abandon.SetActive(false);
        _buy.SetActive(false);
        foreach (var child in _uiCardsList)
        {
            child.SetActive(false);
        }
        _uiCards.SetActive(false);
    }

    // Called by the Buy button
    public async void Buy()
    {
        try
        {
            await ApiWrapper.TransactionBuyCurrentProperty();
            
            Abandon();
        }
        catch (Exception e)
        {
            Debug.Log(e); // TODO: Show error to player
        }
    }
}
