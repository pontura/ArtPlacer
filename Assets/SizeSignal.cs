using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SizeSignal : MonoBehaviour {

    public int id;

    public InputField inputField1;
    public InputField inputField2;

    public Text desc1;
    public Text desc2;

    public int height0 = 1;
    public int height1 = 0;

    private ConfirmSizes confirmSizes;

    void Start()
    {
		Events.ConvertUnits += ConvertUnits;
        confirmSizes = GameObject.Find("confirmSizes").GetComponent<ConfirmSizes>();
        RefreshField();
    }
    public void Init(float _width , float _height)
    {

        inputField1.contentType = InputField.ContentType.IntegerNumber;
        inputField2.contentType = InputField.ContentType.IntegerNumber;

		if (Data.Instance.unidad == Data.UnitSys.CM) {
            print("CM: " + _height);
            Vector4 cms = CustomMath.GetFormatedCentimeters(_height);
            height0 = (int)cms.x;
            height1 = (int)cms.y;

		} else if (Data.Instance.unidad == Data.UnitSys.INCHES) {
            print("inches: " + _height);
            Vector4 cms = CustomMath.GetFormatedInches(_height);
            height0 = (int)cms.x;
            height1 = (int)cms.y;
		}
        RefreshField();
	}
    public int GetHeight()
    {
        int value1 = int.Parse(inputField1.text);
        int value2 = int.Parse(inputField2.text);

		int resu = 0;
		if (Data.Instance.unidad == Data.UnitSys.CM) {
            resu = (value1 * 100) + value2;
		} else if (Data.Instance.unidad == Data.UnitSys.INCHES) {
            int totalInches = CustomMath.GetTotalInches(value1, value2);
            resu = (int)Mathf.Round(CustomMath.inches2cm(totalInches));
		}        
        return resu;
    }
    void Ready()
    {
        print("ready " + GetHeight().ToString());
        confirmSizes.Ready();
    }
    void RefreshField()
    {
        inputField1.text = height0.ToString();
        inputField2.text = height1.ToString();

        print(height0 + " " + height1);

		if (Data.Instance.unidad == Data.UnitSys.CM) {            
            desc1.text = "meters";
            desc2.text = "centimeters";
		} else if (Data.Instance.unidad == Data.UnitSys.INCHES) {
            desc1.text = "feet";
            desc2.text = "inches";
		}
    }
    
	void ConvertUnits()
    {       

        if (inputField1.text == "")
            inputField1.text = "0";
        if(inputField2.text == "")
            inputField2.text = "0";

        int value1 = int.Parse(inputField1.text);
        int value2 = int.Parse(inputField2.text);

		if (Data.Instance.unidad == Data.UnitSys.INCHES) {
            Init(0, (value1 * 100) + value2);
		} else if (Data.Instance.unidad == Data.UnitSys.CM) {
            int totalInches = CustomMath.GetTotalInches(value1, value2);
            int resu = (int)Mathf.Round(CustomMath.inches2cm(totalInches));
            Init(0, resu);
		}
	}

	void OnDestroy()
	{
		Events.ConvertUnits -= ConvertUnits;
	}
}
