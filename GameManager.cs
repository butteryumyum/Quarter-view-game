using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    public int enemyCntD;

    public Transform[] enemyZones;
    public GameObject[] enemies;
    public List<int> enemyList;

    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject overPanel;
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
    public Text curScoreText;
    public Text bestText;

    public void Awake()
    {   
        enemyList = new List<int>();
        maxScoreTxt.text = string.Format("{0:n0}" ,PlayerPrefs.GetInt("MaxScore"));

        if(PlayerPrefs.HasKey("MaxScore"))
            PlayerPrefs.SetInt("MaxScore", 0);
    }

    public void GameStart()
    {
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        
        player.gameObject.SetActive(true);
    }

    public void GameOver()
    {   
        gamePanel.SetActive(false);
        overPanel.SetActive(true);
        curScoreText.text = scoreTxt.text;

        int maxScore = PlayerPrefs.GetInt("MaxScore");
        if (player.score > maxScore) {
            bestText.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore", player.score);
        }
    }

    public void Restart() 
    {
        SceneManager.LoadScene(0);
    }
    public void StageStart()
    {   
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        startZone.SetActive(false);

        foreach(Transform zone in enemyZones)
            zone.gameObject.SetActive(true);
        Debug.Log("스폰존 활성");

        isBattle = true;
        StartCoroutine(InBattle());
    }

    public void StageEnd()
    {   
        player.transform.position = Vector3.up * 0.8f;

        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        startZone.SetActive(true);

         foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(false);
        Debug.Log("스폰존 비활성");
        
        isBattle = false;
        stage++;
    }

    IEnumerator InBattle()
    {   
        if(stage % 5 == 0) {
            enemyCntD++;
            GameObject instantEnemy = Instantiate(enemies[3], enemyZones[0].position, enemyZones[0].rotation);
            Enemy enemy = instantEnemy.GetComponent<Enemy>();
            enemy.target = player.transform;
            enemy.manager = this;
            boss = instantEnemy.GetComponent<Boss>();
        }
        else {
            for (int index=0; index < stage; index++) {
            int ran = Random.Range(0, 3);
            enemyList.Add(ran);

            switch(ran) {
                case 0: 
                    enemyCntA++;
                    break;
                case 1: 
                    enemyCntB++;
                    break;
                case 2: 
                    enemyCntC++;
                    break;
            }
        } 

        while (enemyList.Count > 0) {
            int ranZone = Random.Range(0, 4);
            GameObject instantEnemy = Instantiate(enemies[enemyList[0]], enemyZones[ranZone].position, enemyZones[ranZone].rotation);
            Enemy enemy = instantEnemy.GetComponent<Enemy>();
            enemy.target = player.transform;
            enemy.manager = this;
            enemyList.RemoveAt(0);
            yield return new WaitForSeconds(4f);
            
        }
    }

    while (enemyCntA + enemyCntB + enemyCntC + enemyCntD > 0) {
        yield return null;
    }

    yield return new WaitForSeconds(4f);

    boss = null;
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
        if(boss != null) {
            bossHealthGroup.anchoredPosition = Vector3.down * 30;
            bossHealthBar.localScale = new Vector3((float)boss.curHealth / boss.maxHealth, 1 , 1);
        }
        else {
            bossHealthGroup.anchoredPosition = Vector3.up * 200;
        }
    }
}

