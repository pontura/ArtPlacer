/* 
*   NatCam
*   Copyright (c) 2016 Yusuf Olokoba
*/

using UnityEngine;
using System.Collections;
using NatCamU.Internals;

namespace NatCamU {
    
    [RequireComponent(typeof(RectTransform))] [NCDoc(19)]
    public class NatCamPreviewScaler : NatCamPreviewBehaviour {
        
        [Tooltip(positionTip)] [NCDoc(67)] public bool positionAtScreenCentre = true;
        [Tooltip(scaleTip)] [NCDoc(68)] [NCRef(0)] public ScaleMode scaleMode = ScaleMode.None;
        
        ///<summary>
        ///Scale the UI Panel to conform to a texture.
        ///</summary>
        [NCDoc(137)] [NCCode(3)]
        public void Apply (Texture texture) {
            //Apply
            Apply(new Vector2(texture.width, texture.height).Regularize()).Invoke(this);
        }
        
        ///<summary>
        ///Scale the UI Panel to conform to a custom size.
        ///</summary>
        [NCDoc(138)]
        public void Apply (Vector2 customResolution, bool regularize) {
            //Apply
            Apply(regularize ? customResolution.Regularize() : customResolution).Invoke(this);
        }
        
        protected override void Apply () {
            //Apply
            Apply().Invoke(this);
        }
        
        private IEnumerator Apply (Vector2 resolution = default(Vector2)) {
            //Null checking
            if (Mathf.RoundToInt(resolution.x) == 0 && NatCam.Preview == null) yield break;
            //Just to make sure all values have been updated
            yield return new WaitForEndOfFrame();
            //Canvas
            Canvas canvas = GetComponentInParent<Canvas>();
            //Transform
            RectTransform rectTransform = GetComponent<RectTransform>();
            //Positioning term
            //if (positionAtScreenCentre) rectTransform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, rectTransform.position.z - Camera.main.transform.position.z));
			if (positionAtScreenCentre) rectTransform.position = new Vector3(0.5f, 0.5f, rectTransform.position.z);
            //None check
            if (scaleMode == ScaleMode.None) yield break;
            //Scaling
            Vector2 panelSize = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
            Vector2 previewSize = resolution.x == 0f ? new Vector2(NatCam.Preview.width, NatCam.Preview.height).Regularize() : resolution;
            Vector2 screenSize = new Vector2(Screen.width, Screen.height).Regularize();
            Vector2 panelScale = rectTransform.localScale;
            //Log
            ("Preview Scaler Apply: "+panelSize+", "+previewSize+", "+screenSize+", "+panelScale).Assert();
            //Accommodate for scaled canvases
            panelSize *= canvas.scaleFactor;
            //Switch
            switch (scaleMode) {
                case ScaleMode.FillScreen:
                    panelScale = new Vector2(screenSize.x / panelSize.x, screenSize.y / panelSize.y);
                    float respectWidth = previewSize.Aspect() / panelSize.Aspect();
                    float respectHeight = 1 / respectWidth;
                    Vector3 fillScale = panelScale;
                    if (respectHeight < 1) {
                        float panelAspect = Screen.height / panelSize.x;
                        fillScale = new Vector3(rectTransform.localScale.x * previewSize.Aspect() * panelAspect, panelScale.y, 1);
                    }
                    else {
                        float panelAspect = Screen.width / panelSize.y;
                        fillScale = new Vector3(panelScale.x, rectTransform.localScale.y * panelAspect / previewSize.Aspect(), 1);
                    }
                    rectTransform.localScale = fillScale;
                break;
                case ScaleMode.FixedHeightVariableWidth:
                    StretchWidth(rectTransform, panelScale, previewSize, panelSize);
                break;
                case ScaleMode.FixedWidthVariableHeight:
                    StretchHeight(rectTransform, panelScale, previewSize, panelSize);
                break;
            }
        }
        
        private void StretchWidth (RectTransform rectTransform, Vector2 panelScale, Vector2 previewSize, Vector2 panelSize) {
            rectTransform.localScale = new Vector3(panelScale.x * previewSize.Aspect() / panelSize.Aspect(), panelScale.y, 1);
        }
        
        private void StretchHeight (RectTransform rectTransform, Vector2 panelScale, Vector2 previewSize, Vector2 panelSize) {
            rectTransform.localScale = new Vector3(panelScale.x, panelScale.y * panelSize.Aspect() / previewSize.Aspect(), 1);
        }
        
        private const string positionTip = "This positions the UI Panel at the centre of the screen. Note that this works best when the pivot is centered.";
        private const string scaleTip = "This dictates how NatCam applies scaling considering the active camera's preview resolution.";
    }
}