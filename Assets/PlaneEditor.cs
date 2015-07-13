using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CreatePlaneMesh))]
public class PlaneEditor : Editor {


	/*private void OnSceneGUI () {
		CreatePlaneMesh plane = target as CreatePlaneMesh;

		Transform handleTransform = plane.transform;
		Quaternion handleRotation = handleTransform.rotation;
		//Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

		Vector3 p0 = handleTransform.TransformPoint(plane.mesh.vertices [0]);
		Vector3 p1 = handleTransform.TransformPoint(plane.mesh.vertices [1]);
		Vector3 p2 = handleTransform.TransformPoint(plane.mesh.vertices [2]);
		Vector3 p3 = handleTransform.TransformPoint(plane.mesh.vertices [3]);
		
		Handles.color = Color.white;
		Handles.DrawLine(p0, p1);
		Handles.DrawLine(p0, p2);
		Handles.DrawLine(p2, p3);
		Handles.DrawLine(p1, p3);
		/*Debug.Log (Handles.PositionHandle(p0, handleRotation));
		/*Handles.PositionHandle(p0, handleRotation);
		Handles.PositionHandle(p1, handleRotation);
		Handles.PositionHandle(p2, handleRotation);
		Handles.PositionHandle(p3, handleRotation);


		EditorGUI.BeginChangeCheck();
		p0 = Handles.PositionHandle(p0, handleRotation);
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(plane.mesh, "Move Point");
			EditorUtility.SetDirty(plane);
			plane.mesh.vertices [0] = handleTransform.InverseTransformPoint(p0);
		}
		EditorGUI.BeginChangeCheck();
		p1 = Handles.PositionHandle(p1, handleRotation);
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(plane.mesh, "Move Point");
			EditorUtility.SetDirty(plane);
			plane.mesh.vertices [1] = handleTransform.InverseTransformPoint(p1);
		}
		EditorGUI.BeginChangeCheck();
		p2 = Handles.PositionHandle(p2, handleRotation);
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(plane.mesh, "Move Point");
			EditorUtility.SetDirty(plane.mesh);
			plane.mesh.vertices [2] = handleTransform.InverseTransformPoint(p2);
		}
		EditorGUI.BeginChangeCheck();
		p3 = Handles.PositionHandle(p3, handleRotation);
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(plane.mesh, "Move Point");
			EditorUtility.SetDirty(plane);
			plane.mesh.vertices [3] = handleTransform.InverseTransformPoint(p3);
			EditorUtility.SetDirty(plane);
		}
	}*/
}
