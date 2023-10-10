using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toki : Player
{
    public Toki(string name, float maxHp, float speed) : base(name, maxHp, speed)
    {
        
    }

    protected override void SkillQ()
    {
        base.SkillQ();

    }

    protected override void SkillE()
    {
        base.SkillE();

        if(s2Down)
        {
            isSkill = true;
            anim.SetTrigger("doSkillE");
            Invoke("SkillOut", 4f);
        }
    }

    protected override void SkillOut()
    {
        base.SkillOut();
    }
}
