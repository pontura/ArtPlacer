using UnityEngine;
using System.Collections;

public class RoomImageCreator : MonoBehaviour {

    public Camera cameraToScreen;
    private bool takeShot = false;

    public int resWidth = 800;
    public int resHeight = 600;
    private string path;

    void Start()
    {
        Events.OnGenerateRoomThumb += OnGenerateRoomThumb;
    }
    void OnDestroy()
    {
        Events.OnGenerateRoomThumb -= OnGenerateRoomThumb;
    }
    void OnGenerateRoomThumb(string path)
    {
        Events.ArtworkPreview(false);
        this.path = path;
        takeShot = true;
    }
    public void TakeHiResShot()
    {
        takeShot = true;
    }
    void LateUpdate()
    {
        if (takeShot)
        {
            RenderTexture rt = new RenderTexture(resWidth, resHeight, 32);
            cameraToScreen.targetTexture = rt;
            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);


            cameraToScreen.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);


            Texture2D image = new Texture2D(resWidth, resHeight);
            image.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            image.Apply();
            RenderTexture.active = rt;
            Data.Instance.lastPhotoThumbTexture = image;


            cameraToScreen.targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            Destroy(rt);
           
            byte[] bytes = screenShot.EncodeToPNG();

            string filename = Data.Instance.GetFullPathByFolder("Rooms", path + "_thumb.png");

          //  string filename = ScreenShotName();
           // string filename = path;
            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("Took screenshot to: {0}", filename));
            takeShot = false;
        }
    }

}
