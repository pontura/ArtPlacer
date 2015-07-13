using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DropboxButton : MonoBehaviour {

    public Text field;
    public int id;

	public void Init (string title, int id) {
        field.text = title;
        this.id = id;
	}
}
