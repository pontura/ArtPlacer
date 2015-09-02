using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class ArtPlaced : MonoBehaviour {

    private int defaultHeight = 50;
	public GameObject CreatedPlane;
	public GameObject Thumb;

    public Animation tooltipAddArt;
    public GameObject buttonAddArt;
	public GameObject buttonInfo;

	Camera cam;
	
	string selected = null;
	GameObject selectedArtwork;
	GameObject[] areas;
	GameObject thumbClone;
	SpriteRenderer thumbRenderer;

	bool dragOut = false;

	public int sel_galleryID;
	public int sel_galleryArtID;
	
	void Start () {

		Events.OnSelectFooterArtwork += AddFromFooter;
		Events.ArtworkPreview += Preview;

        tooltipAddArt.gameObject.SetActive(false);

        if (Data.Instance.areaData.areas.Count > 0)
        {
			areas = new GameObject[Data.Instance.areaData.areas.Count];
            for (int i = 0; i < Data.Instance.areaData.areas.Count; i++)
            {
                GameObject obj = Instantiate(CreatedPlane, Data.Instance.areaData.getPosition(i), Quaternion.identity) as GameObject;
				WallPlane wp = obj.GetComponent<WallPlane>();
				wp.area.GetComponent<MeshFilter>().mesh.vertices = Data.Instance.areaData.getPointers(i);
				wp.EnableAreaCollider(true);
				//wp.area.GetComponent<MeshCollider>().sharedMesh = mesh;
                wp.SetId(i);
				Data.Instance.areaData.areas[i].artworkIDCount = 0;
				areas[i] = obj;
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

			if (selected == null) {
				SelectArtwork2Drag();
			} else {
				Ray ray = cam.ScreenPointToRay (Input.mousePosition);
				RaycastHit[] hits;
				hits = Physics.RaycastAll (ray.origin, ray.direction, 100.0F);
				bool hited = false;				
				for (int i = 0; i < hits.Length; i++) {
					if(hits[i].collider.name==selected){
						DragArtWork(hits[i]);
						hited = true;
						break;
					}
				}

				Vector3 mPos = Input.mousePosition;
				mPos.z = 1.0f;
				thumbClone.transform.position = cam.ScreenToWorldPoint(mPos)-new Vector3(thumbClone.transform.localScale.x*0.5f,thumbClone.transform.localScale.y*0.5f,0f);

				dragOut=!hited;
				if(dragOut){
					thumbRenderer.enabled = true;
					selectedArtwork.GetComponent<MeshRenderer>().enabled=false;
				}
			}
		} else {
			if(selected!=null){
				if(dragOut){
					Artwork2Trash(selectedArtwork);
				}else{
					SetArtworkPosition(selectedArtwork);
				}
			}
		}
	}

	void SelectArtwork2Drag(){
		Ray ray = cam.ScreenPointToRay (Input.mousePosition);
		RaycastHit[] hits;
		hits = Physics.RaycastAll (ray.origin, ray.direction, 100.0F);
		
		for (int i = 0; i < hits.Length; i++) {
			RaycastHit hit = hits [i];			
			Renderer rend = hit.transform.GetComponent<Renderer> ();
			
			if (rend == null || rend.material.mainTexture == null)
				return;
			
			Texture2D tex = rend.material.mainTexture as Texture2D;
			Vector2 pixelUV = hit.textureCoord;
			pixelUV.x *= tex.width * rend.material.mainTextureScale.x;
			pixelUV.x += tex.width * rend.material.mainTextureOffset.x;
			pixelUV.y *= tex.height * rend.material.mainTextureScale.y;
			pixelUV.y += tex.height * rend.material.mainTextureOffset.y;
			if (tex.GetPixel ((int)pixelUV.x, (int)pixelUV.y).a == 1) {
				//print (hit.collider.name+" Selected, RGBA: "+tex.GetPixel((int)pixelUV.x,(int)pixelUV.y));
				selected = hit.collider.name;
				selectedArtwork = hit.collider.gameObject;
				selectedArtwork.transform.position = selectedArtwork.transform.position-new Vector3(0,0,0.1f);
				int areaId = hit.collider.GetComponent<DragArtWork>().areaId;
				int artWorkId = hit.collider.GetComponent<DragArtWork>().artWorkId;			
				AreaData.ArtWork aw = Data.Instance.areaData.areas[areaId].artworks.Find(x => x.id==artWorkId);
				Data.Instance.SetLastArtTexture(aw.texture);
				Texture2D t = Data.Instance.lastArtThumbTexture;
				Debug.Log("Gallery Id: "+aw.galleryID);
				Debug.Log("Art Id: "+aw.galleryArtID);
				sel_galleryID = aw.galleryID;
				sel_galleryArtID = aw.galleryArtID;
				thumbClone = Instantiate(Thumb, Data.Instance.areaData.getPosition(areaId), Quaternion.identity) as GameObject;
				thumbClone.name = "thumb_"+selected;
				thumbRenderer = thumbClone.GetComponent<SpriteRenderer> ();
				thumbRenderer.sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.zero);
				thumbRenderer.enabled = false;
				buttonInfo.SetActive(true);
				StartCoroutine(buttonInfo.GetComponent<ButtonFeedback>().InfoButtonFeedback());
				break;
			}					
		}
	}

	void DragArtWork(RaycastHit hit){
				 
		Renderer rend = hit.transform.GetComponent<Renderer> ();	
		Vector2 scale = new Vector2(1/rend.material.mainTextureScale.x,1/rend.material.mainTextureScale.y);
		Vector2 pixelUV = 0.5f*(scale)-hit.textureCoord;
		Vector2 offset = new Vector2(pixelUV.x*rend.material.mainTextureScale.x,pixelUV.y*rend.material.mainTextureScale.y);
		//if(hit.textureCoord.x-0.25f*scale.x>0f&&hit.textureCoord.x+0.25f*scale.x<1f&&hit.textureCoord.y-0.25f*scale.y>0f&&hit.textureCoord.y+0.25f*scale.y<1f){
		if(hit.textureCoord.x>0f&&hit.textureCoord.x<1f&&hit.textureCoord.y>0f&&hit.textureCoord.y<1f){
			thumbRenderer.enabled = false;
			hit.collider.GetComponent<MeshRenderer>().enabled=true;
		}else{
			thumbRenderer.enabled = true;
			hit.collider.GetComponent<MeshRenderer>().enabled=false;
		}		
		rend.material.mainTextureOffset = offset;
		//print ("Hit: "+hit.textureCoord+" Scale: "+scale+" pixelUV: "+pixelUV+" Offset: "+offset);
	}

	void Artwork2Trash(GameObject sel){
		int areaId = sel.GetComponent<DragArtWork>().areaId;
		int artWorkId = sel.GetComponent<DragArtWork>().artWorkId;
		Data.Instance.lastArtTexture = Data.Instance.areaData.areas[areaId].artworks.Find(x => x.id==artWorkId).texture;
		//Data.Instance.lastArtTexture = Data.Instance.areaData.areas[areaId].artworks[artWorkId].texture;
		//Data.Instance.areaData.areas[areaId].artworks.RemoveAt(artWorkId);
		Data.Instance.areaData.areas[areaId].artworks.Remove(Data.Instance.areaData.areas[areaId].artworks.Find(x => x.id==artWorkId));
		//Debug.Log ("Parent: " + sel.transform.parent.gameObject.name);
		sel.transform.parent.gameObject.GetComponent<WallPlane>().artWorkNumber--;
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
	}

	void SetArtworkPosition(GameObject sel){
		sel.transform.position = sel.transform.position+new Vector3(0,0,0.1f);
		int areaId = sel.GetComponent<DragArtWork> ().areaId;
		int artWorkId = sel.GetComponent<DragArtWork> ().artWorkId;
		Renderer rend = sel.transform.GetComponent<Renderer> ();
		//Data.Instance.areaData.areas[areaId].artworks[artWorkId].position = rend.material.mainTextureOffset;
		Data.Instance.areaData.areas[areaId].artworks.Find(x => x.id==artWorkId).position = rend.material.mainTextureOffset;
		if(!sel.GetComponent<MeshRenderer>().enabled)sel.GetComponent<MeshRenderer>().enabled=true;
		Destroy(thumbClone);
		selected=null;
	}

	public void Preview(bool enable){
		if (Data.Instance.areaData.areas.Count > 0){
			for (int i = 0; i < Data.Instance.areaData.areas.Count; i++){				
				areas[i].GetComponent<WallPlane>().area.gameObject.SetActive(enable);				
			}
		}
	}

	public void ArtBrowser()
	{
		tooltipAddArt.gameObject.SetActive(false);
		Data.Instance.LoadLevel("Galleries");
	}

    public void GotoGallery()
    {
        tooltipAddArt.gameObject.SetActive(false);
        Data.Instance.LoadLevel("Artworks");
    }

	public void EditWalls()
	{
		Data.Instance.LoadLevel("Walls");
	}

	public void Ready()
	{
        Events.OnLoading(true);
		Data.Instance.SaveRoom();
		Data.Instance.lastArtTexture = null;
		//print ("Areas Count: " + Data.Instance.areaData.areas.Count);
		Data.Instance.areaData.areas.Clear ();
		//print ("Areas Count: " + Data.Instance.areaData.areas.Count);
        Invoke("ReadyJump", 1);
	}
    void ReadyJump()
    {
        Data.Instance.LoadLevel("Room");
    }

	public void AddFromFooter(){
		if (Data.Instance.areaData.areas.Count > 0) {
			selected = "ArtWork_" + 0 + "_" + Data.Instance.areaData.areas[0].artworkIDCount;
			thumbClone = Instantiate(Thumb, Data.Instance.areaData.getPosition(0), Quaternion.identity) as GameObject;
			thumbClone.name = "thumb_"+selected;
			thumbRenderer = thumbClone.GetComponent<SpriteRenderer> ();
			Texture2D t = Data.Instance.lastArtThumbTexture;				
			thumbRenderer.sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.zero);
			selectedArtwork = AddArt (0);
			selectedArtwork.transform.position = selectedArtwork.transform.position-new Vector3(0,0,0.1f);
		}
	}

	void PlaceArt(int n){

        tooltipAddArt.Stop();
        buttonAddArt.GetComponent<Animation>().Stop();

		GameObject area = areas[n];
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

				int h = Data.Instance.areaData.areas[n].artworks[i].height;
				float aspect = 1f*Data.Instance.areaData.areas[n].artworks[i].texture.width/Data.Instance.areaData.areas[n].artworks[i].texture.height;
				h=h==0?defaultHeight:h;
				int w=(int)(h*aspect);
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

	GameObject AddArt(int n){

		GameObject area = areas[n];
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

		int h = (int)Data.Instance.artData.selectedArtWork.size.y;
		float aspect = 1f*Data.Instance.lastArtTexture.width/Data.Instance.lastArtTexture.height;
        h = h == 0 ? defaultHeight : h;
		//w=w==0?(int)(h*aspect):w;
		int w = (int)(h * aspect);
		Data.Instance.areaData.areas[n].AddArtWork(w,h,Data.Instance.lastArtTexture,Data.Instance.artData.selectedArtWork);
		sel_galleryID = Data.Instance.artData.selectedArtWork.galleryId;
		sel_galleryArtID = Data.Instance.artData.selectedArtWork.artId;
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
		return artWork;	
	}

	public void GetArtInfo(){
		if(Data.Instance.lastArtTexture==null){
			int artWorkId = selectedArtwork.GetComponent<DragArtWork>().artWorkId;
			int areaId = selectedArtwork.GetComponent<DragArtWork>().areaId;
			AreaData.ArtWork aw = Data.Instance.areaData.areas[areaId].artworks.Find(x => x.id==artWorkId);
			Data.Instance.SetLastArtTexture(aw.texture);
		}
		Data.Instance.artData.selectedGallery = sel_galleryID;
		Data.Instance.artData.SetSelectedArtwork (sel_galleryArtID);
		Data.Instance.isArtworkInfo2Place = false;
		Data.Instance.LoadLevel("ConfirmArtWork");
	}

	void OnDestroy()
	{
		Events.OnSelectFooterArtwork -= AddFromFooter;
		Events.ArtworkPreview -= Preview;
	}

}

