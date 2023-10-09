using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toki : PlayerInfo
{
    public Toki(string name, float maxHp) : base(name, maxHp) 
    {
        skills[0] = new Skill("전술적 판단", 10f, 5f);
        skills[1] = new Skill("시스템: 아비 에슈흐", 0f, 10f);
    }
}
