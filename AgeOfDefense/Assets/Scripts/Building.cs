using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
    protected Slider slider;
    public float maxHealth;
    protected float currentHealth;
    protected List<GameObject> defending = new List<GameObject>();
    protected List<GameObject> aggrodBy = new List<GameObject>();
    private Color originalColor;
    public void Start()
    {
        originalColor = GetComponent<SpriteRenderer>().color;
        slider = transform.Find("Canvas/Slider").gameObject.GetComponent<Slider>();
        currentHealth = maxHealth;
        ChangeHealth(0);
    }

    // Update is called once per frame
    public void Update()
    {
        
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
        if (value > 0)
        {
            StartCoroutine(GreenGlow());
        }
        else if (value < 0)
        {
            StartCoroutine(RedGlow());
        }
        if (currentHealth <= 0)
        {
            Die();
        }
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        slider.value = (currentHealth / maxHealth);
    }

    public void AddDefending(GameObject obj)
    {
        defending.Add(obj);
    }

    public void RemoveDefending(GameObject obj)
    {
        defending.Remove(obj);
    }

    public void AddAggrodBy(GameObject obj)
    {
        aggrodBy.Add(obj);
    }

    public void RemoveAggrodBy(GameObject obj)
    {
        aggrodBy.Remove(obj);
    }

    public void Die()
    {
        for (int i = 0; i < defending.Count; i++)
        {
            defending[i].GetComponent<Unit>().RemoveIfAttacking(gameObject);
            defending[i].GetComponent<Unit>().SetState(UnitState.Idle);
        }
        for (int i = 0; i < aggrodBy.Count; i++)
        {
            aggrodBy[i].GetComponent<Unit>().RemoveAggro(gameObject);
        }
        Destroy(gameObject);
    }

    public float GetPercentHealth() { return currentHealth / maxHealth; }
}
