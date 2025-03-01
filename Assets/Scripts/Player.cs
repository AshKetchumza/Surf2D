﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using GoogleMobileAds.Api;

public class Player : MonoBehaviour {

	// public float dir;					//Initial direction is set to 0 on start so the player doesn't move until he taps
	// public float speed = 0.03f;			//Speed for the surfer
	// public float dec = 0.015f;			//Deceleration speed
	// public float acc = 0.015f;			//Acceleration speed
	// public float accTime = 0.4f;		//Acceleration init time
	// public float moveTime = 0.8f;		//Max speed init time

	//private bool isDead = false;		//Used for game over 
	// private Animator animate;			//Access the animator for the player

	//variables
	public float moveSpeed = 300;
	public GameObject character;

	private Rigidbody2D characterBody;
	private float ScreenWidth;

	public AudioSource source;
	public AudioClip explosion;
	public AudioClip collect;
	public AudioClip speedboost;

	public Sprite orangeBoat;
	public Sprite greenBoat;
	public Sprite limeBoat;
	public Sprite pinkBoat;
	public Sprite blackBoat;
	private int boatChoice;

	private InterstitialAd pauseIntersticial;
	
	// Use this for initialization
	void Start () {
		// //Set dir to 0 on initialise
		// dir = 0f;
		// //Get the animator from the player on initialise
		// animate = GetComponent<Animator> ();

		// ScreenWidth = Screen.width;
		// widthRel = character.transform.localScale.x / ScreenWidth;

		characterBody = character.GetComponent<Rigidbody2D>();
		AudioSource[] audioSources = GetComponents<AudioSource>();
		source = audioSources[0];
		explosion = audioSources[0].clip;
		collect = audioSources[1].clip;
		speedboost = audioSources[2].clip;
		if(PlayerPrefs.HasKey("boat")){
			boatChoice = PlayerPrefs.GetInt("boat");
			if(boatChoice==0){
				characterBody.GetComponent<SpriteRenderer>().sprite = orangeBoat;
			} else if(boatChoice==1){
				characterBody.GetComponent<SpriteRenderer>().sprite = greenBoat;
			} else if(boatChoice==2){
				characterBody.GetComponent<SpriteRenderer>().sprite = limeBoat;
			} else if(boatChoice==3){
				characterBody.GetComponent<SpriteRenderer>().sprite = pinkBoat;
			} else if(boatChoice==4){
				characterBody.GetComponent<SpriteRenderer>().sprite = blackBoat;
			}
		} else {
			characterBody.GetComponent<SpriteRenderer>().sprite = blackBoat;
		}
		//explosion = GetComponent<AudioSource> ();
		#if UNITY_ANDROID
            string appId = "ca-app-pub-3541577481831776~2365521691";
        #elif UNITY_IPHONE
            string appId = "unexpected_platform";
        #else
            string appId = "unexpected_platform";
        #endif

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(appId);	
	}	

	private void RequestPauseInterstitial()
	{
		#if UNITY_ANDROID
			string adUnitId = "ca-app-pub-3541577481831776/7973071593";
		#elif UNITY_IPHONE
			string adUnitId = "unexpected_platform";
		#else
			string adUnitId = "unexpected_platform";
		#endif

		// Initialize an InterstitialAd.
		this.pauseIntersticial = new InterstitialAd(adUnitId);
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().Build();
		// Load the interstitial with the request.
		this.pauseIntersticial.LoadAd(request);
	}

	// Update is called once per frame
	void Update () {				

		// Pause the game
		if (GameManager.instance.hasBegun == true && GameManager.instance.gameOver == false && GameManager.instance.isPaused == false && Input.GetKeyDown (KeyCode.Escape)) {
			GameManager.instance.isPaused = true;
			characterBody.velocity = new Vector2(0f,0f);
			GameManager.instance.pauseText.SetActive (true);
			GameManager.instance.pauseButton.SetActive (false);
			GameManager.instance.leftButton.SetActive (false);
			GameManager.instance.rightButton.SetActive (false);
		}

		// If the game is paused, freeze player
		// if (GameManager.instance.isPaused == true && dir == -speed) {
		// 	this.transform.Translate (0, 0, 0);
		// } else if (GameManager.instance.isPaused == true && dir == speed) {
		// 	this.transform.Translate (0, 0, 0);
		// }
		// if (GameManager.instance.isPaused == true) {
		// 	this.transform.Translate (0, 0, 0);
		// }		

	}

	// public void accLeft(){
	// 	dir = -acc;
	// 	animate.SetTrigger ("Left");
	// }

	// public void moveLeft(){
	// 	dir = -speed;
	// }

	// public void accRight(){
	// 	dir = acc;
	// 	animate.SetTrigger ("Right");
	// }

	// public void moveRight(){
	// 	dir = speed;
	// }

	public void ChooseOrangeBoat(){
		PlayerPrefs.SetInt("boat", 0);
		characterBody.GetComponent<SpriteRenderer>().sprite = orangeBoat;
	}
	public void ChooseGreenBoat(){
		PlayerPrefs.SetInt("boat", 1);
		characterBody.GetComponent<SpriteRenderer>().sprite = greenBoat;
	}
	public void ChooseLimeBoat(){
		PlayerPrefs.SetInt("boat", 2);
		characterBody.GetComponent<SpriteRenderer>().sprite = limeBoat;
	}
	public void ChoosePinkBoat(){
		PlayerPrefs.SetInt("boat", 3);
		characterBody.GetComponent<SpriteRenderer>().sprite = pinkBoat;
	}
	public void ChooseBlackBoat(){
		PlayerPrefs.SetInt("boat", 4);
		characterBody.GetComponent<SpriteRenderer>().sprite = blackBoat;
	}


	public void pause(){
		GameManager.instance.isPaused = true;
		GameManager.instance.pauseText.SetActive (true);
		GameManager.instance.pauseButton.SetActive (false);
		GameManager.instance.leftButton.SetActive (false);
		GameManager.instance.rightButton.SetActive (false);
		if (this.pauseIntersticial.IsLoaded()) {
			this.pauseIntersticial.Show();
		}		
	}

	public void unpause(){
		StartCoroutine(unpauseGameMovement());
		GameManager.instance.pauseText.SetActive (false);
		GameManager.instance.pauseButton.SetActive (true);
		GameManager.instance.leftButton.SetActive (true);
		GameManager.instance.rightButton.SetActive (true);
	}

	IEnumerator unpauseGameMovement() {
		yield return new WaitForSeconds(0.1f);
		GameManager.instance.isPaused = false;
	}

	public void startGame(){
		this.RequestPauseInterstitial();				
		StartCoroutine(startGameMovement());
		GameManager.instance.startText.SetActive (false);
		GameManager.instance.pauseButton.SetActive (true);
		GameManager.instance.leftButton.SetActive (true);
		GameManager.instance.rightButton.SetActive (true);
	}

	IEnumerator startGameMovement() {
		yield return new WaitForSeconds(0.1f);
		GameManager.instance.hasBegun = true;
	}

	// public void leaderBoardOpen(){
	// 	GameManager.instance.leaberboardText.SetActive(true);
	// 	GameManager.instance.gameOverText.SetActive(false);
	// }

	public void leaderBoardClose(){
		GameManager.instance.networkError.SetActive(false);
		GameManager.instance.leaderboardText.SetActive(false);
		GameManager.instance.startText.SetActive(true);				
	}

	public void restart(){
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}

	public void mute(){	
		GameManager.instance.muteButton.SetActive(false);
		GameManager.instance.unmuteButton.SetActive(true);
	}

	public void unmute(){
		GameManager.instance.muteButton.SetActive(true);
		GameManager.instance.unmuteButton.SetActive(false);
	}

	// Collision with island kills player
	void OnCollisionEnter2D(Collision2D coll){
		if (coll.gameObject.tag == "Obstacle") {
			character.GetComponent<Collider2D>().enabled = false;
			// Set the player isDead state to true
			//isDead = true;
			//Destroy (this.gameObject);
			// explosion.Play();
			source.PlayOneShot(explosion);
			character.transform.GetChild(0).gameObject.SetActive(false);
			character.transform.GetChild(1).gameObject.SetActive(true);
			// Access and fire SurferDied() from GameManager
			this.pauseIntersticial.Destroy();
			GameManager.instance.SurferDied ();				
		} else if (coll.gameObject.tag == "Diamond") {
			GameManager.instance.CollectCoins ();
			if(GameManager.instance.gameOver != true){
				source.PlayOneShot(collect);
			}			
			Destroy (coll.gameObject);
		} else if (coll.gameObject.tag == "BigDiamond") {
			GameManager.instance.CollectBigDiamonds ();
			if(GameManager.instance.gameOver != true){
				source.PlayOneShot(collect);
			}			
			Destroy (coll.gameObject);
		} else if (coll.gameObject.tag == "Speedboost") {
			GameManager.instance.CollectSpeedboost ();
			GameManager.instance.speed = 600;			
			if(GameManager.instance.gameOver != true){
				source.PlayOneShot(speedboost);
			}			
			Destroy (coll.gameObject);
		}  else if (coll.gameObject.tag == "Slomo") {
			GameManager.instance.CollectSlomo ();
			if(GameManager.instance.gameOver != true){
				source.PlayOneShot(speedboost);
			}			
			Destroy (coll.gameObject);
		}
	}
}
