using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PoolFlag
{
    bullet,
    bulletCase,
    enemyA,
    enemyB,
    enemyC,
    enemyD,
    enemyCMissile,
    bossMissile,
    bossRock,
    bronzeCoin,
    silverCoin,
    goldCoin,
    heart
}

public class PoolManager : MonoBehaviour
{
    [System.Serializable]
    public class Original
    {
        public GameObject obj;
        public int initCount;
    }

    public Original[] originals;

    List<Queue<GameObject>> pools;

    void Awake()
    {
        pools = new List<Queue<GameObject>>();

        for(int index = 0; index < originals.Length; index++)
        {
            pools.Add(new Queue<GameObject>());

            for (int i = 0; i < originals[index].initCount; i++)
            {
                pools[index].Enqueue(CreateObject(originals[index].obj));
            }
        }
    }

    GameObject CreateObject(GameObject obj)
    {
        GameObject temp = GameObject.Instantiate(obj, this.transform);
        temp.gameObject.SetActive(false);
        return temp;
    }

    public GameObject GetPool(PoolFlag flag)
    {
        int index = (int) flag;

        GameObject temp = null;

        if (pools[index].Count == 0)
            temp = CreateObject(originals[index].obj);
        else
            temp = pools[index].Dequeue();

        temp.gameObject.SetActive(true);

        return temp;
    }

    public GameObject GetPool(GameObject objs)
    {
        Debug.Log(objs.name);

        int index = 0;

        switch(objs.name)
        {
            case "Enemy A":
                index = (int) PoolFlag.enemyA;
                break;
            case "Enemy B":
                index = (int) PoolFlag.enemyB;
                break;
            case "Enemy C":
                index = (int) PoolFlag.enemyC;
                break;
            case "Enemy D":
                index = (int) PoolFlag.enemyD;
                break;
            case "Coin Bronze":
                index = (int)PoolFlag.bronzeCoin;
                break;
            case "Coin Silver":
                index = (int)PoolFlag.silverCoin;
                break;
            case "Coin Gold":
                index = (int)PoolFlag.goldCoin;
                break;
        }

        GameObject temp = null;

        if (pools[index].Count == 0)
            temp = CreateObject(originals[index].obj);
        else
            temp = pools[index].Dequeue();

        temp.gameObject.SetActive(true);

        return temp;
    }

    public void SetPool(PoolFlag flag, GameObject obj)
    {
        int index = (int) flag;

        obj.gameObject.SetActive(false);

        pools[index].Enqueue(obj);
    }
}
