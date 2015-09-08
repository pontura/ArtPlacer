using UnityEngine;
using System.Collections;

public class SizeCalculator : MonoBehaviour {
    
    public float height1;
    public float height2;
    public float width2d;
    public float realWidth;

    private WallPlane wallPlane;

	void Start () {
        wallPlane = GetComponent<WallPlane>();
	}
    public float CalculateWidth(float realSize)
    {
        height1 = Vector3.Distance(wallPlane.pointer[3].transform.localPosition, wallPlane.pointer[0].transform.localPosition);
        height2 = Vector3.Distance(wallPlane.pointer[1].transform.localPosition, wallPlane.pointer[2].transform.localPosition);
        width2d = Vector3.Distance(wallPlane.pointer[0].transform.localPosition, wallPlane.pointer[2].transform.localPosition);

        float diff = 1;

        if (height1 > height2)
            diff = height1 / height2;
        else if (height2 > height1)
            diff = height2 / height1;

        diff *= realSize;

        return width2d * diff;
    }
}
