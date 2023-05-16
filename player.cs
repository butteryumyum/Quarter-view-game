using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{   
    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapons;


    public int ammo;
    public int coin;
    public int health;
    public int hasGrenades;

    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;



    float hAxis;
    float vAxis;

    bool wDown;
    bool jDown;
    bool dDown;
    bool iDown;
    bool sDown1;
    bool sDown2;
    bool sDown3;

    bool isJump;
    bool isDodge;
    bool isSwap;
    

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;


    GameObject nearObject;
    GameObject equipWeapon;
    int equipWeaponIndex = -1;

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
        Swap();
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
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
    }

    void Move()
    {
       moveVec = new Vector3(hAxis, 0, vAxis).normalized;

       if(isDodge)
            moveVec = dodgeVec;

        if(isSwap)
            moveVec = Vector3.zero; //스왑할때 못움직임

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
        if (jDown && !isJump && !dDown &&!isDodge &&!isSwap) {
            rigid.AddForce(Vector3.up * 16, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Dodge() //회피
    {
        if (dDown && moveVec != Vector3.zero && !isJump && !isDodge && !isDodge && !isSwap && !isSwap) {
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

    void Swap()
    {   
        if(sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if(sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if(sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;

        int weaponIndex = -1;
        if(sDown1) weaponIndex = 0;
        if(sDown2) weaponIndex = 1;
        if(sDown3) weaponIndex = 2;

        if((sDown1 || sDown2 || sDown3) && !isDodge && !isJump) {
            if(equipWeapon != null)
            equipWeapon.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon =  weapons[weaponIndex];
            equipWeapon.SetActive(true);

            anim.SetTrigger("doSwap"); 

            isSwap = true;

            Invoke ("SwapOut", 0.4f);
        }
    }
    void SwapOut()
    {
    
        isSwap = false;
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

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item") {
            Item item = other.GetComponent<Item>();
            switch (item.type) {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
                case Item.Type.Grenade:
                    hasGrenades += item.value;
                    if (hasGrenades > maxHasGrenades)
                        hasGrenades = maxHasGrenades;
                    break;
        }
        Destroy(other.gameObject);
    }
}


    void OnTriggerStay(Collider other)
    {
        if(other.tag == "Weapon") 
            nearObject = other.gameObject;

        
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
    } 

}
