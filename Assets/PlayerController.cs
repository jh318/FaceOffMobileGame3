using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {
		
		public GameObject playerObjectOne;
		public GameObject playerObjectTwo;
		public Text winText;
		public float parryWindow = 1.0f;
		public float parryWindowJust = 2.0f;

		Touch t;
		bool touched = false;
		bool firstTouch = false;
		bool parryTouchOne = false;
		bool parryTouchTwo = false;
		bool parryTouchOnePrev = false;
		bool parryTouchTwoPrev = false;
		bool parryTouchOneSync = false;
		bool parryTouchTwoSync = false;
		float parryTouchOneTime = -1.0f;
		float parryTouchTwoTime = -1.0f;

		Color playerObjectOneColor;
		Color playerObjectTwoColor;

		Coroutine inputCheck;

		void Start(){
			parryTouchOneTime = -1.0f;
			parryTouchTwoTime = -1.0f;
			playerObjectOneColor = playerObjectOne.GetComponentInChildren<SpriteRenderer>().color;
			playerObjectTwoColor = playerObjectTwo.GetComponentInChildren<SpriteRenderer>().color;
			winText.gameObject.SetActive(false);
		}

		void Update(){
			if(Input.touchCount == 0) return;
			
			t = Input.GetTouch(0);
			UpdateTouch(t);
		}

		void UpdateTouch(Touch t){
			if(t.phase == TouchPhase.Began){
				Ray r = Camera.main.ScreenPointToRay(t.position);
				RaycastHit2D hit = Physics2D.GetRayIntersection(r);
				if(hit.collider != null){
					if(!firstTouch){
						CheckWhichPlayerTouched(hit.collider.gameObject);
					}
					touched = true;
				}
			}
		}

		void CheckWhichPlayerTouched(GameObject touchedObject){
			if(touchedObject.name == "Player1"){
				firstTouch = true;
				Debug.Log("Player1 Touch");
				playerObjectTwo.GetComponentInChildren<SpriteRenderer>().color = Color.white;
				parryTouchOne = true;
				parryTouchOnePrev = true;
				parryTouchOneTime = Time.time;
				StartCoroutine("ParryTimer", playerObjectTwo);
			}
			else if(touchedObject.name == "Player2"){
				firstTouch = true;
				Debug.Log("Player2 Touch");
				playerObjectOne.GetComponentInChildren<SpriteRenderer>().color = Color.white;
				parryTouchTwo = true;
				parryTouchTwoPrev = true;
				parryTouchTwoTime = Time.time;
				StartCoroutine("ParryTimer", playerObjectOne);
			}
		}

		IEnumerator ParryTimer(GameObject otherObject){
			Debug.Log("Start Parry");
			while(parryTouchOne || parryTouchTwo){
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				yield return StartCoroutine("ParryTouch", otherObject);
			}

			if(parryTouchOnePrev == true){
				Debug.Log("Player1 wins");
				playerObjectTwo.GetComponentInChildren<SpriteRenderer>().color = Color.black;
				playerObjectTwo.gameObject.SetActive(false);
				StartCoroutine("WinAndReset", playerObjectOne);
			}
				
			else if(parryTouchTwoPrev == true){
				Debug.Log("Player2 wins");
				playerObjectOne.GetComponentInChildren<SpriteRenderer>().color = Color.black;
				playerObjectOne.gameObject.SetActive(false);
				StartCoroutine("WinAndReset", playerObjectTwo);
			}
			else{
				StartCoroutine("WinAndReset", gameObject);
				Debug.Log("Draw");
			}
		}

		IEnumerator ParryTouch(GameObject otherObject){
			parryTouchOne = false;
			parryTouchTwo = false;

			for(float time = 0.0f; time < parryWindow; time += Time.deltaTime){
				if(t.phase == TouchPhase.Began){
					Ray r = Camera.main.ScreenPointToRay(t.position);
					RaycastHit2D hit = Physics2D.GetRayIntersection(r);
					
					//Check for double input
					if(hit.collider != null && hit.collider.gameObject.name == "Player1" && parryTouchOnePrev){
						Debug.Log("Player1 Cheat");
						hit.collider.gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.black;
						playerObjectOne.gameObject.SetActive(false);
						parryTouchOnePrev = false;
						parryTouchOne = false;
						parryTouchTwo = false;
						parryTouchTwoPrev = true; // Player two wins
						yield break;
					}
					else if(hit.collider != null && hit.collider.gameObject.name == "Player2" && parryTouchTwoPrev){
						Debug.Log("Player2 Cheat");
						hit.collider.gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.black;
						playerObjectTwo.gameObject.SetActive(false);
						parryTouchTwoPrev = false;
						parryTouchOne = false;
						parryTouchTwo = false;
						parryTouchOnePrev = true; // Player 1 wins
						yield break;
					}

					//Player parried
					else if(!parryTouchOne && !parryTouchTwo){
						if(hit.collider != null && hit.collider.gameObject.name == "Player1"){
							parryTouchOneTime = Time.time;
							Debug.Log("Player 1 parry");
							hit.collider.gameObject.GetComponentInChildren<SpriteRenderer>().color = playerObjectOneColor;
							playerObjectTwo.GetComponentInChildren<SpriteRenderer>().color = Color.white;
							parryTouchOne = true;
							parryTouchOnePrev = true; //Previous parry is player 1
							parryTouchTwoPrev = false;

							if(parryTouchOneTime - parryTouchTwoTime < parryWindowJust && parryTouchOneTime - parryTouchTwoTime >= 0.0f){
								Debug.Log("JUST PARRY ONE");
								parryTouchOne = false; //player 1 wins
							}
							yield break;
						}
						else if(hit.collider != null && hit.collider.gameObject.name == "Player2"){
							parryTouchTwoTime = Time.time;
							Debug.Log("Player 2 parry");
							hit.collider.gameObject.GetComponentInChildren<SpriteRenderer>().color = playerObjectTwoColor;
							playerObjectOne.GetComponentInChildren<SpriteRenderer>().color = Color.white;
							parryTouchTwo = true;
							parryTouchTwoPrev = true;
							parryTouchOnePrev = false;

							if(parryTouchTwoTime - parryTouchOneTime < parryWindowJust && parryTouchTwoTime - parryTouchOneTime >= 0.0f){
								Debug.Log("JUST PARRY TWO");
								parryTouchTwo = false; //player 2 wins
							}
							yield break;
						}
					}	
				}

			yield return new WaitForEndOfFrame();
			}
		}

		IEnumerator WinAndReset(GameObject winObject){
			winText.gameObject.SetActive(true);
			if(winObject.name == "Player1"){
				winText.text = "Red Wins!";
			}
			else if(winObject.name == "Player2"){
				winText.text = "Blue wins!";
			}
			else{
				winText.text = "Draw!";
			}
			yield return new WaitForSeconds(3.0f);
			SceneManager.LoadScene("GameScene");
		}
}
