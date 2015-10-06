using UnityEngine;
using System.Collections;

public class SizeCalculator : MonoBehaviour {
    
    public float height1;
    public float height2;
    public float width2d;
    public float width2d2;
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
        width2d2 = Vector3.Distance(wallPlane.pointer[3].transform.localPosition, wallPlane.pointer[1].transform.localPosition);

        float heightDiff = 1 + Mathf.Abs(height1 - height2);

        float heights = height1 + height2;
        float widths = width2d + width2d;

        float diffss = widths / heights;

        return (diffss * realSize) * heightDiff;
    }
}
