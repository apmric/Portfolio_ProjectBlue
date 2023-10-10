using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
                RemainTime = 0;
            else
                RemainTime = value;

            if (coolTimeUI != null)
                coolTimeUI.text = RemainTime.ToString("F1");

            if (RemainTime == 0)
                isOn = true;
        }
    }

    public TextMeshProUGUI coolTimeUI;
    public bool isOn = true;

    public void Use()
    {
        remainTime = coolTime;
        isOn = false;
    }
}
