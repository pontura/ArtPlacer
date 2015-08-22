using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ThumbImage : MonoBehaviour{

    private Sprite sprite;
    private Texture2D texture2d;
	Footer footer;
	int id;

    public void Init(Footer _footer, string url, int _id)
    {
        StartCoroutine(RealLoadRoomImage(url));
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
        StartCoroutine(RealLoadRoomImage(url));
        GetComponent<Button>().onClick.AddListener(() =>
        {
            OnSelectedRoom(rooms, id);
        });
    }

    public void Init(ArtWorks artWorks, string url, int id)
    {
        StartCoroutine(RealLoadImage(url));
        GetComponent<Button>().onClick.AddListener(() =>
        {
            OnSelected(artWorks, id);
        });
    }
    private IEnumerator RealLoadRoomImage(string url)
    {
        WWW imageURLWWW = new WWW(url);

        print("url: " + url);
        yield return imageURLWWW;

        texture2d = imageURLWWW.texture;

        if (imageURLWWW.texture != null)
        {
            sprite = new Sprite();
            sprite = Sprite.Create(ScaleTexture(imageURLWWW.texture, 200, 120), new Rect(0, 0, 200, 120), Vector2.zero);
            GetComponent<UnityEngine.UI.Image>().sprite = sprite;
        }
        yield return null;
    }
    private IEnumerator RealLoadImage(string url)
    {
        WWW imageURLWWW = new WWW(url);        
        yield return imageURLWWW;

		print("url: " + url);
        if (imageURLWWW.texture != null)
        {
            sprite = new Sprite();
            sprite = Sprite.Create(ScaleTexture(imageURLWWW.texture, 200, 120), new Rect(0, 0, 200, 120), Vector2.zero);
            GetComponent<UnityEngine.UI.Image>().sprite = sprite;
        }
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
			Data.Instance.lastArtTexture = sprite.texture;
			Events.OnSelectFooterArtwork();
		}
        footer.OnSelect(id);
    }
    public void OnSelected(ArtWorks artWorks, int id)
    {
        if(sprite)
             Data.Instance.lastArtTexture = sprite.texture;
        artWorks.OnSelect(id);
    }
    public void OnSelectedRoom(Rooms rooms, int id)
    {
        if (sprite)
            Data.Instance.lastPhotoTexture = texture2d;
        rooms.OnSelect(id);
        texture2d = null;
    }
}
