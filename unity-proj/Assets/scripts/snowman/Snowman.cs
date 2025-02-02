﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Snowman : MonoBehaviour {

	private List<Transform> reachableRabbits;
	private Transform targetedRabbit;

	public Snowball snowball;
	private List<Snowball> snowballs;

	public int maxLife = 10;
	public int life = 10;

	public float fireRate = 1;
	private float fireTime = 0;
	public Transform launchPoint;

	bool mIsDead = false;
	bool isReady = false;
	bool isStarting = false;

	List<Transform> mRabbitToRemove;

	public Animation myAnim;

	public AudioSource audioSource;
	public AudioClip birthSound;
	public AudioClip launchSound;
	public float yToDestroy;
	public float meltSpeed;

	// Use this for initialization
	void Start ()
	{
		mRabbitToRemove = new List<Transform>();
		reachableRabbits = new List<Transform> ();
		snowballs = new List<Snowball> ();
		instantiateSnowballs ();
		StartCoroutine (StartAnimation ());
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(mIsDead) {
			transform.Translate(Vector3.down * meltSpeed);
			if(transform.position.y < yToDestroy)
				Destroy(gameObject);
			return;
		}
		if (isReady)
		{
			if (reachableRabbits.Count > 0)
			{
				targetedRabbit = getClosestRabbit();
				if (targetedRabbit != null)
				{
					fire ();
				}
			}
			else
			{
				targetedRabbit = null;
			}

			if (targetedRabbit != null)
			{
				Vector3 target = targetedRabbit.position;
				target.y = transform.position.y;
				transform.LookAt (target);
			}
		}
		else
		{
			if (isStarting)
			{
				if (!myAnim.isPlaying)
				{
					isReady = true;
				}
			}
		}
	}

	private void instantiateSnowballs()
	{
		for (int i = 0; i < 10; i++)
		{
			Snowball s = (Snowball)Instantiate(snowball);
			snowballs.Add(s);
			s.gameObject.SetActive(false);
		}
	}

	public void addRabbit(Transform _rabbitTrsf)
	{
		reachableRabbits.Add (_rabbitTrsf);
	}

	public void removeRabbit(Transform _rabbitTrsf)
	{
		reachableRabbits.Remove (_rabbitTrsf);
	}

	private Transform getClosestRabbit()
	{
		float minDistance = float.MaxValue;
		Transform rabbit = null;

		foreach (Transform trsf in reachableRabbits)
		{
			if(trsf == null){
				mRabbitToRemove.Add(trsf);
				continue;
			}

			BunnyAI ai = trsf.gameObject.GetComponent<BunnyAI>();
			if(ai.IsDead()){
				mRabbitToRemove.Add(ai.transform);
				continue;
			}

			if(!ai.IsAngry()) continue;

			float distance = Vector3.Distance(transform.position, trsf.position);
			if (distance < minDistance)
			{
				minDistance = distance;
				rabbit = trsf;
			}
		}

		while(mRabbitToRemove.Count > 0)
			mRabbitToRemove.RemoveAt(0);

		return rabbit;
	}

	private void fire()
	{
		fireTime += Time.deltaTime;

		if (fireTime >= fireRate)
		{
			fireTime = 0;
			throwSnowball();
		}
	}

	private void throwSnowball()
	{
		Snowball snowball = snowballs [0];
		snowballs.Remove (snowball);

		snowball.transform.position = launchPoint.position;
		snowball.transform.LookAt (targetedRabbit);
		snowball.gameObject.SetActive (true);
		snowball.Launch (this);

		myAnim.Play ("attack");
		audioSource.clip = launchSound;
		audioSource.Play ();
	}

	public void takeBackSnowball(Snowball _snowball)
	{
		snowballs.Add (_snowball);
	}

	public void TakeDamage(int amount){
		life -= amount;
		if(life <= 0){
			Die();
		}
	}

	void Die(){
		mIsDead = true;
		myAnim.Play("death");
	}

	public bool IsDead() {
		return mIsDead;
	}

	private IEnumerator StartAnimation()
	{
		transform.Translate (-Vector3.up * 11.5f);

		yield return new WaitForSeconds(1.0f);
		
		transform.Translate (Vector3.up * 11.5f);
		myAnim.Play ("grow");
		isStarting = true;
		audioSource.clip = birthSound;
		audioSource.Play ();
	}
}
