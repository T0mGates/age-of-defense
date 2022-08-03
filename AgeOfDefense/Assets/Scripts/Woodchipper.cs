using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Woodchipper : Building
{
    public int woodGainedOnInterval = 1;
    public float woodGainInterval = 5;
    private float woodIntervalTimer;
    private PrehistoricManager manager;

    void Start()
    {
        base.Start();
        StartWoodIntervalTimer();
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PrehistoricManager>();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        
        //timer checks for resources
        if (woodIntervalTimer < Time.time)
        {
            manager.ChangeWood(woodGainedOnInterval);
            StartWoodIntervalTimer();
        }

        base.Update();
    }

    public void StartWoodIntervalTimer()
    {
        woodIntervalTimer = Time.time + woodGainInterval;
    }
}
