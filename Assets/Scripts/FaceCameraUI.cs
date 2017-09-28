using UnityEngine;
using System.Collections;

public class FaceCameraUI : MonoBehaviour
{

	Camera referenceCamera;

	void  Awake()
	{
		// if no camera referenced, grab the main camera
		if (!referenceCamera)
			referenceCamera = Camera.main; 
	}


	//Keep UI components (like headshot icons, enemy reticle, etc) looking at the camera
	void  Update()
	{
		transform.LookAt (referenceCamera.transform.position);
	}
}
