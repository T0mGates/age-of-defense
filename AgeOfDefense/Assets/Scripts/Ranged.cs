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
            SwitchTarget(attacking[0].gameObject, aggro[index].gameObject);
        }
        GameObject proj = Instantiate(projectile, shootPoint.position, Quaternion.identity);
        proj.transform.up = (attacking[0].gameObject.transform.position - transform.position).normalized;
        proj.GetComponent<Projectile>().SetStats(unitTarget, targetsToHit, currentProjectileSpeed, currentAttack, currentHeal, gameObject.tag, gameObject);
    }

    public void SwitchTarget(GameObject fromObj, GameObject toObj)
    {
        RemoveAttacking(fromObj.gameObject);
        fromObj.gameObject.GetComponent<Unit>().RemoveDefending(gameObject);

        AddAttacking(toObj.gameObject);
        toObj.gameObject.GetComponent<Unit>().AddDefending(gameObject);

    }

}
