using UnityEngine;
using System.Collections;

public class ComboBox : MonoBehaviour {

	public StyledComboBox comboBox;

	void Start () 
	{   
		// just text
		comboBox.AddItems("GGG", "Test2", "Test3", "Unity", "Needs", "A", "Better", "Encapsulation", "System", "Than", "Prefabs");
	}
}
