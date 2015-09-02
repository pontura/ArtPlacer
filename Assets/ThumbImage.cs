using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ThumbImage : MonoBehaviour{

    private Sprite sprite;
    private Texture2D texture2d;
	Footer footer;
	int id;
	string url;

    public void Init(Footer _footer, string url, int _id)
    {
        StartCoroutine(RealLoadImage(url));
		footer = _footer;
		id=_id;
        /*GetComponent<Button>().OnClick.AddListener(() =>
        {
			print("aca");
            OnSelected(footer, id);
        });*/
    }

	public void OnPointerDown()
	{
		OnSelected(footer, id);
	}

    public void InitRoom(Rooms rooms, string url, int id)
    {
        //StartCoroutine(RealLoadRoomImage(url));
        RealLoadRoomImage(url);
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (texture2d == null) return;
            Data.Instance.lastPhotoThumbTexture = texture2d;
            OnSelectedRoom(rooms, id);
        });
    }

    public void Init(ArtWorks artWorks, string url_, int id)
    {
		url = url_;
        StartCoroutine(RealLoadImage(url));
        GetComponent<Button>().onClick.AddListener(() =>
        {
			Events.OnLoading(true);
            StartCoroutine(OnSelected(artWorks, id));
        });
    }
    private void RealLoadRoomImage(string url)
    {
        var filePath = url;
        if (System.IO.File.Exists(filePath))
        {
            var bytes = System.IO.File.ReadAllBytes(filePath);
            texture2d = new Texture2D(1, 1);
            texture2d.LoadImage(bytes);
            sprite = new Sprite();
            sprite = Sprite.Create(ScaleTexture(texture2d, 200, 120), new Rect(0, 0, 200, 120), Vector2.zero);

            GetComponent<UnityEngine.UI.Image>().sprite = sprite;
        }
    }
    private IEnumerator RealLoadImage(string url)
    {
        WWW imageURLWWW = new WWW(url);        
        yield return imageURLWWW;

		texture2d = imageURLWWW.texture;

		print("url: " + url);
        if (imageURLWWW.texture != null)
        {
            sprite = new Sprite();
            sprite = Sprite.Create(ScaleTexture(imageURLWWW.texture, 200, 120), new Rect(0, 0, 200, 120), Vector2.zero);
            GetComponent<UnityEngine.UI.Image>().sprite = sprite;
        }
        yield return null;
    }

	private IEnumerator RealGetTexture(string url)
	{
		WWW imageURLWWW = new WWW(url);		
		yield return imageURLWWW;
		
		texture2d = imageURLWWW.texture;
		Data.Instance.lastArtTexture = texture2d;

		yield return null;
	}

    private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);

        float incX = (1.0f / (float)targetWidth);
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
    public void OnSelected(Footer footer, int id)
    {
        if (sprite) {
			Data.Instance.SetLastArtTexture(texture2d);
			//Data.Instance.lastArtTexture = sprite.texture;
		}
        footer.OnSelect(id);
    }

	public IEnumerator OnSelected(ArtWorks artWorks, int id)
    {
		WWW imageURLWWW = new WWW(url);		
		yield return imageURLWWW;
		
		texture2d = imageURLWWW.texture;
		Data.Instance.SetLastArtTexture(texture2d);

		artWorks.OnSelect(id);
		yield return null;
    }

    public void OnSelectedRoom(Rooms rooms, int id)
    {
      //  if (sprite)
       //     Data.Instance.lastPhotoTexture = texture2d;
        rooms.OnSelect(id);
        texture2d = null;
    }
}
