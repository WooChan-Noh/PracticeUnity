using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    public GameObject exploEffect;
   void OnCollisionEnter(Collision collision) 
   { 
        if(collision.collider.CompareTag("BULLET"))
        {
            GameObject expl =Instantiate(exploEffect,collision.transform.position, Quaternion.identity);
            Destroy(expl, 0.5f);
            Destroy(collision.gameObject);
            
        }
   }
}
