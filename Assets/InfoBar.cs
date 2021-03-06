﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InfoBar : MonoBehaviour {

    //public Button infoOnButton;
   // public Button infoOffButton;
    public Text field;
  //  public Animation anim;

    void Start()
    {
        field.text = "Gallery: " + Data.Instance.artData.selectedArtWork.gallery + "\n";
       // field.text += "Title: " + Data.Instance.artData.selectedArtWork.title + "\n";
        field.text += "Artist: " + Data.Instance.artData.selectedArtWork.autor + "\n";
        field.text += "Size: " + Data.Instance.artData.selectedArtWork.getSizeWUnits() + "\n";
        if (Data.Instance.artData.selectedArtWork.technique != null)
        {
            if (!Data.Instance.artData.selectedArtWork.gallery.Equals("My Artworks")) 
                field.text += "Description: " + Data.Instance.artData.selectedArtWork.technique + "\n";
        }
		Events.ConvertUnits += ConvertUnits;
    }

	private void ConvertUnits(){
		field.text = "Gallery: " + Data.Instance.artData.selectedArtWork.gallery + "\n";
		// field.text += "Title: " + Data.Instance.artData.selectedArtWork.title + "\n";
		field.text += "Artist: " + Data.Instance.artData.selectedArtWork.autor + "\n";
		field.text += "Size: " + Data.Instance.artData.selectedArtWork.getSizeWUnits() + "\n";

        if (Data.Instance.artData.selectedArtWork.technique != null)
        {
            if (!Data.Instance.artData.selectedArtWork.gallery.Equals("My Artworks"))
                field.text += "Description: " + Data.Instance.artData.selectedArtWork.technique + "\n";
        }
	}

	void OnDestroy()
	{
		Events.ConvertUnits -= ConvertUnits;
	}
}
