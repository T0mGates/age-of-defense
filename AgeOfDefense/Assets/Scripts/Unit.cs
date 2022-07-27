using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Unit : MonoBehaviour
{
    //public
    public AttackType attackType;
    public UnitTarget unitTarget;
    public TargetType targetType;

    public float maxHealth, maxHeal, maxAttack, maxAttackSpeed, maxSpeed, maxProjectileSpeed;

    protected GameObject healthbarUI;
    protected Slider slider;

    //private
    protected UnitState state = UnitState.Idle;
    protected float currentHealth, currentAttack, currentSpeed, currentProjectileSpeed, currentHeal, currentAttackSpeed, attackTimer;

    protected Vector2 moveTowardsTarget;
    protected GameObject priorityTarget = null;
    protected GameManager gameManager;
    protected List<GameObject> attacking = new List<GameObject>();
    protected List<GameObject> defending = new List<GameObject>();
    protected List<GameObject> aggro = new List<GameObject>();
    protected List<GameObject> aggrodBy = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        currentHealth = maxHealth;
        currentAttack = maxAttack;
        currentSpeed = maxSpeed;
        currentProjectileSpeed = maxProjectileSpeed;
        currentHeal = maxHeal;
        currentAttackSpeed = maxAttackSpeed; 
        healthbarUI = transform.Find("Canvas").gameObject;
        slider = transform.Find("Canvas/Slider").gameObject.GetComponent<Slider>();
        HealthUpdate();
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
            if(attackTimer < Time.time)
            {
                Attack();
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
                StartMovingTowardsPoint(moveTowardsTarget);
            }
        }
    }

    protected int GetTargetIndex()
    {
        if(aggro.Count > 0)
        {
            float bestValue = 10000;
            int index = 0;
            for (int i = 0; i < aggro.Count; i++)
            {
                if (targetType == TargetType.Closest)
                {
                    float dist = Vector2.Distance(gameObject.transform.position, aggro[i].transform.position);
                    if (dist < bestValue)
                    {
                        bestValue = dist;
                        index = i;
                    }
                }
                else if (targetType == TargetType.LowestMaxHP)
                {
                    float lowHP = aggro[i].GetComponent<Unit>().maxHealth;
                    if (lowHP < bestValue)
                    {
                        bestValue = lowHP;
                        index = i;
                    }
                }
                else if (targetType == TargetType.LowestPercentHP)
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
        moveTowardsTarget = targetPos;
        if(state != UnitState.Combat)
        {
            state = UnitState.Moving;
        }
    }

    public void StartMovingTowardsObject(GameObject obj)
    {
        priorityTarget = obj;
        if (state != UnitState.Combat)
        {
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
        if (IsValidTarget(collision.gameObject))
        {
            if (state != UnitState.Combat)
            {
                StartCombat(collision.gameObject);
            }
        }
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
            state = UnitState.Idle;
        }
    }

    public void AddAttacking(GameObject obj)
    {
        attacking.Add(obj);
    }

    public void RemoveAttacking(GameObject obj)
    {
        attacking.Remove(obj);
        if(attacking.Count == 0)
        {
            state = UnitState.Idle;
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
        AddAttacking(obj.gameObject);
        obj.gameObject.GetComponent<Unit>().AddDefending(gameObject);
        attackTimer = Time.time + currentAttackSpeed;
    }

    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;
        if(currentHealth > 0)
        {
            HealthUpdate();
        }
        else
        {
            Die();
        }
    }
    protected void Die()
    {
        for(int i = 0; i < defending.Count; i++)
        {
            defending[i].GetComponent<Unit>().RemoveAttacking(gameObject);
            defending[i].GetComponent<Unit>().RemoveDefending(gameObject);
            defending[i].GetComponent<Unit>().SetState(UnitState.Idle);
        }
        Destroy(gameObject);
    }

    public void SetState(UnitState newState)
    {
        state = newState;
    }

    abstract public void Attack();

}
