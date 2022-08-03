using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goldpump : Building
{
    public int coinsGainedOnInterval = 1;
    public float coinGainInterval = 10;
    private float coinIntervalTimer;
    private PrehistoricManager manager;

    void Start()
    {
        base.Start();
        StartCoinIntervalTimer();
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PrehistoricManager>();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();

        //timer checks for resources
        if (coinIntervalTimer < Time.time)
        {
            manager.ChangeCoins(coinsGainedOnInterval);
            StartCoinIntervalTimer();
        }

        base.Update();
    }

    public void StartCoinIntervalTimer()
    {
        coinIntervalTimer = Time.time + coinGainInterval;
    }
}