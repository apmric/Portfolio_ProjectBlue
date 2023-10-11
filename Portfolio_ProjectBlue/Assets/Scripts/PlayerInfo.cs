using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo
{
    public PlayerInfo(string name, float maxHp, float speed)
    {
        this.name = name;
        this.maxHp = maxHp;
        this.currentHp = maxHp;
    }

    public string name { get; private set; }
    public float currentHp;
    public float maxHp { get; private set; }
    public float speed { get; private set; }

    public Skill[] skills = new Skill[2];
}
