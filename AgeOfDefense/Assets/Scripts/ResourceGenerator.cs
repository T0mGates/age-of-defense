using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Start()
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
    }

    // Update is called once per frame
    void Update()
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
    }

    public void StartCoinIntervalTimer()
    {
        coinIntervalTimer = Time.time + coinGainInterval;
    }

    public void StartWoodIntervalTimer()
    {
        woodIntervalTimer = Time.time + woodGainInterval;
    }
}