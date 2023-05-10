using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{   
    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapons;

    float hAxis;
    float vAxis;

    bool wDown;
    bool jDown;
    bool dDown;
    bool iDown;

    bool isJump;
    bool isDodge;
    

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;


    GameObject nearObject;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

  
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Dodge();
        Interation();
    }


    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk"); //걷기
        jDown = Input.GetButtonDown("Jump"); //점프
        dDown = Input.GetButtonDown("Dodge"); //구르기
        iDown = Input.GetButtonDown("Interation"); //상호작용
    }

    void Move()
    {
       moveVec = new Vector3(hAxis, 0, vAxis).normalized;

       if(isDodge)
            moveVec = dodgeVec;

         //걷기 속도감소 및 뛰기
            transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;
        

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown); 
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);
    }

    void Jump()
    {
        if (jDown && !isJump && !dDown &&!isDodge ) {
            rigid.AddForce(Vector3.up * 16, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Dodge() //회피
    {
        if (dDown && moveVec != Vector3.zero && !isJump && !isDodge && !isDodge) {
            dodgeVec = moveVec;            
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;
            Debug.Log("회피");

            Invoke ("DodgeOut", 0.5f);
        }
    }
    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }

    void Interation()
    {
        if(iDown && nearObject != null && !isJump && !isDodge) {
            if(nearObject.tag == "Weapon") {
                Item item =  nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
        } 
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor") {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(other.tag == "Weapon") 
            nearObject = other.gameObject;

        Debug.Log(nearObject.name);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
    }
}
