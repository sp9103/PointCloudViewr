using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;

public class FeatureManager : MonoBehaviour {
	public string dataDir;
	public string dataPath;

	public float featureRadius = 1.0f;
	public float scale;

	public GameObject FeaturePrefab;

	private GameObject featureGroup;
	public List<GameObject> FeatureList; 

	public bool isRand;
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

		TextReader reader = File.OpenText (Application.dataPath + "/Resources/"+dataDir+"/" + dataPath  + ".txt");
		string line;
		while ((line = reader.ReadLine()) != null) {
			string[] bits = line.Split(' ');
			for(int i = 0; i < bits.Length / 3; i++){
				float f_x = float.Parse(bits[i*3 + 0]) * scale;
				float f_y = float.Parse(bits[i*3 + 1]) * scale;
				float f_z = float.Parse(bits[i*3 + 2]) * scale;

				featureCount++;
				
				Color tColor = Featurecolor;
				if(isRand){
					tColor.a = 1.0f;
					int r = ((featureCount*7) % 100);
					int g = ((featureCount*37) % 100);
					int b = ((featureCount*103) % 100);
					tColor.r = r / 100.0f;
					tColor.g = g / 100.0f;
					tColor.b = b / 100.0f;
				}
				GameObject tempFeature = Instantiate(FeaturePrefab);
				tempFeature.transform.localScale = new Vector3(featureRadius, featureRadius, featureRadius);
				tempFeature.GetComponent<Renderer>().material.color = tColor;
				tempFeature.transform.parent = this.transform;
				tempFeature.transform.position = new Vector3(f_x, f_y, f_z) + shift;
			}
		}
	}
}
