using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("# PlayerInfo")]
    public float currentHp;
    public float maxHp;
    public float ulGauge;
    public float ulMaxGauge;
    public float speed;
    public float dodgeTime;
    public Weapon equipWeapon;
    float fireDelay;

    [Header("# PlayerSound")]
    public AudioSource shotSound;
    public AudioSource battleInSound;
    public AudioSource damageSound;
    public AudioSource moveSound;
    public AudioSource retireSound;
    public AudioSource commonSkillSound;
    public AudioSource exSkillSound;
    public AudioSource reloadSound;

    [Header("# UI")]
    public int score;
    public int coin;
    public int maxCoin;

    [Header("# PlayerSkill")]
    public Skill[] skills = new Skill[2];
    Image[] skillCoolTimeImgs = new Image[2];
    TextMeshProUGUI[] skillCoolTimeUIs = new TextMeshProUGUI[2];
    Camera followCamera;

    float hAxis;
    float vAxis;

    protected bool fDown;
    protected bool s1Down;
    protected bool s2Down;
    protected bool rDown;
    protected bool jDown;

    protected bool isFireReady = true;
    protected bool isUltimateReady;
    protected bool isSkill;
    protected bool isReload;
    protected bool isDodge;
    protected bool isDamage;
    protected bool isDead;
    public bool isShop;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    protected Animator anim;
    protected SkinnedMeshRenderer[] meshs;
    protected Transform[] transforms;

    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        equipWeapon = GetComponentInChildren<Weapon>();
        meshs = GetComponentsInChildren<SkinnedMeshRenderer>();
        transforms = GetComponentsInChildren<Transform>();

        followCamera = GameManager.instance.gameCam.gameObject.GetComponent<Camera>();
        skillCoolTimeUIs = GameManager.instance.coolTimeTxt;
        skillCoolTimeImgs = GameManager.instance.coolTimeImg;

        PlayerPrefs.SetInt("MaxScore", score);
    }

    private void Start()
    {
        for (int i = 0; i < 2; i++)
        {
            skills[i].coolTimeImage = skillCoolTimeImgs[i];
            skills[i].coolTimeUI = skillCoolTimeUIs[i];
        }
    }

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

        if (!isFireReady || isSkill || isDead || isShop)
        {
            moveVec = Vector3.zero;
        }

        transform.position += moveVec * speed * (isReload ? 0.3f : 1f) * Time.deltaTime;

        anim.SetBool("isRun", moveVec != Vector3.zero);
    }

    void Turn()
    {
        // 키보드에 의한 회전
        this.transform.LookAt(this.transform.position + moveVec);

        // 마우스에 의한 회전
        if(fDown && equipWeapon.currentAmmo > 0 && !isDead && !isShop)
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
        if(jDown && moveVec != Vector3.zero && !isDodge && skills[0].isOn && !isDead && !isShop)
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

        if (fDown && isFireReady && equipWeapon.currentAmmo > 0 && !isSkill && !isDead && !isShop)
        {
            equipWeapon.Use();
            shotSound.Play();
            anim.SetTrigger("doShot");
            fireDelay = 0;
        }
    }

    protected virtual void SkillQ()
    {
        if (ulGauge < ulMaxGauge)
        {
            GameManager.instance.ultimateBar.localScale = new Vector3(1, (float)(ulGauge / ulMaxGauge), 1);
        }

        if (ulGauge >= ulMaxGauge)
        {
            isUltimateReady = true;
            ulGauge = 100f;
            GameManager.instance.ultimateBar.localScale = new Vector3(1, 1, 1);
        }
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

        if(rDown && isFireReady && !isDead)
        {
            reloadSound.Play();

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
        if(other.CompareTag("Item"))
        {
            Item item = other.GetComponent<Item>();
            switch(item.type)
            {
                case Item.Type.Heart:
                    currentHp += item.value;
                    if (currentHp > maxHp)
                        currentHp = maxHp;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;
            }
            Destroy(other.gameObject);
        }

        if (other.CompareTag("EnemyBullet"))
        {
            damageSound.Play();

            Bullet enemyBullet = other.GetComponent<Bullet>();
            currentHp -= enemyBullet.damage;

            bool isBossAtk = other.name == "Boss Melee Area";

            StartCoroutine(OnDamage(isBossAtk));
        }

        if (other.GetComponent<Rigidbody>() != null)
        {
            Debug.Log(other.gameObject.name);

            switch(other.gameObject.name)
            {
                case "BossMissile(Clone)":
                    GameManager.instance.poolManager.SetPool(PoolFlag.bossMissile, other.gameObject);
                    break;
                case "Boss Rock(Clone)":
                    GameManager.instance.poolManager.SetPool(PoolFlag.bossRock, other.gameObject);
                    break;
                case "Missile(Clone)":
                    GameManager.instance.poolManager.SetPool(PoolFlag.enemyCMissile, other.gameObject);
                    break;
            }
        }      
    }

    IEnumerator OnDamage(bool isBossAtk)
    {
        isDamage = true;

        foreach (SkinnedMeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.gray;
        }

        if (isBossAtk)
            rigid.AddForce(this.transform.forward * -25, ForceMode.Impulse);

        if (currentHp <= 0 && !isDead)
            OnDie();

        yield return new WaitForSeconds(1f);

        isDamage = false;

        foreach (SkinnedMeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }

        if (isBossAtk)
            rigid.velocity = Vector3.zero;
    }

    void OnDie()
    {
        retireSound.Play();
        anim.SetTrigger("doDie");
        isDead = true;
        GameManager.instance.GameOver();
    }
}
