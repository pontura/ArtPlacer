using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class Data : MonoBehaviour
{
    public Color selectedColor;
    public Texture2D lastArtTexture;
    public Texture2D lastPhotoTexture;
    public ArtData artData;
	public AreaData areaData;
    public MainMenu mainMenu;
    public RoomsData roomsData;

    const string PREFAB_PATH = "Data";
    private Fade fade;
    static Data mInstance = null;
    

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
    string lastScene = "";
    public void LoadLevel(string aLevelName)
    {
        lastScene = Application.loadedLevelName;
        LoadLevel(aLevelName, 0.01f, 0.01f, Color.black);
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
        string imagesFolderPath = Path.Combine(Application.persistentDataPath, "Rooms");

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
    public void SavePhotoTaken()
    {
        byte[] bytes = lastPhotoTexture.EncodeToPNG();

        string path = System.DateTime.Now.ToString("yyyyMMddHHmmss");

        areaData.url = path;
        areaData.Save();

        string folder = Path.Combine(Application.persistentDataPath, "Rooms");

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        var filePath = Path.Combine(folder, path);
        File.WriteAllBytes(filePath + ".png", bytes);
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
    
}
