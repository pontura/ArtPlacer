using UnityEngine;
using System.Collections;

public class CustomMath{

	public static float cm2inches(float cm){return cm * 0.3937007874f;}
	public static float inches2cm(float inches){return inches * 2.54f;}	                                       
	public static float inches2feet(float inches){	return inches * 0.0833333333f;}
	public static float feet2inches(float feet){	return feet * 12f;}
   // public static float feet2cm(float feet) { return (feet * 30.48f) /100; }
   // public static float cm2inches(float cm) { return (cm * 0.393701f); }


	public static bool LineIntersectionPoint(out Vector3 result, Vector3 ps1, Vector3 pe1, Vector3 ps2, Vector3 pe2){
		
		result = Vector3.zero;
		
		Vector3 lineVec1 = (pe1 - ps1).normalized;
		Vector3 lineVec2 = (pe2 - ps2).normalized;
		
		float a = Vector3.Dot(lineVec1, lineVec1);
		float b = Vector3.Dot(lineVec1, lineVec2);
		float e = Vector3.Dot(lineVec2, lineVec2);
		
		float d = a*e - b*b;
		
		//lines are not parallel
		if(d > 0 || d < 0){
			Vector3 r = ps1 - ps2;
			float c = Vector3.Dot(lineVec1, r);
			float f = Vector3.Dot(lineVec2, r);
			
			float s = (b*f - c*e) / d;
			float t = (a*f - c*b) / d;
			
			result = ps1 + lineVec1 * s;
			
			return true;
		}
		
		else{
			return false;
		}
	}
    public static Vector4 GetFormatedCentimeters(float value)
    {
        Vector4 vect4 = new Vector4();

        string str = value.ToString();

        if (str.Length < 1) str = "0000";
        else if (str.Length < 2) str = "000" + str;
        else if (str.Length < 3) str = "00" + str;
        else if (str.Length < 4) str = "0" + str;

        vect4.x = int.Parse(str.Substring(0, 1));
        vect4.y = int.Parse(str.Substring(1, 1));
        vect4.w = int.Parse(str.Substring(2, 1));
        vect4.z = int.Parse(str.Substring(3, 1));

        return vect4;
    }
    public static Vector4 GetFormatedInches(float value)
    {
        Vector4 vect4 = new Vector4();

        float inches = CustomMath.cm2inches(value);
        int feet = (int)Mathf.Floor(inches / 12);
        int restInches = (int)Mathf.Round(inches - (feet * 12));

        string str = feet.ToString();
        if (feet < 10) str = "0" + feet;
        if (restInches < 10) str += "0" + restInches;       

        vect4.x = int.Parse(str.Substring(0, 1));
        vect4.y = int.Parse(str.Substring(1, 1));
        vect4.w = int.Parse(str.Substring(2, 1));
        vect4.z = int.Parse(str.Substring(3, 1));
        Debug.Log("inches: " + inches + "    value: " + value + "   feet: " + feet + "   restInches: " + restInches);
        return vect4;
    }
    public static int GetTotalInches(int value0, int value1, int value2, int value3)
    {
        string feets_txt = value0 + "" + value1;
        string inches_txt = value2 + "" + value3;
        int feets = int.Parse(feets_txt);
        int inches = int.Parse(inches_txt);
        int totalInches = (feets * 12) + inches;

        return totalInches;
    }
}
