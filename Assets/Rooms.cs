using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using ImageVideoContactPicker;

public class Rooms : MonoBehaviour
{
    public Animation anim;
    public GameObject Add;
    public GameObject Add_On;
    public GameObject SubMenu;

    public Text title;
    public Text helpField;
    public ThumbImage button;
    public GameObject container;
    public int cols;

    private int selectionId;
    private bool isOn;
    private int id;

    private RoomsData roomData;


    void Start()
    {
		Events.OnLoading (true);
        Close();

        Events.HelpShow();

        Data.Instance.SetTitle("ROOMS");
        Events.Back += Back;
        PickerEventListener.onImageLoad += OnImageLoad;

        Data.Instance.SetMainMenuActive(true);

        List<RoomsData.Room> rooms;
        roomData = Data.Instance.roomsData;

        /*switch (roomData.type)
        {
            case RoomsData.types.LOCAL:
                title.text = "SAVED PROJECTS";
                rooms = Data.Instance.roomsData.rooms;
                break;
            default:
                title.text = "PUBLIC ROOMS";
                rooms = Data.Instance.roomsData.onlineRooms;
                break;
        }*/
		title.text = "SAVED PROJECTS";
		rooms = Data.Instance.roomsData.rooms;
        Invoke("timeOut", 0.2f);
    }
    void timeOut()
    {
        List<RoomsData.Room> rooms = Data.Instance.roomsData.rooms;
        if (rooms.Count == 0)
        {
            Events.HelpChangeState(true);
            helpField.text = "There are no rooms added";
        }
        else
        {

            foreach (RoomsData.Room room in rooms)
            {
                ThumbImage newButton = Instantiate(button);

                string folder = Data.Instance.GetRoomsPath();

                string filePath;

                if (roomData.type == RoomsData.types.LOCAL)
                {
                    Data.Instance.SetRoomFromLocalFiles(true);
                    filePath = GetUrlPath(room.url + "_thumb.png");
                }
                else
                    filePath = room.url;

                AddThumb(filePath);

            }
        }
		Events.OnLoading (false);
    }
    void OnDestroy()
    {
        PickerEventListener.onImageLoad -= OnImageLoad;
        Events.Back -= Back;
    }
    public void OnImageLoad(string imgPath, Texture2D tex)
    {
        Data.Instance.SetRoomFromLocalFiles(true);

		float currAspect = Screen.currentResolution.width * 0.8f / Screen.currentResolution.height;
		float texAspect = tex.width / tex.height;
		if (texAspect > currAspect) {
			Texture2D result = new Texture2D ((int)(tex.width * 1.2f), (int)(tex.height * 1.2f), tex.format, true);
			for (int y = 0; y < result.height; y++) {
				for (int x = 0; x < result.width; x++) {
					if (y > (result.height * 0.1f) && y < (result.height * 0.9f) && x > (result.width * 0.1f) && x < (result.width * 0.9f)) {
						result.SetPixel (x, y, tex.GetPixel (x - (int)(tex.width * 0.1f), y - (int)(tex.height * 0.1f)));
					} else {
						result.SetPixel (x, y, Color.black);
					}
				}
			}				
			result.Apply ();
			Data.Instance.lastPhotoTexture = result;
		} else {
			Data.Instance.lastPhotoTexture = tex;
		}
		DestroyImmediate(tex);
        Data.Instance.LoadLevel("ConfirmPhoto");
    }

    void Back()
    {
        Data.Instance.LoadLevel("Intro");
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
    public void Open()
    {
       // print(StoreData.Instance.GetComponent<StoreSettings>().loaded  + "  " + Data.Instance.roomsData.rooms.Count + "  ---  " + StoreData.Instance.GetComponent<StoreSettings>().max_rooms);
        if (
            !StoreData.Instance.fullVersion && 
            StoreData.Instance.GetComponent<StoreSettings>().loaded &&
            Data.Instance.roomsData.rooms.Count >= StoreData.Instance.GetComponent<StoreSettings>().max_rooms
            )
        {
            Events.OnGetFullVersion(StoreData.Instance.GetComponent<StoreSettings>().msg_rooms);
            return;
        }
        Add.SetActive(false);
        Add_On.SetActive(true);
        SubMenu.SetActive(true);
        anim.Play("subMenuOpen");
    }
    public void Close()
    {
        Add.SetActive(true);
        Add_On.SetActive(false);
        SubMenu.SetActive(false);
        //Data.Instance.LoadLevel("LoadRoom");
    }
    public void TakePhoto()
    {
        EventsAnalytics.SendScreen("NEW_ROOM_PHOTO");
        Data.Instance.SetRoomFromLocalFiles(false);
        Data.Instance.isPhoto4Room = true;
        Data.Instance.LoadLevel("TakePhoto");
    }
    public void Browse()
    {
        EventsAnalytics.SendScreen("NEW_ROOM_BROWSE");
        Events.OnPicker(true);
        Invoke("Delay", 0.1f);
    }
    void Delay()
    {
#if UNITY_ANDROID
        AndroidPicker.BrowseImage();
#elif UNITY_IPHONE
			IOSPicker.BrowseImage();
#endif

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
