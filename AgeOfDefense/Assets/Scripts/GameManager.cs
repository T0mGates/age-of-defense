using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameObject mouseClicking = null;
    public float spawnInterval = 5;
    private float spawnTimer;
    public GameObject[] enemies;
    private List<GameObject> enemiesArray = new List<GameObject>();
    private List<GameObject> alliesArray = new List<GameObject>();
    private float minX, maxX, spawnY;
    private LineRenderer line;
    // Start is called before the first frame update
    void Start()
    {
        ResetTimer();
        float verticalExtent = Camera.main.orthographicSize;
        float horizontalExtent = verticalExtent * Screen.width / Screen.height;
        minX = -1 * horizontalExtent;
        maxX = horizontalExtent;
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
            }
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
        float randX = Random.Range(minX + 1, maxX - 1);
        GameObject spawnedEnemy = Instantiate(enemies[0], new Vector3(randX, spawnY, 0), Quaternion.identity);
        spawnedEnemy.GetComponent<Unit>().StartMovingTowardsPoint(new Vector2(randX, -1 * spawnY));
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
}
