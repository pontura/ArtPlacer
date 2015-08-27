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
        foreach (RoomsData.RoomArea roomArea in room.area)
        {
            Data.Instance.areaData.AddAreas(-1, roomArea.pointers, roomArea.position, roomArea.width, roomArea.height);
        }
        Data.Instance.LoadLevel("ArtPlaced");
       
    }
}
