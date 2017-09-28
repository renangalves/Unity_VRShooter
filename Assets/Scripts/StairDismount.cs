using UnityEngine;
using System.Collections;

public class StairDismount : MonoBehaviour {
	//Declare a member variables for distributing the impacts over several frames
	[HideInInspector]
	public float impactEndTime=0;
	[HideInInspector]
	public Rigidbody impactTarget=null;
	[HideInInspector]
	public Vector3 impact;
	//Current score
	public int score;
	//A prefab for displaying points (floats up, fades out, instantiated by the RagdollPartScript)
	public GameObject scoreTextTemplate;


	
	void Update () {
		//Check if we need to apply an impact
		if (Time.time<impactEndTime)
		{
			impactTarget.AddForce(impact,ForceMode.VelocityChange);
		}
	}
}
