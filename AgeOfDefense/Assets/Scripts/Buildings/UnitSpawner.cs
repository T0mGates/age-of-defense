using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSpawner : Building
{
    public GameObject[] units;
    public int[] unitCosts;
    public float[] unitQueueWait;
    private Queue<GameObject> unitQueue = new Queue<GameObject>();
    private PrehistoricManager manager;
    private GameManager mainManager;
    private Slider queueSlider;
    private float queueWaitTimer;
    private bool queueStarted = false;
    private int currentUnitIndex;

    public override void Start()
    {
        base.Start();
        queueSlider = transform.Find("Canvas/QueueSlider").gameObject.GetComponent<Slider>();
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PrehistoricManager>();
        mainManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        queueSlider.gameObject.SetActive(false);
    }

    public override void Update()
    {
        base.Update();

        if (queueStarted)
        {
            if(queueWaitTimer < Time.time)
            {
                SpawnNextUnit();
            }
            else
            {
                QueueTimeSliderUpdate();
            }
        }
    }

    public void BuyUnit(int index) 
    {
        if (mainManager.BuySomething(Resource.Coin, unitCosts[index]))
        {
            unitQueue.Enqueue(units[index]);
            queueStarted = true;
            if (unitQueue.Count == 1)
            {
                queueSlider.gameObject.SetActive(true);
                StartNextUnitInQueue();
            }
        }
    }

    private void StartNextUnitInQueue()
    {
        currentUnitIndex = System.Array.IndexOf(units, unitQueue.Peek());
        queueWaitTimer = Time.time + (unitQueueWait[currentUnitIndex]);
        QueueTimeSliderUpdate();
    }

    private void QueueTimeSliderUpdate()
    {
        queueSlider.value = ((queueWaitTimer - Time.time) / unitQueueWait[currentUnitIndex]);
    }

    private void SpawnNextUnit()
    {
        float randX = Random.Range(gameObject.transform.position.x - 2, gameObject.transform.position.x + 2);
        float randY = Random.Range(gameObject.transform.position.y + 0.75f, gameObject.transform.position.y + 2);

        GameObject unit = Instantiate(units[currentUnitIndex], new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.identity);
        unit.GetComponent<Unit>().StartMovingTowardsPoint(new Vector2(randX, randY));
        unitQueue.Dequeue();
        if(unitQueue.Count > 0)
        {
            StartNextUnitInQueue();
        }
        else
        {
            queueStarted = false;
            queueSlider.gameObject.SetActive(false);
        }
    }
}
