using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo
{
    public PlayerInfo(string name, float maxHp)
    {
        this.name = name;
        this.maxHp = maxHp;
        this.currentHp = maxHp;
    }

    public string name { get; private set; }
    public float currentHp;
    public float maxHp { get; private set; }

    public Skill[] skills = new Skill[2];

    public virtual string SkillQ()
    {
        return "Q스킬";
    }

    public virtual string SkillE()
    {
        return "E스킬";
    }
}
