using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class Walls : MonoBehaviour {

   // public Animation tooltipAddWall;
    public Animation tooltipSelectWall;
    public Animation tooltipFitEdges;

    
    public Button AddButton;
    public Button deleteButton;
    public Button confirmButton;

	void Start () {

        Events.HelpShow();

        Data.Instance.SetTitle("");
        
        GetComponent<PhotoAddWall>().DeactiveAdd();

        //  tooltipAddWall.gameObject.SetActive(false);
        tooltipSelectWall.gameObject.SetActive(false);
        tooltipFitEdges.gameObject.SetActive(false);
        
        if (Data.Instance.areaData.areas.Count > 0)
			Started();
		else
        	Reseted();

        Events.Back += Back;
        Events.OnNumWallsChanged += OnNumWallsChanged;
        Events.OnWallEdgeSelected += OnNumWallsChanged;
        Invoke("timeOut", 0.2f);
    }
    void timeOut()
    {
        if (Data.Instance.areaData.areas.Count == 0)
            Events.HelpChangeState(true);
    }
    public void Back()
    {
        Data.Instance.LoadLevel("Rooms");
    }
    void OnDestroy()
    {
        Events.Back -= Back;
        Events.OnNumWallsChanged -= OnNumWallsChanged;
        Events.OnWallEdgeSelected -= OnNumWallsChanged;
    }
    private int totalWalls;

    void OnNumWallsChanged(int qty)
    {
        adding = false;
		qty += Data.Instance.areaData.areas.Count;
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
    private bool adding;
    public void Add()
    {
        if (!adding)
        {
            AddButton.GetComponent<Animation>().Stop();
            AddButton.GetComponentInChildren<Image>().color = Data.Instance.selectedColor;
            // tooltipAddWall.gameObject.SetActive(false);
            tooltipSelectWall.gameObject.SetActive(true);

            tooltipSelectWall.Play("tooltipOnVertical");
            Invoke("AddAvailable", 0.5f);
        }
        else
        {
            AddButton.GetComponentInChildren<Image>().color = Color.white;
            GetComponent<PhotoAddWall>().DeactiveAdd();
        }
        adding = !adding;
    }
    void AddAvailable()
    {
        GetComponent<PhotoAddWall>().ActiveAdd();
    }
    public void Reseted()
    {
        // tooltipAddWall.gameObject.SetActive(true);
        // tooltipAddWall.Play("tooltipOn");
        Events.HelpChangeStep(1);
        deleteButton.interactable = false;
        confirmButton.interactable = false;
    }
    public void Started()
    {
        Events.HelpChangeStep(2);
        deleteButton.interactable = true;
        confirmButton.interactable = true;
    }
}
