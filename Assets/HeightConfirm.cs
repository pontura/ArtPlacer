
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HeightConfirm : MonoBehaviour {

    public GameObject panel;

    public int result;

    public int num1_cm;
    public int num2_cm;

    public int num1_inches;
    public int num2_inches;

    public GameObject cms;
    public GameObject inches;

    public ScrollSnap conten1;
    public ScrollSnap conten2;

    public ScrollSnap conten1b;
    public ScrollSnap conten2b;

    public Slider slider;

    public int areaActiveID;

    void Start()
    {
        panel.SetActive(false);

    }
    public void Init(int result)
    {
        if (result < 1) result = 150;
        panel.SetActive(true);
        areaActiveID = 0;

        slider.value = Data.Instance.unitSlider.value;

        Events.ToggleUnit += ToggleUnits;
        this.result = result;
       // float _height = Data.Instance.areaData.areas[areaActiveID].height;
        if (result > 0)
        {
            CalculateNewCM();
            CalculateNewInches();
        }

        conten1.Init(num1_cm);
        conten2.Init(num2_cm);

        conten1b.Init(num1_inches);
        conten2b.Init(num2_inches);

        ChangeUnits();
        RefreshUnits();
    }
    void CalculateNewCM()
    {
        Vector4 cms = CustomMath.GetFormatedCentimeters(result);
        num1_cm = (int)cms.x;
        num2_cm = (int)cms.y;
    }
    void CalculateNewInches()
    {
        Vector3 inches = CustomMath.GetFormatedInches(result);
        num1_inches = (int)inches.x;
        num2_inches = (int)inches.y;
    }
    void ChangeUnits()
    {
        if (Data.Instance.unidad == Data.UnitSys.CM)
        {
            CalculateNewCM();
            cms.SetActive(true);
            inches.SetActive(false);
        }
        else
        {
            CalculateNewInches();
            cms.SetActive(false);
            inches.SetActive(true);
        }        
    }
    void RefreshUnits()
    {
        conten1.ChangeValue(num1_cm);
        conten2.ChangeValue(num2_cm);

        conten1b.ChangeValue(num1_inches);
        conten2b.ChangeValue(num2_inches);
    }
    public void Hide()
    {
        panel.SetActive(false);
    }
    void OnDestroy()
    {
        Events.ToggleUnit -= ToggleUnits;
    }
    void Update()
    {
        int new_num1_cm = int.Parse(conten1.GetActive().ToString());
        int new_num2_cm = int.Parse(conten2.GetActive().ToString());

        int new_num1_inches = int.Parse(conten1b.GetActive().ToString());
        int new_num2_inches = int.Parse(conten2b.GetActive().ToString());

        if (new_num1_cm != num1_cm)
            ChangeCM(0, new_num1_cm);
        if (new_num2_cm != num2_cm)
            ChangeCM(1, new_num2_cm);
        if (new_num1_inches != num1_inches)
            ChangeInches(0, new_num1_inches);
        if (new_num2_inches != num2_inches)
            ChangeInches(1, new_num2_inches);
    }
    void ChangeCM(int var, int value)
    {
        print("ChangeCM var: " + var + " value: " + value);
        switch (var)
        {
            case 0: num1_cm = value; break;
            case 1: num2_cm = value; break;
        }
        string cms = num2_cm.ToString();
        if (num2_cm < 10) cms = "0" + num2_cm.ToString();
        string num = conten1.active.ToString() + cms;
        result = int.Parse(num);
       // CalculateNewInches();
    }
    void ChangeInches(int var, int value)
    {
        print("ChangeInches var: " + var + " value: " + value);
        switch (var)
        {
            case 0: num1_inches = value; break;
            case 1: num2_inches = value; break;
        }
        float totalInches = CustomMath.GetTotalInches(num1_inches, num2_inches);
        result = (int)CustomMath.inches2cm(totalInches);
       // CalculateNewCM();
    }
    void AddConfirmSizes(AreaData.Area area)
    {
       // Invoke("SelectArea", 0.1f);
    }
    public void ToggleUnits()
    {
        if (slider.value == 1)
        {
            slider.value = 0;
        }
        else if (slider.value == 0)
        {
            slider.value = 1;
        }
        ChangeUnits();
        RefreshUnits();
    }
    public void ToggleUnit()
    {
        Events.ToggleUnit();
    }

    void SelectArea()
    {
       // GetComponent<WallCreator>().SelectArea(areaActiveID);
    }

	public void Back()
	{
		Data.Instance.LoadLevel("Walls");
    }
    public void Ready()
    {

        print("______________height: " + result);
        Data.Instance.areaData.areas[areaActiveID].height = result;
        Data.Instance.areaData.areas[areaActiveID].width = GetComponent<WallCreator>().createdPlanes[areaActiveID].GetComponent<SizeCalculator>().CalculateWidth(result);
        		
		Events.SaveAreas();
        //Data.Instance.areaData.Save();

        areaActiveID++;

        if (Data.Instance.areaData.areas.Count > areaActiveID)
        {
            AddConfirmSizes(Data.Instance.areaData.areas[areaActiveID]);
        }
        else
        {
            Data.Instance.LoadLevel("ArtPlaced");
        }
    }

}
