using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class Walls : MonoBehaviour {

   // public Animation tooltipAddWall;
    public states state;
    public enum states
    {
        EMPTY,
        ADDING,
        EDITTING_WALL,
        EDITING_HEIGHT,
        READY
    }
    public Animation tooltipSelectWall;
    public Animation tooltipFitEdges;

    public bool editingSizes;

    public Button AddButton;
    public Button deleteButton;
    public Button confirmButton;
    public Button readyButton;


    public int numWalls;

	void Start () {

        readyButton.interactable = false;
        Events.HelpShow();

        Data.Instance.SetTitle("");
        
        GetComponent<PhotoAddWall>().DeactiveAdd();

        //  tooltipAddWall.gameObject.SetActive(false);
        tooltipSelectWall.gameObject.SetActive(false);
        tooltipFitEdges.gameObject.SetActive(false);
        
        //if (Data.Instance.areaData.areas.Count > 0)
        //    Started();
        //else
        	Reseted();

        Events.Back += Back;
        Events.OnNumWallsChanged += OnNumWallsChanged;
        Events.OnWallEdgeSelected += OnNumWallsChanged;
        Events.OnWallEdgeSelected += OnWallEdgeSelected;
        Events.OnWallActive += OnWallActive;
        Invoke("timeOut", 0.2f);

       
    }
    void SetState()
    {
        switch (state)
        {
            case states.EMPTY:
                Data.Instance.backButon.SetActive(true);
                Events.HelpChangeStep(1);
                deleteButton.interactable = false;
                confirmButton.interactable = false;
                AddButton.interactable = true;
                readyButton.interactable = false;
                break;
            case states.ADDING:
                Events.HelpChangeStep(2);
                deleteButton.interactable = false;
                confirmButton.interactable = false;
                AddButton.interactable = true;
                readyButton.interactable = false;
                break;
            case states.EDITTING_WALL:
                Events.HelpChangeStep(2);
                deleteButton.interactable = true;
                confirmButton.interactable = true;
                AddButton.interactable = false;
                readyButton.interactable = false;
                break;
            case states.EDITING_HEIGHT:
                Events.HelpChangeStep(3);
                deleteButton.interactable = false;
                confirmButton.interactable = true;
                AddButton.interactable = false;
                readyButton.interactable = false;
                break;
            case states.READY:
                Data.Instance.backButon.SetActive(true);
                Events.HelpChangeStep(2);
                deleteButton.interactable = false;
                confirmButton.interactable = false;
                AddButton.interactable = true;
                readyButton.interactable = true;
                break;
        }
    }
    void timeOut()
    {
        if (Data.Instance.areaData.areas.Count == 0)
            Events.HelpChangeState(true);
        else
        {
            state = states.READY;
            SetState();
        }
    }
    public void Back()
    {
        if (Data.Instance.areaData.areas.Count == 0)
        {
            Events.HelpChangeState(true);
            return;
        }
		if (Data.Instance.areaData.url.Equals (""))
			Data.Instance.LoadLevel("ConfirmPhoto");
		else
			Data.Instance.LoadLevel("Artplaced");
        
    }
    void OnDestroy()
    {
        Events.Back -= Back;
        Events.OnNumWallsChanged -= OnNumWallsChanged;
        Events.OnWallEdgeSelected -= OnNumWallsChanged;
        Events.OnWallEdgeSelected -= OnWallEdgeSelected;
        Events.OnWallActive -= OnWallActive;
    }
    public int totalWalls;

    void OnWallEdgeSelected()
    {
        state = states.EDITTING_WALL;
        SetState();
    }
    private WallPlane wallPlane;
    void OnWallActive(WallPlane _wallPlane)
    {
        this.wallPlane = _wallPlane;
    }
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
        Data.Instance.backButon.SetActive(false);
    }
    public void GotoLoadRoom()
    {
        Data.Instance.LoadLevel("LoadRoom");
    }
    public void Confirm()
    {
        AddButton.interactable = false;
        if (editingSizes)
        {
            editingSizes = false;
            GetComponent<HeightConfirm>().Hide();
            Events.ResetPointers();
            Reseted();
            state = states.READY;
            SetState();
            wallPlane.areaHeight = GetComponent<HeightConfirm>().result;
        }
        else
        {
            editingSizes = true;
            GetComponent<HeightConfirm>().Init(wallPlane.areaHeight);
            state = states.EDITING_HEIGHT;
            SetState();
        }
    }
    public void Ready()
    {
		Events.SaveAreas ();
        Data.Instance.LoadLevel("ArtPlaced");
    }
    public bool adding;
    public void Add()
    {
        state = states.ADDING;
        SetState();

		Events.OnWallEdgeUnSelected ();
        Events.HelpChangeState(false);

        if (!adding)
        {
           AddButton.GetComponentInChildren<Image>().color = Data.Instance.selectedColor;
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
        state = states.EMPTY;
        SetState();
    }
    public void Started()
    {
        state = states.EDITTING_WALL;
        SetState();
    }
}
