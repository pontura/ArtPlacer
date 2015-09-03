using UnityEngine;
using System.Collections;

public class BrowseDevice : MonoBehaviour {

	public void OpenGallery()
	{
		#if UNITY_ANDROID

		//instantiate the class Intent
		AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
		//instantiate the object Intent
		AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

		//call setAction setting ACTION_SEND as parameter
		intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_PICK"));


		//instantiate the class Uri
		//AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
		//instantiate the object Uri with the parse of the url's file
		//AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "content://media/internal/images/media");
		//call putExtra with the uri object of the file
		//intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);

		//set the type of file
		intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
		#region [ startActivity(intent); ]
		//instantiate the class UnityPlayer
		AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		//instantiate the object currentActivity
		AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
		//call the activity with our Intent
		currentActivity.Call("startActivity", intentObject);
		#endregion [ startActivity(intent); ]

		#endif
	}

}
