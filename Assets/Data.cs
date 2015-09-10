using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class Data : MonoBehaviour
{
    public Color selectedColor;
    public Texture2D lastArtTexture;
	public Texture2D lastArtThumbTexture;
    public Texture2D lastPhotoTexture;
    public Texture2D lastPhotoThumbTexture;
    public ArtData artData;
	public AreaData areaData;
    public MainMenu mainMenu;
    public RoomsData roomsData;
    public string lastScene = "";

    const string PREFAB_PATH = "Data";
    private Fade fade;
    static Data mInstance = null;

	public bool isPhoto4Room = true;

	public bool isArtworkInfo2Place = true;
	public int thumbHeight = 100;

	string jsonUrl = "http://www.pontura.com/works/artplacer/artplacer.json";

    public static Data Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = FindObjectOfType<Data>();

                if (mInstance == null)
                {
                    GameObject go = Instantiate(Resources.Load<GameObject>(PREFAB_PATH)) as GameObject;
                    mInstance = go.GetComponent<Data>();
                    go.transform.localPosition = new Vector3(0, 0, 0);
                }
            }
            return mInstance;
        }
    }
//    public RawImage SetTexture(RawImage rawImage, Texture2D texture2d)
//    {
//        rawImage.texture = texture2d;
//#if UNITY_IOS
//       rawImage.transform.localScale = new Vector3(-1, -1, 1);
//#endif
//        return rawImage;
//    }

    public void Back()
    {
        if (lastScene != "")
        LoadLevel(lastScene);
    }
    
    public void LoadLevel(string aLevelName)
    {
        Events.OnLoading(true);
        lastScene = Application.loadedLevelName;
        LoadLevel(aLevelName, 0.005f, 0.005f, Color.black);
    }
    public void LoadLevel(string aLevelName, float aFadeOutTime, float aFadeInTime, Color aColor)
    {
        fade.LoadLevel(aLevelName, aFadeOutTime, aFadeInTime, aColor);
    }
    void Awake()
    {
        
        if (!mInstance)
            mInstance = this;
        //otherwise, if we do, kill this thing
        else
        {
            Destroy(this.gameObject);
            return;
        }

		StartCoroutine(GetServerData(jsonUrl));

        fade = GetComponentInChildren<Fade>();
        areaData = GetComponent<AreaData>();

        fade.gameObject.SetActive(true);

        roomsData = GetComponent<RoomsData>();
        artData = GetComponent<ArtData>();

		DontDestroyOnLoad(this.gameObject);
        
    }
    public void Reset()
    {

    }
    public string GetRoomsPath()
    {
       // string imagesFolderPath = Path.Combine(Application.persistentDataPath, "Rooms");

        string imagesFolderPath = string.Format("{0}/Resources/images/rooms/", Application.dataPath);

#if UNITY_ANDROID
        //imagesFolderPath = "file:///" + imagesFolderPath;
#endif
        return imagesFolderPath;
    }

	public string GetArtPath()
	{
		string imagesFolderPath = Path.Combine(Application.persistentDataPath, "Artworks");
		
		#if UNITY_ANDROID
		imagesFolderPath = "file:///" + imagesFolderPath;
		#endif
		return imagesFolderPath;
	}

    public FileInfo[] GetFilesIn(string folderName)
    {
        string info = Path.Combine(Application.persistentDataPath, folderName);
        var folder = new DirectoryInfo(info);
        FileInfo[] fileInfo = folder.GetFiles();
        return fileInfo;
    }
    public void SaveRoom()
    {
        Events.OnTooltipOn("SAVING ROOM");

        byte[] bytes = lastPhotoTexture.EncodeToPNG();

        string path = GetUniqueName();

		if (areaData.url.Equals (""))
			path = areaData.url = GetUniqueName ();
		else
			path = areaData.url;

		print ("Path: " + path + " " + areaData.url);
        areaData.Save();

        File.WriteAllBytes(GetFullPathByFolder("Rooms", path + ".png"), bytes);

        Events.OnGenerateRoomThumb(path);
		lastPhotoTexture = null;
    }

	public void DeletePhotoRoom(string path)
	{
		File.Delete (GetFullPathByFolder("Rooms", path + ".png"));
		File.Delete (GetFullPathByFolder("Rooms", path + "_thumb.png"));
	}

    public string GetFullPathByFolder(string FolderName, string fileName)
    {
         string folder = Path.Combine(Application.persistentDataPath, FolderName);
		if (!Directory.Exists(folder))
			Directory.CreateDirectory(folder);
         return Path.Combine(folder, fileName);
    }
    public string GetUniqueName()
    {
        return System.DateTime.Now.ToString("yyyyMMddHHmmss");
    }
	public void SavePhotoArt(string name, string author, float width, float height)
	{
		byte[] bytes = lastArtTexture.EncodeToPNG();

		string path = "";
		if (artData.selectedGallery == -2) {
			path = artData.selectedArtWork.url;
			artData.SaveArtWork(path, name, author, width, height, artData.selectedArtWork.artId);
		} else {
			path = GetUniqueName ();
			artData.SaveArtWork(path, name, author, width, height);
		}
		
		//string folder = Path.Combine(Application.persistentDataPath, "Artworks");
        string folder = Path.Combine(Application.persistentDataPath, "Artworks");
		
		if (!Directory.Exists(folder))
			Directory.CreateDirectory(folder);
		
		var filePath = Path.Combine(folder, path);
		File.WriteAllBytes(filePath + ".png", bytes);
	}
	public void DeletePhotoArt(string path)
	{
		//string folder = Path.Combine(Application.persistentDataPath, "Artworks");
		string folder = Path.Combine(Application.persistentDataPath, "Artworks");
		
		if (!Directory.Exists(folder))
			Directory.CreateDirectory(folder);
		
		var filePath = Path.Combine(folder, path);
		File.Delete (filePath + ".png");
	}

	public void AddArea(int id, Vector3[] pointers, Vector3 position, float width, float height){
        areaData.AddAreas(id, pointers, position, width, height);
	}

    private bool mainMenuOpened;
    public GameObject hamburguerButon;
    public void SetMainMenuActive(bool stateActivation)
    {
        hamburguerButon.SetActive(stateActivation);
    }
    public void ToggleMainMenu()
    {
        if (mainMenuOpened)
            mainMenu.Close();
        else
        {
            mainMenu.gameObject.SetActive(true);
            mainMenu.Open();
        }
        mainMenuOpened = !mainMenuOpened;              
    }
    public void ResetApp()
    {
        PlayerPrefs.DeleteAll();
        Application.LoadLevel("Intro");
        Destroy(gameObject);
    }

	public void SetLastArtTexture(Texture2D tex){
		lastArtTexture = tex;
		SetlastArtThumbTexture (thumbHeight);
	}

	private void SetlastArtThumbTexture(int height){
		lastArtThumbTexture = ScaleTexture (lastArtTexture, height);
	}
    
	private Texture2D ScaleTexture(Texture2D source, int targetHeight)
	{
		float aspect = 1f*source.width / source.height;
		
		Texture2D result = new Texture2D((int)(aspect * targetHeight), targetHeight, source.format, false);
		
		float incX = (1.0f / aspect * targetHeight);
		float incY = (1.0f / (float)targetHeight);
		
		for (int i = 0; i < result.height; ++i)
		{
			for (int j = 0; j < result.width; ++j)
			{
				Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
				result.SetPixel(j, i, newColor);
			}
		}
		
		result.Apply();
		return result;
	}

	public IEnumerator GetServerData(string url)
	{
		Debug.Log (url);
		WWW textURLWWW = new WWW(url);		
		yield return textURLWWW;	

		artData.LoadArtFromServer(textURLWWW.text);
		yield return null;
	}
}
