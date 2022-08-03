using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifetime = 12;

    private UnitTarget unitTarget;
    private int hitCounter = 0;
    private float speed, attack = 0, heal = 0;
    private int targetsToHit;
    private string ownerTag;
    private GameObject owner;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * Time.deltaTime * speed;
    }

    public void SetStats(UnitTarget unitTrgt, int targetsToHitParam, float speedParam, float attackParam, float healParam, string ownerTagParam, GameObject ownerParam)
    {
        unitTarget = unitTrgt;
        targetsToHit = targetsToHitParam;
        speed = speedParam;
        attack = attackParam;
        heal = healParam;
        ownerTag = ownerTagParam;
        owner = ownerParam;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if((collision.gameObject.tag == unitTarget.ToString() ||
            (collision.gameObject.tag == "Building" && (ownerTag == "Enemy" && attack > 0)) ||
            ((collision.gameObject.tag == "Ally" || 
            collision.gameObject.tag == "Enemy") && 
            unitTarget == UnitTarget.Both)) && 
            collision.gameObject != owner.gameObject &&
            !collision.isTrigger)
        {
            hitCounter++;
            if(ownerTag == collision.gameObject.tag)
            {
                collision.GetComponent<Unit>().ChangeHealth(heal);
            }
            else
            {
                if(collision.gameObject.tag != "Building")
                {
                    collision.GetComponent<Unit>().ChangeHealth(-attack, false, gameObject);
                }
                else
                {
                    collision.GetComponent<Building>().ChangeHealth(-attack);
                }
            }
            if(hitCounter == targetsToHit)
            {
                Destroy(gameObject);
            }
        }
    }
}
