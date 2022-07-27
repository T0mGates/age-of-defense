using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshotter : Unit
{
    public GameObject projectile;
    public Transform shootPoint;

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
        proj.GetComponent<Projectile>().speed = currentProjectileSpeed;
    }

    public void SwitchTarget(GameObject fromObj, GameObject toObj)
    {
        RemoveAttacking(fromObj.gameObject);
        fromObj.gameObject.GetComponent<Unit>().RemoveDefending(gameObject);

        AddAttacking(toObj.gameObject);
        toObj.gameObject.GetComponent<Unit>().AddDefending(gameObject);

    }

}
