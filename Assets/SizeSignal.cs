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

	private int height0 = 0;
    private int height1 = 2;
    private int height2 = 0;
    private int height3 = 0;

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
			height0 = (int)(_height * 0.001);
			height1 = (int)(_height * 0.01 - (height0 * 10));
			height2 = (int)(_height * 0.1 - (height0 * 100) - (height1 * 10));
			height3 = (int)(_height - (height0 * 1000) - (height1 * 100)- (height2 * 10));
		} else if (Data.Instance.unidad == Data.UnitSys.INCHES) {
			float inches = CustomMath.cm2inches(_height);
			float feet = CustomMath.inches2feet(inches);
			height0 = (int)(feet*0.1);
			height1 = (int)(feet - (height0*10));
			height2 = (int)((feet - (height0*10+height1))*10);
			height3 = (int)((feet - (height0*10+height1+height2*0.1))*10);
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
			float inches = CustomMath.feet2inches(float.Parse(result)*0.01f);
			resu = (int)Mathf.Round(CustomMath.inches2cm(inches));
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
		inputField.text = ""+height0 + height1 + "." + height2 + height3;
		if (Data.Instance.unidad == Data.UnitSys.CM) {
			desc.text = ""+height0 + height1 + " meters, " + height2 + "" + height3 + " centimeters";
		} else if (Data.Instance.unidad == Data.UnitSys.INCHES) {
			desc.text = ""+height0+height1 + " feet, " + height2 + "" + height3 + " inches";
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
			float inches = CustomMath.feet2inches(float.Parse(result)*0.01f);
			Init(0,(int)Mathf.Round(CustomMath.inches2cm(inches)));
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
