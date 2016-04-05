using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ConfirmSizes : MonoBehaviour {

    public int num1;
    public int num2;

    public gridsnap conten1;
    public gridsnap conten2;
    public gridsnap conten3;
    public gridsnap conten4;

    public Animation tooltipSizes;
    public GameObject container;
    public Slider slider;

    public int areaActiveID;

    void Start()
    {
        Data.Instance.SetTitle("");
        Events.Back += Back;
        tooltipSizes.gameObject.SetActive(false);

        areaActiveID = 0;

        //if (Data.Instance.areaData.areas.Count > 0)
        AddConfirmSizes(Data.Instance.areaData.areas[0]);

        Invoke("startTooltip", 0.5f);

        slider.value = Data.Instance.unitSlider.value;

        Events.ToggleUnit += ToggleUnits;
    }
    void OnDestroy()
    {
        Events.Back -= Back;
        Events.ToggleUnit -= ToggleUnits;
    }
    void Update()
    {
        num1 = int.Parse(conten1.active.ToString() + conten2.active.ToString());
        num2 = int.Parse(conten3.active.ToString() + conten4.active.ToString());
    }
    void AddConfirmSizes(AreaData.Area area)
    {
        Invoke("SelectArea", 0.1f);
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
    }
    public void ToggleUnit()
    {
        Events.ToggleUnit();
    }

    void SelectArea()
    {
        GetComponent<WallCreator>().SelectArea(areaActiveID);
    }
    void startTooltip()
    {
        tooltipSizes.gameObject.SetActive(true);
        tooltipSizes.Play("tooltipOnVertical");
    }
	public void Back()
	{
		Data.Instance.LoadLevel("Walls");
    }
    public void Ready()
    {
        string num = conten1.active.ToString() + conten2.active.ToString() + conten3.active.ToString() + conten4.active.ToString();
        int _height = int.Parse(num);
        print("______________height: " + _height);
        Data.Instance.areaData.areas[areaActiveID].height = _height;
        Data.Instance.areaData.areas[areaActiveID].width = GetComponent<WallCreator>().createdPlanes[areaActiveID].GetComponent<SizeCalculator>().CalculateWidth(_height);
        		
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
