using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ConfirmArtworkSize : MonoBehaviour
{
    public int result;

    public int num1_cm;
    public int num2_cm;

    public int num1_inches;
    public int num2_inches;

    public GameObject cms;
    public GameObject inches;

    public ScrollSnap conten1;
    public ScrollSnap conten2;

    public ScrollSnap conten1b;
    public ScrollSnap conten2b;

    public Slider slider;

    public RawImage rawImage;

    public ArtworkSignal signal;

    public int areaActiveID;

    void Start()
    {
        Events.Back += Back;
        Data.Instance.SetTitle("My Artwork");

        if (Data.Instance.artData.selectedGallery == -2 && Data.Instance.artData.selectedArtWork.url != "")
        {
            result = (int)Data.Instance.artData.selectedArtWork.size.y;
            signal.name.text = Data.Instance.artData.selectedArtWork.title;
            signal.author.text = Data.Instance.artData.selectedArtWork.autor;
        }
        else
        {
            result = 150;
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

        
        areaActiveID = 0;

        slider.value = Data.Instance.unitSlider.value;

        Events.ToggleUnit += ToggleUnits;
        // float _height = Data.Instance.areaData.areas[areaActiveID].height;
        if (result > 0)
        {
            CalculateNewCM();
            CalculateNewInches();
        }

        conten1.Init(num1_cm);
        conten2.Init(num2_cm);

        conten1b.Init(num1_inches);
        conten2b.Init(num2_inches);

        ChangeUnits();
        RefreshUnits();
    }
    void CalculateNewCM()
    {
        Vector4 cms = CustomMath.GetFormatedCentimeters(result);
        num1_cm = (int)cms.x;
        num2_cm = (int)cms.y;
    }
    void CalculateNewInches()
    {
        Vector3 inches = CustomMath.GetFormatedInches(result);
        num1_inches = (int)inches.x;
        num2_inches = (int)inches.y;
    }
    void ChangeUnits()
    {
        if (Data.Instance.unidad == Data.UnitSys.CM)
        {
            CalculateNewCM();
            cms.SetActive(true);
            inches.SetActive(false);
        }
        else
        {
            CalculateNewInches();
            cms.SetActive(false);
            inches.SetActive(true);
        }
    }
    void RefreshUnits()
    {
        conten1.ChangeValue(num1_cm);
        conten2.ChangeValue(num2_cm);

        conten1b.ChangeValue(num1_inches);
        conten2b.ChangeValue(num2_inches);
    }
    void OnDestroy()
    {
        Events.ToggleUnit -= ToggleUnits;
        Events.Back -= Back;
    }
    void Update()
    {
        int new_num1_cm = int.Parse(conten1.GetActive().ToString());
        int new_num2_cm = int.Parse(conten2.GetActive().ToString());

        int new_num1_inches = int.Parse(conten1b.GetActive().ToString());
        int new_num2_inches = int.Parse(conten2b.GetActive().ToString());

        if (new_num1_cm != num1_cm)
            ChangeCM(0, new_num1_cm);
        if (new_num2_cm != num2_cm)
            ChangeCM(1, new_num2_cm);
        if (new_num1_inches != num1_inches)
            ChangeInches(0, new_num1_inches);
        if (new_num2_inches != num2_inches)
            ChangeInches(1, new_num2_inches);
    }
    void ChangeCM(int var, int value)
    {
        print("ChangeCM var: " + var + " value: " + value);
        switch (var)
        {
            case 0: num1_cm = value; break;
            case 1: num2_cm = value; break;
        }
        string cms = num2_cm.ToString();
        if (num2_cm < 10) cms = "0" + num2_cm.ToString();
        string num = conten1.active.ToString() + cms;
        result = int.Parse(num);
        // CalculateNewInches();
    }
    void ChangeInches(int var, int value)
    {
        print("ChangeInches var: " + var + " value: " + value);
        switch (var)
        {
            case 0: num1_inches = value; break;
            case 1: num2_inches = value; break;
        }
        float totalInches = CustomMath.GetTotalInches(num1_inches, num2_inches);
        result = (int)CustomMath.inches2cm(totalInches);
        // CalculateNewCM();
    }
    void AddConfirmSizes(AreaData.Area area)
    {
        // Invoke("SelectArea", 0.1f);
    }
    public void ToggleUnits()
    {
        if (slider.value == 1)
        {
            slider.value = 0;
        }
        else if (slider.value == 0)
        {
            slider.value = 1;
        }
        ChangeUnits();
        RefreshUnits();
    }
    public void ToggleUnit()
    {
        Events.ToggleUnit();
    }

    void SelectArea()
    {
        // GetComponent<WallCreator>().SelectArea(areaActiveID);
    }

    public void Back()
    {
        Data.Instance.LoadLevel("ConfirmArtwork");
    }
    public void Ready()
    {
        int _height = (result);

        float aspect = 1f * Data.Instance.lastArtTexture.width / Data.Instance.lastArtTexture.height;
        int w = (int)(_height * aspect);
        Data.Instance.SavePhotoArt(signal.GetName(), signal.GetAuthor(), w, result);
		Data.Instance.lastArtTexture = null;
		Data.Instance.isPhoto4Room = true;
        Data.Instance.LoadLevel("artworks");
    }

}


//public class ConfirmArtworkSize : MonoBehaviour {

//    public RawImage rawImage;
//    public int num1;
//    public int num2;

//    public GameObject container;

//    public gridsnap conten1;
//    public gridsnap conten2;
//    public gridsnap conten3;
//    public gridsnap conten4;

//    public ArtworkSignal signal;
//    public Slider slider;

//    void Start()
//    {
//        Events.HelpHide();
//        Events.Back += Back;
//        Events.ToggleUnit += ToggleUnits;

//        Data.Instance.SetTitle("My Artwork");

//        if (Data.Instance.artData.selectedGallery == -2 && Data.Instance.artData.selectedArtWork.url != "")
//        {
//            signal.Init (Data.Instance.artData.selectedArtWork.size.x, Data.Instance.artData.selectedArtWork.size.y);
//            signal.name.text = Data.Instance.artData.selectedArtWork.title;
//            signal.author.text = Data.Instance.artData.selectedArtWork.autor;
//        } else {
//            signal.Init (50, 50);
//        }
//        slider.value = Data.Instance.unitSlider.value;

//        float maxWidth = rawImage.rectTransform.sizeDelta.x;
//        float maxHeight = rawImage.rectTransform.sizeDelta.y;
//        float aspect = maxWidth / maxHeight;
//        float textAspect = 1f * Data.Instance.lastArtTexture.width / Data.Instance.lastArtTexture.height;
//        if (aspect > textAspect)
//        {
//            rawImage.rectTransform.sizeDelta = new Vector2(maxHeight * textAspect, maxHeight);
//        }
//        else if (aspect < textAspect)
//        {
//            rawImage.rectTransform.sizeDelta = new Vector2(maxWidth, maxWidth / textAspect);
//        }

//        rawImage.texture = Data.Instance.lastArtTexture;
//    }
//    void ToggleUnits()
//    {
//        if (slider.value == 1)
//        {
//            slider.value = 0;
//        }
//        else if (slider.value == 0)
//        {
//            slider.value = 1;
//        }
//    }
//    public void ToggleUnit()
//    {
//        Events.ToggleUnit();
//    }
//    void Update()
//    {
//        num1 = int.Parse(conten1.active.ToString() + conten2.active.ToString());
//        num2 = int.Parse(conten3.active.ToString() + conten4.active.ToString());
//    }
//    void OnDestroy()
//    {
//        Events.Back -= Back;
//        Events.ToggleUnit -= ToggleUnits;
//    }

//    public void Back()
//    {
//        Data.Instance.LoadLevel("Artworks");
//    }
//    public void Ready()
//    {
//        string num = conten1.active.ToString() + conten2.active.ToString() + conten3.active.ToString() + conten4.active.ToString();
//        int _height = int.Parse(num);

//        float aspect = 1f*Data.Instance.lastArtTexture.width / Data.Instance.lastArtTexture.height;
//        int w = (int)(_height * aspect);
//        Data.Instance.SavePhotoArt (signal.GetName (), signal.GetAuthor(), w, signal.GetHeight ());		      
//        Data.Instance.LoadLevel("Artworks");
       
//    }

//}
