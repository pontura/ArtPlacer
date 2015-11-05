using UnityEngine;
using System.Collections;

public class HelpManager : MonoBehaviour {

    public GameObject button;
    public GameObject button_on;
    public GameObject button_off;

    public bool activated;

	void Start () {
        Events.HelpHide += HelpHide;
        activated = true;
        Toogle();
	}	
    void HelpHide()
    {
        button.SetActive(false);
        activated = false;
	}
    public void Toogle()
    {
        activated = !activated;
        if (activated)
        {
            button_on.SetActive(true);
            button_off.SetActive(false);
        }
        else
        {
            button_on.SetActive(false);
            button_off.SetActive(true);
        }
        Events.HelpChangeState(activated);
    }
}
