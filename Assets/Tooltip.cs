using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tooltip : MonoBehaviour {

    public Text field;
    public GameObject asset;

    void Start()
    {
        Events.OnTooltipOn += OnTooltipOn;
        Events.OnTooltipOff += OnTooltipOff;
        OnTooltipOff();
    }
    void OnDestroy()
    {
        Events.OnTooltipOn -= OnTooltipOn;
        Events.OnTooltipOff -= OnTooltipOff;
    }
    void OnTooltipOn(string text)
    {
        SetOn();
        field.text = text;
    }
    public void SetOn()
    {
        asset.SetActive(true);
    }
    public void OnTooltipOff()
    {
        asset.SetActive(false);
    }
}
