using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class Room : MonoBehaviour
{
    public RawImage rawImage;
    private int defaultArtHeight = 50;
    private int totalArtworks2Load = 0;
    private int LoadedArtwork = 0;


    void Start()
    {

        Events.Back += Back;
        Data.Instance.SetTitle("All Rooms");

        float maxWidth = rawImage.rectTransform.sizeDelta.x;
        float maxHeight = rawImage.rectTransform.sizeDelta.y;
        float aspect = maxWidth / maxHeight;

        float textAspect = 1f * Data.Instance.lastPhotoThumbTexture.width / Data.Instance.lastPhotoThumbTexture.height;

        if (aspect > textAspect)
            rawImage.rectTransform.sizeDelta = new Vector2(maxHeight * textAspect, maxHeight);
        else if (aspect < textAspect)
            rawImage.rectTransform.sizeDelta = new Vector2(maxWidth, maxWidth / textAspect);

        rawImage.texture = Data.Instance.lastPhotoThumbTexture;

        Events.OnTooltipOff();
    }
	void OnDestroy()
	{
		Events.Back -= Back;
	}
    public void Back()
    {
        Data.Instance.LoadLevel("Rooms");
    }
    public void Delete()
    {
		int id = Data.Instance.roomsData.actualRoomId;
		string path = Data.Instance.roomsData.rooms [id].url;

		print ("Path: " + path);

		string DataName = Data.Instance.roomsData.GetRoomName(path);		
		print ("DataName: " + DataName);
		PlayerPrefs.DeleteKey(DataName);
		Data.Instance.DeletePhotoRoom (path);
		Data.Instance.roomsData.ReadRoomsData ();
		Back ();
    }
    public void Share()
    {
		int id = Data.Instance.roomsData.actualRoomId;
		string url = Data.Instance.roomsData.rooms [id].url;
		string path = Data.Instance.GetFullPathByFolder("Rooms", url + "_thumb.png");

		gameObject.GetComponent<Share>().ShareImage(path,"Artplaced","preview how flat art will look on a wall","like?");
    }

	public void Open()
	{
		//Data.Instance.lastArtTexture = null;

		int id = Data.Instance.roomsData.actualRoomId;
		RoomsData.Room room = Data.Instance.roomsData.rooms[id];
		foreach (RoomsData.RoomArea roomArea in room.area)
        {
            Data.Instance.areaData.AddAreas(-1, roomArea.pointers, roomArea.position, roomArea.width, roomArea.height);
            foreach (RoomsData.RoomAreaArtWork areaArtwork in roomArea.artworks)
            {			
                totalArtworks2Load++;
                StartCoroutine(GetArtData(areaArtwork, Data.Instance.areaData.areas.Count - 1, room));          
            }
        }
        Events.OnLoading(true);
		if(totalArtworks2Load==0)LoadRoomTexture();
    }
    public void Walls()
    {
        Data.Instance.LoadLevel("LoadRoom");
    }
    public void Galleries()
    {
        Data.Instance.LoadLevel("Galleries");
    }
    public IEnumerator GetArtData(RoomsData.RoomAreaArtWork areaArtwork, int areaId, RoomsData.Room room)
    {
		ArtData.GalleryData.ArtData artData;
		//print ("GallID: " + areaArtwork.galleryID + " ArtId: " + areaArtwork.galleryArtID);
		Texture2D tex = null;
		if (areaArtwork.galleryID == -2) {
			artData = Data.Instance.artData.myArtWorks.artWorksData [areaArtwork.galleryArtID];
			tex = TextureUtils.LoadLocal (artData.GetUrl ());
		} else {
			artData = Data.Instance.artData.galleries [areaArtwork.galleryID].artWorksData [areaArtwork.galleryArtID];
			yield return StartCoroutine(TextureUtils.LoadRemote(artData.GetUrl (), value => tex = value));
		}        
        
		print("GetArtData" + areaArtwork + "   " + areaId + "   " + room.url + " " + artData.url);        

        int h = (int)artData.size.y;
        float aspect = 1f * tex.width / tex.height;
        h = h == 0 ? defaultArtHeight : h;
        int w = (int)(h * aspect);
        Data.Instance.areaData.areas[areaId].AddArtWork(w, h, tex, artData);
        Data.Instance.areaData.areas[areaId].artworks[Data.Instance.areaData.areas[areaId].artworks.Count - 1].position = areaArtwork.position;
        Data.Instance.artData.selectedGallery = areaArtwork.galleryID;
        LoadedArtwork++;
        if (totalArtworks2Load == LoadedArtwork)
            LoadRoomTexture();
        yield return null;
    }
    void LoadRoomTexture()
    {
        print("LoadRoomTexture");
		string filePath = GetUrlPath(Data.Instance.areaData.url + ".png");

		Texture2D texture2d = TextureUtils.LoadLocal (filePath);
		if (texture2d != null) {
			Data.Instance.lastPhotoTexture = texture2d;
		} else {
			print("ROOM IMAGE IS NULL");
		}
		Data.Instance.LoadLevel ("ArtPlaced");
    }
    public string GetUrlPath(string fileName)
    {
        string url = Data.Instance.GetFullPathByFolder("Rooms", fileName);
        print("CARGA ROOM DE: " + url);
        return url;
    }


}
