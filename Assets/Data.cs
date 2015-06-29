using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Data : MonoBehaviour
{

    const string PREFAB_PATH = "Data";
    private Fade fade;
    static Data mInstance = null;
    public Texture2D lastPhotoTexture;

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
        fade = GetComponentInChildren<Fade>();

        fade.gameObject.SetActive(true);
        if (!mInstance)
            mInstance = this;
        //otherwise, if we do, kill this thing
        else
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);

    }
    public void Reset()
    {

    }
}
