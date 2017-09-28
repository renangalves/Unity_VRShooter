using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	//An instance of GamaManager is created to be easily accessed by other scripts
	//Generally I use only one instance in the most important class, which is the GameManager, to avoid problems with other static members
	public static GameManager instance
	{
		get
		{
			if (_instance == null) {
				_instance = FindObjectOfType<GameManager> ();
			}

			return _instance;
		}
	}

	private static GameManager _instance;

	public List<GameObject> hearts = new List<GameObject> ();

	public GameObject warriorEnemy;

	SequenceManager seqManager;

	public Transform enemySpawnPos;

	public Animator damageAnim;

	int playerHealth = 3;
	[HideInInspector]
	public int currentWaveEnemiesLeft;

	float invulnerableTimer = 2;

	bool isInvulnerable;

	
	void Start ()
	{
		seqManager = FindObjectOfType<SequenceManager> ();
	}

	void Update()
	{
		//For testing purposes
		if (Input.GetKeyDown (KeyCode.LeftControl)) 
		{
			Instantiate (warriorEnemy, enemySpawnPos.position, Quaternion.identity);
		}

		//Reset scene
		if (Input.GetKeyDown (KeyCode.Space)) 
		{
			SceneManager.LoadScene ("GameScene");
		}
	}


	//This method is called by other classes when damage is taken by the player
	public void DamageTaken()
	{
		if (!isInvulnerable) 
		{
			playerHealth--;

			switch (playerHealth) 
			{
			case 2:
				hearts [2].SetActive (false);
				break;
			case 1:
				hearts [1].SetActive (false);
				break;
			case 0:
				hearts [0].SetActive (false);
				break;
			}
				
			isInvulnerable = true;
			damageAnim.SetTrigger ("Damage");
			StartCoroutine (InvulnerableTimer ());
		}
	}
		

	IEnumerator InvulnerableTimer()
	{
		yield return new WaitForSeconds (invulnerableTimer);
		isInvulnerable = false;
	}


	//When an enemy dies, the GameManager keeps track of how many enemies are left to proceed to the next wave
	//This method is called by other classes
	public void EnemyDied()
	{
		currentWaveEnemiesLeft--;
		if (currentWaveEnemiesLeft == 0) 
		{
			StartCoroutine (seqManager.NextWaveTrigger ());
		}
	}
}
