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

    Vector3 moveVec;

    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");

        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);

        Turn();
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
}
