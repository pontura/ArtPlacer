using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SizeSignal : MonoBehaviour {

    public int id;

    public InputField width_m;
    public InputField width_cm;

    public InputField height_m;
    public InputField height_cm;

    public void Init(float _width , float _height)
    {
        if (_width > 0)
            width_m.text = _width.ToString();

        int intpart = (int)_width;
        double decpart = _width - intpart;

        if (decpart > 0)
            width_cm.text = decpart.ToString();



        if (_height > 0)
            height_m.text = _height.ToString();

        intpart = (int)_height;
        decpart = _height - intpart;

        if (decpart > 0)
            height_cm.text = decpart.ToString();
	}
    public float GetHeight()
    {
        string str =  height_m.text + "." + height_cm.text;
        float result = 0;
        float.TryParse(str, out result);
        return result;
    }
    public float GetWidth()
    {
        string str = width_m.text + "." + width_cm.text;
        float result = 0;
        float.TryParse(str, out result);
        return result;
    }
}
