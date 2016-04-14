using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ConfirmArtworkSize : MonoBehaviour {

    public RawImage rawImage;
    public int num1;
    public int num2;

    public GameObject container;

    public gridsnap conten1;
    public gridsnap conten2;
    public gridsnap conten3;
    public gridsnap conten4;

    public ArtworkSignal signal;
    public Slider slider;

    void Start()
    {
        Events.HelpHide();
        Events.Back += Back;
        Events.ToggleUnit += ToggleUnits;

        Data.Instance.SetTitle("My Artwork");

        if (Data.Instance.artData.selectedGallery == -2 && Data.Instance.artData.selectedArtWork.url != "")
        {
			signal.Init (Data.Instance.artData.selectedArtWork.size.x, Data.Instance.artData.selectedArtWork.size.y);
			signal.name.text = Data.Instance.artData.selectedArtWork.title;
			signal.author.text = Data.Instance.artData.selectedArtWork.autor;
		} else {
			signal.Init (50, 50);
		}
        slider.value = Data.Instance.unitSlider.value;

        float maxWidth = rawImage.rectTransform.sizeDelta.x;
        float maxHeight = rawImage.rectTransform.sizeDelta.y;
        float aspect = maxWidth / maxHeight;
        float textAspect = 1f * Data.Instance.lastArtTexture.width / Data.Instance.lastArtTexture.height;
        if (aspect > textAspect)
        {
            rawImage.rectTransform.sizeDelta = new Vector2(maxHeight * textAspect, maxHeight);
        }
        else if (aspect < textAspect)
        {
            rawImage.rectTransform.sizeDelta = new Vector2(maxWidth, maxWidth / textAspect);
        }

        rawImage.texture = Data.Instance.lastArtTexture;
    }
    void ToggleUnits()
    {
        if (slider.value == 1)
        {
            slider.value = 0;
        }
        else if (slider.value == 0)
        {
            slider.value = 1;
        }
    }
    public void ToggleUnit()
    {
        Events.ToggleUnit();
    }
    void Update()
    {
        num1 = int.Parse(conten1.active.ToString() + conten2.active.ToString());
        num2 = int.Parse(conten3.active.ToString() + conten4.active.ToString());
    }
    void OnDestroy()
    {
        Events.Back -= Back;
        Events.ToggleUnit -= ToggleUnits;
    }

	public void Back()
	{
        Data.Instance.LoadLevel("Artworks");
    }
    public void Ready()
    {
        string num = conten1.active.ToString() + conten2.active.ToString() + conten3.active.ToString() + conten4.active.ToString();
        int _height = int.Parse(num);

		float aspect = 1f*Data.Instance.lastArtTexture.width / Data.Instance.lastArtTexture.height;
        int w = (int)(_height * aspect);
		Data.Instance.SavePhotoArt (signal.GetName (), signal.GetAuthor(), w, signal.GetHeight ());		      
       	Data.Instance.LoadLevel("Artworks");
       
    }

}
