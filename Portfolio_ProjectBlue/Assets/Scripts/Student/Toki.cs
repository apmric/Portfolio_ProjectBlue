using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toki : Player
{
    // 변신할 때 쓰이는 에니메이션 컨트롤러
    public RuntimeAnimatorController[] transfrom;
    // 변신할 때 쓰이는 무기
    public Weapon[] transWeapon;
    // 변신 했는지, 안했는지
    bool isTransfrom;
    bool isUseUltimate;

    protected override void Awake()
    {
        base.Awake();
        skills[0] = new Skill("회피기", 0f, dodgeTime);
        skills[1] = new Skill("전술적 판단", 30f, 15f);
    }

    protected override void SkillQ()
    {
        base.SkillQ();

        if (s1Down && isUltimateReady & !isUseUltimate && !isDead && !isShop)
        {
            exSkillSound.Play();

            isTransfrom = true;
            isUseUltimate = true;
            maxHp = 200;
            currentHp = 200;
            equipWeapon.currentAmmo = 99;
            equipWeapon.maxAmmo = 99;

            foreach (Transform mesh in transforms)
            {
                if (mesh.name == "CH0187_A_Body")
                    mesh.GetComponent<SkinnedMeshRenderer>().enabled = false;
                if (mesh.name == "CH0187_B_Body")
                    mesh.GetComponent<SkinnedMeshRenderer>().enabled = true;
            }

            equipWeapon = transWeapon[Convert.ToInt32(isTransfrom)];
            anim.runtimeAnimatorController = transfrom[Convert.ToInt32(isTransfrom)];
        }
        
        if (isUseUltimate)
        {
            if (ulGauge > 0)
            {
                ulGauge -= Time.deltaTime * 10f;
            }
            else if (ulGauge <= 0)
            {
                exSkillOut();
            }
        }
    }

    protected override void SkillE()
    {
        base.SkillE();

        if(s2Down && skills[1].isOn && !isUseUltimate && !isDead && !isShop)
        {
            commonSkillSound.Play();

            UseSkill(1);

            isSkill = true;
            anim.SetTrigger("doSkillE");

            Bullet bullet = equipWeapon.bullet.GetComponent<Bullet>();
            bullet.damage = 14;

            equipWeapon.currentAmmo = 30;

            StartCoroutine(SkillShot());

            Invoke("SkillOut", 3f);
        }
    }

    protected override void SkillOut()
    {
        base.SkillOut();
        Bullet bullet = equipWeapon.bullet.GetComponent<Bullet>();
        bullet.damage = 7;
    }

    void exSkillOut()
    {
        isTransfrom = false;
        isUltimateReady = false;
        isUseUltimate = false;
        equipWeapon = transWeapon[Convert.ToInt32(isTransfrom)];

        maxHp = 100;
        currentHp = 100;
        equipWeapon.currentAmmo = 30;
        equipWeapon.maxAmmo = 30;

        foreach (Transform mesh in transforms)
        {
            if (mesh.name == "CH0187_A_Body")
                mesh.GetComponent<SkinnedMeshRenderer>().enabled = true;
            if (mesh.name == "CH0187_B_Body")
                mesh.GetComponent<SkinnedMeshRenderer>().enabled = false;

        }

        anim.runtimeAnimatorController = transfrom[Convert.ToInt32(isTransfrom)];
    }

    IEnumerator SkillShot()
    {
        yield return new WaitForSeconds(1f);

        while (isSkill)
        {
            Debug.Log(isSkill);
            equipWeapon.Use();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
