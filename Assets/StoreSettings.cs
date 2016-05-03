using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using SimpleJSON;
public class StoreSettings : MonoBehaviour {

    private string URL = "http://artplacer.com/getalldata.php?type=settings";

    public bool locked;
    public int max_favourites;
    public int max_artworks;
    public int max_rooms;
    public string msg_favourites;
    public string msg_artworks;
    public string msg_rooms;
    public string msg_general;

    public bool loaded;

	void Start () {
        StartCoroutine(LoadData());
	}
    public IEnumerator LoadData()
    {
        WWW textURLWWW = new WWW(URL);
        yield return textURLWWW;

        var N = JSON.Parse(textURLWWW.text);

        if (int.Parse(N["settings"]["locked"]) == 1)
            locked = true;
        else
            locked = false;

        max_favourites = int.Parse(N["settings"]["max_favourites"]);
        max_artworks = int.Parse(N["settings"]["max_artworks"]);
        max_rooms = int.Parse(N["settings"]["max_rooms"]);
        msg_favourites = N["settings"]["msg_favourites"];
        msg_artworks = N["settings"]["msg_artworks"];
        msg_rooms = N["settings"]["msg_rooms"];
        msg_general = N["settings"]["msg_general"];

        loaded = true;
    }
}
