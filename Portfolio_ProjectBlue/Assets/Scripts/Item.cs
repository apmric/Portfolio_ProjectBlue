using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type
    {
        Heart,
        Coin
    }
    public Type type;
    public int value;

    void Update()
    {
        this.transform.Rotate(Vector3.up * 20f* Time.deltaTime);

        if (!GameManager.instance.isBattle)
        {
            this.transform.position = Vector3.zero;
        }
    }
}
