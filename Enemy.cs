using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{   
    public enum Type {A, B , C};
    public Type enemyType;
    public int maxHealth;
    public int curHealth;
    public Transform target;
    public BoxCollider meleeArea;
    public GameObject bullet;
    public bool isChase;
    public bool isAttack;
    

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
       
        
        if(nav.enabled){
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }
            
    }

    void FreezeVelocity()
    {
        if (isChase){
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    void Targerting() //몹 사거리
    {
        float targetRadius = 0;
        float targetRange = 0;

        switch (enemyType) {
            case Type.A: 
                targetRadius = 1f;
                targetRange = 3f;//감지 범위
                break;
            case Type.B:
                targetRadius = 0.7f;
                targetRange = 10f;
                break;
            case Type.C:
                targetRadius = 0.5f;
                targetRange = 25f;

                break;

        }
       
       

         RaycastHit[] rayHits = 
            Physics.SphereCastAll(transform.position,
                                targetRadius, 
                                transform.forward,
                                targetRange, 
                                LayerMask.GetMask("Player"));
        if(rayHits.Length > 0 && !isAttack) {
            StartCoroutine(Attack());
        }
    }   

    IEnumerator Attack()
    {   
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true); 

        switch(enemyType) {
            case Type.A: //일반 몹
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true; //공격 활성화

                yield return new WaitForSeconds(1f); 
                meleeArea.enabled = false; //공격 비활성화

                yield return new WaitForSeconds(1f);
                isChase = true;
                isAttack = false;
                anim.SetBool("isAttack", false);
            break;
            case Type.B: //돌격형 몹
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(transform.forward * 20,ForceMode.Impulse); //돌진
                meleeArea.enabled = true;

                yield return new WaitForSeconds(0.5f); 
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;

                yield return new WaitForSeconds(1.7f);
            break;
            case Type.C: //원거리 몹
                yield return new WaitForSeconds(0.5f); 
                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20;

                yield return new WaitForSeconds(2f);
            break;
            
        }
        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false); 
       
    }

    void FixedUpdate() 
    {   
        Targerting();
        FreezeVelocity();
        rigid.angularVelocity = Vector3.zero;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee" ) {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec, false));

            Debug.Log("Melee :" + curHealth);//근거리 공격
        }
        else if (other.tag == "Bullet") {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec, false));
            Destroy(other.gameObject);
            
            Debug.Log("Range :" + curHealth);//원거리 공격
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

                rigid.freezeRotation = false; //수류탄 맞았을때 날라가는 모션
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
