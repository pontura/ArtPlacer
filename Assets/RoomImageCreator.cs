using UnityEngine;
using System.Collections;

public class RoomImageCreator : MonoBehaviour {

    public Camera cameraToScreen;
    private bool takeShot = false;

    public int resWidth = 1600;
    public int resHeight = 1200;

    private string path;

    void Start()
    {
        Events.OnGenerateRoomThumb += OnGenerateRoomThumb;
		resWidth = Screen.width;
		resHeight = Screen.height;
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
			Texture2D screenShot2 = null;


            cameraToScreen.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);

			int left, top, right, bottom;
			left=top=right=bottom=0;

			for (int y = 0; y < screenShot.height; y++) {
				for (int x = 0; x < screenShot.width; x++) {
					if(screenShot.GetPixel (x,y)!=Color.black){
						left = x;
						top = y;
						y = screenShot.height;
						x = screenShot.width;
						break;
					}
				}
			}

			for (int y = screenShot.height; y > 0; y--) {
				for (int x = screenShot.width; x > 0; x--) {
					if(screenShot.GetPixel (x,y)!=Color.black){
						right = x;
						bottom = y;
						y = 0;
						x = 0;
						break;
					}	
				}
			}

			if(left!=0||top!=0||right!=0||bottom!=0){
				screenShot2 = new Texture2D(right-left, bottom-top, TextureFormat.RGB24, false);

				Color32[] pixels = screenShot.GetPixels32();
				Color32[] pixelCopy = new Color32[(right-left)*(bottom-top)];
				for (int y = top; y < screenShot.height; y++) {
					for (int x = left; x < screenShot.width; x++) {
						if(x<right&&y<bottom){
							pixelCopy[(x-left) + ((y-top)*(right-left))] = pixels[x+y*screenShot.width];
							//screenShot2.SetPixel(x-left,y-top,screenShot.GetPixel(x,y));
						}
					}
				}
				screenShot2.SetPixels32(pixelCopy);
				screenShot2.Apply ();
			}

            Texture2D image = new Texture2D(resWidth, resHeight);
            image.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            image.Apply();
            RenderTexture.active = rt;
            Data.Instance.lastPhotoThumbTexture = image;


            cameraToScreen.targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            Destroy(rt);
           
			byte[] bytes = screenShot2!=null?screenShot2.EncodeToPNG():screenShot.EncodeToPNG();

            string filename = Data.Instance.GetFullPathByFolder("Rooms", path + "_thumb.png");

          //  string filename = ScreenShotName();
           // string filename = path;
            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("Took screenshot to: {0}", filename));
            takeShot = false;
        }
    }

}
