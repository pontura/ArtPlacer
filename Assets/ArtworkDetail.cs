using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ArtworkDetail : MonoBehaviour {

    public Animation  anim;
    public Footer footer;
    public bool state;
    public Image thumb;
    public GameObject closeFooter;

	void Start () {
        Events.SetArtworkDetail += SetArtworkDetail;
        Close();
	}
    void OnDestroy()
    {
        Events.SetArtworkDetail -= SetArtworkDetail;
    }
    void SetArtworkDetail(bool _state)
    {
        Texture2D texture2d = Data.Instance.lastArtThumbTexture;
        thumb.sprite = Sprite.Create(texture2d, new Rect(0, 0, texture2d.width, texture2d.height), new Vector2(0, 0));

        if (state == _state) return;

        if (_state) 
            Open();
        else
            Close();        
    }
    public void Move(int id)
    {
		Events.MoveButton(id);
    }
    public void Hide()
    {
        anim.Play("FooterOff");
        closeFooter.SetActive(true);
    }
    public void Show()
    {
        anim.Play("FooterOn");
        closeFooter.SetActive(false);
    }
    public void Open()
    {
        
        closeFooter.SetActive(false);
        state = true;
        anim.Play("FooterOn");
        footer.FooterPanel.SetActive(false);        
    }
    public void Close()
    {
        state = false;
        anim.Play("FooterOff");
        footer.FooterPanel.SetActive(true);
        footer.Open();
    }
}
