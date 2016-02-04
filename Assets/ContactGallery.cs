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
        ArtData.GalleryData data = Data.Instance.artData.GetGallery(Data.Instance.artData.selectedArtWork.galleryId);
        Events.ContactGalleryOpenPopup(data);
    }
}
