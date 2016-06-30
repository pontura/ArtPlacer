using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using NatCamU;

public class Unitygram : UnitygramBase {
    
    [Header("Barcodes")]
    public BarcodeFormat detectionFormat = BarcodeFormat.ALL;
    public bool continuousDetection = true;
    
    [Header("UI")]
    public Button switchCamButton;
    public Button flashButton;
    //public Button barcodeButton;
    public Image checkIco;
    public Image flashIco;
    public Text flashText;
    //public Text barcodeText;

	public Slider zSlider;

	private bool photoTaken;
	private bool ready;
    
	// Use this for initialization
	public override void Start () {
        //Start base
        //base.Start();
		//Initialize NatCam
		NatCam.Initialize(Interface, PreviewType, DetectBarcodes, VerboseMode);
		//NatCam.Initialize();
		//Set the active camera
		NatCam.ActiveCamera = Facing == Facing.Front ? DeviceCamera.FrontCamera : DeviceCamera.RearCamera;
		//Null checking
		if (NatCam.ActiveCamera == null) {
			ScreenDebugger.Log("Cam Null");
			return;
		}
		//Set the camera's resolution
		NatCam.ActiveCamera.SetResolution(Resolution);
		//Set the camera's framerate
		NatCam.ActiveCamera.SetFramerate(Framerate);
		//Set the camera's focus mode
		NatCam.ActiveCamera.FocusMode = FocusMode;

		//ScreenDebugger.Log("Flash: "+NatCam.ActiveCamera.FlashMode);
		//Debug.Log("Flash: "+NatCam.ActiveCamera.FlashMode);

		flashButton.gameObject.SetActive (NatCam.ActiveCamera.IsFlashSupported);
		NatCam.ActiveCamera.FlashMode = NatCam.ActiveCamera.IsFlashSupported ? FlashMode.Auto : FlashMode.Off;
		//Set the flash icon
		SetFlashIcon();

		zSlider.gameObject.SetActive(NatCam.ActiveCamera.IsZoomSupported);

		//Play
		NatCam.Play(NatCam.ActiveCamera);
		string previewS = NatCam.Preview == null ? "null" : "" + NatCam.Preview.width;
		//ScreenDebugger.Log("NatCam Play: "+NatCam.IsPlaying+" tex: "+previewS);
		//Define a delegate to be executed once the preview starts //Note that this is a MUST when assigning the preview texture to anything
		NatCam.ExecuteOnPreviewStart(() => {
			//Set the RawImage texture once the preview starts
			if (RawImage != null) RawImage.texture = NatCam.Preview;
			//ScreenDebugger.Log("Preview Start");
			Events.OnZoom += OnZoom;
		});

        //Create a barcode detection request
		//BarcodeRequest request = new BarcodeRequest(OnDetectBarcodes, BarcodeFormat.EAN_13 | BarcodeFormat.QR, !continuousDetection); //DEBUG //detectionFormat //Negate continuousDetection because when true, we don't want to automatically unsubscribe our callback
		//Request detection
		//NatCam.RequestBarcode(request);        

		if (Data.Instance.isPhoto4Room) 
			Data.Instance.lastPhotoTexture = null;
		else 
			Data.Instance.lastArtTexture = null;

		Vector3 scale = RawImage.transform.localScale;

		#if UNITY_IOS
		//scale.x *= -1;
		//rawImage.transform.localEulerAngles = new Vector3(0, 0, 180);
		#endif

		/*Vector2 winRes = new Vector2(Screen.currentResolution.width,Screen.currentResolution.height);
		float winAspect = 1f*winRes.x/winRes.y;
		float photoAspec = 1280f / 720f;
		float scaleFactor = 1f;
		if (photoAspec > winAspect) {
			scaleFactor = 720f / winRes.y;
		} else if (photoAspec < winAspect) {
			scaleFactor = 1280f / winRes.x;
		}*/

		RawImage.transform.localScale = scale*1.0f;		

		Events.OnLoading (false);
		//Invoke ("SetZoomDelegate", 10.0f);
	}

	void SetZoomDelegate(){
		Events.OnZoom += OnZoom;
	}

	void Update()
	{
		if (ready) return;
		if (photoTaken)
		{
			if (Data.Instance.isPhoto4Room && Data.Instance.lastPhotoTexture != null)
				Ready();
			else if (!Data.Instance.isPhoto4Room && Data.Instance.lastArtTexture != null)
				Ready();
		}
	}
	void Ready()
	{
		ready = true;
		Data.Instance.LoadLevel("ConfirmPhoto");
	}

	void OnDestroy()
	{		
		Events.OnZoom -= OnZoom;
		NatCam.Release();
	}

	
	#region --NatCam and UI Callbacks--
    
    public void CapturePhoto () {
		Events.OnLoading (true);
		photoTaken = true;
		//takePhotoButton.gameObject.SetActive(false);

		if (Data.Instance.isPhoto4Room)
		{
			
			NatCam.CapturePhoto(
			(Texture2D photo) => {
			//Data.Instance.lastPhotoTexture = photo;
			//We don't need to manually unregister this delegate

			/*float maxWidth = Data.Instance.defaultCamSize.x;
			float maxHeight = Data.Instance.defaultCamSize.y;
			float aspect = maxWidth / maxHeight;
			float textAspect = 1f*photo.width / photo.height;
			if (aspect > textAspect) {						
			Data.Instance.lastPhotoTexture = TextureUtils.ResizeTexture(photo,TextureUtils.ImageFilterMode.Nearest,maxHeight/photo.height);
			} else if (aspect < textAspect) {						
			Data.Instance.lastPhotoTexture = TextureUtils.ResizeTexture(photo,TextureUtils.ImageFilterMode.Nearest,maxWidth/photo.width);
			} else {
			Data.Instance.lastPhotoTexture = TextureUtils.ResizeTexture(photo,TextureUtils.ImageFilterMode.Nearest,maxWidth/photo.width);
			//texture.Resize(texture.width,texture.height);
			//texture.Apply();
			}*/

			//RawImage.GetComponent<NatCamPreviewScaler>().Apply(photo);			
			Data.Instance.lastPhotoTexture = Data.Instance.Resize2Fit(photo);
			DestroyImmediate(photo);
			});

		}
		else
		{
			
			NatCam.CapturePhoto(
			(Texture2D photo) => {
			//Set a material's main texture to be the captured photo
			//Data.Instance.lastArtTexture = photo;
			//We don't need to manually unregister this delegate

			Data.Instance.lastArtTexture = Data.Instance.Resize2Fit(photo);
			DestroyImmediate(photo);
			});
			
		}

		//  Data.Instance.LoadLevel("ConfirmPhoto");
    }
    
    void OnCapturedPhoto (Texture2D photo) {
        //Set the rawImage
        RawImage.texture = photo;
        //Scale it according to the photo
        RawImage.GetComponent<NatCamPreviewScaler>().Apply(photo);
        //Enable the check icon
        checkIco.gameObject.SetActive(true);
        //Disable the switch camera button
        switchCamButton.gameObject.SetActive(false);
        //Disable the flash button
        flashButton.gameObject.SetActive(false);
    }
    
    void OnDetectBarcodes (List<Barcode> codes) {
		//Get the first code detected
		Barcode code = codes[0];
        //Check if it is a hyperlink
        bool hyperlink = code.data.StartsWith("http") || code.data.StartsWith("www");
        //Set the button's interactable state
        //barcodeButton.interactable = hyperlink;
        //Add a callback to open the link if it is a hyperlink
        //barcodeButton.onClick.AddListener(() => Application.OpenURL(code.data));
        //Set the barcode's text
        //barcodeText.text = string.Format(hyperlink ? "<i>{0}</i>" : "{0}", code.data);
        //Disable the barcode button after a while
        Invoke("DisableBarcodeButton", 4f);
	}

	public void OnZoom (float z) {		
		RawImage.GetComponent<NatCamPreviewZoomer>().zoomRatio = z;
		NatCam.ActiveCamera.ZoomRatio = z*z*z*z;
	}

    #endregion
    
    
    #region --UI Ops--
    
    public void SwitchCamera () {
        //Switch camera with null checking
        NatCam.ActiveCamera = NatCam.ActiveCamera != null ? NatCam.ActiveCamera ==  DeviceCamera.RearCamera ? DeviceCamera.FrontCamera == null ? NatCam.ActiveCamera : DeviceCamera.FrontCamera : DeviceCamera.RearCamera : NatCam.ActiveCamera;
        //Set the flash icon
        SetFlashIcon();
    }
    
    public void ToggleFlashMode () {
        //Set the active camera's flash mode
        NatCam.ActiveCamera.FlashMode = NatCam.ActiveCamera.IsFlashSupported ? NatCam.ActiveCamera.FlashMode == FlashMode.Auto ? FlashMode.On : NatCam.ActiveCamera.FlashMode == FlashMode.On ? FlashMode.Off : FlashMode.Auto : NatCam.ActiveCamera.FlashMode;
        //Set the flash icon
		//ScreenDebugger.Log("ACA - "+ NatCam.ActiveCamera.FlashMode);
        SetFlashIcon();
    }

    void OnCheckedPhoto () {
        //Disable the check icon
        //checkIco.gameObject.SetActive(false);
        //Set the rawImage to the camera preview
        RawImage.texture = NatCam.Preview;
        //Scale the rawImage
        RawImage.GetComponent<NatCamPreviewScaler>().Apply(NatCam.Preview);
        //Enable the switch camera button
        switchCamButton.gameObject.SetActive(true);
        //Enable the flash button
        flashButton.gameObject.SetActive(true);
    }
    
    void SetFlashIcon () {
        //Null checking
        if (NatCam.ActiveCamera == null) return;
        //Set the icon
        flashIco.color = !NatCam.ActiveCamera.IsFlashSupported || NatCam.ActiveCamera.FlashMode == FlashMode.Off ? (Color)new Color32(127, 127, 127, 255) : Color.white;
        //Set the auto text for flash
        flashText.text = NatCam.ActiveCamera.IsFlashSupported && NatCam.ActiveCamera.FlashMode == FlashMode.Auto ? "A" : "";
    }
    #endregion    

    
    #region --Misc--
    
    void DisableBarcodeButton () {
        //Set interactable false
        //barcodeButton.interactable = false;
        //Clear the button's callbacks
        //barcodeButton.onClick.RemoveAllListeners();
        //Empty the text
        //barcodeText.text = "";
    }
    #endregion
}
