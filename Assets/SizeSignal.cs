using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SizeSignal : MonoBehaviour {

    public int id;

	public GameObject cursor0;
    public GameObject cursor1;
    public GameObject cursor2;
    public GameObject cursor3;

    public Text inputField;
    public Text desc;

    public int height0 = 0;
    public int height1 = 1;
    public int height2 = 0;
    public int height3 = 0;

    private int activeNum = 0;
    private ConfirmSizes confirmSizes;

    void Start()
    {
		Events.ConvertUnits += ConvertUnits;
        confirmSizes = GameObject.Find("confirmSizes").GetComponent<ConfirmSizes>();
        RefreshField();
    }
    public void Init(float _width , float _height)
    {
		if (Data.Instance.unidad == Data.UnitSys.CM) {
            print("CM: " + _height);
            Vector4 cms = CustomMath.GetFormatedCentimeters(_height);
            height0 = (int)cms.x;
            height1 = (int)cms.y;
            height2 = (int)cms.w;
            height3 = (int)cms.z;

		} else if (Data.Instance.unidad == Data.UnitSys.INCHES) {
            Vector4 cms = CustomMath.GetFormatedInches(_height);
            height0 = (int)cms.x;
            height1 = (int)cms.y;
            height2 = (int)cms.w;
            height3 = (int)cms.z;
		}
        RefreshField();
	}
    public int GetHeight()
    {
		string result = "" + (height0 + "" + height1 + "" + height2 + "" + height3);
		int resu = 0;
		if (Data.Instance.unidad == Data.UnitSys.CM) {
			resu = int.Parse(result);
		} else if (Data.Instance.unidad == Data.UnitSys.INCHES) {
            int totalInches = CustomMath.GetTotalInches(height0, height1, height2, height3);
            resu = (int)Mathf.Round(CustomMath.inches2cm(totalInches));
		}        
        return resu;
    }
    public void OnPress(string key)
    {
        switch(key)
        {
            case "<":
                Delete();
                break;
            case "ok":
                Ready();
                break;
            default:
                Add(key);
                break;
        }
    }
    void Add(string key)
    {
        switch (activeNum)
        {
            case 0: height0 = int.Parse(key); break;
            case 1: height1 = int.Parse(key); break;
            case 2: height2 = int.Parse(key); break;
			case 3: height3 = int.Parse(key); break;
        }
        if (activeNum < 3)
        activeNum++;
        RefreshField();
    }
    void Ready()
    {
        print("ready " + GetHeight().ToString());
        confirmSizes.Ready();
    }
    void Delete()
    {
		height0 = 0;
        height1 = 0;
        height2 = 0;
        height3 = 0;
        activeNum = 0;
        RefreshField();
    }
    void Next()
    {

    }
    void RefreshField()
    {
		
		if (Data.Instance.unidad == Data.UnitSys.CM) {
            inputField.text = "" + height0 + height1 + "." + height2 + height3;
			desc.text = ""+height0 + height1 + " meters, " + height2 + "" + height3 + " centimeters";
		} else if (Data.Instance.unidad == Data.UnitSys.INCHES) {
            inputField.text = "" + height0 + height1 + "´" + height2 + height3 + "´´";
            desc.text = "" + height0 + height1 + " feet, " + height2 + height3 + " inches";
		}
        
        RefreshCursor();
    }
    void RefreshCursor()
    {
		cursor0.SetActive(false);
        cursor1.SetActive(false);
        cursor2.SetActive(false);
        cursor3.SetActive(false);

        GameObject cursorActive = cursor0;
        if (activeNum == 1) cursorActive = cursor1;
        else if (activeNum == 2) cursorActive = cursor2;
		else if (activeNum == 3) cursorActive = cursor3;
        cursorActive.SetActive(true);
    }
    
	void ConvertUnits(){
		string result = "" + (height0 + ""+height1 + "" + height2 + "" + height3);
		if (Data.Instance.unidad == Data.UnitSys.INCHES) {
			Init(0,int.Parse(result));
		} else if (Data.Instance.unidad == Data.UnitSys.CM) {
            int totalInches = CustomMath.GetTotalInches(height0, height1, height2, height3);
            int resu = (int)Mathf.Round(CustomMath.inches2cm(totalInches));
            Init(0, resu);
		}
	}

    //public float GetWidth()
    //{
    //    string str = width_m.text + "." + width_cm.text;
    //    float result = 0;
    //    float.TryParse(str, out result);
    //    return result;
    //}

	void OnDestroy()
	{
		Events.ConvertUnits -= ConvertUnits;
	}
}
