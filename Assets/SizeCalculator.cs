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

		Debug.Log ("H1: "+height1 + " H2: " + height2 + " W: " + width2d);

		float aspect = width2d / ((height1 + height2) * 0.5f);

        float diff = 0;

        if (height1 > height2)
            diff = height1 / height2;
        else if (height2 > height1)
            diff = height2 / height1;

		Debug.Log ("Diff: " + diff);

		if (diff == 0f) {
			return realSize * aspect;
		} else {

			diff *= realSize;

			Debug.Log ("Diff: " + diff);
			float res=0;
			if(aspect>1f)
				res = width2d * diff * (width2d / ((height1 + height2) * 0.5f));
			else
				res = width2d * diff;

			Debug.Log ("Res: " + res);

			return res;
		}
    }
}
