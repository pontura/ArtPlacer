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
    public Button barcodeButton;
    public Image checkIco;
    public Image flashIco;
    public Text flashText;
    public Text barcodeText;

    
	// Use this for initialization
	public override void Start () {
        //Start base
        base.Start();
        //Create a barcode detection request
		BarcodeRequest request = new BarcodeRequest(OnDetectBarcodes, BarcodeFormat.EAN_13 | BarcodeFormat.QR, !continuousDetection); //DEBUG //detectionFormat //Negate continuousDetection because when true, we don't want to automatically unsubscribe our callback
		//Request detection
		NatCam.RequestBarcode(request);
        //Set the flash icon
        SetFlashIcon();
	}
	
	#region --NatCam and UI Callbacks--
    
    public void CapturePhoto () {
        //Divert control if we are checking the captured photo
        if (checkIco.gameObject.activeInHierarchy) {
            OnCheckedPhoto();
            //Terminate this control chain
            return;
        }
        //Capture photo with our callback
        NatCam.CapturePhoto(OnCapturedPhoto);
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
        barcodeButton.interactable = hyperlink;
        //Add a callback to open the link if it is a hyperlink
        barcodeButton.onClick.AddListener(() => Application.OpenURL(code.data));
        //Set the barcode's text
        barcodeText.text = string.Format(hyperlink ? "<i>{0}</i>" : "{0}", code.data);
        //Disable the barcode button after a while
        Invoke("DisableBarcodeButton", 4f);
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
        SetFlashIcon();
    }
    
    void OnCheckedPhoto () {
        //Disable the check icon
        checkIco.gameObject.SetActive(false);
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
        flashIco.color = !NatCam.ActiveCamera.IsFlashSupported || NatCam.ActiveCamera.FlashMode == FlashMode.Off ? (Color)new Color32(200, 200, 200, 255) : Color.white;
        //Set the auto text for flash
        flashText.text = NatCam.ActiveCamera.IsFlashSupported && NatCam.ActiveCamera.FlashMode == FlashMode.Auto ? "A" : "";
    }
    #endregion
    
    
    #region --Misc--
    
    void DisableBarcodeButton () {
        //Set interactable false
        barcodeButton.interactable = false;
        //Clear the button's callbacks
        barcodeButton.onClick.RemoveAllListeners();
        //Empty the text
        barcodeText.text = "";
    }
    #endregion
}
