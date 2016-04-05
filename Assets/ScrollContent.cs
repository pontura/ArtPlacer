using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollContent : MonoBehaviour {

    public Text field;
    public int id;

    public void Init(int id)
    {
        field.text = id.ToString();
        this.id = id;


        print("id: " + id);
    }
}
