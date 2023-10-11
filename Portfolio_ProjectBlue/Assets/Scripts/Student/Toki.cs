using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toki : Player
{
    public RuntimeAnimatorController transfrom;

    protected override void Awake()
    {
        base.Awake();
        skills[0] = new Skill("회피기", 0f, dodgeTime);
        skills[1] = new Skill("전술적 판단", 30f, 15f);
    }

    protected override void SkillQ()
    {
        base.SkillQ();

        if (s1Down)
        {
            anim.SetTrigger("doSkillQ");

            foreach (Transform mesh in transforms)
            {
                if (mesh.name == "CH0187_A_Body")
                    mesh.GetComponent<SkinnedMeshRenderer>().enabled = false;
                if (mesh.name == "CH0187_B_Body")
                    mesh.GetComponent<SkinnedMeshRenderer>().enabled = true;
            }

            anim.runtimeAnimatorController = transfrom;

            //Invoke("SkillOut", 3f);
        }
    }

    protected override void SkillE()
    {
        base.SkillE();

        if(s2Down && skills[1].isOn)
        {
            UseSkill(1);

            isSkill = true;
            anim.SetTrigger("doSkillE");

            StartCoroutine(SkillShot());

            Invoke("SkillOut", 3f);
        }
    }

    protected override void SkillOut()
    {
        base.SkillOut();

        foreach (Transform mesh in transforms)
        {
            if (mesh.name == "CH0187_A_Body")
                mesh.GetComponent<SkinnedMeshRenderer>().enabled = true;
            if (mesh.name == "CH0187_B_Body")
                mesh.GetComponent<SkinnedMeshRenderer>().enabled = false;
        }
    }

    IEnumerator SkillShot()
    {
        yield return new WaitForSeconds(1f);

        while (isSkill)
        {
            Debug.Log(isSkill);
            equipWeapon.Use();
            yield return new WaitForSeconds(0.3f);
        }
    }
}
