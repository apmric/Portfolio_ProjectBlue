using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isMelee;
    public bool isRock;

    private void OnCollisionEnter(Collision collision)
    {
        if (!isRock && collision.gameObject.tag == "Floor")
        {
            switch(this.name)
            {
                case "Bullet(Clone)":
                    GameManager.instance.poolManager.SetPool(PoolFlag.bullet, this.gameObject);
                    break;
                case "Bullet Case(Clone)":
                    GameManager.instance.poolManager.SetPool(PoolFlag.bulletCase, this.gameObject);
                    break;
                case "Missile(Clone)":
                    GameManager.instance.poolManager.SetPool(PoolFlag.enemyCMissile, this.gameObject);
                    break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isMelee && other.gameObject.tag == "Wall")
        {
            switch (this.name)
            {
                case "Bullet(Clone)":
                    GameManager.instance.poolManager.SetPool(PoolFlag.bullet, this.gameObject);
                    break;
                case "Bullet Case(Clone)":
                    GameManager.instance.poolManager.SetPool(PoolFlag.bulletCase, this.gameObject);
                    break;
                case "Missile(Clone)":
                    GameManager.instance.poolManager.SetPool(PoolFlag.enemyCMissile, this.gameObject);
                    break;
                case "Boss Rock(Clone)":
                    GameManager.instance.poolManager.SetPool(PoolFlag.bossRock, this.gameObject);
                    break;
            }
        }
    }
}
