using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using DG.Tweening;

public class Enemy : MonoBehaviour {

	NavMeshAgent agent;

	Camera referenceCamera;

	Animator anim;

	SkinnedMeshRenderer enemyMaterial;

	public GameObject reticleCanvas;

	public Transform movePos;

	public Image reticleImage;

	public Slider healthSlider;

	float duration = 5;
	float timerToShootPlayer = 0;
	public float shootTime = 1;

	public int order;
	public int wave;
	public int tankHealth = 6;

	bool rotateToCamera;
	[HideInInspector]
	public bool isDead;
	public bool isTank;

	//Allow changing the behaviour of the enemy placed in the scene with a dropdown menu
	public enum EnemyBehaviour
	{
		crouched,
		moving
	}

	public EnemyBehaviour behaviour;



	void Start () {

		//Get the main camera, which is the player viewpoint
		if (!referenceCamera)
			referenceCamera = Camera.main; 

		agent = GetComponent<NavMeshAgent> ();

		anim = GetComponent<Animator> ();

		enemyMaterial = GetComponentInChildren<SkinnedMeshRenderer> ();

		//When the enemy spawns, if his behaviour is crouched then he gets up
		if (behaviour == EnemyBehaviour.crouched) {
			anim.SetTrigger ("GetUp");
		}

		//When the enemy spawns, if his behaviour is crouched then he gets up
		if (behaviour == EnemyBehaviour.moving) {
			agent.SetDestination (movePos.position);
		}

		//If the enemy is set as a tank type enemy in the inspector
		if (isTank) {
			healthSlider.maxValue = tankHealth;
			healthSlider.value = tankHealth;
		}

	}
	


	void Update () {

		//Makes the enemy play the running or idle animation depending on his current velocity
		anim.SetFloat ("Velocity", agent.velocity.magnitude);

		//If the enemy reaches his destination, aim at the player
		if (agent.remainingDistance <= 0.1f) {
			anim.SetBool ("Aiming", true);
			Vector3 targetPostition = new Vector3 (referenceCamera.transform.position.x, this.transform.position.y, referenceCamera.transform.position.z);
			this.transform.DOLookAt(targetPostition, 0.2f);
		}
			
		if (timerToShootPlayer < shootTime && !isDead) { 
			ColorChanger ();
		} else {
			if (!isDead)
				GameManager.instance.DamageTaken ();
		}

		//If the enemy is dead, remove his body when the player isn't looking at it
		if (isDead) {
			if (!enemyMaterial.isVisible)
				Destroy (this.gameObject);
		}
				
	}


	//Change the color of the reticle around the enemy to red as time passes
	void ColorChanger()
	{
		reticleImage.color = Color.Lerp(Color.blue, Color.red, timerToShootPlayer);
		timerToShootPlayer += Time.deltaTime/duration;
	}


}
