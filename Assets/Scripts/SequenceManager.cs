using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SequenceManager : MonoBehaviour {

	public List<Transform> posTriggers = new List<Transform> ();
	public List<Sequence> sequence = new List<Sequence> ();

	public class Sequence
	{
		public List<GameObject> sequenceEnemies = new List<GameObject>();
		public int order;
	}

	public List<Enemy> enemies = new List<Enemy> ();

	public NavMeshAgent playerAgent;
//
	public int posCount = 0;
	int enemyWave = 0;
	int enemiesLeft = 0;

	bool waveStarted;
	bool movementStart;


	
	void Start () {

		int i = -1;
		foreach (Enemy enemy in enemies) {
			if (i != enemy.wave) {
				Sequence newWave = new Sequence ();
				newWave.sequenceEnemies.Add (enemy.gameObject);
				newWave.order = enemy.order;
				sequence.Add (newWave);
				i = enemy.wave;
			} else {
				sequence [i].sequenceEnemies.Add (enemy.gameObject);
			}
		}

	}
	
	
	void Update () {

        //Start the game
		if (Input.GetKeyDown (KeyCode.S))
			movementStart = true;

        //Moves the player to the destination
		if(movementStart)
			playerAgent.SetDestination (posTriggers [posCount].position);

        //When the player reaches the destination, checks if the next wave if ordered or not
		if (playerAgent.remainingDistance <= 0 && !waveStarted && movementStart) {
            //If it's not ordered, then spawn all enemies of the current wave
			if (sequence [enemyWave].order == 0) {
				foreach (GameObject enemy in sequence[enemyWave].sequenceEnemies) {
					enemy.SetActive (true);
					GameManager.instance.currentWaveEnemiesLeft++;
				}
			} else {
                //If it's ordered
				StartCoroutine (WaveSequence ());
			}
			waveStarted = true;
		}

	}


    //Spawn enemies in order. As the previous enemy dies a new one spawns, until the list for that wave is over
    IEnumerator WaveSequence()
    {
        GameObject currentEnemy = sequence [enemyWave].sequenceEnemies [0];
        currentEnemy.SetActive (true);

        //Loops through the list of enemies of the current wave. When an enemy dies move to the next enemy, or leave the loop when the list is empty
        while (sequence [enemyWave].sequenceEnemies.Count > 0) {
            if (currentEnemy.GetComponent<Enemy> ().isDead) {
                sequence [enemyWave].sequenceEnemies.RemoveAt (0);
                if(sequence [enemyWave].sequenceEnemies.Count > 0)
                    currentEnemy = sequence [enemyWave].sequenceEnemies [0];
                currentEnemy.SetActive (true);
            }
            //keep this while loop having 0.1 second intervals to avoid excessive amounts of looping for performance
            yield return new WaitForSeconds (0.1f);
        }

        //When the wave ends, go to the next wave
        StartCoroutine (NextWaveTrigger ());
    }



    
	public IEnumerator NextWaveTrigger()
	{
		waveStarted = false;
		posCount++;
		playerAgent.SetDestination (posTriggers [posCount].position); //Set the next destination for the player to move
		playerAgent.isStopped = true;
		if(enemyWave < sequence.Count)
			enemyWave++;

		yield return new WaitForSeconds (3f); //Give the player a little breather before moving to the next wave

		playerAgent.isStopped = false;
	}


	
}
