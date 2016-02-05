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
    public static Vector2 GetFormatedCentimeters(float value)
    {
        Vector2 res = new Vector2();

        string str = value.ToString();

        res.x = (int)value / 100;

        if (value < 100)
            res.y = value;
        else
            res.y = value - (res.x * 100);

        return res;
    }
    public static Vector2 GetFormatedInches(float value)
    {
        
        float inches = CustomMath.cm2inches(value);
        int feet = (int)Mathf.Floor(inches / 12);
        int restInches = (int)Mathf.Round(inches - (feet * 12));

        Vector2 res = new Vector2(feet, restInches);
        return res;
    }
    public static int GetTotalInches(int value0, int value1)
    {
        Debug.Log("GetTotalInches  " + value0 + "      " + value1);

        float inches = CustomMath.feet2inches(value0);

        return (int)inches + value1;
    }
}
