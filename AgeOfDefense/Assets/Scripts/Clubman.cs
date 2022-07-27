using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clubman : Unit
{
    public float ok;
    override public void Attack()
    {
        attackTimer = Time.time + currentAttackSpeed;
        //animation and whatever
        attacking[0].GetComponent<Unit>().TakeDamage(currentAttack);
    }
}
