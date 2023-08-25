using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    Camera followCamera;

    [SerializeField]
    float speed;

    float hAxis;
    float vAxis;
    
    bool wDown;
    bool fDown;

    bool isFireReady = true;

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
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        fDown = Input.GetButton("Fire1");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);

        //Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
        //RaycastHit rayHit;
        //if(Physics.Raycast(ray, out rayHit, 100))
        //{
        //    Vector3 nextVec =  rayHit.point - transform.position;
        //    transform.LookAt(transform.position + nextVec);
        //}
    }

    void Attack()
    {
        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if (fDown && isFireReady)
        {
            equipWeapon.Use();
            anim.SetTrigger("doShot");
            fireDelay = 0;
        }
    }
}
