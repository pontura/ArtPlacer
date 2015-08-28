using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;

public class Rooms : MonoBehaviour
{
    public Text title;
    public ThumbImage button;
    public GameObject container;
    public ScrollLimit scrollLimit;

    public Vector2 thumbSize = new Vector2(180, 180);
    public Vector2 separation = new Vector2(2, 2);
    public int cols;

    private int selectionId;
    private bool isOn;
    private int separationY = 0;
    private int separationx = 0;
    private int id;

    private RoomsData roomData;
	private int defaultArtHeight = 50;
	private int totalArtworks2Load=0;
	private int LoadedArtwork=0;

    void Start()
    {
        thumbSize += separation;
      //  ArtData.GalleryData currentGallery = Data.Instance.artData.GetCurrentGallery();

        List<RoomsData.Room> rooms;
        roomData = Data.Instance.roomsData;

        switch (roomData.type)
        {
            case RoomsData.types.LOCAL:
                title.text = "YOUR SAVED ROOMS";
                rooms = Data.Instance.roomsData.rooms;
                break;
            default:
                title.text = "PUBLIC ROOMS";
                rooms = Data.Instance.roomsData.onlineRooms;
                break;
        }

        foreach (RoomsData.Room room in rooms)
        {
            ThumbImage newButton = Instantiate(button);

            string folder = Data.Instance.GetRoomsPath();

            string filePath;

            if (roomData.type == RoomsData.types.LOCAL)
                filePath = Path.Combine(folder, room.url + ".png");
            else
                filePath = room.url;

            AddThumb(filePath);

        }
        //if (separationY > 3) scrollLimit.SetMaxScroll(100);
    }
    private void AddThumb(string url)
    {
        ThumbImage newButton = Instantiate(button);
        newButton.transform.SetParent(container.transform);
        newButton.transform.localScale = Vector3.one;
        newButton.transform.localPosition = Vector3.zero;
        newButton.InitRoom(this, url, id);
        float _x = (thumbSize.x / 2) + (thumbSize.x * separationx);
        float _y = (-thumbSize.y / 2) + (-1 * (thumbSize.y * separationY));
        newButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(_x, _y, 0);
        if (separationx == cols - 1) { separationY++; separationx = 0; } else separationx++;
        id++;
    }
    void SetOff()
    {
        scrollLimit.ResetScroll();
        separationY = 0;
        separationx = 0;
        foreach (ThumbImage child in container.GetComponentsInChildren<ThumbImage>())
            Destroy(child.gameObject);
    }
    public void Galleries()
    {
            Data.Instance.LoadLevel("Galleries");
    }
    public void GotoRooms()
    {
        Data.Instance.LoadLevel("LoadRoom");
    }
    public void OnSelect(int id)
    {
        RoomsData.Room room;

        if (roomData.type == RoomsData.types.LOCAL)
            room = Data.Instance.roomsData.rooms[id];
        else
            room = Data.Instance.roomsData.onlineRooms[id];

		Data.Instance.areaData.areas.Clear ();
        Data.Instance.areaData.url = room.url;
        Data.Instance.areaData.id = 1;

		foreach (RoomsData.RoomArea rArea in room.area)
			totalArtworks2Load += rArea.artworks.Count;

        foreach (RoomsData.RoomArea roomArea in room.area)
        {
            Data.Instance.areaData.AddAreas(-1, roomArea.pointers, roomArea.position, roomArea.width, roomArea.height);
			foreach (RoomsData.RoomAreaArtWork areaArtwork in roomArea.artworks){
				StartCoroutine(GetArtData(areaArtwork,Data.Instance.areaData.areas.Count-1,room));
			}
        }

		Debug.Log ("Loading...");
    }

	public IEnumerator GetArtData(RoomsData.RoomAreaArtWork areaArtwork, int areaId, RoomsData.Room room)
	{
		ArtData.GalleryData.ArtData artData = Data.Instance.artData.galleries[areaArtwork.galleryID].artWorksData[areaArtwork.galleryArtID];
		WWW imageURLWWW = new WWW(artData.url);		
		yield return imageURLWWW;
		
		Texture2D tex = imageURLWWW.texture;

		int h = (int)artData.size.y;		
		float aspect = 1f*tex.width/tex.height;
		h = h == 0 ? defaultArtHeight : h;
		int w = (int)(h * aspect);
		Data.Instance.areaData.areas[areaId].AddArtWork(w,h,tex,artData);
		Data.Instance.areaData.areas [areaId].artworks [Data.Instance.areaData.areas [areaId].artworks.Count - 1].position = areaArtwork.position;
		Data.Instance.artData.selectedGallery = areaArtwork.galleryID;
		LoadedArtwork++;
		if(totalArtworks2Load==LoadedArtwork)Data.Instance.LoadLevel("ArtPlaced");
		yield return null;
	}
}
