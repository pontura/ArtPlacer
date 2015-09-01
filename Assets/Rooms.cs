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
    public int cols;

    private int selectionId;
    private bool isOn;
    private int id;

    private RoomsData roomData;


    void Start()
    {

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
                 filePath = GetUrlPath(room.url + "_thumb.png");
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
        id++;
    }
    void SetOff()
    {
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

        Data.Instance.roomsData.actualRoomId = id;

        if (roomData.type == RoomsData.types.LOCAL)
            room = Data.Instance.roomsData.rooms[id];
        else
            room = Data.Instance.roomsData.onlineRooms[id];

        Data.Instance.areaData.areas.Clear();
        Data.Instance.areaData.url = room.url;
        Data.Instance.areaData.id = 1;
        
        Data.Instance.LoadLevel("Room");
    }
    public string GetUrlPath(string fileName)
    {
        string url = Data.Instance.GetFullPathByFolder("Rooms", fileName);
        return url;
    }
}
