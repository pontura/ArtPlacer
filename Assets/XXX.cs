using UnityEngine;
using System.Collections;

public class XXX : MonoBehaviour {

    public float Original_X;
    public float Actual_X;

    public GameObject container;
    public bool moving = false;

    void Start()
    {
        InitMove();
    }
    void InitMove()
    {
        Original_X = -Screen.width;
        Actual_X = Original_X;
    }
    public void Update()
    {
        if (!moving) return;
        Actual_X += 10;
        if (Actual_X >= 0) return;
        Vector2 pos = container.transform.localPosition;
        pos.x = Actual_X;
        container.transform.localPosition = pos;
    }
    public void Move()
    {
        moving = true;
        InitMove();
    }

}
