using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class GalleryButton : MonoBehaviour {

    public Text title;
    public Image rawImage;

    public void Init(Galleries galleries, int id, string _title, string _url)
    {
        title.text = _title;
        GetComponent<Button>().onClick.AddListener(() =>
        {
            OnSelected(galleries, id);
        });

        ArtData.GalleryData.ArtData artData = null;

        if (id == -1 && Data.Instance.artData.favorites.Count>0)
            artData = Data.Instance.artData.GetArtData(Data.Instance.artData.favorites[0].galleryId, Data.Instance.artData.favorites[0].artId);
		else if(id == -2 && Data.Instance.artData.myArtWorks.artWorksData.Count>0)
			artData = Data.Instance.artData.myArtWorks.artWorksData[0];
        else  if (id > -1)
            artData = Data.Instance.artData.GetArtData(id, 0);

        if (artData != null)
        {
            string url = artData.url;

			if(id == -2){
				string folder = Data.Instance.GetArtPath();		
				url = Path.Combine(folder, url + ".png");
			}
            if (url.Length > 6)
            {
                StartCoroutine(LoadThumb(url));
            }
        }
    }
    public void OnSelected(Galleries galleries, int id)
    {
        galleries.OnSelect(id);
    }


    private IEnumerator LoadThumb(string url)
    {
        WWW imageURLWWW = new WWW(url);

        yield return imageURLWWW;

        Texture2D texture2d = imageURLWWW.texture;

        if (imageURLWWW.texture != null)
        {
            Sprite sprite = new Sprite();
            sprite = Sprite.Create(ScaleTexture(imageURLWWW.texture, 100, 100), new Rect(0, 0, 100, 100), Vector2.zero);
            rawImage.sprite = sprite;            
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
}
