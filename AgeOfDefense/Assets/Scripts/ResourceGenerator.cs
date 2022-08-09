using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceGenerator : Building
{
    [Header("Coins")]
    public bool producesCoins = false;
    public int coinsGainedOnInterval = 1;
    public float coinGainInterval = 10;
    private float coinIntervalTimer;

    [Header("Wood")]
    public bool producesWood = false;
    public int woodGainedOnInterval = 1;
    public float woodGainInterval = 5;
    private float woodIntervalTimer;
    private PrehistoricManager manager;
    private GameManager mainManager;

    [Header("+Resource | -Resource/s")]
    public int[] costs;

    private Slider queueSlider;

    public override void Start()
    {
        base.Start();
        if (producesCoins)
        {
            StartCoinIntervalTimer();
        }
        if (producesWood)
        {
            StartWoodIntervalTimer();
        }
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PrehistoricManager>();
        mainManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        queueSlider = transform.Find("Canvas/QueueSlider").gameObject.GetComponent<Slider>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        //timer checks for resources
        if (producesCoins)
        {
            if (coinIntervalTimer < Time.time)
            {
                manager.ChangeCoins(coinsGainedOnInterval);
                StartCoinIntervalTimer();
            }
        }

        if (producesWood)
        {
            if (woodIntervalTimer < Time.time)
            {
                manager.ChangeWood(woodGainedOnInterval);
                StartWoodIntervalTimer();
            }
        }

        SliderUpdate();
    }

    public void StartCoinIntervalTimer()
    {
        coinIntervalTimer = Time.time + coinGainInterval;
    }

    public void StartWoodIntervalTimer()
    {
        woodIntervalTimer = Time.time + woodGainInterval;
    }

    public void BuyCoinsGained()
    {
        if(mainManager.BuySomething("wood", costs[0])){
            coinsGainedOnInterval += 1;
        }
    }

    public void BuyCoinsInterval()
    {
        if (mainManager.BuySomething("wood", costs[1]))
        {
            coinGainInterval -= 1;
            if(coinGainInterval == 1)
            {
                transform.Find("Canvas/Buttons/CoinIntervalUpgrade").gameObject.SetActive(false);
            }
        }
    }

    public void BuyWoodGained()
    {
        if (mainManager.BuySomething("coin", costs[0]))
        {
            woodGainedOnInterval += 1;
        }
    }

    public void BuyWoodInterval()
    {
        if (mainManager.BuySomething("coin", costs[1]))
        {
            woodGainInterval -= 1;
            if (woodGainInterval == 1)
            {
                transform.Find("Canvas/Buttons/WoodIntervalUpgrade").gameObject.SetActive(false);
            }
        }
    }

    private void SliderUpdate()
    {
        if (producesCoins)
        {
            queueSlider.value = ((coinIntervalTimer - Time.time) / coinGainInterval);
        }
        if (producesWood)
        {
            queueSlider.value = ((woodIntervalTimer - Time.time) / woodGainInterval);
        }
    }
}