using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class Walls : MonoBehaviour {

    public GameObject ResetedContainer;
    public GameObject ReadyContainer;

    public RawImage rawImage;

	void Start () {
        rawImage.texture = Data.Instance.lastPhotoTexture;
        Reseted();
        Events.OnNumWallsChanged += OnNumWallsChanged;
	}
    void OnDestroy()
    {
        Events.OnNumWallsChanged -= OnNumWallsChanged;
    }
    void OnNumWallsChanged(int qty)
    {
        if (qty < 1)
            Reseted();
        else
            Started();
    }
    public void GotoLoadRoom()
    {
        Data.Instance.LoadLevel("LoadRoom");
    }
    public void ArtBrowser()
    {
        Data.Instance.LoadLevel("ArtBrowser");
    }
    public void Reseted()
    {
        ResetedContainer.gameObject.SetActive(true);
        ReadyContainer.gameObject.SetActive(false);
    }
    public void Started()
    {
        ResetedContainer.gameObject.SetActive(false);
        ReadyContainer.gameObject.SetActive(true);
    }
}
