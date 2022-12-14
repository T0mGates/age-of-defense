using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    private GameObject mouseClicking = null;
    public float spawnInterval = 5;
    private float spawnTimer;
    public GameObject[] enemies;
    private float minX, maxX, spawnY, minY, maxY;
    private LineRenderer line;
    private GameObject shownButtonsBuilding = null;
    public TextMeshProUGUI gameTimeTxt;
    private int minutes = 0, seconds= 0;
    private string minutesString, secondsString;
    private bool running = true;

    // Start is called before the first frame update
    void Start()
    {
        ResetTimer();
        StartCoroutine(StartGameTime());
        float verticalExtent = Camera.main.orthographicSize;
        float horizontalExtent = verticalExtent * Screen.width / Screen.height;
        minX = -1 * horizontalExtent;
        maxX = horizontalExtent;
        minY = -1 * verticalExtent;
        maxY = verticalExtent;
        spawnY = verticalExtent;
        line = GetComponent<LineRenderer>();
        line.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(mouseClicking != null)
        {
            UpdateLine(mouseClicking.transform.position ,Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
        else
        {
            line.enabled = false;
        }
        if(spawnTimer < Time.time)
        {
            Spawn();
            ResetTimer();
        }
        if (Input.GetMouseButtonDown(0))
        {
            GameObject clickedObj = ObjectOnMouse();
            if (clickedObj != null)
            {
                if(clickedObj.tag == "Ally")
                {
                    mouseClicking = clickedObj;
                    StartLine(mouseClicking.transform.position);
                }
                else if(clickedObj.tag == "Building")
                {
                    if(shownButtonsBuilding != null)
                    {
                        shownButtonsBuilding.GetComponent<Building>().ShowButtons(false);
                    }
                    clickedObj.GetComponent<Building>().ShowButtons(true);
                    shownButtonsBuilding = clickedObj;
                }
            }
            CheckForBuildingButtons(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
        if (Input.GetMouseButtonUp(0))
        {
            if(mouseClicking != null)
            {
                GameObject obj = ObjectOnMouse();
                if (obj == null)
                {
                    mouseClicking.GetComponent<Unit>().ResetMoveTarget();
                    mouseClicking.GetComponent<Unit>().StartMovingTowardsPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    mouseClicking = null;
                }
                else
                {
                    if (mouseClicking.GetComponent<Unit>().IsValidTarget(obj))
                    {
                        mouseClicking.GetComponent<Unit>().ResetMoveTarget();
                        mouseClicking.GetComponent<Unit>().StartMovingTowardsObject(obj);
                        mouseClicking = null;
                    }
                    else
                    {
                        mouseClicking.GetComponent<Unit>().ResetMoveTarget();
                        mouseClicking.GetComponent<Unit>().StartMovingTowardsPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                        mouseClicking = null;
                    }
                }
            }
        }
    }

    private IEnumerator StartGameTime()
    {
        while (running)
        {
            secondsString = seconds.ToString();
            minutesString = minutes.ToString();

            if (seconds < 10)
            {
                secondsString = "0" + seconds.ToString();
            }
            if (minutes < 10)
            {
                minutesString = "0" + minutes.ToString();
            }

            seconds++;
            if (seconds == 60)
            {
                minutes++;
                seconds = 0;
            }

            gameTimeTxt.text = minutesString + ":" + secondsString;

            yield return new WaitForSeconds(1);
        }
    }

    public void StopTime()
    {
        running = false;
    }

    public void StartTime()
    {
        running = true;
    }

    private void CheckForBuildingButtons(Vector2 pos)
    {
        if(shownButtonsBuilding != null)
        {
            if (Vector2.Distance(shownButtonsBuilding.transform.position, pos) > 3)
            {
                shownButtonsBuilding.GetComponent<Building>().ShowButtons(false);
                shownButtonsBuilding = null;
            }
        }
    }

    private void StartLine(Vector3 point)
    {
        line.enabled = true;
        line.SetPosition(0, point);
        line.SetPosition(1, point);
    }

    private void UpdateLine(Vector3 firstPoint, Vector3 secondPoint)
    {
        line.SetPosition(0, firstPoint);
        line.SetPosition(1, secondPoint);
    }

    
    private void ResetTimer()
    {
        spawnTimer = Time.time + spawnInterval;
    }

    private void Spawn()
    {
        float randX = Random.Range(minX + 0.5f, maxX - 0.5f);
        int randEnemy = Random.Range(0, enemies.Length);
        GameObject spawnedEnemy = Instantiate(enemies[randEnemy], new Vector3(randX, spawnY + 0.5f, 0), Quaternion.identity);
        if(spawnedEnemy.GetComponent<Unit>().attackType == AttackType.Ranged)
        {
            spawnedEnemy.GetComponent<Unit>().StartCoroutine(spawnedEnemy.GetComponent<Unit>().EnemyAIMovement(minX + 0.5f, maxX - 0.5f, spawnY * 0.617f, spawnY * 0.875f));
        }
        else
        {
            spawnedEnemy.GetComponent<Unit>().StartCoroutine(spawnedEnemy.GetComponent<Unit>().EnemyAIMovement(minX + 0.5f, maxX - 0.5f, -spawnY, -spawnY));

        }
    }

    private GameObject ObjectOnMouse()
    {
        Vector2 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray, Vector2.zero);
        if (hit.collider != null)
        {
            return (hit.collider.gameObject);
        }
        return null;
    }

    public Vector2 GetRandomPointOnMap()
    {
        float randX = Random.Range(minX + 1, maxX - 1);
        float randY = Random.Range(minY + 1, maxY - 1);
        return new Vector2(randX, randY);
    }

    public bool BuySomething(Resource resource, int cost)
    {
        //resource can be: wood, coin
        switch (resource)
        {
            case Resource.Wood:
                if(cost <= GetComponent<PrehistoricManager>().wood)
                {
                    GetComponent<PrehistoricManager>().ChangeWood(-cost);
                    return true;
                }
                break;

            case Resource.Coin:
                if(cost <= GetComponent<PrehistoricManager>().coins)
                {
                    GetComponent<PrehistoricManager>().ChangeCoins(-cost);
                    return true;
                }
                break;
        }
        return false;
    }
}
