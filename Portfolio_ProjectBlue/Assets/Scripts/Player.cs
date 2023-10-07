using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    public Camera followCamera;

    public float speed;
    public float health;

    float hAxis;
    float vAxis;
    
    bool wDown;
    bool fDown;
    bool rDown;
    bool jDown;

    bool isFireReady = true;
    bool isReload;
    bool isDodge;
    bool isDamage;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;
    MeshRenderer[] meshs;

    Weapon equipWeapon;
    float fireDelay;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        equipWeapon = GetComponentInChildren<Weapon>();
        meshs = GetComponentsInChildren<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
        Dodge();
        Turn();
        Attack();
        Reload();
    }

    void FixedUpdate()
    {
        FreezeRotation();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButton("Jump");
        fDown = Input.GetButton("Fire1");
        rDown = Input.GetButtonDown("Reload");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if (isDodge)
            moveVec = dodgeVec;

        if (!isFireReady)
        {
            moveVec = Vector3.zero;
        }

        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
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
        if(jDown && !isDodge)
        {
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

        if (fDown && isFireReady && equipWeapon.currentAmmo > 0)
        {
            equipWeapon.Use();
            anim.SetTrigger("doShot");
            fireDelay = 0;
        }
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
            health -= enemyBullet.damage;

            if (other.GetComponent<Rigidbody>() != null)
                Destroy(other.gameObject);

            StartCoroutine(OnDamage());
        }
    }

    IEnumerator OnDamage()
    {
        isDamage = true;

        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.yellow;
        }

        yield return new WaitForSeconds(1f);

        isDamage = false;

        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }
    }
}
