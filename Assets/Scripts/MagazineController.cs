using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This method controls the hand which will pick up magazines to reload the gun
public class MagazineController : MonoBehaviour {

	private SteamVR_TrackedObject trackedObj;
	private SteamVR_Controller.Device device { get { return SteamVR_Controller.Input ((int)trackedObj.index);  }}

	public GameObject controllerModel;
	MeshRenderer meshRend;

	bool hasMagazine;


	void Start () {
		trackedObj = this.gameObject.GetComponentInParent<SteamVR_TrackedObject> ();
		meshRend = GetComponent<MeshRenderer> ();
		//Set the mesh renderer to false so the magazine is not appearing
		meshRend.enabled = false;
		controllerModel.SetActive (true);
	}


	void OnTriggerEnter(Collider col)
	{
		//If the controller collides with the magazines trigger, hide the controller model and show the magazine
		if (col.tag == "Magazine") {
			meshRend.enabled = true;
			controllerModel.SetActive (false);
			hasMagazine = true;
			device.TriggerHapticPulse (3000); //Rumble feedback in case the player gets a magazine without looking
		}

		//If the player collides the magazine with the pistol, it will be reloaded
		if (col.tag == "Pistol") {
			if (hasMagazine) {
				meshRend.enabled = false;
				controllerModel.SetActive (true);
				col.GetComponentInParent<GunController> ().ReloadPistol ();
				hasMagazine = false;
			}
		}
	}

}
