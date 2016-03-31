using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MoveWalls : MonoBehaviour {

    public Animation  anim;
    public bool opened;
    public GameObject closeFooter;

	private GameObject wall;
	private int pointer;

	void Start () {
        closeFooter.SetActive(false);
        anim.gameObject.SetActive(false);
        Events.OnWallEdgeSelected += OnWallEdgeSelected;
	}
    void OnDestroy()
    {
        closeFooter.SetActive(false);
        anim.gameObject.SetActive(false);
        Events.OnWallEdgeSelected -= OnWallEdgeSelected;
    }
    void OnWallEdgeSelected()
    {
        if (opened) return;

        Open();     
    }
    public void Move(int id)
    {
		Events.MoveButton(id);
    }
    public void Open()
    {
        anim.gameObject.SetActive(true);
        closeFooter.SetActive(false);
        opened = true;
        anim.Play("FooterOn");       
    }
    public void Close()
    {
        opened = false;
        anim.Play("FooterOff") ;
        closeFooter.SetActive(true);
    }

	public void setWall(GameObject w, int p){
		wall = w;
		pointer = p;
	}
}
