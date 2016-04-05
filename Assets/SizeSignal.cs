using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SizeSignal : MonoBehaviour {

    public int id;

    public Text desc1;
    public Text desc2;

    public int height0;
    public int height1;

    private ConfirmSizes confirmSizes;

    public float totalCentimeters;
    public int sumaAnterior;

    void Start()
    {
		Events.ConvertUnits += ConvertUnits;
        confirmSizes = GameObject.Find("confirmSizes").GetComponent<ConfirmSizes>();
        float _height = Data.Instance.areaData.areas[confirmSizes.areaActiveID].height;
        if (_height > 0)
        {
            Vector4 cms = CustomMath.GetFormatedCentimeters(_height);
            height0 = (int)cms.x;
            height1 = (int)cms.y;
        }
        Invoke("RefreshField", 0.1f);
    }
    public void Init(float _width , float _height)
    {
        print("if (Data.Instance.unidad: " + Data.Instance.unidad + "   totalCentimeters: " + totalCentimeters);

		if (Data.Instance.unidad == Data.UnitSys.CM) {
           // print("CM: " + _height);
            if (totalCentimeters != 0)
                _height = totalCentimeters;
            Vector4 cms = CustomMath.GetFormatedCentimeters(_height);
            height0 = (int)cms.x;
            height1 = (int)cms.y;

		} else if (Data.Instance.unidad == Data.UnitSys.INCHES) {
            //print("inches: " + _height);
            Vector4 cms = CustomMath.GetFormatedInches(_height);
            height0 = (int)cms.x;
            height1 = (int)cms.y;
		}
        RefreshField();
	}
    public int GetHeight()
    {
        int value1 = int.Parse(confirmSizes.conten1.active + "" + confirmSizes.conten2.active);
        int value2 = int.Parse(confirmSizes.conten3.active + "" + confirmSizes.conten4.active);

        int suma = int.Parse(confirmSizes.conten1.active + "" + confirmSizes.conten2.active + "" + confirmSizes.conten3.active + "" + confirmSizes.conten4.active);

		int resu = 0;
		if (Data.Instance.unidad == Data.UnitSys.CM) {
            resu = suma;
		} else if (Data.Instance.unidad == Data.UnitSys.INCHES) {
            int totalInches = (int)CustomMath.GetTotalInches(value1, value2);
            resu = (int)Mathf.Round(CustomMath.inches2cm(totalInches));
		}
        return resu;
    }
    
    void RefreshField()
    {
        print(height0 + " " + height1);

        int value1 = 0;
        int value2 = 0;
        int value3 = 0;
        int value4 = 0;

        value1 = (int)Mathf.Floor(height0 / 10);
        value2 = height0 - (value1*10);

        value3 = (int)Mathf.Floor(height1 / 10);
        value4 = height1 - (value3 * 10);

        confirmSizes.conten1.SetValue(value1);
        confirmSizes.conten2.SetValue(value2);
        confirmSizes.conten3.SetValue(value3);
        confirmSizes.conten4.SetValue(value4);

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
        int value1 = int.Parse(confirmSizes.conten1.active + "" + confirmSizes.conten2.active);
        int value2 = int.Parse(confirmSizes.conten3.active + "" + confirmSizes.conten4.active);

        int suma = int.Parse(confirmSizes.conten1.active + "" + confirmSizes.conten2.active + "" + confirmSizes.conten3.active + "" + confirmSizes.conten4.active);

		if (Data.Instance.unidad == Data.UnitSys.INCHES) {
            totalCentimeters = suma;
            Init(0, suma);
		} else if (Data.Instance.unidad == Data.UnitSys.CM) {
            int resu = suma;
                
                print("RECALCULA sumaAnterior " +  sumaAnterior + " suma " + suma);
                sumaAnterior = (int)CustomMath.inches2cm(suma);
                int totalInches = (int)Mathf.Floor(CustomMath.GetTotalInches(value1, value2));
                float inches2cm = CustomMath.inches2cm(totalInches);
                resu = (int)Mathf.Round(inches2cm);
     
            Init(0, resu);
            
		}
        
	}

	void OnDestroy()
	{
		Events.ConvertUnits -= ConvertUnits;
	}


    void Ready()
    {
        print("ready " + GetHeight().ToString());
        confirmSizes.Ready();
    }
}
