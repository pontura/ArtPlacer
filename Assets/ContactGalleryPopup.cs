﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ContactGalleryPopup : MonoBehaviour {

    public GameObject Panel;
    public Text title;
    public bool isOn;
    public Animation anim;

    public string phone;
    public string email;
    public string web;
    private ArtData.GalleryData data;

	void Start () {
        Events.ContactGalleryOpenPopup += ContactGalleryOpenPopup;
        SetOff2();
	}
    void ContactGalleryOpenPopup(ArtData.GalleryData data)
    {
        print("ContactGalleryOpenPopup" + data);
        this.data = data;
        if (data != null)
        {
            isOn = true;
            Panel.SetActive(true);
            anim.Play("ContactPopupOn");
            // ArtData.GalleryData data =  Data.Instance.artData.GetCurrentGallery();

            title.text = data.title;
            phone = data.phone;
            email = data.email;
            web = data.web;
        }
    }
    public void SetOff()
    {
        Invoke("SetOff2", 0.12f);
        anim.Play("ContactPopupOff");
    }
    void SetOff2()
    {
        isOn = false;
        Panel.SetActive(false);
    }
    public void Phone()
    {
        Application.OpenURL("tel://" + phone);
        EventsAnalytics.ContactGallery(data.title, "PHONE");
    }
    public void Email()
    {
        //string email = "pontura@gmail.com";
        string subject = MyEscapeURL("Request sent from ArtPlacer");
        string body = MyEscapeURL("");
        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
        EventsAnalytics.ContactGallery(data.title, "EMAIL");
    }
    string MyEscapeURL(string url)
    {
        return WWW.EscapeURL(url).Replace("+", "%20");
    }
    public void Www()
    {
        Application.OpenURL(web);
        EventsAnalytics.ContactGallery(data.title, "WWW");
    }
       
}
