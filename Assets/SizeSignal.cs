using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SizeSignal : MonoBehaviour {

    public InputField field;
    public int id;

	public void Init (int size) {

        print("SizeSignal" + size);
        if(size>0)
            field.text = size.ToString();
	}
}
