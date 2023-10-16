using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int damage;
    public float rate;
    public int maxAmmo;
    public int currentAmmo;

    public Transform bulletPos;
    public GameObject bullet;

    public Transform bulletCasePos;
    public GameObject bulletCase;

    public void Use()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
            StartCoroutine(Shot());
        }
    }

    IEnumerator Shot()
    {
        yield return new WaitForSeconds(0.01f);

        // √—æÀ πﬂªÁ
        GameObject instantBullet = GameManager.instance.poolManager.GetPool(PoolFlag.bullet);
        instantBullet.transform.position = bulletPos.position;
        instantBullet.transform.rotation = bulletPos.rotation;
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50f;

        yield return null;

        // ≈∫«« πË√‚
        GameObject instantBulletCase = GameManager.instance.poolManager.GetPool(PoolFlag.bulletCase);
        instantBulletCase.transform.position = bulletCasePos.position;
        instantBulletCase.transform.rotation = bulletCasePos.rotation;
        Rigidbody bulletCaseRigid = instantBullet.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        bulletCaseRigid.AddForce(caseVec, ForceMode.Impulse);
    }
}
