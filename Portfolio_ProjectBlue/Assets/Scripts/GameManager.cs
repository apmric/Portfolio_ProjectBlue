using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject menuCam;
    public GameObject gameCam;
    public GameObject[] studentPrefabs;
    public Player player;
    public Transform respown;
    public Boss boss;
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
    public GameObject studentPanel;
    public GameObject gamePanel;
    public GameObject overPanel;
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
    public TextMeshProUGUI curScoreText;
    public TextMeshProUGUI bestScoreText;


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
    }

    void Update()
    {
        if(isBattle)
            playTime += Time.deltaTime;
    }

    void LateUpdate()
    {
        // ��� UI
        stageTxt.text = "STAGE " + stage;

        int hour = (int) (playTime / 3600);
        int min = (int) ((playTime - hour * 3600) / 60);
        int sec = (int) (playTime % 60);
        playTimeTxt.text = string.Format("{0:00}", hour) + ":" +
            string.Format("{0:00}", min) + ":" + string.Format("{0:00}", sec);

        // �÷��̾� UI
        if (player != null)
        {
            scoreTxt.text = string.Format("{0:n0}", player.score);
            playerHealthTxt.text = player.currentHp.ToString() + "/" + player.maxHp.ToString();
            playerAmmoTxt.text = player.equipWeapon.currentAmmo.ToString() + "/" + player.equipWeapon.maxAmmo.ToString();
            playerCoinTxt.text = string.Format("{0:n0}", player.coin);
        }

        // ���� ���� UI
        enemyATxt.text = enemyCntA.ToString();
        enemyBTxt.text = enemyCntB.ToString();
        enemyCTxt.text = enemyCntC.ToString();

        if (boss != null)
        {
            bossHealthGroup.anchoredPosition = Vector3.down * 30;
            // ���� ü�� UI
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

        isBattle = false;
        stage++;
    }

    IEnumerator InBattle()
    {
        if (stage % 5 == 0)
        {
            enemyCntD++;
            GameObject instantEnemy = Instantiate(enemies[3], enemyZones[0].position, enemyZones[0].rotation);
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
                GameObject instantEnemy = Instantiate(enemies[enemyList[0]], enemyZones[ranZone].position, enemyZones[ranZone].rotation);
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

        yield return new WaitForSeconds(4f);
        boss = null;
        StageEnd();
    }
}