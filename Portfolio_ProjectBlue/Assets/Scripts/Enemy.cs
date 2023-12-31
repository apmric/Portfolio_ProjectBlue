using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type {
        A,
        B,
        C,
        D
    };
    public Type enemyType;
    public int maxHealth;
    public int curHealth;
    public int score;
    public float ulGauge;
    public Transform target;
    public BoxCollider meleeArea;
    public GameObject bullet;
    public bool isChase;
    public bool isAttack;
    public bool isDead;
    public GameObject[] coins;
    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs;
    public NavMeshAgent nav;
    public Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        if (enemyType != Type.D)
            Invoke("ChaseStart", 2);
    }

    void OnEnable()
    {
        if (enemyType != Type.D)
            Invoke("ChaseStart", 2);
    }

    void Update()
    {
        if (nav.enabled && enemyType != Type.D)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }
    }

    void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 reactVec = this.transform.position - other.transform.position;

            GameManager.instance.poolManager.SetPool(PoolFlag.bullet, other.gameObject);

            StartCoroutine(OnDamage(reactVec));
        }
    }

    IEnumerator OnDamage(Vector3 reactVec)
    {
        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.red;

        yield return new WaitForSeconds(0.01f);

        if (curHealth > 0)
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.white;
        }
        else
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.gray;

            gameObject.layer = 12;
            isDead = true;
            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("doDie");

            GameManager.instance.player.score += score;
            GameManager.instance.player.ulGauge += ulGauge;

            int ranCoin = Random.Range(0, 3);
            GameObject instantCoin = GameManager.instance.poolManager.GetPool(coins[ranCoin]);
            instantCoin.transform.position = this.transform.position;
            instantCoin.transform.rotation = Quaternion.identity;

            switch(enemyType)
            {
                case Type.A:
                    GameManager.instance.enemyCntA--;
                    break; 
                case Type.B:
                    GameManager.instance.enemyCntB--;
                    break;
                case Type.C:
                    GameManager.instance.enemyCntC--;
                    break;
                case Type.D:
                    GameManager.instance.enemyCntD--;
                    break;
            }

            reactVec = reactVec.normalized;
            reactVec += Vector3.up;

            rigid.AddForce(reactVec * 5, ForceMode.Impulse);

            Invoke("ReturnObject", 4);
        }
    }

    void ReturnObject()
    {
        switch (enemyType)
        {
            case Type.A:
                GameManager.instance.poolManager.SetPool(PoolFlag.enemyA, this.gameObject);
                break;
            case Type.B:
                GameManager.instance.poolManager.SetPool(PoolFlag.enemyB, this.gameObject);
                break;
            case Type.C:
                GameManager.instance.poolManager.SetPool(PoolFlag.enemyC, this.gameObject);
                break;
            case Type.D:
                GameManager.instance.poolManager.SetPool(PoolFlag.enemyD, this.gameObject);
                break;
        }
    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }

    void FreezeVelocity()
    {
        if(isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    void Targeting()
    {
        if(!isDead && enemyType != Type.D)
        {
            float targetRadius = 0;
            float targetRange = 0;

            switch (enemyType)
            {
                case Type.A:
                    targetRadius = 1.5f;
                    targetRange = 3f;
                    break;
                case Type.B:
                    targetRadius = 1f;
                    targetRange = 12f;
                    break;
                case Type.C:
                    targetRadius = 0.5f;
                    targetRange = 25f;
                    break;
            }

            RaycastHit[] rayHits =
                Physics.SphereCastAll(transform.position,
                                      targetRadius,
                                      transform.forward,
                                      targetRange,
                                      LayerMask.GetMask("Player"));

            if (rayHits.Length > 0 && !isAttack)
            {
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);

        switch(enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;

                yield return new WaitForSeconds(1f);

                break;
            case Type.B:
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(this.transform.forward * 20, ForceMode.Impulse);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;

                yield return new WaitForSeconds(2f);
                break;
            case Type.C:
                yield return new WaitForSeconds(0.5f);
                GameObject instantBullet = GameManager.instance.poolManager.GetPool(PoolFlag.enemyCMissile);
                instantBullet.transform.position = this.transform.position;
                instantBullet.transform.rotation = this.transform.rotation;
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = this.transform.forward * 20f;

                yield return new WaitForSeconds(2f);
                break;
        }

        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
    }
}
