using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Skill
{
    public Skill(string name, float damage, float coolTime)
    {
        this.name = name;
        this.damage = damage;
        this.coolTime = coolTime;
    }

    public string name { get; private set; }
    public float damage { get; private set; }
    public float coolTime;
    public float RemainTime = 0;
    public float remainTime
    {
        get
        {
            return RemainTime;
        }
        set
        {
            if (value < 0)
            {
                coolTimeImage.gameObject.SetActive(false);
                RemainTime = 0;
            }
            else
            {
                coolTimeImage.gameObject.SetActive(true);
                RemainTime = value;
            }

            if (coolTimeUI != null)
            {
                coolTimeUI.text = RemainTime.ToString("F1");
            }

            if (RemainTime == 0)
                isOn = true;
        }
    }

    public Image coolTimeImage;
    public TextMeshProUGUI coolTimeUI;
    public bool isOn = true;

    public void Use()
    {
        remainTime = coolTime;
        isOn = false;
    }
}
