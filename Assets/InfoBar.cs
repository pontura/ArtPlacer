﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InfoBar : MonoBehaviour {

    public Button infoOnButton;
    public Button infoOffButton;
    public Text field;
    public Animation anim;

    void Start()
    {
        field.text = "Gallery: " + Data.Instance.artData.selectedArtWork.gallery + "\n";
        field.text += "Title: " + Data.Instance.artData.selectedArtWork.title + "\n";
        field.text += "Autor: " + Data.Instance.artData.selectedArtWork.autor + "\n";
        field.text += "Sizes: " + Data.Instance.artData.selectedArtWork.size + "\n";
        field.text += "Technique: " + Data.Instance.artData.selectedArtWork.technique + "\n";
    }
	public void SetOn()
    {
        anim.Play("InfoBarOn");
        infoOnButton.gameObject.SetActive(false);
        infoOffButton.gameObject.SetActive(true);
	}
    public void SetOff()
    {
        infoOnButton.gameObject.SetActive(true);
        infoOffButton.gameObject.SetActive(false);
        anim.Play("InfoBarOff");
    }
}
