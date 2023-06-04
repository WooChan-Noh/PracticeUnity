using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour
{
    public GameObject expEffect;
    public Texture[] textures;
    private new MeshRenderer renderer;
    private Transform tr;
    private Rigidbody rb;

    private int cnt = 0;
    public float radius = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
        tr=GetComponent<Transform>(); 
        rb=GetComponent<Rigidbody>();
        renderer = GetComponentInChildren<MeshRenderer>();    
        int idx = Random.Range(0, textures.Length);
        renderer.material.mainTexture = textures[idx];
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("BULLET"))
        {
            cnt++;
            if(cnt == 3) { ExpBarrel(); }
        }
    }
    void ExpBarrel()
    {
        GameObject exp =Instantiate(expEffect,tr.position,Quaternion.identity);
        Destroy(exp, 0.5f) ;

        //rb.mass = 1.0f;
        //rb.AddForce(Vector3.up * 1500.0f);

        IndirectDamage(tr.position);
        Destroy(gameObject, 3.0f);
    }
    void IndirectDamage(Vector3 pos)
    {
        Collider[] colls = Physics.OverlapSphere(pos, radius, 1 << 3);
        foreach(var coll in colls)
        {
            rb=coll.GetComponent<Rigidbody>();
            rb.mass = 1.0f;
            rb.constraints = RigidbodyConstraints.None;
            rb.AddExplosionForce(1500.0f, pos, radius, 1200f);
        }
    }
    // Update is called once per frame
   
}
