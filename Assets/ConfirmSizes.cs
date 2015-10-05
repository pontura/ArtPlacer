using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ConfirmSizes : MonoBehaviour {

    public SizeSignal sizeSignal;
    public Animation tooltipSizes;
    public GameObject container;

    public List<SizeSignal> sizeSignals;

    private int areaActiveID;
    private SizeSignal newSizeSignal;



    void Start()
    {
        tooltipSizes.gameObject.SetActive(false);

        areaActiveID = 0;

        //if (Data.Instance.areaData.areas.Count > 0)
        AddConfirmSizes(Data.Instance.areaData.areas[0]);

        Invoke("startTooltip", 0.5f);
    }
    void AddConfirmSizes(AreaData.Area area)
    {
        
     //   float _x = Data.Instance.areaData.areas[areaActiveID].position.x;
     //   float _y = Data.Instance.areaData.areas[areaActiveID].position.y;

        newSizeSignal = Instantiate(sizeSignal) as SizeSignal;
        newSizeSignal.id = areaActiveID;
        newSizeSignal.transform.SetParent(container.transform);
        newSizeSignal.transform.localPosition = new Vector3(0,0,0);
        newSizeSignal.transform.localScale = Vector3.one;
        newSizeSignal.Init(Data.Instance.areaData.areas[areaActiveID].width, Data.Instance.areaData.areas[areaActiveID].height);
        sizeSignals.Add(newSizeSignal);

        Invoke("SelectArea", 0.1f);
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
        foreach (SizeSignal sizeSignal in sizeSignals)
        {
            int _height =  sizeSignal.GetHeight();
            Data.Instance.areaData.areas[sizeSignal.id].height = _height;
			Data.Instance.areaData.areas[sizeSignal.id].width = GetComponent<WallCreator>().createdPlanes[sizeSignal.id].GetComponent<SizeCalculator>().CalculateWidth(_height);
        }
		
		Events.SaveAreas();
        //Data.Instance.areaData.Save();

        areaActiveID++;

        if (Data.Instance.areaData.areas.Count > areaActiveID)
        {
            Destroy(newSizeSignal);
            AddConfirmSizes(Data.Instance.areaData.areas[areaActiveID]);
        }
        else
        {
            Data.Instance.LoadLevel("ArtPlaced");
        }
    }

}
