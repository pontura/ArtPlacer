using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class GalleryButton : MonoBehaviour {

    public Text title;
    public Image rawImage;
    public ArtData.GalleryData.ArtData artData;

    public void Init(Galleries galleries, int id, string _title, string _url)
    {
		//Debug.Log ("B_ID: " + id);
        title.text = _title;
        GetComponent<Button>().onClick.AddListener(() =>
        {
            OnSelected(galleries, id);
        });

        //ArtData.GalleryData.ArtData artData = null;

        if (id == -1 && Data.Instance.artData.favorites.Count>0)
            artData = Data.Instance.artData.GetArtData(Data.Instance.artData.favorites[0].galleryId, Data.Instance.artData.favorites[0].artId);
		else if(id == -2 && Data.Instance.artData.myArtWorks.artWorksData.Count>0)
			artData = Data.Instance.artData.myArtWorks.artWorksData[0];
<<<<<<< Updated upstream
        else  if (id > -1)
			artData = Data.Instance.artData.GetArtDataList(id).Count>0?Data.Instance.artData.GetArtDataList(id)[0]:null;
=======
        else  //if (id > -1)
            artData = Data.Instance.artData.GetArtData(id, 0);
>>>>>>> Stashed changes


        if (_url != "")
            StartCoroutine(LoadThumb(_url));

        //if (artData != null)
        //{
        //    string url = artData.GetUrl();
        //    if(artData.isLocal)
        //        RealLoadLocalImage(url);
        //    else{
        //        if (url.Length > 6){
        //            StartCoroutine(LoadThumb(url));
        //        }
        //    }
        //}
    }
    public void OnSelected(Galleries galleries, int id)
    {
        galleries.OnSelect(id);
    }


    private IEnumerator LoadThumb(string url)
    {
       // print(" LoadThumb " + url);

		Texture2D texture2d = null;
		yield return StartCoroutine(TextureUtils.LoadRemote(url, value => texture2d = value));

		if (texture2d != null)
        {
            Sprite sprite = new Sprite();
			sprite = Sprite.Create(ScaleTexture(texture2d, 100, 100), new Rect(0, 0, 100, 100), Vector2.zero);
            rawImage.sprite = sprite;            
        }
        yield return null;
    }

	private void RealLoadLocalImage(string url)
	{
		Texture2D texture2d = TextureUtils.LoadLocal (url);
		if(texture2d!=null){
			Sprite sprite = new Sprite();
			sprite = Sprite.Create(ScaleTexture(texture2d, 100, 100), new Rect(0, 0, 100, 100), Vector2.zero);			
			rawImage.sprite = sprite; 
		}
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
