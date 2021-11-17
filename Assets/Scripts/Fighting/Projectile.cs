using System;
using System.Collections;
using System.Collections.Generic;
using RPG.HealthObject;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    
    [SerializeField] float speed = 1;
    [SerializeField] bool isHoming = true;
    [SerializeField] GameObject hitEffect = null;
    [SerializeField] float MaxLifeTime = 5f;
    [SerializeField] GameObject[] destroyOnHit = null;
    [SerializeField] float lifeAfterImpact = 0;
    [SerializeField] UnityEvent onHit;


    Health target = null;
    GameObject instigator = null;
    float damage = 0;

    private void Start() 
    {
        transform.LookAt(GetAimLocation());
    }

    void Update()
    {
        if (target == null)return;
        if (isHoming && !target.Killed())
        {
            transform.LookAt(GetAimLocation());
        }
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    public void SetTarget(Health target, GameObject instigator, float damage)
    {
        this.target = target;
        this.instigator = instigator;
        this.damage = damage;

        Destroy(gameObject, MaxLifeTime);
    }

    private Vector3 GetAimLocation()
    {
        CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
        if ( targetCapsule == null)
        {
            return target.transform.position;
        }
        return target.transform.position + Vector3.up * targetCapsule.height/4;
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Health>() != target || target.Killed() )
        {
           return;
        }

        target.DamageHealth(instigator, damage);

        onHit.Invoke();

        if(hitEffect != null)
        {
            Instantiate(hitEffect, GetAimLocation(), transform.rotation);
        }

        foreach (GameObject toDestroy in destroyOnHit)
        {
            Destroy(toDestroy);
        }
        Destroy(gameObject, lifeAfterImpact);
        
    }
}
