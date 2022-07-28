using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Unit : MonoBehaviour
{
    [Header("Unit Settings")]
    //public
    public AttackType attackType;
    public UnitTarget unitTarget;
    public TargetType targetType;

    public float maxHealth, maxHeal, maxAttack, maxAttackSpeed, maxSpeed;

    protected GameObject healthbarUI;
    protected Slider slider;

    //private
    protected UnitState state = UnitState.Idle;
    protected float currentHealth, currentAttack, currentSpeed, currentHeal, currentAttackSpeed, attackTimer;

    protected Vector2 moveTowardsTarget;
    protected GameObject priorityTarget = null;
    protected GameManager gameManager;
    protected GameObject attacking;
    protected List<GameObject> defending = new List<GameObject>();
    protected List<GameObject> aggro = new List<GameObject>();
    protected List<GameObject> aggrodBy = new List<GameObject>();

    private Color originalColor;

    [HideInInspector]
    public bool canMove = true;
    // Start is called before the first frame update
    public void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        currentHealth = maxHealth;
        currentAttack = maxAttack;
        currentSpeed = maxSpeed; 
        currentHeal = maxHeal;
        currentAttackSpeed = maxAttackSpeed; 
        healthbarUI = transform.Find("Canvas").gameObject;
        slider = transform.Find("Canvas/Slider").gameObject.GetComponent<Slider>();
        HealthUpdate();
        originalColor = GetComponent<SpriteRenderer>().color;
        moveTowardsTarget = gameManager.GetRandomPointOnMap();
    }

    // Update is called once per frame
    public void Update()
    {
        if(state == UnitState.Moving)
        {
            Move();
        }
        else if(state == UnitState.Combat)
        {
            if(attacking == null)
            {
                state = UnitState.Idle;
                return;
            }
            if (attackType == AttackType.Melee)
            {
                if (Vector2.Distance(gameObject.transform.position, attacking.transform.position) < 1.3f)
                {
                    if (attackTimer < Time.time)
                    {
                        Attack();
                    }
                }
                else
                {
                    attackTimer = Time.time + currentAttackSpeed;
                    priorityTarget = attacking;
                    attacking.GetComponent<Unit>().RemoveDefending(gameObject);
                    attacking = null;
                    state = UnitState.Moving;
                }
            }
            else
            {
                if (attackTimer < Time.time)
                {
                    Attack();
                }
            }
        }
        else if (state == UnitState.Idle)
        {
            //check in aggro range, use target type to target, check if melee or ranged kinda thing..
            if(aggro.Count > 0)
            {
                int index = GetTargetIndex();
                if(attackType == AttackType.Ranged)
                {
                    StartCombat(aggro[index]);
                }
                else if(attackType == AttackType.Melee)
                {
                    StartMovingTowardsObject(aggro[index]);
                }
            }
            else
            {
                if(moveTowardsTarget != null)
                {
                    StartMovingTowardsPoint(moveTowardsTarget);
                }
            }
        }
    }

    protected int GetTargetIndex()
    {
        if(aggro.Count > 0)
        {
            float bestValue = 10000;
            int index = 0;
            bool hasPrioTarget = false;
            for(int i = 0; i < aggro.Count; i++)
            {
                if(gameObject.tag == aggro[i].gameObject.tag)
                {
                    hasPrioTarget = true;
                }
            }
            for (int i = 0; i < aggro.Count; i++)
            {
                if (targetType == TargetType.Closest && (!hasPrioTarget || (hasPrioTarget && gameObject.tag == aggro[i].gameObject.tag)))
                {
                    float dist = Vector2.Distance(gameObject.transform.position, aggro[i].transform.position);
                    if (dist < bestValue)
                    {
                        bestValue = dist;
                        index = i;
                    }
                }
                else if (targetType == TargetType.LowestMaxHP && (!hasPrioTarget || (hasPrioTarget && gameObject.tag == aggro[i].gameObject.tag)))
                {
                    float lowHP = aggro[i].GetComponent<Unit>().maxHealth;
                    if (lowHP < bestValue)
                    {
                        bestValue = lowHP;
                        index = i;
                    }
                }
                else if (targetType == TargetType.LowestPercentHP && (!hasPrioTarget || (hasPrioTarget && gameObject.tag == aggro[i].gameObject.tag)))
                {
                    float lowHP = aggro[i].GetComponent<Unit>().GetPercentHealth();
                    if (lowHP < bestValue)
                    {
                        bestValue = lowHP;
                        index = i;
                    }
                }
            }
            return index;
        }
        return -1;
    }
    public void StartMovingTowardsPoint(Vector2 targetPos)
    {
        bool defendingMelee = false;
        if(defending.Count > 0)
        {
            for(int i = 0; i < defending.Count; i++)
            {
                if(defending[i].GetComponent<Unit>().attackType == AttackType.Melee && defending[i].tag != gameObject.tag)
                {
                    defendingMelee = true;
                }
            }
        }

        moveTowardsTarget = targetPos;
        if(state != UnitState.Combat || !defendingMelee)
        {
            if (!defendingMelee && state == UnitState.Combat)
            {
                attacking.GetComponent<Unit>().RemoveDefending(gameObject);
                attacking.GetComponent<Unit>().StartCoroutine(attacking.GetComponent<Unit>().CombatEndBuffer());
                attacking = null;
            }
            state = UnitState.Moving;
            priorityTarget = null;
        }
    }

    public IEnumerator CombatEndBuffer()
    {
        currentSpeed = 0;
        yield return new WaitForSeconds(0.2f);
        currentSpeed = maxSpeed;
    }

    public void StartMovingTowardsObject(GameObject obj)
    {
        bool defendingMelee = false;
        if (defending.Count > 0)
        {
            for (int i = 0; i < defending.Count; i++)
            {
                if (defending[i].GetComponent<Unit>().attackType == AttackType.Melee && defending[i].tag != gameObject.tag)
                {
                    defendingMelee = true;
                }
            }
        }

        priorityTarget = obj;
        if (state != UnitState.Combat || !defendingMelee)
        {
            if(!defendingMelee && state == UnitState.Combat)
            {
                attacking.GetComponent<Unit>().RemoveDefending(gameObject);
                attacking.GetComponent<Unit>().StartCoroutine(attacking.GetComponent<Unit>().CombatEndBuffer());
                attacking = null;
            }
            state = UnitState.Moving;
        }
    }

    protected void Move()
    {
        float step = currentSpeed * Time.deltaTime;
        Vector2 targetPos = moveTowardsTarget;
        if(priorityTarget != null)
        {
            targetPos = priorityTarget.transform.position;
        }
        if(targetPos == null)
        {
            state = UnitState.Idle;
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, step);
            if (Vector2.Distance(transform.position, targetPos) < 0.05f)
            {
                if (state == UnitState.Moving)
                {
                    state = UnitState.Idle;
                }
            }
        }
    }

    public float GetPercentHealth() { return currentHealth / maxHealth; }
    public float GetCurrentHealth() { return currentHealth;  }

    public void ResetMoveTarget()
    {
        priorityTarget = null;
    }

    private void HealthUpdate()
    {
        slider.value = (currentHealth / maxHealth);
    }

    protected void OnCollisionStay2D(Collision2D collision)
    {
        if (IsValidTarget(collision.gameObject) || IsOnOtherTeam(collision.gameObject))
        {
            if (state != UnitState.Combat)
            {
                StartCombat(collision.gameObject);
            }
        }
    }

    protected bool IsOnOtherTeam(GameObject obj)
    {
        return ((obj.tag == "Ally" || obj.tag == "Enemy") &&
            obj.tag != gameObject.tag);
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsValidTarget(collision.gameObject) && !collision.isTrigger)
        {
            AddAggro(collision.gameObject);
            collision.gameObject.GetComponent<Unit>().AddAggrodBy(gameObject);
        }
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (IsValidTarget(collision.gameObject) && !collision.isTrigger)
        {
            RemoveAggro(collision.gameObject);
            collision.gameObject.GetComponent<Unit>().RemoveAggrodBy(gameObject);
        }
    }

    public void AddDefending(GameObject obj)
    {
        defending.Add(obj);
    }

    public void RemoveDefending(GameObject obj)
    {
        defending.Remove(obj);
        if(defending.Count == 0)
        {
            state = UnitState.Moving;
        }
    }

    public void AddAggro(GameObject obj)
    {
        aggro.Add(obj);
    }

    public void RemoveAggro(GameObject obj)
    {
        aggro.Remove(obj);
    }

    public void AddAggrodBy(GameObject obj)
    {
        aggrodBy.Add(obj);
    }

    public void RemoveAggrodBy(GameObject obj)
    {
        aggrodBy.Remove(obj);
    }

    public bool IsValidTarget(GameObject obj)
    {
        return (obj.gameObject.tag == unitTarget.ToString() || ((obj.gameObject.tag == "Ally" || obj.gameObject.tag == "Enemy") && unitTarget == UnitTarget.Both));
    }

    public void StartCombat(GameObject obj)
    {
        state = UnitState.Combat;
        attacking = obj.gameObject;
        obj.gameObject.GetComponent<Unit>().AddDefending(gameObject);
        attackTimer = Time.time + currentAttackSpeed;
        //if()
    }

    private IEnumerator GreenGlow()
    {
        GetComponent<SpriteRenderer>().color = Color.green;
        yield return new WaitForSeconds(0.12f);
        GetComponent<SpriteRenderer>().color = originalColor;
    }

    private IEnumerator RedGlow()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.12f);
        GetComponent<SpriteRenderer>().color = originalColor;
    }

    public void ChangeHealth(float value)
    {
        currentHealth += value;
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        
        if(value > 0)
        {
            StartCoroutine(GreenGlow());
        }
        else if(value < 0)
        {
            StartCoroutine(RedGlow());
        }

        if(currentHealth > 0)
        {
            HealthUpdate();
        }
        else
        {
            Die();
        }
    }

    protected void RemoveIfAttacking(GameObject obj)
    {
        if(attacking == obj)
        {
            attacking = null;
        }
    }
    protected void Die()
    {
        for(int i = 0; i < defending.Count; i++)
        {
            defending[i].GetComponent<Unit>().RemoveIfAttacking(gameObject);
            defending[i].GetComponent<Unit>().SetState(UnitState.Idle);
        }

        if(attacking != null)
        {
            attacking.GetComponent<Unit>().RemoveDefending(gameObject);
        }
        Destroy(gameObject);
    }

    public void SetState(UnitState newState)
    {
        state = newState;
    }

    abstract public void Attack();

}
