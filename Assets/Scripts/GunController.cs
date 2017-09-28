using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GunController : MonoBehaviour {

	public List<GameObject> bullets = new List<GameObject> ();
	public GameObject muzzleFlashObject;
	public GameObject hitMarker;
	public GameObject wallHitMarker;
	public GameObject headshotIcon;
	public GameObject reloadCanvas;

	public LineRenderer lineRend;

	private SteamVR_TrackedObject trackedObj;
	private SteamVR_Controller.Device device { get { return SteamVR_Controller.Input ((int)trackedObj.index);  }}

	private SteamVR_TrackedController controller;

	public GameObject bullet;

	public Animator reloadAnim;

	public Transform muzzleTransform;

	AudioSource source;
	public AudioClip gunfireSound;
	public AudioClip reloadSound;

	public float timeBetweenShots = 0.1f;
	float muzzleFlashTimer;
	float muzzleFlashTime = 0.1f;
	float raycastDistance = 5000;

	int bulletsLeft = 6;

	bool weaponWasShot;


	void Start () {
		//Get the SteamVR components
		controller = this.gameObject.GetComponent<SteamVR_TrackedController> ();
		controller.TriggerClicked += TriggerPressed;
		trackedObj = this.gameObject.GetComponent<SteamVR_TrackedObject> ();

		source = GetComponent<AudioSource> ();

		lineRend.SetPosition (0, muzzleTransform.position);
		lineRend.SetPosition (1, muzzleTransform.position);
		lineRend.enabled = false;
	}




	void Update () {

		//if the gun is shot, the muzzle flash is shown for a period of time
		if (weaponWasShot) 
		{
			muzzleFlashTimer += Time.deltaTime;

			if (muzzleFlashTimer >= muzzleFlashTime) {
				muzzleFlashObject.SetActive (false);
				muzzleFlashTimer = 0;
				weaponWasShot = false;
			}
		}



	}



	private void TriggerPressed(object sender, ClickedEventArgs e)
	{
		if (bulletsLeft >= 0) 
		{
			weaponWasShot = true;
			bullets [bulletsLeft].SetActive (false);
			bulletsLeft--;
			ShootWeapon ();

			if (bulletsLeft < 0) 
			{
				reloadCanvas.SetActive (true);
				SoundManager.instance.PlayReloadSound ();
			}
		}
	}


	//este metodo vai cuidar da arma atirando com raycasting e colisao com as frutas
	public void ShootWeapon()
	{
		
		RaycastHit hit = new RaycastHit ();
		Ray ray = new Ray (muzzleTransform.position, muzzleTransform.forward);
		source.clip = gunfireSound;
		source.Play ();

		//Rumble the controller
		device.TriggerHapticPulse (2000);

		ShowMuzzleFlash();

		if (Physics.Raycast (ray, out hit, raycastDistance)) {

			//If the shot hits a wall, show the wall hit marker (which is blue instead of red)
			if (hit.collider.tag == "Wall") {
				Destroy (Instantiate (wallHitMarker, hit.point, Quaternion.identity), 2);
				//Instantiate a bullet at the wall to show where the shot hit
				Instantiate (bullet, hit.point, Quaternion.identity);
			}


			if (hit.collider.tag == "Target") {
				Animator targetAnim = hit.collider.gameObject.GetComponentInParent<Animator> ();
				targetAnim.SetTrigger ("Shot");
				hit.collider.enabled = false;
				Destroy (Instantiate (hitMarker, hit.point, Quaternion.identity), 2);
				GameObject shot = Instantiate (bullet, hit.point, Quaternion.identity) as GameObject;
				shot.transform.parent = hit.collider.transform;
			}

			//If the object shot has a rigidbody, or in case, is an enemy
			if (hit.rigidbody != null) {
				ManageEnemyHit(hit, ray);
			}

			//Draw a line renderer from the gun to the shot area
			StartCoroutine (ShowShotLine (hit.point));

		} else {
			//If the player shoots at nothing, like the sky
			Vector3 aimDistance = ray.origin + ray.direction * 100;
			//Draws a line renderer to a certain distance
			StartCoroutine (ShowShotLine (aimDistance));
		}

		Debug.DrawRay (ray.origin, hit.point, Color.red);
	}



	void ManageEnemyHit(RaycastHit hit, Ray ray)
	{
		//find the RagdollHelper component and activate ragdolling
		RagdollHelper helper = hit.collider.GetComponentInParent<RagdollHelper> ();

		StairDismount stDis = hit.collider.GetComponentInParent<StairDismount> ();

		Enemy enemyHit = hit.collider.gameObject.GetComponentInParent<Enemy> ();

		if (!enemyHit.isTank) {
			helper.ragdolled = true;
			enemyHit.reticleCanvas.SetActive (false);
			if (!enemyHit.isDead) {
				if (enemyHit.order == 0)
					GameManager.instance.EnemyDied ();
			}
			enemyHit.isDead = true;

			//set the impact target to whatever the ray hit
			stDis.impactTarget = hit.rigidbody;

			//impact direction also according to the ray
			stDis.impact = ray.direction * 2.0f;

			//the impact will be reapplied for the next 250ms
			//to make the connected objects follow even though the simulated body joints
			//might stretch
			stDis.impactEndTime = Time.time + 0.25f;

			//Show a hit marker where the enemy was shot
			Destroy (Instantiate (hitMarker, hit.point, Quaternion.identity), 2);

			//Show an headshot icon when the player hits the enemy's head
			if (hit.collider.tag == "Face") {
				Destroy (Instantiate (headshotIcon, hit.point, Quaternion.identity), 2);
			}
		} else {
			enemyHit.tankHealth -= 1;
			enemyHit.healthSlider.value = enemyHit.tankHealth;

			if (enemyHit.tankHealth <= 0) {
				helper.ragdolled = true;
				enemyHit.reticleCanvas.SetActive (false);
				if (!enemyHit.isDead) {
					if (enemyHit.order == 0)
						GameManager.instance.EnemyDied ();
				}
				enemyHit.isDead = true;

				//set the impact target to whatever the ray hit
				stDis.impactTarget = hit.rigidbody;

				//impact direction also according to the ray
				stDis.impact = ray.direction * 2.0f;

				//the impact will be reapplied for the next 250ms
				//to make the connected objects follow even though the simulated body joints
				//might stretch
				stDis.impactEndTime = Time.time + 0.25f;

				if (hit.collider.tag == "Face") {
					Destroy (Instantiate (headshotIcon, hit.point, Quaternion.identity), 2);
				}
			}

			Destroy (Instantiate (hitMarker, hit.point, Quaternion.identity), 2);

		}
	}



	public void ReloadPistol()
	{
		source.clip = reloadSound;
		source.Play ();
		reloadAnim.SetTrigger ("Reload");
		foreach (GameObject bullet in bullets)
			bullet.SetActive (true);

		bulletsLeft = 6;
		reloadCanvas.SetActive (false);
	}



	//Show the muzzle flash and randomly rotate it in the Z position
	void ShowMuzzleFlash()
	{
		muzzleFlashObject.SetActive (true);
		Vector3 muzzlePos = new Vector3(muzzleFlashObject.transform.rotation.x, muzzleFlashObject.transform.rotation.y, Random.Range(0,360));
		muzzleFlashObject.transform.Rotate (muzzlePos);
		muzzleFlashTimer = 0;
	}



	IEnumerator ShowShotLine(Vector3 hitPoint)
	{
		lineRend.enabled = true;
		lineRend.SetPosition (0, muzzleTransform.position);
		lineRend.SetPosition (1, hitPoint);

		yield return new WaitForSeconds (0.1f);

		lineRend.enabled = false;
	}

		

//	//
//	IEnumerator SequentialShots()
//	{
//		while (controller.triggerPressed) {
//			ShootWeapon ();
//			yield return new WaitForSeconds (timeBetweenShots);
//		}
//	}
}
	
