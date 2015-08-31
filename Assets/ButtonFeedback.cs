using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonFeedback : MonoBehaviour {

	public float time;

	private Animation infoAnim;
	private Image infoImage;

	public void Start(){
		infoAnim = gameObject.GetComponent<Animation> ();
		infoImage = gameObject.GetComponentInChildren<Image>();
	}

	public IEnumerator InfoButtonFeedback(){
		infoAnim.Play ();
		infoImage.color = Data.Instance.selectedColor;
		yield return new WaitForSeconds(time);
		infoAnim.Stop ();
		infoImage.color = Color.white;
	}
}
