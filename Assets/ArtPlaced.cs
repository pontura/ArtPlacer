using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class ArtPlaced : MonoBehaviour {

	public GameObject CreatedPlane;
	public GameObject Thumb;

    public Animation tooltipAddArt;
    public GameObject buttonAddArt;

	public GameObject bg;
	public GameObject edit;
	public GameObject addArtwork;
	public GameObject preview;
	public GameObject back;

	Camera cam;
	
	string selected = null;
	GameObject thumbClone;

	bool dragOut = false;
	
	void Start () {

        tooltipAddArt.gameObject.SetActive(false);

        if (Data.Instance.areaData.areas.Count > 0)
        {
            for (int i = 0; i < Data.Instance.areaData.areas.Count; i++)
            {
                GameObject obj = Instantiate(CreatedPlane, Data.Instance.areaData.getPosition(i), Quaternion.identity) as GameObject;
				obj.GetComponent<WallPlane>().area.GetComponent<MeshFilter>().mesh.vertices = Data.Instance.areaData.getPointers(i);
				obj.GetComponent<WallPlane>().EnableAreaCollider();
				//obj.GetComponent<WallPlane>().area.GetComponent<MeshCollider>().sharedMesh = mesh;
                obj.GetComponent<WallPlane>().SetId(i);
				Data.Instance.areaData.areas[i].artworkIDCount = 0;
                PlaceArt(i);
            }
        }


		foreach (Camera c in Camera.allCameras) {
			if(c.name == "CameraWallArea"){
				cam = c;
				break;
			}
		}
        CheckOpenHelp();       
    }
    void CheckOpenHelp()
    {
        if (Data.Instance.areaData.CountArtPlaced() == 0 && Data.Instance.lastArtTexture == null)
            Invoke("startTooltip", 0.5f);
        else buttonAddArt.GetComponent<Animation>().Stop();
    }
    void startTooltip()
    {
        tooltipAddArt.gameObject.SetActive(true);
		tooltipAddArt.Play ("tooltipOn");
        
    }

	void Update() {

		if (Input.GetButton ("Fire1")) {
			Ray ray = cam.ScreenPointToRay (Input.mousePosition);
			if (selected == null) {
				RaycastHit[] hits;
				hits = Physics.RaycastAll (ray.origin, ray.direction, 100.0F);
		
				for (int i = 0; i < hits.Length; i++) {
					RaycastHit hit = hits [i];			
					Renderer rend = hit.transform.GetComponent<Renderer> ();

					if (rend == null || rend.material.mainTexture == null)
						return;

					Texture2D tex = rend.material.mainTexture as Texture2D;
					Vector2 pixelUV = hit.textureCoord;
					//print (rend.material.mainTextureOffset.x);
					pixelUV.x *= tex.width * rend.material.mainTextureScale.x;
					pixelUV.x += tex.width * rend.material.mainTextureOffset.x;
					//print (pixelUV.x);
					pixelUV.y *= tex.height * rend.material.mainTextureScale.y;
					pixelUV.y += tex.height * rend.material.mainTextureOffset.y;
					if (tex.GetPixel ((int)pixelUV.x, (int)pixelUV.y).a == 1) {
						//print (hit.collider.name+" Selected, RGBA: "+tex.GetPixel((int)pixelUV.x,(int)pixelUV.y));
						selected = hit.collider.name;

						int areaId = hit.collider.GetComponent<DragArtWork>().areaId;
						int artWorkId = hit.collider.GetComponent<DragArtWork>().artWorkId;

						Texture2D t = Data.Instance.areaData.areas[areaId].artworks.Find(x => x.id==artWorkId).texture;

						thumbClone = Instantiate(Thumb, Data.Instance.areaData.getPosition(areaId), Quaternion.identity) as GameObject;
						thumbClone.name = "thumb_"+selected;
						thumbClone.GetComponent<SpriteRenderer>().sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.zero);
						thumbClone.GetComponent<SpriteRenderer>().enabled = false;
						break;
					}					
				}
			} else {
				GameObject sel = GameObject.Find(selected);
				RaycastHit[] hits;
				hits = Physics.RaycastAll (ray.origin, ray.direction, 100.0F);
				bool hited = false;
				
				for (int i = 0; i < hits.Length; i++) {
					if(hits[i].collider.name==selected){
						//Debug.Log (hits[i].collider.name);
						RaycastHit hit = hits [i];
						Renderer rend = hit.transform.GetComponent<Renderer> ();
						Vector2 scale = new Vector2(1/rend.material.mainTextureScale.x,1/rend.material.mainTextureScale.y);
						Vector2 pixelUV = 0.5f*(scale)-hit.textureCoord;
						Vector2 offset = new Vector2(pixelUV.x*rend.material.mainTextureScale.x,pixelUV.y*rend.material.mainTextureScale.y);
						//if(hit.textureCoord.x-0.25f*scale.x>0f&&hit.textureCoord.x+0.25f*scale.x<1f&&hit.textureCoord.y-0.25f*scale.y>0f&&hit.textureCoord.y+0.25f*scale.y<1f){
						if(hit.textureCoord.x>0f&&hit.textureCoord.x<1f&&hit.textureCoord.y>0f&&hit.textureCoord.y<1f){
							thumbClone.GetComponent<SpriteRenderer>().enabled = false;
							hits[i].collider.GetComponent<MeshRenderer>().enabled=true;
						}else{
							thumbClone.GetComponent<SpriteRenderer>().enabled = true;
							hits[i].collider.GetComponent<MeshRenderer>().enabled=false;
						}

						rend.material.mainTextureOffset = offset;
						//print ("Hit: "+hit.textureCoord+" Scale: "+scale+" pixelUV: "+pixelUV+" Offset: "+offset);
						hited = true;
						break;
					}
					/*else{
						Debug.Log (hits[i].collider.name);
					}*/
				}

				Vector3 mPos = Input.mousePosition;
				mPos.z = 1.0f;
				thumbClone.transform.position = cam.ScreenToWorldPoint(mPos)-thumbClone.transform.localScale;
				dragOut=!hited;
				if(dragOut){
					thumbClone.GetComponent<SpriteRenderer>().enabled = true;
					sel.GetComponent<MeshRenderer>().enabled=false;
				}
			}
		} else {
			if(selected!=null){
				GameObject sel = GameObject.Find(selected);
				int areaId = sel.GetComponent<DragArtWork>().areaId;
				int artWorkId = sel.GetComponent<DragArtWork>().artWorkId;
				if(dragOut){
					Data.Instance.lastArtTexture = Data.Instance.areaData.areas[areaId].artworks.Find(x => x.id==artWorkId).texture;
					//Data.Instance.lastArtTexture = Data.Instance.areaData.areas[areaId].artworks[artWorkId].texture;
					//Data.Instance.areaData.areas[areaId].artworks.RemoveAt(artWorkId);
					Data.Instance.areaData.areas[areaId].artworks.Remove(Data.Instance.areaData.areas[areaId].artworks.Find(x => x.id==artWorkId));
					GameObject.Find ("CreatedPlane_" + areaId).GetComponent<WallPlane>().artWorkNumber--;
					Destroy(sel);
					Destroy(thumbClone);
					selected=null;
					Ray ray = cam.ScreenPointToRay (Input.mousePosition);
					RaycastHit[] hits;
					hits = Physics.RaycastAll (ray.origin, ray.direction, 100.0F);						
					for (int i = 0; i < hits.Length; i++) {
						//Debug.Log(hits[i].collider.gameObject.transform.parent);
						if(hits[i].collider.gameObject.transform.parent.name.Contains("CreatedPlane_")){
							int newAreaID = hits[i].collider.gameObject.transform.parent.GetComponent<WallPlane>().AreaId;
							AddArt(newAreaID);
							break;
						}
					}

				}else{
					Renderer rend = sel.transform.GetComponent<Renderer> ();
					//Data.Instance.areaData.areas[areaId].artworks[artWorkId].position = rend.material.mainTextureOffset;
					Data.Instance.areaData.areas[areaId].artworks.Find(x => x.id==artWorkId).position = rend.material.mainTextureOffset;
					if(!sel.GetComponent<MeshRenderer>().enabled)sel.GetComponent<MeshRenderer>().enabled=true;
					Destroy(thumbClone);
					selected=null;
				}
			}
		}
	}

	public void Back(){
		bg.gameObject.SetActive(true);
		edit.gameObject.SetActive(true);
		addArtwork.gameObject.SetActive(true);
		preview.gameObject.SetActive(true);
		back.gameObject.SetActive(false);

		if (Data.Instance.areaData.areas.Count > 0){
			for (int i = 0; i < Data.Instance.areaData.areas.Count; i++){				
				GameObject area = GameObject.Find ("CreatedPlane_" + i);
				area.GetComponent<WallPlane>().area.gameObject.SetActive(true);				
				/*if (Data.Instance.areaData.areas[i].artworks.Count > 0) {				
					for (int j=0; j<Data.Instance.areaData.areas[i].artworks.Count; j++) {
						print ("Cursor_ArtWork_" + i +"_"+j);
						GameObject.Find ("Cursor_ArtWork_" + i +"_"+j).GetComponent<MeshRenderer>().enabled = true;
					}
				}*/
			}
		}
	}

	public void Preview(){
		bg.gameObject.SetActive(false);
		edit.gameObject.SetActive(false);
		addArtwork.gameObject.SetActive(false);
		preview.gameObject.SetActive(false);
		back.gameObject.SetActive(true);

		if (Data.Instance.areaData.areas.Count > 0){
			for (int i = 0; i < Data.Instance.areaData.areas.Count; i++){				
				GameObject area = GameObject.Find ("CreatedPlane_" + i);
				area.GetComponent<WallPlane>().area.gameObject.SetActive(false);				
				/*if (Data.Instance.areaData.areas[i].artworks.Count > 0) {				
					for (int j=0; j<Data.Instance.areaData.areas[i].artworks.Count; j++) {
						print ("Cursor_ArtWork_" + i +"_"+j);
						GameObject.Find ("Cursor_ArtWork_" + i +"_"+j).GetComponent<MeshRenderer>().enabled = false;
					}
				}*/
			}
		}

	}

	public void ArtBrowser()
	{
		tooltipAddArt.gameObject.SetActive(false);
		Data.Instance.LoadLevel("Galleries");
	}

	public void EditWalls()
	{
		Data.Instance.LoadLevel("Walls");
	}

	void PlaceArt(int n){

        tooltipAddArt.Stop();
        buttonAddArt.GetComponent<Animation>().Stop();

		GameObject area = GameObject.Find ("CreatedPlane_" + n);
		if (Data.Instance.areaData.areas[n].artworks.Count > 0) {

			tooltipAddArt.gameObject.SetActive(false);

			float aW = Data.Instance.areaData.areas[n].width*100;
			float aH = Data.Instance.areaData.areas[n].height*100;

			for (int i=0; i<Data.Instance.areaData.areas[n].artworks.Count; i++) {

				GameObject artWork = Instantiate (area.GetComponent<WallPlane> ().artWork, new Vector3 (0f, 0f, 0f), Quaternion.identity) as GameObject;
				artWork.name = "ArtWork_" + n +"_"+ Data.Instance.areaData.areas[n].artworkIDCount;
				artWork.transform.position = new Vector3(area.transform.position.x,area.transform.position.y,area.transform.position.z-0.01f);
				//artWork.transform.position = area.transform.position;
				artWork.transform.SetParent (area.transform);
				artWork.GetComponent<CustomPlane>().SetPointers(Data.Instance.areaData.getPointers(n));
				artWork.GetComponent<CustomPlane>().CustomMesh();
				artWork.GetComponent<DragArtWork> ().SetAreaId(n);

				Texture2D tex = new Texture2D(Data.Instance.areaData.areas[n].artworks[i].texture.width*2,Data.Instance.areaData.areas[n].artworks[i].texture.height*2);
				tex.wrapMode = TextureWrapMode.Clamp;
				int offSetX = tex.width/4;
				int offSetY = tex.height/4;

				for (int y = 0; y < Data.Instance.areaData.areas[n].artworks[i].texture.height; y++) {
					for (int x = 0; x < Data.Instance.areaData.areas[n].artworks[i].texture.width; x++) {
						Color color = Data.Instance.areaData.areas[n].artworks[i].texture.GetPixel(x,y);
						tex.SetPixel(offSetX+x, offSetY+y, color);
					}
				}				
				tex.Apply();

				//artWork.GetComponent<Renderer> ().material.SetTexture ("_Tex"+area.GetComponent<WallPlane> ().artWorkNumber,tex);
				artWork.GetComponent<Renderer>().material.mainTexture = tex;

				int w = Data.Instance.areaData.areas[n].artworks[i].width;
				int h = Data.Instance.areaData.areas[n].artworks[i].height;
				float aspect = 1f*Data.Instance.areaData.areas[n].artworks[i].texture.width/Data.Instance.areaData.areas[n].artworks[i].texture.height;
				h=h==0?100:h;
				w=w==0?(int)(h*aspect):w;
				//artWork.GetComponent<Renderer> ().material.SetTextureScale("_Tex"+area.GetComponent<WallPlane> ().artWorkNumber,new Vector2(0.5f*aW/w,0.5f*aH/h));
				artWork.GetComponent<Renderer> ().material.mainTextureScale = new Vector2(0.5f*aW/w,0.5f*aH/h);
				//area.GetComponent<Homography> ().SetHomography (artWork.name);
				artWork.GetComponent<Renderer> ().material.mainTextureOffset = Data.Instance.areaData.areas[n].artworks[i].position;

				Data.Instance.areaData.areas [n].artworks [area.GetComponent<WallPlane> ().artWorkNumber].id = Data.Instance.areaData.areas [n].artworkIDCount;

				artWork.GetComponent<DragArtWork> ().SetArtWorkId(i);
				area.GetComponent<WallPlane> ().artWorkNumber++;
				Data.Instance.areaData.areas[n].artworkIDCount++;
			}

		}

		if (Data.Instance.lastArtTexture != null) {
			AddArt(n);					
		}
	}

	void AddArt(int n){

		GameObject area = GameObject.Find ("CreatedPlane_" + n);
		float aW = Data.Instance.areaData.areas[n].width*100;
		float aH = Data.Instance.areaData.areas[n].height*100;
		
		GameObject artWork = Instantiate (area.GetComponent<WallPlane> ().artWork, new Vector3 (0f, 0f, 0f), Quaternion.identity) as GameObject;
		artWork.name = "ArtWork_" + n + "_" + Data.Instance.areaData.areas[n].artworkIDCount;
		artWork.transform.position = new Vector3(area.transform.position.x,area.transform.position.y,area.transform.position.z-0.01f);
		//artWork.transform.position = area.transform.position;
		artWork.transform.SetParent (area.transform);
		artWork.GetComponent<CustomPlane>().SetPointers(Data.Instance.areaData.getPointers(n));
		artWork.GetComponent<CustomPlane>().CustomMesh();
		artWork.GetComponent<DragArtWork> ().SetAreaId(n);
		
		int w = (int)Data.Instance.artData.selectedArtWork.size.x;
		int h = (int)Data.Instance.artData.selectedArtWork.size.y;
		float aspect = 1f*Data.Instance.lastArtTexture.width/Data.Instance.lastArtTexture.height;
		h=h==0?100:h;
		w=w==0?(int)(h*aspect):w;
		Data.Instance.areaData.areas[n].AddArtWork(w,h,Data.Instance.lastArtTexture);
		Texture2D tex = new Texture2D(Data.Instance.lastArtTexture.width*2,Data.Instance.lastArtTexture.height*2);
		tex.wrapMode = TextureWrapMode.Clamp;
		int offSetX = tex.width/4;
		int offSetY = tex.height/4;
		
		for (int y = 0; y < Data.Instance.lastArtTexture.height; y++) {
			for (int x = 0; x < Data.Instance.lastArtTexture.width; x++) {
				Color color = Data.Instance.lastArtTexture.GetPixel(x,y);
				tex.SetPixel(offSetX+x, offSetY+y, color);
			}
		}
		tex.Apply();
		
		Renderer rend = artWork.GetComponent<Renderer> ();
		rend.material.mainTexture = tex;
		rend.material.mainTextureScale = new Vector2(0.5f*aW/w,0.5f*aH/h);
		
		Vector2 scale = new Vector2(1/rend.material.mainTextureScale.x,1/rend.material.mainTextureScale.y);
		Vector2 pixelUV = 0.5f*(scale)-new Vector2(0.5f,0.5f);
		Vector2 offset = new Vector2(pixelUV.x*rend.material.mainTextureScale.x,pixelUV.y*rend.material.mainTextureScale.y);
		rend.material.mainTextureOffset = offset;

		Data.Instance.areaData.areas[n].artworks[area.GetComponent<WallPlane> ().artWorkNumber].position = offset;
		
		//artWork.GetComponent<Renderer> ().material.mainTextureOffset = new Vector2();
		//artWork.GetComponent<Renderer> ().material.t
		//area.GetComponent<Homography> ().SetHomography (artWork.name);
		
		Data.Instance.lastArtTexture = null;
		artWork.GetComponent<DragArtWork> ().SetArtWorkId(Data.Instance.areaData.areas [n].artworkIDCount);
		Data.Instance.areaData.areas [n].artworks [area.GetComponent<WallPlane> ().artWorkNumber].id = Data.Instance.areaData.areas [n].artworkIDCount;
		area.GetComponent<WallPlane> ().artWorkNumber++;
		Data.Instance.areaData.areas [n].artworkIDCount++;
	}
}

