using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject menuCam;
    public GameObject gameCam;
    public Player player;
    public Boss boss;
    public GameObject itemShop;
    public GameObject weaponShop;
    public GameObject startZone;

    public int stage;
    public float playTime;
    public bool isBattle;
    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;

    public GameObject menuPanel;
    public GameObject gamePanel;
    public Text maxScoreTxt;
    public Text scoreTxt;
    public Text stageTxt;
    public Text playTimeTxt;
    public Text playerHealthTxt;
    public Text playerAmmoTxt;
    public Text playerCoinTxt;
    public Image weapon1Img;
    public Image weapon2Img;
    public Image weapon3Img;
    public Image weaponRImg;
    public Text enemyATxt;
    public Text enemyBTxt;
    public Text enemyCTxt;
    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;

    public void Awake()
    {
        maxScoreTxt.text = string.Format("{0:n0}" ,PlayerPrefs.GetInt("MaxScore"));
    }

    public void GameStart()
    {
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
    }

    public void StageStart()
    {   
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        startZone.SetActive(false);

        isBattle = true;
        StartCoroutine(InBattle());
    }

    public void StageEnd()
    {   
        player.transform.position = Vector3.up * 0.8f;

        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        startZone.SetActive(true);

        
        isBattle = false;
        stage++;
    }

    IEnumerator InBattle()
    {
        yield return new WaitForSeconds(5);
        StageEnd();
    }

    void Update()
    {
        if(isBattle)
            playTime += Time.deltaTime; //전투 중일때만 시간세기
    }

    void LateUpdate()
    {
        scoreTxt.text = string.Format("{0:n0}" ,player.score);
        stageTxt.text = "STAGE " + stage; //현재 스테이지

        int hour = (int)(playTime / 3600); //플레이타임 계산
        int min = (int)((playTime - hour * 3600)  /60);
        int second = (int)(playTime % 60);

        playTimeTxt.text = string.Format("{0:00}" ,hour) + ":" + string.Format("{0:00}" ,min) + ":" + string.Format("{0:00}" ,second);
        
        playerHealthTxt.text = player.health + " / " + player.maxHealth; //player 체력
        playerCoinTxt.text = string.Format("{0:n0}" ,player.coin); // player 코인
        if(player.equipWeapon == null) //무기 여부
            playerAmmoTxt.text = "- / " + player.ammo;
        else if (player.equipWeapon.type == Weapon.Type.Melee)
            playerAmmoTxt.text = "- / " + player.ammo;
        else 
            playerAmmoTxt.text = player.equipWeapon.curAmmo + " / " + player.ammo; 

        //무기 ui
        weapon1Img.color = new Color(1, 1, 1, player.hasWeapons[0] ? 1 : 0);
        weapon2Img.color = new Color(1, 1, 1, player.hasWeapons[1] ? 1 : 0);
        weapon3Img.color = new Color(1, 1, 1, player.hasWeapons[2] ? 1 : 0);
        weaponRImg.color = new Color(1, 1, 1, player.hasGrenades > 0 ? 1 : 0);
        //잡몹 ui
        enemyATxt.text = enemyCntA.ToString();
        enemyBTxt.text = enemyCntB.ToString();
        enemyCTxt.text = enemyCntC.ToString();
        //보스체력바 변화
        bossHealthBar.localScale = new Vector3((float)boss.curHealth / boss.maxHealth, 1 , 1);
    }
}

