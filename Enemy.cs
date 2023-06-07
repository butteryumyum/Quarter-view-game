using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;
    public Transform target;
    public bool isChase;
    

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;
    NavMeshAgent nav;
    Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        

        Invoke("ChaseStart", 2);
    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }

    void Update()
    {   
       
        
        if(isChase)
            nav.SetDestination(target.position);
    }

    void FreezeVelocity()
    {
        if (isChase){
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    void FixedUpdate() 
    {
        FreezeVelocity();
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee" ) {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec, false));

            Debug.Log("Melee :" + curHealth);
        }
        else if (other.tag == "Bullet") {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec, false));
            Destroy(other.gameObject);
            
            Debug.Log("Range :" + curHealth);
        }
    }

    public void HitByGrenade(Vector3 explosionPos) //수류탄 터졌을떄 
    {
        curHealth -= 100;
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(OnDamage(reactVec, true));
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade) // 수류탄 이외 다른것을 이용해 데미지를 입음
    {
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if(curHealth > 0){
            mat.color = Color.white;
        }
        else {
            mat.color = Color.gray;
            gameObject.layer = 12; //적 사망
            isChase = false; 
            nav.enabled = false;
            anim.SetTrigger("doDie");
            
            if (isGrenade)  { //수류탄에 맞았을 경우 코루틴
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3; 

                rigid.freezeRotation = false; //수류탄 맞았을때 회전
                rigid.AddForce(reactVec * 5,ForceMode.Impulse);
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse); 
            }
            else {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up; 
                rigid.AddForce(reactVec * 5,ForceMode.Impulse);
                 
            }
            
            Destroy(gameObject, 4);
        }

    }
}
