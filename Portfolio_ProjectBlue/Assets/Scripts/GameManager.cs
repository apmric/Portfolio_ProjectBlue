using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject menuCam;
    public GameObject gameCam;
    public Player[] player;
    public Boss boss;
    public int stage;
    public float playTime;
    public bool isBattle;
    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;

    public GameObject menuPanel;
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
    public RectTransform ultimateBar;
    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;

    void Awake()
    {

    }

    public void GameStart()
    {
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player[0].gameObject.SetActive(true);
    }

    void LateUpdate()
    {
        scoreTxt.text = "0";

        playerHealthTxt.text = player[0].currentHp.ToString() + " / " + player[0].maxHp.ToString();
        playerAmmoTxt.text = player[0].equipWeapon.currentAmmo.ToString() + " / " + player[0].equipWeapon.maxAmmo.ToString();
    }
}
