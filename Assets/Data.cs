using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class Data : MonoBehaviour
{
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
    public RawImage SetTexture(RawImage rawImage, Texture2D texture2d)
    {
        rawImage.texture = texture2d;
#if UNITY_IOS
       rawImage.transform.localScale = new Vector3(-1, -1, 1);
#endif
        return rawImage;
    }
    public void LoadLevel(string aLevelName)
    {
        LoadLevel(aLevelName, 1, 1, Color.black);
    }
    public void LoadLevel(string aLevelName, float aFadeOutTime, float aFadeInTime, Color aColor)
    {
        Time.timeScale = 1;
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
        string imagesFolderPath = Path.Combine(Application.dataPath, "Rooms");

        if (!Directory.Exists(imagesFolderPath))
            Directory.CreateDirectory(imagesFolderPath);

        return imagesFolderPath;
    }
    public FileInfo[] GetFilesIn(string folderName)
    {
        string info = Path.Combine(Application.dataPath, folderName);
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

        string folder = Data.Instance.GetRoomsPath();
        var filePath = Path.Combine(folder, path);
        File.WriteAllBytes(filePath + ".png", bytes);
    }
	public void AddArea(int id, Vector3[] pointers, Vector3 position){
		areaData.AddAreas(id,pointers,position);
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
}
