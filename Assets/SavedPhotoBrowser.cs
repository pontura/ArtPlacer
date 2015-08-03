using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;

public class SavedPhotoBrowser : MonoBehaviour
{
    public Text debbugText;
    public Text titleField;

    public ThumbImage button;
    public GameObject container;
    
    [Serializable]
    public class ThumbData
    {
        public string title;
        public int id;
    }

    public ThumbData[] data;
    public Vector2 thumbSize = new Vector2(195, 120);
    public Vector2 separation = new Vector2(2, 2);
    public int cols;

    private bool isOn;
    private bool onlineRooms;
    private RoomsData roomData;


    void Start()
    {
        Data.Instance.SetMainMenuActive(true);

        roomData = Data.Instance.roomsData;

        thumbSize += separation;

        int id = 0;
        int separationY = 0;
        int separationx = 0;

        List<RoomsData.Room> rooms;
        string title;

        switch (roomData.type)
        {
            case RoomsData.types.LOCAL: 
                title = "YOUR SAVED ROOMS";  
                rooms = Data.Instance.roomsData.rooms; 
                break;
            default: 
                title = "PUBLIC ROOMS"; 
                rooms = Data.Instance.roomsData.onlineRooms; 
                break;
        }

        titleField.text = title;

        foreach (RoomsData.Room room in rooms)
        {
            ThumbImage newButton = Instantiate(button);

            string folder = Data.Instance.GetRoomsPath();

            string filePath;

            if (roomData.type == RoomsData.types.LOCAL)
                filePath = Path.Combine(folder, room.url + ".png");
            else
                filePath = room.url;


          //  debbugText.text = filePath;

            print(filePath);

            newButton.InitRoom(this, filePath, id);

            newButton.transform.SetParent(container.transform);
            newButton.transform.localScale = Vector3.one;
            newButton.transform.localPosition = Vector3.zero;
            float _x = (thumbSize.x / 2) + (thumbSize.x * separationx);
            float _y = (-thumbSize.y / 2) + (-1 * (thumbSize.y * separationY));

            newButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(_x, _y, 1);

            if (separationx == cols - 1)
            {
                separationY++;
                separationx = 0;
            }
            else separationx++;
            id++;
        }
    }
    public void OnSelect(int id)
    {
        RoomsData.Room room;

        if (roomData.type == RoomsData.types.LOCAL)
            room = Data.Instance.roomsData.rooms[id];
        else
            room = Data.Instance.roomsData.onlineRooms[id];

        Data.Instance.areaData.url = room.url;
        Data.Instance.areaData.id = 1;
        foreach (RoomsData.RoomArea roomArea in room.area)
        {
            Data.Instance.areaData.AddAreas(-1, roomArea.pointers, roomArea.position, roomArea.width, roomArea.height);
        }
        Data.Instance.LoadLevel("ArtPlaced");
    }
    void SetOff()
    {
        foreach (DropboxButton child in container.GetComponentsInChildren<DropboxButton>())
        {
            Destroy(child.gameObject);
        }
    }
}
