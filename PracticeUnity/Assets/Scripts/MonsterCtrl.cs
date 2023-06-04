using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class MonsterCtrl : MonoBehaviour
{
    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent agent;
    private Animator anime;
    private GameObject hitEffect;
    public enum State{ IDLE,PATROL,TRACE,ATTACK,DIE }
    public State state=State.IDLE;
    public float traceDist = 10.0f;
    public float attackDist = 2.0f;
    public bool isDie=false;

    private readonly int hashTrace = Animator.StringToHash("IsTrace");
    private readonly int hashAttack = Animator.StringToHash("IsAttack");
    private readonly int hashHit = Animator.StringToHash("Hit");
    private readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashDie = Animator.StringToHash("Die");

    private int hp = 100;
    // Start is called before the first frame update
    void OnEnable()
    {
        PlayerCtrl.OnPlayerDie += this.OnPlayerDie;
        StartCoroutine(CheckMonsterState());
        StartCoroutine(MonsterAction());
    }
    private void OnDisable()
    {
        PlayerCtrl.OnPlayerDie -= this.OnPlayerDie;
    }
    void Awake()
    {
        monsterTr = GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("PLAYER").GetComponent<Transform>();
        agent =  GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        anime = GetComponent<Animator>();
        hitEffect = Resources.Load<GameObject>("PoisonGas");
    }
    private void Update()
    {
        if(agent.remainingDistance>=2.0f)
        {
            Vector3 direction = agent.desiredVelocity;
            Quaternion rot = Quaternion.LookRotation(direction);
            monsterTr.rotation = Quaternion.Slerp(monsterTr.rotation, rot, Time.deltaTime * 10.0f);
        }
    }
    IEnumerator CheckMonsterState()
    {
        while(!isDie)
        {
            yield return new WaitForSeconds(0.3f);
            float distance =Vector3.Distance(playerTr.position,monsterTr.position);
            if (state == State.DIE) yield break;
            if(distance <=attackDist) { state= State.ATTACK; }
            else if (distance <=traceDist) { state = State.TRACE; }
            else { state = State.IDLE; }
        }
    }
    IEnumerator MonsterAction()
    {
        while(!isDie)
        {
            switch (state)
            {
                case State.IDLE:
                    agent.isStopped = true;
                    anime.SetBool(hashTrace, false);
                    break;

                case State.ATTACK: 
                    anime.SetBool(hashAttack, true);
                    break;

                case State.TRACE:
                    agent.SetDestination(playerTr.position);
                    agent.isStopped = false;
                    anime.SetBool(hashTrace, true);
                    anime.SetBool(hashAttack, false);
                    break;

                case State.DIE:
                    isDie = true;
                    agent.isStopped = true;
                    anime.SetTrigger(hashDie);
                    GetComponent<CapsuleCollider>().enabled = false;
                    yield return new WaitForSeconds(3.0f);
                    hp = 100;
                    isDie = false;
                    state = State.IDLE;
                    GetComponent<CapsuleCollider>().enabled = true;
                    this.gameObject.SetActive(false);
                    break;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("BULLET"))
        {
            Destroy(collision.gameObject);
            //anime.SetTrigger(hashHit);

            //Vector3 pos = collision.GetContact(0).point;
            //Quaternion rot = Quaternion.LookRotation(-collision.GetContact(0).normal);
            //ShowHitEffect(pos, rot);
            //hp -= 10;
            //if (hp <= 0)
            //{
            //    state = State.DIE;
            //    GameManager.instance.DisplayScore(50);
            //} 
        }
    }
    public void OnDamage(Vector3 pos, Vector3 normal)
    {
        anime.SetTrigger(hashHit);
        Quaternion rot = Quaternion.LookRotation(normal);
        ShowHitEffect(pos, rot);
        hp -= 30;
        if (hp < 0)
        {
            state = State.DIE;
            GameManager.instance.DisplayScore(50);
        }    
    }
    void ShowHitEffect(Vector3 pos, Quaternion rot)
    {
        GameObject gas = Instantiate<GameObject>(hitEffect, pos, rot, monsterTr);
        Destroy(gas, 1.0f) ;
    }
    private void OnDrawGizmos()
    {
        if(state==State.TRACE)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, traceDist);
        }

        if(state==State.ATTACK) 
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackDist);
        }

    }
    void OnPlayerDie()
    {
        StopAllCoroutines();
        agent.isStopped = true;
        anime.SetFloat(hashSpeed, Random.Range(0.0f, 1.5f));
        anime.SetTrigger(hashPlayerDie);
       
    }
    
}
