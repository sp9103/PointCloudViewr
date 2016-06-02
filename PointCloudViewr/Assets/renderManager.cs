using UnityEngine;
using System.Collections;

public class renderManager : MonoBehaviour {
	public GameObject GraspingPosCNN;
	public GameObject Feature;
	public GameObject GraspingPos;
	public GameObject PointCloud;
	public GameObject GraspingPosPre;

	public string FileName;

	// Use this for initialization
	void Start () {
		GraspingPosCNN.GetComponent<GraspingCNNPosManager> ().dataPath = FileName;
		Feature.GetComponent<FeatureManager> ().dataPath = FileName;
		GraspingPos.GetComponent<GraspingPosManager> ().dataPath = FileName;
		PointCloud.GetComponent<PointCloudSquare> ().dataPath = FileName;
		GraspingPosPre.GetComponent<GraspingCNNPosManager> ().dataPath = FileName;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
