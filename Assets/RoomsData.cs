using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RoomsData : MonoBehaviour {

    //se fija si algo cambió par grabar o salir
    public bool changesMade;

    public types type;
    public enum types
    {
        ONLINE,
        LOCAL
    }

    public int actualRoomId;

    [Serializable]
    public class Room
    {
        public string url;
		public int id;
        public List<RoomArea> area;
    }
    [Serializable]
    public class RoomArea
    {
		public float width;
        public float height;
        public Vector3[] pointers;
        public Vector3 position;
		public List<RoomAreaArtWork> artworks;
    }
    public List<Room> rooms;
    public List<Room> onlineRooms;

	[Serializable]
	public class RoomAreaArtWork{
		public Vector3 position;
		public int height;
		public int id;
		public int galleryID;
		public int galleryArtID;
	}

	void Start () {
        ReadRoomsData();
	}
    public void ReadRoomsData()
    {
        rooms.Clear();
        for (var id = 0; id < 100; id++)
        {
            string roomData = PlayerPrefs.GetString("room_" + id);

            if (roomData != "" && roomData != null)
            {
              //  print("room_" + id + "   ---->  " + roomData);
                Room room = new Room();

                string[] result = roomData.Split(":"[0]);
                room.url = result[0];
				room.id = id;

                string[] areas = result[1].Split("+"[0]);

                room.area = new List<RoomArea>();

                foreach (string area in areas)
                {
                    string[] res = area.Split("_"[0]);
                   // print("area: " + area);
                    if (res.Length > 1)
                    {
                        RoomArea roomArea = new RoomArea();
						roomArea.width = float.Parse(res[0]);
                        roomArea.height = float.Parse(res[1]);
                        roomArea.position = new Vector3(GetFloat(res[2]), GetFloat(res[3]), 1);

                        roomArea.pointers = new Vector3[4];

                        roomArea.pointers[0] = new Vector3(GetFloat(res[4]), GetFloat(res[5]), 0);
                        roomArea.pointers[1] = new Vector3(GetFloat(res[6]), GetFloat(res[7]), 0);
                        roomArea.pointers[2] = new Vector3(GetFloat(res[8]), GetFloat(res[9]), 0);
                        roomArea.pointers[3] = new Vector3(GetFloat(res[10]), GetFloat(res[11]), 0);

						string[] artworks = res[12].Split("*"[0]);

						roomArea.artworks = new List<RoomAreaArtWork>();

						foreach (string artwork in artworks){
							string[] resu = artwork.Split("/"[0]);
							// print("area: " + area);
							if (resu.Length > 1){
								RoomAreaArtWork roomArt = new RoomAreaArtWork();
								roomArt.position = new Vector3(GetFloat(resu[0]),GetFloat(resu[1]),0);
								roomArt.galleryID = int.Parse(resu[2]);
								roomArt.galleryArtID = int.Parse(resu[3]);
								roomArea.artworks.Add(roomArt);
							}
						}

                        room.area.Add(roomArea);
                    }
                }
                rooms.Add(room);
            }
        }
    }
	public string GetRoomName(string url)
	{
		int id = 0;
		return GetRoomName(out id, url);
	}
    public string GetRoomName(out int _id, string url)
    {
        int id = 0;
		bool firstFreeID = false;
        foreach (Room room in rooms)
        {
            if (room.url == url)
            {
				_id = id;
                return "room_" + room.id;
             }
			//id=room.id+1;
			if(!firstFreeID){
				if(room.id>id)
					firstFreeID=true;
				else
					id++;
			}
			
        }
		_id = id;
        return "room_" + id;
    }
    private float GetFloat(string stringValue)
    {
        float result = 0;
        float.TryParse(stringValue, out result);
        return result;
    }
    public void RemoveArtFromRoom(int GalleryArtID)
    {
        ChangesMade(true);
        RoomAreaArtWork toDelete = null;
        RoomArea roomAreaToDelete = null;

        foreach(Room room in rooms)
        {
             foreach(RoomArea roomArea in room.area)
             {
                 foreach (RoomAreaArtWork artwork in roomArea.artworks)
                 {
                     if(artwork.id == GalleryArtID)
                     {
                         toDelete = artwork;
                         roomAreaToDelete = roomArea;
                     }
                 }
             }
        }
        if (roomAreaToDelete != null)
        {
            print("roomAreaToDelete " + toDelete.id);
            roomAreaToDelete.artworks.Remove(toDelete);
        }
    }
    public void ChangesMade(bool made)
    {
        changesMade = made;
    }

}
