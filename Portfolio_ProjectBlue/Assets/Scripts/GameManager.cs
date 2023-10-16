using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("# Manager")]
    public PoolManager poolManager;

    [Header("# Camera")]
    public GameObject menuCam;
    public GameObject gameCam;

    [Header("# Player")]
    public GameObject[] studentPrefabs;
    public Player player;
    public Transform respown;

    [Header("# Enemy")]
    public Transform[] enemyZones;
    public GameObject[] enemies;
    public List<int> enemyList;
    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;
    public int enemyCntD;
    public Boss boss;

    [Header("# Stage")]
    public GameObject startZone;
    public int stage;
    public float playTime;
    public bool isBattle;

    [Header("# Shop")]
    public int[] itemPrice;
    public GameObject shopText;

    [Header("# Panel")]
    public GameObject menuPanel;
    public GameObject studentPanel;
    public GameObject gamePanel;
    public GameObject overPanel;
    public GameObject clearPanel;
    public TextMeshProUGUI maxScoreTxt;
    public TextMeshProUGUI scoreTxt;
    public TextMeshProUGUI stageTxt;
    public TextMeshProUGUI playTimeTxt;
    public TextMeshProUGUI playerHealthTxt;
    public TextMeshProUGUI playerAmmoTxt;
    public TextMeshProUGUI playerCoinTxt;
    public TextMeshProUGUI enemyATxt;
    public TextMeshProUGUI enemyBTxt;
    public TextMeshProUGUI enemyCTxt;
    public Image[] coolTimeImg;
    public TextMeshProUGUI[] coolTimeTxt;
    public RectTransform ultimateBar;
    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;
    public RectTransform shopUiGroup;
    public TextMeshProUGUI curScoreText;
    public TextMeshProUGUI bestScoreText;

    [Header("# Sound")]
    public AudioSource menuSound;
    public AudioSource backGroundSound;

    void Awake()
    {
        instance = this;
        enemyList = new List<int> { };
        maxScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));

        if (PlayerPrefs.HasKey("MaxScore"))
            PlayerPrefs.SetInt("MaxScore", 0);
    }

    public void GameStart()
    {
        menuPanel.SetActive(false);
        studentPanel.SetActive(true);
    }

    public void GameOver()
    {
        gamePanel.SetActive(false);
        overPanel.SetActive(true);
        curScoreText.text = scoreTxt.text;

        int maxScore = PlayerPrefs.GetInt("MaxScore");
        if (player.score > maxScore)
        {
            bestScoreText.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore", player.score);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void ChoiceStudent(int student)
    {
        switch (student)
        {
            case 0:
                player = Instantiate(studentPrefabs[0]).GetComponent<Player>();
                player.transform.position = respown.position;
                break;
            //case 1:
            //    player = Instantiate(studentPrefabs[1]).GetComponent<Player>();
            //    player.transform.position = respown.position;
            //    break;
            //case 2:
            //    player = Instantiate(studentPrefabs[2]).GetComponent<Player>();
            //    player.transform.position = respown.position;
            //    break;
            //case 3:
            //    player = Instantiate(studentPrefabs[3]).GetComponent<Player>();
            //    player.transform.position = respown.position;
            //    break;
            //case 4:
            //    player = Instantiate(studentPrefabs[4]).GetComponent<Player>();
            //    player.transform.position = respown.position;
            //    break;
        }

        studentPanel.SetActive(false);
        menuCam.SetActive(false);
        gameCam.SetActive(true);
        gamePanel.SetActive(true);
        player.gameObject.SetActive(true);

        menuSound.Stop();
        backGroundSound.Play();
    }

    void Update()
    {
        if(isBattle)
            playTime += Time.deltaTime;
    }

    void LateUpdate()
    {
        // 상단 UI
        stageTxt.text = "STAGE " + stage;

        int hour = (int) (playTime / 3600);
        int min = (int) ((playTime - hour * 3600) / 60);
        int sec = (int) (playTime % 60);
        playTimeTxt.text = string.Format("{0:00}", hour) + ":" +
            string.Format("{0:00}", min) + ":" + string.Format("{0:00}", sec);

        // 플레이어 UI
        if (player != null)
        {
            scoreTxt.text = string.Format("{0:n0}", player.score);
            playerHealthTxt.text = player.currentHp.ToString() + "/" + player.maxHp.ToString();
            playerAmmoTxt.text = player.equipWeapon.currentAmmo.ToString() + "/" + player.equipWeapon.maxAmmo.ToString();
            playerCoinTxt.text = string.Format("{0:n0}", player.coin);
        }

        // 몬스터 숫자 UI
        enemyATxt.text = enemyCntA.ToString();
        enemyBTxt.text = enemyCntB.ToString();
        enemyCTxt.text = enemyCntC.ToString();

        if (boss != null)
        {
            bossHealthGroup.anchoredPosition = Vector3.down * 30;
            // 보스 체력 UI
            bossHealthBar.localScale = new Vector3((float)boss.curHealth / boss.maxHealth, 1, 1);
        }
        else
        {
            bossHealthGroup.anchoredPosition = Vector3.up * 200;
        }
    }

    public void StageStart()
    {
        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(true);

        startZone.SetActive(false);

        isBattle = true;
        StartCoroutine(InBattle());
    }

    public void StageEnd()
    {
        player.transform.position = Vector3.up * 0.8f;

        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(false);

        startZone.SetActive(true);
        ShopEnter();

        isBattle = false;
        stage++;
    }

    IEnumerator InBattle()
    {
        if (stage % 5 == 0)
        {
            enemyCntD++;
            GameObject instantEnemy = poolManager.GetPool(enemies[3]);
            instantEnemy.transform.position = enemyZones[0].position;
            instantEnemy.transform.rotation = enemyZones[0].rotation;
            Enemy enemy = instantEnemy.GetComponent<Enemy>();
            enemy.target = player.transform;
            boss = instantEnemy.GetComponent<Boss>();
        }
        else
        {
            for (int index = 0; index < stage; index++)
            {
                int ran = Random.Range(0, 3);
                enemyList.Add(ran);

                switch (ran)
                {
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

            while (enemyList.Count > 0)
            {
                int ranZone = Random.Range(0, 4);

                GameObject instantEnemy = poolManager.GetPool(enemies[enemyList[0]]);
                instantEnemy.transform.position = enemyZones[ranZone].position;
                instantEnemy.transform.rotation = enemyZones[ranZone].rotation;

                Enemy enemy = instantEnemy.GetComponent<Enemy>();
                enemy.target = player.transform;

                enemyList.RemoveAt(0);

                yield return new WaitForSeconds(5f);
            }
        }

        while(enemyCntA + enemyCntB + enemyCntC + enemyCntD > 0)
        {
            yield return null;
        }

        clearPanel.gameObject.SetActive(true);

        yield return new WaitForSeconds(4f);
        boss = null;
        clearPanel.gameObject.SetActive(false);
        StageEnd();
    }

    public void ShopEnter()
    {
        shopUiGroup.anchoredPosition = Vector3.zero;
        player.isShop = true;
    }

    public void ShopExit()
    {
        shopUiGroup.anchoredPosition = Vector3.down * 1000;
        player.isShop = false;
    }

    public void ShopBuy(int index)
    {
        int price = itemPrice[index];
        if (price > player.coin) 
        {
            shopText.gameObject.SetActive(true);
            return;
        }

        shopText.gameObject.SetActive(false);

        player.coin -= price;

        switch (index)
        {
            case 0:
                player.currentHp = 100;
                break;
            case 1:
                Bullet bullet = player.equipWeapon.bullet.GetComponent<Bullet>();
                bullet.damage += 7;
                break;
            case 2:
                player.ulGauge = 100;
                break;
        }
    }
}