using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject menuCam;
    public GameObject gameCam;
    public GameObject[] studentPrefabs;
    public Player player;
    public Transform respown;
    public Boss boss;
    public int stage;
    public float playTime;
    public bool isBattle;
    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;

    public GameObject menuPanel;
    public GameObject studentPanel;
    public GameObject gamePanel;
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

    void Awake()
    {
        instance = this;
    }

    public void GameStart()
    {
        menuPanel.SetActive(false);
        studentPanel.SetActive(true);
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

    void LateUpdate()
    {
        scoreTxt.text = "0";

        if(player != null)
        {
            playerHealthTxt.text = player.currentHp.ToString() + "/" + player.maxHp.ToString();
            playerAmmoTxt.text = player.equipWeapon.currentAmmo.ToString() + "/" + player.equipWeapon.maxAmmo.ToString();
        }
    }
}