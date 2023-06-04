using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerCtrl : MonoBehaviour
{
   
    private Transform tr;
    private Animation anime;

    public float moveSpeed = 10.0f;
    public float turnSpeed = 80.0f;

    private readonly float initHP = 100.0f;
    public float currHP;
    private Image hpBar;

    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        hpBar=GameObject.FindGameObjectWithTag("HP_BAR")?.GetComponent<Image>();
        currHP = initHP;
        tr = GetComponent<Transform>();
        anime = GetComponent<Animation>();
        anime.Play("Idle");
        turnSpeed = 0.0f;
        yield return new WaitForSeconds(0.3f);
        turnSpeed = 80.0f;
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float r = Input.GetAxis("Mouse X");

        //Debug.Log("h= " + h);
        //Debug.Log("v= " + v);
        Vector3 moveDir = (Vector3.forward  *v) + (Vector3.right * h);
        tr.Translate(moveDir.normalized*moveSpeed*Time.deltaTime);
        tr.Rotate(Vector3.up*turnSpeed*Time.deltaTime*r*10);
        PlayerAnime(h, v);
    }
    
    void PlayerAnime(float h, float v)
    {
        if(v>=0.1f){  anime.CrossFade("RunF",0.25f); }
        else if(v<=-0.1f){ anime.CrossFade("RunB", 0.25f); }
        else if (h >= 0.1f){ anime.CrossFade("RunL", 0.25f); }
        else if (h <= -0.1f){ anime.CrossFade("RunR", 0.25f); }
        else{ anime.CrossFade("Idle", 0.25f); }
    }
    void OnTriggerEnter(Collider coll)
    {
        if(currHP>=0.0f && coll.CompareTag("PUNCH"))
        {
            currHP -= 10.0f;
            DisplayHealth();
            Debug.Log($"Player hp = {currHP / initHP}");

            if (currHP <= 0.0f) PlayerDie();
        }
    }
    void PlayerDie()
    {

        Debug.Log("Player Die ! ! ! ! ! ");
        OnPlayerDie();


        //GameObject.Find("GameMgr").GetComponent<GameManager>().IsGameOver = true;


        //GameObject[] monsters = GameObject.FindGameObjectsWithTag("MONSTER");
        //foreach(GameObject monster in monsters)
        //{
        //    monster.SendMessage("OnPlayerDie",SendMessageOptions.DontRequireReceiver);
        //}
        GameManager.instance.IsGameOver = true;
    }
    void DisplayHealth()
    {
        hpBar.fillAmount = currHP / initHP;
    }

}
