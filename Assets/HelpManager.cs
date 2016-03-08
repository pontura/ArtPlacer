using UnityEngine;
using System.Collections;

public class HelpManager : MonoBehaviour {

    public GameObject button;
    public GameObject button_on;
    public GameObject button_off;

    public bool activated;

	void Start () {
        Events.HelpHide += HelpHide;
        Events.HelpShow += HelpShow;
        Events.HelpChangeState += HelpChangeState;
        activated = true;
        Toogle();
	}
    void OnDestroy()
    {
        Events.HelpHide -= HelpHide;
        Events.HelpShow -= HelpShow;
        Events.HelpChangeState -= HelpChangeState;
    }
    void HelpShow()
    {
        activated = true;
        button.SetActive(activated);        
    }
    void HelpHide()
    {
        activated = false;
        button.SetActive(activated);        
	}
    public void Toogle()
    {
        activated = !activated;        
        Events.HelpChangeState(activated);
    }
    void HelpChangeState(bool isActive)
    {
        if (isActive)
        {
            button_on.SetActive(true);
            button_off.SetActive(false);
        }
        else
        {
            button_on.SetActive(false);
            button_off.SetActive(true);
        }
        activated = isActive;
    }
    //void Update()
    //{
    //    if (activated)
    //    {
    //        if (Input.anyKeyDown)
    //        {
    //            Toogle();
    //            activated = true;
    //            Invoke("timeOut", 0.5f);
    //        }
    //    }
    //}
    void timeOut()
    {
        activated = false;
    }
}
