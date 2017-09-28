using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyZombie : MonoBehaviour {

	NavMeshAgent agent;
	Animator anim;

	//referente to the chair where the player is sitting
	public GameObject chair;

	Slider healthSlider;

	int zombieHealth = 3;

	float attackCooldown = 2;

	bool attackTrigger;
	bool isDead;



	void Start () {
		agent = GetComponent<NavMeshAgent> ();
		anim = GetComponent<Animator> ();
		healthSlider = GetComponentInChildren<Slider> ();

		agent.SetDestination (chair.transform.position);

		healthSlider.maxValue = zombieHealth;
		healthSlider.value = zombieHealth;
	}
	



	void Update () {

		//When the zombie reaches a certain distance of the player, start attacking
		if (Vector3.Distance (this.gameObject.transform.position, chair.transform.position) < 2.5f && !attackTrigger) {
			agent.isStopped = true;
			attackTrigger = true;
			StartCoroutine (AttackPattern ());
		}

	}


	//When receiving damage, updates the health bar and stops his movement if he's dead
	public void Damage(int damage)
	{
		zombieHealth -= damage;
		healthSlider.value = zombieHealth;

		if (zombieHealth <= 0) {
			agent.isStopped = true;
			anim.SetTrigger ("Dead");
		}
	}


	//Keep looping the zombie attack until he is killed
	IEnumerator AttackPattern()
	{
		anim.SetTrigger ("Attack");
		GameManager.instance.DamageTaken (); //Player took damage
		yield return new WaitForSeconds (attackCooldown);

		//If the zombie is not dead, repeat the attack
		if(!isDead)
			StartCoroutine (AttackPattern ());
	}
}
