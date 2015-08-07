using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ConfirmSizes : MonoBehaviour {

    public SizeSignal sizeSignal;
    public Animation tooltipSizes;
    public GameObject container;

    public List<SizeSignal> sizeSignals;


    void Start()
    {
        tooltipSizes.gameObject.SetActive(false);

        if (Data.Instance.areaData.areas.Count > 0)
        {
            int id = 0;
            for (int i = 0; i < Data.Instance.areaData.areas.Count; i++)
            {
                float _x = Data.Instance.areaData.areas[i].position.x;
                float _y = Data.Instance.areaData.areas[i].position.y;              

                SizeSignal newSizeSignal = Instantiate(sizeSignal);
                newSizeSignal.id = id;
                newSizeSignal.transform.SetParent(container.transform);
                newSizeSignal.transform.localPosition = new Vector3(_x * 250, _y * 250, 0);
                newSizeSignal.transform.localScale = Vector3.one;
                newSizeSignal.Init(Data.Instance.areaData.areas[i].width, Data.Instance.areaData.areas[i].height);
                sizeSignals.Add(newSizeSignal);
				
				id++;
			}
		}
        Invoke("startTooltip", 0.5f);
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
            Data.Instance.areaData.areas[sizeSignal.id].height = sizeSignal.GetHeight();
            Data.Instance.areaData.areas[sizeSignal.id].width = sizeSignal.GetWidth();
        }
		
		Events.SaveAreas();
        Data.Instance.areaData.Save();
        Data.Instance.LoadLevel("ArtPlaced");
    }

}
