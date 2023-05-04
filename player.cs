using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{   
    public float speed;
    float hAxis;
    float vAxis;
    bool wDown;
    bool jDown;
    bool dDown;

    bool isJump;
    bool isDodge;
    

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

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
    }


    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk"); //걷기
        jDown = Input.GetButtonDown("Jump"); //점프
        dDown = Input.GetButtonDown("Dodge"); 
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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor") {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }
}
