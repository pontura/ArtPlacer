using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollContent : MonoBehaviour {

    public Text field;
    public int id;

    public void Init(int id, string addSimbol)
    {
        if (id >= 0)
        {
            string texto = id.ToString() + addSimbol;

            if (id < 10)
                field.text = "0" + texto;
            else
                field.text = texto;
           
        }
        else
            field.text = "";
        this.id = id;
    }
}
