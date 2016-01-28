using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ImageVideoContactPicker;

public class Fade : MonoBehaviour
{
    public GameObject panel;

    private void Start()
    {
        Events.OnPicker += OnPicker;
        SetOff();
	}
    void OnPicker(bool state)
    {
        if (state)
            SetOn();
        else
            SetOff();
    }
    public void SetOn()
    {
        panel.SetActive(true);
        Invoke("SetOff", 3);
    }
    public void SetOff()
    {
        panel.SetActive(false);
    }
}