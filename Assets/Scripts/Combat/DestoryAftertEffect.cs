using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryAffertEffect : MonoBehaviour
{
    
    void Update()
    {
        if (!GetComponent<ParticleSystem>().IsAlive())
        {
            Destroy(gameObject);
        }
    }
}
