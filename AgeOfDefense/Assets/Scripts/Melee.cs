using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : Unit
{
    override public void Attack()
    {
        attackTimer = Time.time + currentAttackSpeed;
        //animation and whatever
        if(gameObject.tag == attacking.gameObject.tag)
        {
            attacking.GetComponent<Unit>().ChangeHealth(currentHeal);
        }
        else
        {
            attacking.GetComponent<Unit>().ChangeHealth(-currentAttack);
        }
    }
}
