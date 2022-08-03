using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranged : Unit
{
    [Header("Ranged Settings")]
    public float maxProjectileSpeed;
    public GameObject projectile;
    public Transform shootPoint;
    public int targetsToHit = 1;

    private float currentProjectileSpeed;

    private void Start()
    {
        currentProjectileSpeed = maxProjectileSpeed;
        base.Start();
    }
    private void Update()
    {
        base.Update();
    }

    public override void Attack()
    {
        attackTimer = Time.time + currentAttackSpeed;
        //animation and whatever
        //attacking[0].GetComponent<Unit>().TakeDamage(currentAttack);
        int index = GetTargetIndex();
        if(index != -1)
        {
            if(attacking != null)
            {
                SwitchTarget(attacking.gameObject, aggro[index].gameObject);
            }
            else
            {
                attacking = aggro[index].gameObject;
                if (aggro[index].gameObject.tag != "Building")
                {
                    aggro[index].gameObject.GetComponent<Unit>().AddDefending(gameObject);
                }
                else
                {
                    aggro[index].gameObject.GetComponent<Building>().AddDefending(gameObject);
                }
            }
        }
        GameObject proj = Instantiate(projectile, shootPoint.position, Quaternion.identity);
        proj.transform.up = (attacking.gameObject.transform.position - transform.position).normalized;
        proj.GetComponent<Projectile>().SetStats(unitTarget, targetsToHit, currentProjectileSpeed, currentAttack, currentHeal, gameObject.tag, gameObject);
    }

    public void SwitchTarget(GameObject fromObj, GameObject toObj)
    {
        if (fromObj.gameObject.tag != "Building")
        {
            fromObj.gameObject.GetComponent<Unit>().RemoveDefending(gameObject);
        }
        else
        {
            fromObj.gameObject.GetComponent<Building>().RemoveDefending(gameObject);
        }
        attacking = toObj.gameObject;
        if(toObj.gameObject.tag != "Building")
        {
            toObj.gameObject.GetComponent<Unit>().AddDefending(gameObject);
        }
        else
        {
            toObj.gameObject.GetComponent<Building>().AddDefending(gameObject);
        }

    }

}
