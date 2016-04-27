using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ThumbImage : MonoBehaviour{
	
    public GameObject loadingAsset;
	private Sprite sprite;
	private Texture2D texture2d;
    public Image RawImage;
	Footer footer;
	int id;
	string url;
	
	public void Init(Footer _footer, string url, int _id, bool local)
	{
        if (!local && loadingAsset) loadingAsset.SetActive(true);
        else if (loadingAsset) loadingAsset.SetActive(false);

		if (local) 
			RealLoadLocalImage (url);
		else
			StartCoroutine (RealLoadImage (url));
		
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
        RawImage.color = Color.red;
		Events.OnLoading (true);
		Invoke ("CallOnSelect", 0.25f);
	}

	public void OnPointerUp()
	{
        RawImage.color = Color.white;
		Invoke ("stopLoading", 0.5f);
	}

	void stopLoading(){
		Events.OnLoading(false);
	}

	void CallOnSelect(){
		OnSelected(footer, id);
	}
	
	public void InitRoom(Rooms rooms, string url, int id)
	{
		//StartCoroutine(RealLoadRoomImage(url));
		RealLoadLocalImage(url);
		GetComponent<Button>().onClick.AddListener(() =>
		{
            Data.Instance.SetRoomFromLocalFiles(false);
			if (texture2d == null) return;
			Data.Instance.lastPhotoThumbTexture = texture2d;
			OnSelectedRoom(rooms, id);

		});
	}
    private bool local;
	public void Init(ArtWorks artWorks, string url_, int id, bool local)
	{
		url = url_;
		if (local) {
            if (loadingAsset) loadingAsset.SetActive(false);
			RealLoadLocalImage (url);
			GetComponent<Button>().onClick.AddListener(() =>			                                           {
				Events.OnLoading(true);
				OnSelectedLocal(artWorks, id);
			});
		} else {
			StartCoroutine (RealLoadImage (url));
			GetComponent<Button>().onClick.AddListener(() =>
			                                           {
				Events.OnLoading(true);
				StartCoroutine(OnSelected(artWorks, id));
			});
		}
		
	}
    private int _w = 400;
    private int _h = 400;

	private void RealLoadLocalImage(string url)
	{
		texture2d = TextureUtils.LoadLocal (url);
		if(texture2d!=null){
			sprite = new Sprite();
            sprite = Sprite.Create(TextureUtils.ScaleTexture(texture2d, _w, _h), new Rect(0, 0, _w, _h), Vector2.zero);

            RawImage.sprite = sprite;
		}
	}
	
	private IEnumerator RealLoadImage(string url)
	{
		yield return StartCoroutine(TextureUtils.LoadRemote(url, value => texture2d = value));
		//print("url: " + url);
        if (texture2d != null)
        {
            sprite = new Sprite();
            sprite = Sprite.Create(TextureUtils.ScaleTexture(texture2d, _w, _h), new Rect(0, 0, _w, _h), Vector2.zero);
            RawImage.sprite = sprite;
            LoadReady();
        }
        else
        {
            if (!local)
            {
                StartCoroutine(RealLoadImage(url));
            }
        }
		yield return null;
	}
    void LoadReady()
    {
        if (loadingAsset) loadingAsset.SetActive(false);
    }
    //private IEnumerator RealGetTexture(string url)
    //{
    //    yield return StartCoroutine(TextureUtils.LoadRemote(url, value => texture2d = value));
    //    Data.Instance.lastArtTexture = texture2d;
		
    //    yield return null;
    //}
	
	
	public void OnSelected(Footer footer, int id)
	{
		if (sprite) {
			Data.Instance.SetLastArtTexture(texture2d);
			//Data.Instance.lastArtTexture = sprite.texture;
		}
		footer.OnSelect(id);
	}
	
	public void OnSelectedLocal(ArtWorks artWorks, int id)
	{
		if (sprite) {
			Data.Instance.SetLastArtTexture(texture2d);
			//Data.Instance.lastArtTexture = sprite.texture;
		}
		artWorks.OnSelect(id);
	}
	
	public IEnumerator OnSelected(ArtWorks artWorks, int id)
	{
		yield return StartCoroutine(TextureUtils.LoadRemote(url, value => texture2d = value));
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
