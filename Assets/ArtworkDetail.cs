﻿using UnityEngine;
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
        if (Data.Instance.lastArtTexture == null)
        {
            anim.gameObject.SetActive(false);
        } else{
           
            Close();
        }
	}
    void OnDestroy()
    {
        Events.SetArtworkDetail -= SetArtworkDetail;
    }
    void SetArtworkDetail(bool _state)
    {
        anim.gameObject.SetActive(true);

        Texture2D texture2d = Data.Instance.lastArtThumbTexture;
        thumb.sprite = Sprite.Create(texture2d, new Rect(0, 0, texture2d.width, texture2d.height), new Vector2(0, 0));

        if (state == _state) return;

        if (_state) 
            Open();
        else
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
