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
            if(attacking.gameObject.tag != "Building")
            {
                attacking.GetComponent<Unit>().ChangeHealth(-currentAttack, false, gameObject);
            }
            else
            {
                attacking.GetComponent<Building>().ChangeHealth(-currentAttack);
            }
        }
    }
}
