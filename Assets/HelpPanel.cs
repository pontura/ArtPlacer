using UnityEngine;
using System.Collections;

public class HelpPanel : MonoBehaviour {

    public int step;
    public GameObject[] steps;

	void Start () {
        HidePanel();
        Events.HelpChangeState += HelpChangeState;
        Events.HelpChangeStep += HelpChangeStep;
        if (steps != null && steps.Length > 0)
            step = 1;
	}
    void OnDestroy()
    {
        Events.HelpChangeState -= HelpChangeState;
        Events.HelpChangeStep -= HelpChangeStep;
    }
    void HelpChangeStep(int _step)
    {
        step = _step;
    }
    void HelpChangeState(bool isActive)
    {
        if (isActive)
        {
            ShowPanel();
        }
        else
        {
            HidePanel();
        }
	}
    void ShowPanel()
    {
        gameObject.SetActive(true);
        if (steps != null && steps.Length > 0)
        {
            foreach (GameObject thisStep in steps)
            {
                thisStep.SetActive(false);
            }
            steps[step-1].SetActive(true);
        }
    }
    void HidePanel()
    {
        gameObject.SetActive(false);
    }
    public void Close()
    {
        Events.HelpChangeState(false);
        print("close");
    }
}
