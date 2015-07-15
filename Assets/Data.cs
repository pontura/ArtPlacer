using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class Data : MonoBehaviour
{
    public Texture2D lastArtTexture;
    public Texture2D lastPhotoTexture;
    public ArtData artData;

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
                }
            }
            return mInstance;
        }
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
        artData = GetComponent<ArtData>();

        fade.gameObject.SetActive(true);
        DontDestroyOnLoad(this.gameObject);



        


        
    }
    public void Reset()
    {

    }
    public string GetImagesPath()
    {
        string imagesFolderPath = Path.Combine(Application.persistentDataPath, "Images");

        if (!Directory.Exists(imagesFolderPath))
            Directory.CreateDirectory(imagesFolderPath);

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

        string path = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        string folder = Data.Instance.GetImagesPath();
        var filePath = Path.Combine(folder, path);
        File.WriteAllBytes(filePath, bytes);
    }
}
