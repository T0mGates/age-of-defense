using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barricade : Building
{
    private GameManager mainManager;

    public override void Start()
    {
        mainManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        base.Start();
    }

    public override void Die() 
    {
        mainManager.StopTime();
        base.Die();
    }
}
