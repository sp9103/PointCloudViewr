using UnityEngine;
using System;
using System.Collections;
using System.IO;

public class PointCloudSquare : MonoBehaviour {

	// File
	public GameObject vertex;
	public string dataPath;
	private string filename;

	// GUI
	private float progress = 0;
	private string guiText;
	private bool loaded = false;

	// PointCloud
	private GameObject pointCloud;

	public float scale = 1;
	public bool invertYZ = false;
	public bool forceReload = false;
	public bool textureload = false;
	public bool isTransform = false;

	public int numPoints;
	public int numPointGroups;
	public float vertexScale;
	private int limitPoints = 65000;

	private Vector3[] points;
	private Color[] colors;
	private Vector3 minValue;

	private bool isTexture = false;
	private Texture2D texture = null;

	private float[] x_tran = {5.455197f, 978.384766f, 291.861450f, 60.463535f};
	private float[] y_tran = {985.812805f, -4.218239f, 177.40733f, -214.983505f};
	private float[] z_tran = {204.117462f, 223.972076f, -903.639038f, 877.720520f};

	void Start () {

		// Get Filename
		filename = Path.GetFileName(dataPath);

		loadScene ();
	}



	void loadScene(){
		// Check if the PointCloud was loaded previously
		if(!Directory.Exists (Application.dataPath + "/Resources/PointCloudMeshes/" + filename)){
			loadPointCloud ();
		} else if (forceReload){
			loadPointCloud ();
		} 
	}
	
	
	void loadPointCloud(){
		Debug.Log ("file path : " + Application.dataPath + "/Resources/Binary/" + dataPath);
		if(File.Exists(Application.dataPath + "/Resources/Binary/"+dataPath + ".bin"))
		   StartCoroutine("loadOFF", dataPath);
		else 
			Debug.Log ("File '" + dataPath + "' could not be found"); 
		
	}
	
	// Start Coroutine of reading the points from the OFF file and creating the meshes
	IEnumerator loadOFF(string dPath){

		// Read file
		FileStream sr = new FileStream (Application.dataPath + "/Resources/Binary/" + dataPath  + ".bin", FileMode.Open);
		BinaryReader br = new BinaryReader(sr);
		int width = br.ReadInt32 ();
		int height = br.ReadInt32 ();
		int Type = br.ReadInt32 ();
		int dataCount = 0;

		Debug.Log ("width : " + width);
		Debug.Log ("Height : " + height);

		numPoints = width * height;
		points = new Vector3[numPoints];
		colors = new Color[numPoints];
		minValue = new Vector3();

		int datasize = width * height * 3 * sizeof(float);
		byte[] temp = new byte[datasize];
		temp = br.ReadBytes (datasize);

		GameObject pointGroup = new GameObject (filename);
		pointCloud = new GameObject (filename);

		//Texture mapping
		if (textureload) {
			Debug.Log("Texture path : " + Application.dataPath + "/Resources/Binary/" + dataPath + ".jpg");
			if (File.Exists (Application.dataPath + "/Resources/Binary/" + dataPath + ".jpg")) {
				isTexture = true;
				WWW www = new WWW("file://" + Application.dataPath + "/Resources/Binary/" + dataPath + ".jpg");
				texture = new Texture2D(160, 160, TextureFormat.ARGB32, false);
				while(true)
					if(www.isDone)	break;
				
				Debug.Log("Texture load complete!");
				www.LoadImageIntoTexture(texture);
			}
		}

		for (int i = 0; i< numPoints; i++){
			//buffer = sr.ReadLine ().Split ();
			float x = BitConverter.ToSingle(temp, 4*3*i + 0*4);
			float y = BitConverter.ToSingle(temp, 4*3*i + 1*4);
			float z = BitConverter.ToSingle(temp, 4*3*i + 2*4);

			if( z <= 0){
				continue;
			}

			if(isTransform){
				float t_x = (x * x_tran[0] + y * x_tran[1] + z * x_tran[2] + x_tran[3]);
				float t_y = (x * y_tran[0] + y * y_tran[1] + z * y_tran[2] + y_tran[3]);
				float t_z = (x * z_tran[0] + y * z_tran[1] + z * z_tran[2] + z_tran[3]);

				x = t_x; y = t_y; z = t_z;
			}

			if (!invertYZ)
				points[i] = new Vector3 (x*scale, y*scale,z*scale) ;
			else
				points[i] = new Vector3 (x*scale, z*scale,y*scale) ;

			colors[i] = Color.cyan;

			GameObject tempVertex = Instantiate(vertex);
			tempVertex.transform.position = points[i];
			tempVertex.transform.parent = pointCloud.transform;
			tempVertex.transform.localScale = new Vector3(vertexScale, vertexScale, vertexScale);

			if(textureload && isTexture){
				tempVertex.GetComponent<Renderer>().material.color = texture.GetPixel(i % width, width - (i / width));
			}
			// GUI
			progress = i *1.0f/(numPoints-1)*1.0f;
			if (i%Mathf.FloorToInt(numPoints/20) == 0){
				guiText=i.ToString() + " out of " + numPoints.ToString() + " loaded";
				yield return null;
			}

			dataCount++;
		}
		pointGroup.transform.parent = pointCloud.transform;

		numPoints = dataCount;

		loaded = true;
	}

	void OnGUI(){


		if (!loaded){
			GUI.BeginGroup (new Rect(Screen.width/2-100, Screen.height/2, 400.0f, 20));
			GUI.Box (new Rect (0, 0, 200.0f, 20.0f), guiText);
			GUI.Box (new Rect (0, 0, progress*200.0f, 20), "");
			GUI.EndGroup ();
		}
	}

}
