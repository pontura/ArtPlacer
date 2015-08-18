using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class Walls : MonoBehaviour {

    public Animation tooltipAddWall;
    public Animation tooltipSelectWall;
    public Animation tooltipFitEdges;

    public GameObject State2;
    public Button AddButton;

	void Start () {

        GetComponent<PhotoAddWall>().DeactiveAdd();

        tooltipAddWall.gameObject.SetActive(false);
        tooltipSelectWall.gameObject.SetActive(false);
        tooltipFitEdges.gameObject.SetActive(false);
        
        if (Data.Instance.areaData.areas.Count > 0)
			Started();
		else
        	Reseted();

        Events.OnNumWallsChanged += OnNumWallsChanged;
        Events.OnWallEdgeSelected += OnNumWallsChanged;
    }
    void OnDestroy()
    {
        Events.OnNumWallsChanged -= OnNumWallsChanged;
        Events.OnWallEdgeSelected -= OnNumWallsChanged;
    }
    private int totalWalls;

    void OnNumWallsChanged(int qty)
    {
        if (totalWalls < qty)
        {
            tooltipFitEdges.gameObject.SetActive(true);
            tooltipFitEdges.Play("tooltipOnVertical");
        }
        AddButton.GetComponentInChildren<Image>().color = Color.white;
        tooltipSelectWall.gameObject.SetActive(false);

        if (qty < 1)
            Reseted();
        else
            Started();

        totalWalls = qty;
    }
    void OnNumWallsChanged()
    {
        tooltipFitEdges.gameObject.SetActive(false);
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
    public void Add()
    {
        AddButton.GetComponent<Animation>().Stop();
        AddButton.GetComponentInChildren<Image>().color = Data.Instance.selectedColor;
        tooltipAddWall.gameObject.SetActive(false);
        tooltipSelectWall.gameObject.SetActive(true);

        tooltipSelectWall.Play("tooltipOnVertical");
        Invoke("AddAvailable", 0.5f);
    }
    void AddAvailable()
    {
        GetComponent<PhotoAddWall>().ActiveAdd();
    }
    public void Reseted()
    {
        tooltipAddWall.gameObject.SetActive(true);
        tooltipAddWall.Play("tooltipOn");
        State2.gameObject.SetActive(false);
    }
    public void Started()
    {
        State2.gameObject.SetActive(true);
    }
}
