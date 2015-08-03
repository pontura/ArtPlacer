using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class Walls : MonoBehaviour {

    public GameObject ResetedContainer;
    public GameObject ReadyContainer;

	void Start () {
        if (Data.Instance.areaData.areas.Count > 0)
			Started();
		else
        	Reseted();
        Events.OnNumWallsChanged += OnNumWallsChanged;
	}
    void OnDestroy()
    {
        Events.OnNumWallsChanged -= OnNumWallsChanged;
    }
    void OnNumWallsChanged(int qty)
    {
        if (qty < 1)
            Reseted();
        else
            Started();
    }
    public void GotoLoadRoom()
    {
        Data.Instance.LoadLevel("LoadRoom");
    }
    public void Ready()
    {
		Events.SaveAreas ();
        Data.Instance.LoadLevel("ConfirmSizes");
    }
    public void Reseted()
    {
        ResetedContainer.gameObject.SetActive(true);
        ReadyContainer.gameObject.SetActive(false);
		GetComponent<PhotoAddWall>().ActiveAdd();
    }
    public void Started()
    {
        ResetedContainer.gameObject.SetActive(false);
        ReadyContainer.gameObject.SetActive(true);
		GetComponent<PhotoAddWall> ().DeactiveAdd ();
    }
}
