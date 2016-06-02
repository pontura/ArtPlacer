using UnityEngine;
using UnityEngine.UI;
using NatCamU.Internals;

namespace NatCamU {
	
	[NCDoc(140)]
	public class UnitygramBase : MonoBehaviour { //This class is easily subclassed and overriden
		
		[Header("Preview")]
		[NCDoc(149)] public RawImage RawImage;
		
		[Header("Parameters")]
		[NCDoc(141)] public NatCamInterface Interface = NatCamInterface.NativeInterface;
		[NCDoc(142)] public PreviewType PreviewType = PreviewType.NonReadable;
		[NCDoc(143)] public Switch DetectBarcodes = Switch.Off;
		[NCDoc(144)] public Facing Facing = Facing.Rear;
		[NCDoc(145)] public ResolutionPreset Resolution = ResolutionPreset.MediumResolution;
		[NCDoc(146)] public FrameratePreset Framerate = FrameratePreset.Default;
		[NCDoc(147)] public FocusMode FocusMode = FocusMode.AutoFocus;
		
		[Header("Debugging")]
		[NCDoc(148)] public Switch VerboseMode;
		
		// Use this for initialization
		[NCDoc(150)]
		public virtual void Start () {
			//Initialize NatCam
			NatCam.Initialize(Interface, PreviewType, DetectBarcodes, VerboseMode);
			//Set the active camera
			NatCam.ActiveCamera = Facing == Facing.Front ? DeviceCamera.FrontCamera : DeviceCamera.RearCamera;
			//Null checking
			if (NatCam.ActiveCamera == null) return;
			//Set the camera's resolution
			NatCam.ActiveCamera.SetResolution(Resolution);
			//Set the camera's framerate
			NatCam.ActiveCamera.SetFramerate(Framerate);
			//Set the camera's focus mode
			NatCam.ActiveCamera.FocusMode = FocusMode;
			//Play
			NatCam.Play();
			//Define a delegate to be executed once the preview starts //Note that this is a MUST when assigning the preview texture to anything
			NatCam.ExecuteOnPreviewStart(() => {
				//Set the RawImage texture once the preview starts
				if (RawImage != null) RawImage.texture = NatCam.Preview;
				//Log
				else "Unitygram: Preview RawImage has not been set".Warn();
				//Log
				("Unitygram: Active camera resolution: " + NatCam.ActiveCamera.ActiveResolution).Assert();
			});
		}
	}
}