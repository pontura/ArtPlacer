using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ThumbImage : MonoBehaviour {

    private Sprite sprite;

    public void InitRoom(string url, int id)
    {
        StartCoroutine(RealLoadImage(url));
    }

    public void Init(ArtWorks artWorks, string url, int id)
    {
        StartCoroutine(RealLoadImage(url));
        GetComponent<Button>().onClick.AddListener(() =>
        {
            OnSelected(artWorks, id);
        });
    }

    private IEnumerator RealLoadImage(string url)
    {
        WWW imageURLWWW = new WWW(url);

        print("url: " + url);
        yield return imageURLWWW;
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
    public void OnSelected(ArtWorks artWorks, int id)
    {
        print(artWorks + " _ " + id);
        if(sprite)
             Data.Instance.lastArtTexture = sprite.texture;
        artWorks.OnSelect(id);
    }
}
