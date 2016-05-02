using UnityEngine;
using System.Collections;

public class RotateAutomatic : MonoBehaviour {

    void Update()
    {
        transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z + 2);
    }
}
