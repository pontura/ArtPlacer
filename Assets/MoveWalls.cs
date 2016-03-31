using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MoveWalls : MonoBehaviour {

    public Animation  anim;
    public bool opened;
    public GameObject closeFooter;

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
        anim.Play("FooterOff");
        closeFooter.SetActive(true);
    }
}
