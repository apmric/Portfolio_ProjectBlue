using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System;
using System.Reflection;

public class Player : MonoBehaviour
{
    public float currentHp;
    public float maxHp;
    public float speed;
    public float dodgeTime;

    public Skill[] skills = new Skill[2];

    Image[] skillCoolTimeImgs = new Image[2];
    TextMeshProUGUI[] skillCoolTimeUIs = new TextMeshProUGUI[2];
    Camera followCamera;

    float hAxis;
    float vAxis;

    protected bool wDown;
    protected bool fDown;
    protected bool s1Down;
    protected bool s2Down;
    protected bool rDown;
    protected bool jDown;

    protected bool isFireReady = true;
    protected bool isSkill;
    protected bool isReload;
    protected bool isDodge;
    protected bool isDamage;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    protected Animator anim;
    protected MeshRenderer[] meshs;
    protected Transform[] transforms;

    public Weapon equipWeapon;
    float fireDelay;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        equipWeapon = GetComponentInChildren<Weapon>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        transforms = GetComponentsInChildren<Transform>();

        followCamera = GameManager.instance.gameCam.gameObject.GetComponent<Camera>();
        skillCoolTimeUIs = GameManager.instance.coolTimeTxt;
        skillCoolTimeImgs = GameManager.instance.coolTimeImg;
    }

    private void Start()
    {
        for (int i = 0; i < 2; i++)
        {
            skills[i].coolTimeImage = skillCoolTimeImgs[i];
            skills[i].coolTimeUI = skillCoolTimeUIs[i];
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        GetInput();
        Move();
        Dodge();
        Turn();
        Attack();
        SkillQ();
        SkillE();
        Reload();

        for (int i = 0; i < 2; i++)
        {
            if (!skills[i].isOn)
            {
                skills[i].remainTime -= Time.deltaTime; // 프레임과 프레임 사이 걸린 시간
            }
        }
    }

    void FixedUpdate()
    {
        FreezeRotation();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        jDown = Input.GetButton("Jump");
        fDown = Input.GetButton("Fire1");
        s1Down = Input.GetButton("SkillQ");
        s2Down = Input.GetButton("SkillE");
        rDown = Input.GetButtonDown("Reload");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if (isDodge)
            moveVec = dodgeVec;

        if (!isFireReady || isSkill)
        {
            moveVec = Vector3.zero;
        }

        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;

        anim.SetBool("isRun", moveVec != Vector3.zero);
    }

    void Turn()
    {
        // 키보드에 의한 회전
        this.transform.LookAt(this.transform.position + moveVec);

        // 마우스에 의한 회전
        if(fDown && equipWeapon.currentAmmo > 0)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);

            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - this.transform.position;
                nextVec.y = 0;
                this.transform.LookAt(this.transform.position + nextVec);
            }
        }
    }

    void Dodge()
    {
        if(jDown && !isDodge && skills[0].isOn)
        {
            UseSkill(0);

            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.5f);
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }

    void Attack()
    {
        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if (fDown && isFireReady && equipWeapon.currentAmmo > 0 && !isSkill)
        {
            equipWeapon.Use();
            anim.SetTrigger("doShot");
            fireDelay = 0;
        }
    }

    protected virtual void SkillQ()
    {

    }

    protected virtual void SkillE()
    {

    }

    protected virtual void SkillOut()
    {
        isSkill = false;
    }

    public void UseSkill(int index)
    {
        if (!skills[index].isOn)
            return;

        skills[index].Use();
    }

    void Reload()
    {
        if (isReload)
            return;

        if(rDown && isFireReady)
        {
            anim.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 2f);
        }
    }

    void ReloadOut()
    {
        equipWeapon.currentAmmo = equipWeapon.maxAmmo;

        isReload = false;
    }

    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBullet"))
        {
            Bullet enemyBullet = other.GetComponent<Bullet>();
            currentHp -= enemyBullet.damage;

            bool isBossAtk = other.name == "Boss Melee Area";

            StartCoroutine(OnDamage(isBossAtk));
        }

        if (other.GetComponent<Rigidbody>() != null)
            Destroy(other.gameObject);
    }

    IEnumerator OnDamage(bool isBossAtk)
    {
        isDamage = true;

        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.yellow;
        }

        if (isBossAtk)
            rigid.AddForce(this.transform.forward * -25, ForceMode.Impulse);

        yield return new WaitForSeconds(1f);

        isDamage = false;

        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }

        if (isBossAtk)
            rigid.velocity = Vector3.zero;
    }
}
