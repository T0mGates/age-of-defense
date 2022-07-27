using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : Unit
{
    override public void Attack()
    {
        attackTimer = Time.time + currentAttackSpeed;
        //animation and whatever
        if(gameObject.tag == attacking[0].gameObject.tag)
        {
            attacking[0].GetComponent<Unit>().ChangeHealth(currentHeal);
        }
        else
        {
            attacking[0].GetComponent<Unit>().ChangeHealth(-currentAttack);
        }
    }
}
