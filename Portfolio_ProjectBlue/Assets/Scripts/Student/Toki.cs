using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toki : PlayerInfo
{
    public Toki(string name, float maxHp) : base(name, maxHp) 
    {
        skills[0] = new Skill("������ �Ǵ�", 10f, 5f);
        skills[1] = new Skill("�ý���: �ƺ� ������", 0f, 10f);
    }
}
