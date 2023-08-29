using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    public Camera followCamera;

    public float speed;

    float hAxis;
    float vAxis;
    
    bool wDown;
    bool fDown;
    bool rDown;

    bool isFireReady = true;
    bool isReload;

    Vector3 moveVec;

    Rigidbody rigid;
    Animator anim;

    Weapon equipWeapon;
    float fireDelay;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        equipWeapon = GetComponentInChildren<Weapon>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();

        Move();

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
        fDown = Input.GetButton("Fire1");
        rDown = Input.GetButtonDown("Reload");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if (!isFireReady || isReload)
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
        transform.LookAt(transform.position + moveVec);

        // 마우스에 의한 회전
        if(fDown && equipWeapon.currentAmmo > 0)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }
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
}
