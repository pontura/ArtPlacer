using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ArtworkSignal : MonoBehaviour {

    public int id;
    public Text label;

	public InputField name;
	public InputField author;

    public GameObject cursor0;
	public GameObject cursor1;
	public GameObject cursor2;
	public GameObject cursor3;
	
	public Text inputField;
	public Text desc;
    public string _heightStr;
    public int height0 = 0;
    public int height1 = 1;
    public int height2 = 0;
    public int height3 = 0;

    public int activeNum = 0;

   // public int height_In_CM;

	private ConfirmArtworkSize confirmSizes;
	
	void Start()
	{	
		Events.ConvertUnits += ConvertUnits;
		confirmSizes = GameObject.Find("confirmArtworkSize").GetComponent<ConfirmArtworkSize>();
		RefreshField();
	}
    public void Init(float _width, float _height)
    {
        label.text = "";
        if (Data.Instance.unidad == Data.UnitSys.CM)
        {
            Vector4 cms = CustomMath.GetFormatedCentimeters(_height);
            height0 = (int)cms.x;
            height1 = (int)cms.y;
            height2 = (int)cms.w;
            height3 = (int)cms.z;

        }
        else if (Data.Instance.unidad == Data.UnitSys.INCHES)
        {
            Vector4 cms = CustomMath.GetFormatedInches(_height);
            height0 = (int)cms.x;
            height1 = (int)cms.y;
            height2 = (int)cms.w;
            height3 = (int)cms.z;
        }
        RefreshField();
    }
    //public void Init(float _width , float _height)
    //{
    //    print("INIT (cm) " + _height);
    //    height_In_CM = (int)_height;
    //    ChangeUnits(height_In_CM);
    //    ConvertUnits();
    //}
    public int GetHeight()
    {
        string result = "" + (height0 + "" + height1 + "" + height2 + "" + height3);
        int resu = 0;
        if (Data.Instance.unidad == Data.UnitSys.CM)
        {
            resu = int.Parse(result);
        }
        else if (Data.Instance.unidad == Data.UnitSys.INCHES)
        {
            int totalInches = CustomMath.GetTotalInches(height0, height1, height2, height3);
            resu = (int)Mathf.Round(CustomMath.inches2cm(totalInches));
        }
        return resu;
    }

    //public int GetHeight()
    //{
    //    string all = height0 + "" + height + "" + height2 + "" + height3;
    //    int num = int.Parse(all);
    //    if (Data.Instance.unidad == Data.UnitSys.INCHES)
    //    {
    //        //int feets = int.Parse(height0 + "" + height);
    //        //int inches = int.Parse(height2 + "" + height3);
    //        //inches += (int)CustomMath.feet2inches(feets);
    //       // height_In_CM = (int)CustomMath.inches2cm(inches);
    //        height_In_CM = (int)CustomMath.inches2cm(num);
    //    }
    //    else height_In_CM = num;

    //    Debug.Log("GRABA: " + height_In_CM + "    _ " + Data.Instance.unidad);

    //    return height_In_CM;
    //}
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
	public void Ready()
	{
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

        if (Data.Instance.unidad == Data.UnitSys.CM)
        {
            inputField.text = "" + height0 + height1 + "." + height2 + height3;
            desc.text = "" + height0 + height1 + " meters, " + height2 + "" + height3 + " centimeters";
        }
        else if (Data.Instance.unidad == Data.UnitSys.INCHES)
        {
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

    void ConvertUnits()
    {
        string result = "" + (height0 + "" + height1 + "" + height2 + "" + height3);
        if (Data.Instance.unidad == Data.UnitSys.INCHES)
        {
            Init(0, int.Parse(result));
        }
        else if (Data.Instance.unidad == Data.UnitSys.CM)
        {
            int totalInches = CustomMath.GetTotalInches(height0, height1, height2, height3);
            int resu = (int)Mathf.Round(CustomMath.inches2cm(totalInches));
            Init(0, resu);
        }
    }


    //void RefreshField()
    //{
    //    string result =  height0  + "" + height + "." + height2 + height3;
       
    //    int num = int.Parse(height0  + "" + height  + height2 + height3);
    //    if (Data.Instance.unidad == Data.UnitSys.CM) {
    //        desc.text = height0 + "" + height + " meters, " + height2 + "" + height3 + " centimeters";
    //        inputField.text = result;
    //        label.text = "cm";
    //    } else if (Data.Instance.unidad == Data.UnitSys.INCHES) {
    //        //inputField.text = height0 + "" + height + "´" +  height2 + height3;
    //        //int feets = (int)num/12;
    //        //int inches = num - (feets*12);
    //        //desc.text = height0 + "" + height + " feet, " + height2 + height3 + " inches";
    //       // label.text = "inches";

    //        result = height0 + "" + height + "" + height2 + height3;
    //        desc.text = height0 + "" + height + "" + height2 + "" + height3 + " inches";
    //        inputField.text = result;
    //        label.text = "inches";
    //    }
		
    //    RefreshCursor();
    //}
    //void RefreshCursor()
    //{
    //    cursor0.SetActive(false);
    //    cursor1.SetActive(false);
    //    cursor2.SetActive(false);
    //    cursor3.SetActive(false);
		
    //    GameObject cursorActive = cursor0;
    //    if (activeNum == 1) cursorActive = cursor1;
    //    else if (activeNum == 2) cursorActive = cursor2;
    //    else if (activeNum == 3) cursorActive = cursor3;

    //    cursorActive.SetActive(true);
    //}

    //public void ConvertUnits(){

    //    string result = height0 + "" + height + "" + height2 + "" + height3;

    //    if (Data.Instance.unidad == Data.UnitSys.INCHES) {
    //        float cm2inches = Mathf.Round(CustomMath.cm2inches(height_In_CM));
    //        ChangeUnits((int)cm2inches);
    //    } else if (Data.Instance.unidad == Data.UnitSys.CM) {
    //        ChangeUnits(height_In_CM);
           
    //    }

    //}
    //void ChangeUnits(int _height)
    //{
    //    Debug.Log("ChangeUnits a unidad: " + Data.Instance.unidad + "    _height: " + _height);
    //    _heightStr = "";

    //    if (_height < 10)
    //        _heightStr = "000" + _height.ToString();
    //    else if (_height < 100)
    //        _heightStr = "00" + _height.ToString();
    //    else if (_height < 1000)
    //        _heightStr = "0" + _height.ToString();
    //    else
    //        _heightStr = _height.ToString();
        
    //    height0 = int.Parse(_heightStr.Substring(0, 1));
    //    height = int.Parse(_heightStr.Substring(1, 1));
    //    height2 = int.Parse(_heightStr.Substring(2, 1));
    //    height3 = int.Parse(_heightStr.Substring(3, 1));

    //    RefreshField();
    //}

	public string GetName()
	{
		return name.text;
	}

	public string GetAuthor()
	{
		return author.text;
	}

	void OnDestroy()
	{
		Events.ConvertUnits -= ConvertUnits;
	}
}
