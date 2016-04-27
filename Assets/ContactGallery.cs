using UnityEngine;
using System.Collections;

public class ContactGallery : MonoBehaviour {

    void Start()
    {
        if (Data.Instance.artData.selectedGallery == -2)
            gameObject.SetActive(false);
    }
    public void Contact()
    {
        ArtData.GalleryData data;
        if (Data.Instance.artData.selectedGallery == -3 || Data.Instance.artData.selectedGallery == -1)
            data = Data.Instance.artData.GetGallery(Data.Instance.artData.selectedArtWork.galleryId);
        else
            data = Data.Instance.artData.GetGallery(Data.Instance.artData.selectedGallery);

        Events.ContactGalleryOpenPopup(data);
    }
}
