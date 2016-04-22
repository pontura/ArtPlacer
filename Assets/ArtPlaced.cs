using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ArtPlaced : MonoBehaviour {

    private int defaultHeight = 50;
	public GameObject CreatedPlane;
	public GameObject Thumb;

	public GameObject buttonInfo;
	public GameObject saveDialog;

	Camera cam;
	
	string selected = null;
	GameObject selectedArtwork;
	GameObject[] areas;
	GameObject thumbClone;
	SpriteRenderer thumbRenderer;

	public List<GameObject> artworkList;

	bool dragOut = false;

	public int sel_galleryID;
	public int sel_galleryArtID;

	private float moveStep = 0.005f;
	
	void Start () {

        EventsAnalytics.SendScreen("Enter_Local_Room");

		Events.OnLoading (false);
        //buttonInfo.SetActive(false);
        Data.Instance.SetTitle("Rooms");
        Events.Back += Back;
		Data.Instance.SetBackActive (true);
		Events.OnSelectFooterArtwork += AddFromFooter;
		Events.ArtworkPreview += Preview;
        Events.HelpShow();

		Events.MoveButton += MoveButton;

		artworkList = new List<GameObject> ();

		buttonInfo.GetComponent<Button>().interactable = false;

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
				Data.Instance.areaData.areas[i].artworkCount = 0;
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
        Invoke("timeOut", 0.2f);

    }
    void timeOut()
    {
        if(Data.Instance.lastScene == "Walls")
            Data.Instance.SaveRoom(false);

        if (artworkList.Count == 0)
            Events.HelpChangeState(true);
        else Events.HelpChangeStep(2);
    }
    void Back()
    {
        if (Data.Instance.areaData.areas.Count == 0)
        {
            Data.Instance.LoadLevel("Rooms");
        }
        else
        {
            Data.Instance.LoadLevel("NewRoomConfirmation");
        }
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
					Events.OnLoading(false);
				}else{
					Events.OnLoading(false);
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
				int areaId = hit.collider.GetComponent<DragArtWork>().areaIndex;
				int artWorkId = hit.collider.GetComponent<DragArtWork>().artWorkIndex;			
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
				buttonInfo.GetComponent<Button>().interactable = true;
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
		int areaId = sel.GetComponent<DragArtWork>().areaIndex;
		int artWorkId = sel.GetComponent<DragArtWork>().artWorkIndex;
		Data.Instance.lastArtTexture = Data.Instance.areaData.areas[areaId].artworks.Find(x => x.id==artWorkId).texture;
		//Data.Instance.lastArtTexture = Data.Instance.areaData.areas[areaId].artworks[artWorkId].texture;
		//Data.Instance.areaData.areas[areaId].artworks.RemoveAt(artWorkId);
		Data.Instance.areaData.areas[areaId].artworks.Remove(Data.Instance.areaData.areas[areaId].artworks.Find(x => x.id==artWorkId));
		//Debug.Log ("Parent: " + sel.transform.parent.gameObject.name);
		sel.transform.parent.gameObject.GetComponent<WallPlane>().artWorkNumber--;
		Destroy(sel);
		Destroy(thumbClone);
		buttonInfo.GetComponent<Button>().interactable = false;
		selected=null;
		Ray ray = cam.ScreenPointToRay (Input.mousePosition);
		RaycastHit[] hits;
		hits = Physics.RaycastAll (ray.origin, ray.direction, 100.0F);						
		for (int i = 0; i < hits.Length; i++) {
			//Debug.Log(hits[i].collider.gameObject.transform.parent);
			if(hits[i].collider.gameObject.transform.parent.name.Contains("CreatedPlane_")){
				Data.Instance.artData.selectedGallery = sel_galleryID;
				Data.Instance.artData.SetSelectedArtworkByArtID(sel_galleryArtID);
				int newAreaID = hits[i].collider.gameObject.transform.parent.GetComponent<WallPlane>().AreaId;
				selectedArtwork = AddArt(newAreaID);
				selectedArtwork.transform.position = selectedArtwork.transform.position-new Vector3(0,0,0.1f);

				break;
			}
		}
	}

	void SetArtworkPosition(GameObject sel){
		sel.transform.position = sel.transform.position+new Vector3(0,0,0.1f);
		int areaId = sel.GetComponent<DragArtWork> ().areaIndex;
		int artWorkId = sel.GetComponent<DragArtWork> ().artWorkIndex;
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
		//tooltipAddArt.gameObject.SetActive(false);
		Data.Instance.lastArtTexture = null;
		Data.Instance.LoadLevel("Galleries");
	}

    public void GotoGallery()
    {
       // tooltipAddArt.gameObject.SetActive(false);
		Data.Instance.lastArtTexture = null;
        Data.Instance.LoadLevel("Artworks");
    }

	public void EditWalls()
	{
		Data.Instance.lastArtTexture = null;
		Data.Instance.LoadLevel("Walls");
	}

	public void SaveDialog(){
		if (!Data.Instance.areaData.url.Equals ("")) {
			bool active = !saveDialog.gameObject.activeSelf;
			saveDialog.gameObject.SetActive (active);
			SetArtworkColliderActive(!active);
		} else {
			Ready (false);
		}
	}
    public void CloseSaveDialog()
    {
        EventsAnalytics.SendScreen("Save_Room");
        saveDialog.gameObject.SetActive(false);
    }
	void SetArtworkColliderActive(bool active){
		foreach (GameObject artwork in artworkList) {
			if(artwork!=null)artwork.GetComponent<MeshCollider>().enabled=active;
		}
	}

	public void Ready(bool isNew)
	{
        CloseSaveDialog();
        Events.OnLoading(true);
		if (isNew)
			Data.Instance.areaData.SetAsNew ();
		Data.Instance.SaveRoom(true);
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
			selected = "ArtWork_" + 0 + "_" + Data.Instance.areaData.areas[0].artworkCount;
			thumbClone = Instantiate(Thumb, Data.Instance.areaData.getPosition(0), Quaternion.identity) as GameObject;
			thumbClone.name = "thumb_"+selected;
			thumbRenderer = thumbClone.GetComponent<SpriteRenderer> ();
			Texture2D t = Data.Instance.lastArtThumbTexture;				
			thumbRenderer.sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.zero);
			Data.Instance.artData.selectedArtWork.setSizes();
			selectedArtwork = AddArt (0);
			selectedArtwork.transform.position = selectedArtwork.transform.position-new Vector3(0,0,0.1f);
			if(Input.mousePosition.y<Screen.height*0.2f&&!Input.GetButton("Fire1")){
				selected=null;
				Destroy(thumbClone);
			}
		}
	}

	void PlaceArt(int n){

        Events.HelpChangeStep(2);
       // tooltipAddArt.Stop();
       // buttonAddArt.GetComponent<Animation>().Stop();

		GameObject area = areas[n];
		if (Data.Instance.areaData.areas[n].artworks.Count > 0) {

			//tooltipAddArt.gameObject.SetActive(false);

			float aW = Data.Instance.areaData.areas[n].width;
			float aH = Data.Instance.areaData.areas[n].height;

			for (int i=0; i<Data.Instance.areaData.areas[n].artworks.Count; i++) {

				GameObject artWork = Instantiate (area.GetComponent<WallPlane> ().artWork, new Vector3 (0f, 0f, 0f), Quaternion.identity) as GameObject;
				artWork.name = "ArtWork_" + n +"_"+ Data.Instance.areaData.areas[n].artworkCount;
				artWork.transform.position = new Vector3(area.transform.position.x,area.transform.position.y,area.transform.position.z-0.01f);
				//artWork.transform.position = area.transform.position;
				artWork.transform.SetParent (area.transform);
				Vector3[] pointers = Data.Instance.areaData.getPointers(n);
				artWork.GetComponent<CustomPlane>().SetPointers(pointers);
				artWork.GetComponent<CustomPlane>().CustomMesh();
				artWork.GetComponent<DragArtWork> ().areaIndex = n;

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
				Renderer rend = artWork.GetComponent<Renderer> ();
				//rend.material.SetFloat("_Top", Vector3.Distance(pointers[1],pointers[3])>Vector3.Distance(pointers[2],pointers[0])?1f:0f);
				rend.material.SetFloat("_Left", Vector3.Distance(pointers[0],pointers[3])>Vector3.Distance(pointers[1],pointers[2])?1f:0f);
				rend.material.mainTexture = tex;

				int w = Data.Instance.areaData.areas[n].artworks[i].width;
				float aspect = 1f*Data.Instance.areaData.areas[n].artworks[i].texture.height/Data.Instance.areaData.areas[n].artworks[i].texture.width;
				w=w==0?defaultHeight:w;
				int h=(int)(w*aspect);
				//rend.material.SetTextureScale("_Tex"+area.GetComponent<WallPlane> ().artWorkNumber,new Vector2(0.5f*aW/w,0.5f*aH/h));
				rend.material.mainTextureScale = new Vector2(0.5f*aW/w,0.5f*aH/h);
				//area.GetComponent<Homography> ().SetHomography (artWork.name);
				rend.material.mainTextureOffset = Data.Instance.areaData.areas[n].artworks[i].position;

				Data.Instance.areaData.areas [n].artworks [area.GetComponent<WallPlane> ().artWorkNumber].id = Data.Instance.areaData.areas [n].artworkCount;

				artWork.GetComponent<DragArtWork> ().artWorkIndex= i;
				artWork.GetComponent<DragArtWork> ().artWorkID = Data.Instance.areaData.areas [n].artworks [i].galleryArtID;
				area.GetComponent<WallPlane> ().artWorkNumber++;
				Data.Instance.areaData.areas[n].artworkCount++;
				artworkList.Add(artWork);
			}

		}

		if (Data.Instance.lastArtTexture != null) {
			AddArt(n);					
		}
	}

	GameObject AddArt(int n){

		GameObject area = areas[n];
		float aW = Data.Instance.areaData.areas[n].width;
		float aH = Data.Instance.areaData.areas[n].height;
		
		GameObject artWork = Instantiate (area.GetComponent<WallPlane> ().artWork, new Vector3 (0f, 0f, 0f), Quaternion.identity) as GameObject;
		artWork.name = "ArtWork_" + n + "_" + Data.Instance.areaData.areas[n].artworkCount;
		artWork.transform.position = new Vector3(area.transform.position.x,area.transform.position.y,area.transform.position.z-0.01f-artworkList.Count*0.01f);
		//artWork.transform.position = area.transform.position;
		artWork.transform.SetParent (area.transform);
		Vector3[] pointers = Data.Instance.areaData.getPointers (n);
		artWork.GetComponent<CustomPlane>().SetPointers(pointers);
		artWork.GetComponent<CustomPlane>().CustomMesh();
		artWork.GetComponent<DragArtWork> ().areaIndex = n;

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
		//rend.material.SetFloat("_Top", Vector3.Distance(pointers[1],pointers[3])>Vector3.Distance(pointers[2],pointers[0])?1f:0f);
		rend.material.SetFloat("_Left", Vector3.Distance(pointers[0],pointers[3])>Vector3.Distance(pointers[1],pointers[2])?1f:0f);
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
		artWork.GetComponent<DragArtWork>().artWorkIndex = Data.Instance.areaData.areas [n].artworkCount;
		artWork.GetComponent<DragArtWork> ().artWorkID = Data.Instance.areaData.areas [n].artworks [Data.Instance.areaData.areas[n].artworks.Count-1].galleryArtID;
		Data.Instance.areaData.areas [n].artworks [area.GetComponent<WallPlane> ().artWorkNumber].id = Data.Instance.areaData.areas [n].artworkCount;
		area.GetComponent<WallPlane> ().artWorkNumber++;
		Data.Instance.areaData.areas [n].artworkCount++;
		artworkList.Add(artWork);
		return artWork;	
	}

	public void GetArtInfo(){
		/*if(Data.Instance.lastArtTexture==null){
			//int artWorkId = selectedArtwork.GetComponent<DragArtWork>().artWorkID;
			//int areaId = selectedArtwork.GetComponent<DragArtWork>().areaIndex;
			//AreaData.ArtWork aw = Data.Instance.areaData.areas[areaId].artworks.Find(x => x.id==artWorkId);
			//Data.Instance.SetLastArtTexture(aw.texture);
			for(int i=0;i<Data.Instance.areaData.areas.Count;i++){
				for(int j=0;j<Data.Instance.areaData.areas[i].artworks.Count;j++){
					Data.Instance.SetLastArtTexture(Data.Instance.areaData.areas[i].artworks[j].texture);
					sel_galleryID = Data.Instance.areaData.areas[i].artworks[j].galleryID;
					sel_galleryArtID = Data.Instance.areaData.areas[i].artworks[j].galleryArtID;
					j=Data.Instance.areaData.areas[i].artworks.Count;
					i=Data.Instance.areaData.areas.Count;
					break;
				}
			}
		}*/

		Data.Instance.artData.selectedGallery = sel_galleryID;
		Data.Instance.artData.SetSelectedArtworkByArtID (sel_galleryArtID);
		Data.Instance.isArtworkInfo2Place = false;
		Data.Instance.LoadLevel ("ConfirmArtWork");

	}

	void MoveButton(int moveId){
		int areaId = selectedArtwork.GetComponent<DragArtWork> ().areaIndex;
		int artWorkId = selectedArtwork.GetComponent<DragArtWork> ().artWorkIndex;

		Vector3 posDif = Vector3.zero;		
		if (moveId == 1) {//LEFT
			posDif = new Vector3(-moveStep,0f,0f);
		} else if (moveId == 2) {//RIGHT
			posDif = new Vector3(moveStep,0f,0f);
		} else if (moveId == 3) {//UP
			posDif = new Vector3(0f,moveStep,0f);
		} else if (moveId == 4) {//DOWN
			posDif = new Vector3(0f,-moveStep,0f);
		}

		Renderer rend = selectedArtwork.GetComponent<Renderer> ();
		rend.material.mainTextureOffset -= new Vector2(posDif.x,posDif.y);
		Data.Instance.areaData.areas [areaId].artworks.Find (x => x.id == artWorkId).position = rend.material.mainTextureOffset;
	}

	void OnDestroy()
	{
		Data.Instance.SetBackActive (true);
		Events.OnSelectFooterArtwork -= AddFromFooter;
		Events.ArtworkPreview -= Preview;
        Events.Back -= Back;
		Events.MoveButton -= MoveButton;
	}

}

