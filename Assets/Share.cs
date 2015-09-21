using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class Share : MonoBehaviour {

	#if UNITY_IPHONE
	
	[DllImport("__Internal")]
	private static extern void _TAG_ShareTextWithImage (string iosPath, string message);
	
	[DllImport("__Internal")]
	private static extern void _TAG_ShareSimpleText (string message);	
	#endif

	public void ShareImage(string imageFileName, string subject, string title, string message)
	{
		/*byte[] bytes = Data.Instance.lastPhotoThumbTexture.EncodeToPNG();
		imageFileName = Application.persistentDataPath + "/MyImage.png";
		File.WriteAllBytes(imageFileName, bytes);*/
		
		#if UNITY_ANDROID
		
		AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
		AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
		
		intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
		intentObject.Call<AndroidJavaObject>("setType", "image/*");
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), subject);
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TITLE"), title);
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), message);
		
		AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
		AndroidJavaObject fileObject = new AndroidJavaObject("java.io.File", imageFileName);
		AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromFile", fileObject);
		
		bool fileExist = fileObject.Call<bool>("exists");
		Debug.Log("File exist : " + fileExist);
		// Attach image to intent
		if (fileExist)
			intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
		AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
		currentActivity.Call ("startActivity", intentObject);
		
		#elif UNITY_IPHONE		
		_TAG_ShareTextWithImage (imageFileName, message);
		#endif
	}
}
