using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed, lifetime;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * Time.deltaTime * speed;
    }
}
