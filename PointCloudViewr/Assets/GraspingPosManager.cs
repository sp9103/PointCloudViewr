using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;

public class GraspingPosManager : MonoBehaviour {
	public string dataDir;
	public string dataPath;
	
	public float featureRadius = 1.0f;
	public float scale;
	
	public GameObject FeaturePrefab;
	
	private GameObject featureGroup;
	public List<GameObject> FeatureList; 

	public Color Featurecolor;
	public Vector3 shift;
	
	private int featureCount = 0;
	private bool isLoad;
	// Use this for initialization
	void Start () {
		isLoad = false;

	}
	
	// Update is called once per frame
	void Update () {
		if (!isLoad) {
			loadFeature ();
			isLoad = true;
		}
	}
	
	void loadFeature(){
		if (!File.Exists (Application.dataPath + "/Resources/"+dataDir+"/" + dataPath + ".txt")) {
			Debug.Log (Application.dataPath + "/Resources/"+dataDir+"/" + dataPath + ".txt not found");
			return;
		}
		
		TextReader reader = File.OpenText (Application.dataPath + "/Resources/" +dataDir+ "/" + dataPath  + ".txt");
		string line;

		//ground truth
		line = reader.ReadLine ();
		string[] bits = line.Split(' ');
		for(int i = 0; i < (bits.Length-1) / 3; i++){
			float f_x = float.Parse(bits[i*3 + 0]) * scale * 100.0f;
			float f_y = float.Parse(bits[i*3 + 1]) * scale * 100.0f;
			float f_z = float.Parse(bits[i*3 + 2]) * scale * 100.0f;
			
			Color tColor = Featurecolor;
			tColor.a = 1.0f;
			int r = 100;
			int g = 0;
			int b = 0;
			tColor.r = r / 100.0f;
			tColor.g = g / 100.0f;
			tColor.b = b / 100.0f;

			GameObject tempFeature = Instantiate(FeaturePrefab);
			tempFeature.transform.localScale = new Vector3(featureRadius, featureRadius, featureRadius);
			tempFeature.GetComponent<Renderer>().material.color = tColor;
			tempFeature.transform.parent = this.transform;
			tempFeature.transform.position = new Vector3(f_x, f_y, f_z) + shift;
		}

		//calcuate
		line = reader.ReadLine ();
		string[] gmm = line.Split(' ');
		float alphamax = -1.0f;
		float[] pos = new float[9];
		for (int i = 0; i < (gmm.Length-1) / 11; i++) {
			float alpha = float.Parse(gmm[11*i]);
			if(alphamax < alpha){
				alphamax = alpha;
				for(int j = 0; j < 9; j ++)
					pos[j] = float.Parse(gmm[11*i + 1 + j]);
			}
		}
		for(int i = 0; i < 3; i++){
			float f_x = pos[i*3 + 0] * scale * 100.0f;
			float f_y = pos[i*3 + 1] * scale * 100.0f;
			float f_z = pos[i*3 + 2] * scale * 100.0f;
			
			Color tColor = Featurecolor;
			tColor.a = 1.0f;
			int r = 0;
			int g = 0;
			int b = 100;
			tColor.r = r / 100.0f;
			tColor.g = g / 100.0f;
			tColor.b = b / 100.0f;

			GameObject tempFeature = Instantiate(FeaturePrefab);
			tempFeature.transform.localScale = new Vector3(featureRadius, featureRadius, featureRadius);
			tempFeature.GetComponent<Renderer>().material.color = tColor;
			tempFeature.transform.parent = this.transform;
			tempFeature.transform.position = new Vector3(f_x, f_y, f_z) + shift;
		}
	}
}
