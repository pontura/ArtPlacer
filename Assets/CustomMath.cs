using UnityEngine;
using System.Collections;

public class CustomMath{

	public static float cm2inches(float cm){return cm * 0.3937007874f;}
	public static float inches2cm(float inches){return inches * 2.54f;}	                                       
	public static float inches2feet(float inches){	return inches * 0.0833333333f;}
	public static float feet2inches(float feet){	return feet * 12f;}
}
