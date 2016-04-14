using UnityEngine;
using System.Collections;

public static class EventsAnalytics {

    public static System.Action<string> SendScreen = delegate { };
    public static System.Action<string, string> ContactGallery = delegate { };
    public static System.Action<string, string> NewFavourite = delegate { };
    public static System.Action<string> EnterGallery = delegate { };

}