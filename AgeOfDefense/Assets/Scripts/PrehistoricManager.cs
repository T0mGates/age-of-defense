using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PrehistoricManager : MonoBehaviour
{
    public int wood = 0, coins = 0;
    public TextMeshProUGUI coinsTxt, woodTxt;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ChangeCoins(int change)
    {
        coins += change;
        coinsTxt.text = "Coins: " + coins;
    }

    public void ChangeWood(int change)
    {
        wood += change;
        woodTxt.text = "Wood: " + wood;
    }
}
