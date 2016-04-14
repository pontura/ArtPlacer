using UnityEngine;
using System.Collections;

public class AnalyticsManager : MonoBehaviour {
    
	void Start () {
        EventsAnalytics.SendScreen += SendScreen;
        EventsAnalytics.ContactGallery += ContactGallery;
        EventsAnalytics.NewFavourite += NewFavourite;
        EventsAnalytics.EnterGallery += EnterGallery;
	}
    void SendScreen(string field)
    {
        Debug.Log("<SHARE> SendScreen: " + field);
        GoogleAnalyticsV3.instance.LogScreen(field);       
    }
    void ContactGallery(string GalleryName, string tpyeOfContact)
    {
        Debug.Log("<SHARE> ContactGallery: " + GalleryName + " tpyeOfContact: " + tpyeOfContact);

        EventHitBuilder eventHitBuilder = new EventHitBuilder();
        eventHitBuilder.SetEventCategory("Contact_");
        eventHitBuilder.SetEventLabel(tpyeOfContact);
        eventHitBuilder.SetEventAction(GalleryName);
        GoogleAnalyticsV3.instance.LogEvent(eventHitBuilder);
    }
    void NewFavourite(string GalleryID, string ArtID)
    {
        Debug.Log("<SHARE> NewFavourite GalleryID: " + GalleryID + " ArtID: " + ArtID);

        EventHitBuilder eventHitBuilder = new EventHitBuilder();
        eventHitBuilder.SetEventCategory("New_Favourite");
        eventHitBuilder.SetEventLabel(GalleryID);
        eventHitBuilder.SetEventAction(ArtID);
        GoogleAnalyticsV3.instance.LogEvent(eventHitBuilder);
    }
    void EnterGallery(string GalleryName)
    {
        Debug.Log("<SHARE> EnterGallery: " + GalleryName);

        EventHitBuilder eventHitBuilder = new EventHitBuilder();
        eventHitBuilder.SetEventCategory("Enter_Gallery");
        eventHitBuilder.SetEventAction(GalleryName);
        GoogleAnalyticsV3.instance.LogEvent(eventHitBuilder);
    }
}
