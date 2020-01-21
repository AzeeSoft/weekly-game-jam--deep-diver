using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderwaterCreature : UnderwaterRigidbodyEntity
{
    public float damage = 10f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Diver"))
        {
            var diverModel = other.GetComponent<DiverModel>();
            diverModel.health.TakeDamage(damage);
        }
    }
}
