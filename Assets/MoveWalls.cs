using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MoveWalls : MonoBehaviour {

    public Animation  anim;
    public bool opened;
    public GameObject closeFooter;

	private GameObject wall;
	private int pointer;

    public bool closedByUser;

	void Start () {
        closeFooter.SetActive(false);
        anim.gameObject.SetActive(false);
        Events.OnWallEdgeSelected += OnWallEdgeSelected;
		Events.OnWallEdgeUnSelected += OnWallEdgeUnSelected;
	}
    void OnDestroy()
    {
        closeFooter.SetActive(false);
        anim.gameObject.SetActive(false);
        Events.OnWallEdgeSelected -= OnWallEdgeSelected;
		Events.OnWallEdgeUnSelected -= OnWallEdgeUnSelected;
    }
    void OnWallEdgeSelected()
    {
        if (closedByUser) return;
        if (opened) return;

        Open();     
    }

	void OnWallEdgeUnSelected()
	{
        if (closedByUser) return;
		if (!opened) return;
		
		Close();     
	}


    float sec = 0;
    private bool Clicking;
    private int id;
    void Update()
    {
        if (!Clicking) return;
        sec += Time.deltaTime * 20;
        if (sec > 1)
        {
            sec = 0;
            Events.MoveButton(id);
        }
    }
    public void Move(int _id)
    {
        print("Move");
        this.id = _id;
        Clicking = true;
    }
    public void OnRelease()
    {
        Clicking = false;
        print("RELEASE");

    }


    public void Open()
    {
        closedByUser = false;
        anim.gameObject.SetActive(true);
        closeFooter.SetActive(false);
        opened = true;
        anim.Play("FooterOn");       
    }
    public void Close()
    {
        closedByUser = true;
        opened = false;
        anim.Play("FooterOff") ;
        closeFooter.SetActive(true);
    }

	public void setWall(GameObject w, int p){
		wall = w;
		pointer = p;
	}
}
