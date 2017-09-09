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
		public float parryWindowJustLate = 0.1f;
		public float parryWindowJustEarly = 0.1f;
		public KeyCode playerOneButton;
		public KeyCode playerTwoButton;
		public KeyCode quitButton;

		Touch t;
		bool firstTouch = false;
		bool parryTouchOne = false;
		bool parryTouchTwo = false;
		bool parryTouchOnePrev = false;
		bool parryTouchTwoPrev = false;
		bool parryTouchOneSync = false;
		bool parryTouchTwoSync = false;
		float parryTouchOneTime = -1.0f;
		float parryTouchTwoTime = -1.0f;
		bool justParry = false;
		bool playerOnePressed = false;
		bool playerTwoPressed = false;
		bool quitPressed = false;

		Color playerObjectOneColor;
		Color playerObjectTwoColor;

		Coroutine inputCheck;

		void Start(){
			parryTouchOneTime = -1;
			parryTouchTwoTime = -1;
			playerObjectOneColor = playerObjectOne.GetComponentInChildren<SpriteRenderer>().color;
			playerObjectTwoColor = playerObjectTwo.GetComponentInChildren<SpriteRenderer>().color;
			winText.gameObject.SetActive(false);
			Cursor.visible = false;
		}

		void Update(){
			playerOnePressed = Input.GetKeyDown(playerOneButton);
			playerTwoPressed = Input.GetKeyDown(playerTwoButton);
			quitPressed = Input.GetKeyDown(quitButton);
			if(quitPressed){
				Application.Quit();
			}
			if(playerOnePressed) Debug.Log("P1 press");
			if(playerTwoPressed) Debug.Log("P2 press");

			if(Input.touchCount == 0 && !playerOnePressed && !playerTwoPressed) return;

			if (Input.touchCount > 0) {
				t = Input.GetTouch(0);
				UpdateTouch(t);
			}
			else if (!firstTouch) {
				UpdateButton();
			}
		}

		void UpdateButton() {
			if (playerOnePressed) {
				CheckWhichPlayerTouched(playerObjectOne);
			}
			else if (playerTwoPressed) {
				CheckWhichPlayerTouched(playerObjectTwo);
			}
		}

		void UpdateTouch(Touch t){
			
			if(t.phase == TouchPhase.Began){
				Ray r = Camera.main.ScreenPointToRay(t.position);
				RaycastHit2D hit = Physics2D.GetRayIntersection(r);
				if(hit.collider != null){
					if(!firstTouch){
						CheckWhichPlayerTouched(hit.collider.gameObject);
					}
				}
			}
		}

		void CheckWhichPlayerTouched(GameObject touchedObject){
			if(touchedObject.name == "Player1" || playerOnePressed){
				firstTouch = true;
				 Debug.Log("Player1 Touch");
				playerObjectTwo.GetComponentInChildren<SpriteRenderer>().color = Color.white;
				parryTouchOne = true;
				parryTouchOnePrev = true;
				parryTouchOneTime = Time.time;
				StartCoroutine("ParryTimer", playerObjectTwo);
			}
			else if(touchedObject.name == "Player2" || playerTwoPressed){
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
			//yield return StartCoroutine("JustParryCheck");
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
				if((t.phase == TouchPhase.Began && Input.touchCount > 0) || playerOnePressed || playerTwoPressed){
					Ray r = Camera.main.ScreenPointToRay(t.position);
					RaycastHit2D hit = Physics2D.GetRayIntersection(r);
					
				
					//Set touch time
					if((hit.collider != null && hit.collider.gameObject.name == "Player1") || playerOnePressed){
						parryTouchOneTime = Time.time;
						Debug.Log("PRESSED BUTTON TIME ONE");
						 Debug.Log(parryTouchOneTime);
						 Debug.Log(parryTouchOneTime - parryTouchTwoTime);
					}
					else if((hit.collider != null && hit.collider.gameObject.name == "Player2") || playerTwoPressed){
						parryTouchTwoTime = Time.time;
						Debug.Log("PRESSED BUTTON TIME TWO");
						 Debug.Log(parryTouchTwoTime);
						Debug.Log(parryTouchTwoTime - parryTouchOneTime);
					}
					
					//Check for early Just Parry or Double Input
					if(((hit.collider != null && hit.collider.gameObject.name == "Player1") || playerOnePressed) && parryTouchOnePrev){
						yield return StartCoroutine("JustParryCheck");
						if(justParry){
							yield break;
						}

						Debug.Log("Player1 Cheat");
						playerObjectOne.GetComponentInChildren<SpriteRenderer>().color = Color.black;
						playerObjectOne.gameObject.SetActive(false);
						parryTouchOnePrev = false;
						parryTouchOne = false;
						parryTouchTwo = false;
						parryTouchTwoPrev = true; // Player two wins
						yield break;
					}
					else if(((hit.collider != null && hit.collider.gameObject.name == "Player2") || playerTwoPressed) && parryTouchTwoPrev){
						yield return StartCoroutine("JustParryCheck");
						if(justParry){
							yield break;
						}

						Debug.Log("Player2 Cheat");
						playerObjectTwo.gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.black;
						playerObjectTwo.gameObject.SetActive(false);
						parryTouchTwoPrev = false;
						parryTouchOne = false;
						parryTouchTwo = false;
						parryTouchOnePrev = true; // Player 1 wins
						yield break;
					}

					//Player parried
					if(!parryTouchOne && !parryTouchTwo){
						if((hit.collider != null && hit.collider.gameObject.name == "Player1") || playerOnePressed){
							parryTouchOneTime = Time.time;
							Debug.Log("Player 1 parry");
							playerObjectOne.GetComponentInChildren<SpriteRenderer>().color = playerObjectOneColor;
							playerObjectTwo.GetComponentInChildren<SpriteRenderer>().color = Color.white;
							parryTouchOne = true;
							parryTouchOnePrev = true; 
							parryTouchTwoPrev = false;

							//Check late Just Parry
							if(parryTouchOneTime - parryTouchTwoTime <= parryWindowJustLate){
								PlayerOneJustParry();
								yield break;
							}
							
							yield break;
						}
						else if((hit.collider != null && hit.collider.gameObject.name == "Player2") || playerTwoPressed){
							parryTouchTwoTime = Time.time;
							Debug.Log("Player 2 parry");
							playerObjectTwo.GetComponentInChildren<SpriteRenderer>().color = playerObjectTwoColor;
							playerObjectOne.GetComponentInChildren<SpriteRenderer>().color = Color.white;
							parryTouchTwo = true;
							parryTouchTwoPrev = true;
							parryTouchOnePrev = false;

							//Check late Just Parry
							if(parryTouchTwoTime - parryTouchOneTime <= parryWindowJustLate){
								PlayerTwoJustParry();
								yield break;
							}

							yield break;
						}
					}	
				}

			yield return new WaitForEndOfFrame();
			}
		}

		IEnumerator JustParryCheck(){
			for(float time = 0.0f; time < parryWindowJustEarly; time += Time.deltaTime){
				//Debug.Log("check");
				//playerOnePressed = Input.GetKeyDown(playerOneButton);
				//playerTwoPressed = Input.GetKeyDown(playerTwoButton);
				if((t.phase == TouchPhase.Began || playerOnePressed || playerTwoPressed)){
					Debug.Log("JustCheck");
					Ray r = Camera.main.ScreenPointToRay(t.position);
					RaycastHit2D hit = Physics2D.GetRayIntersection(r);

					if(((hit.collider != null && hit.collider.gameObject.name == "Player2") || playerTwoPressed) && parryTouchOnePrev){
						PlayerOneJustParry();
						Debug.Log("Early One");
						yield break;
					}
					else if(((hit.collider != null && hit.collider.gameObject.name == "Player1") || playerOnePressed) && parryTouchTwoPrev){
						PlayerTwoJustParry();
						Debug.Log("Early Two");
						yield break;
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

		void PlayerOneJustParry(){
			Debug.Log("JUST PARRY ONE!!!");
			Debug.Log(parryTouchOneTime - parryTouchTwoTime);
			justParry = true;
			parryTouchOnePrev = true; //Previous parry is player 1
			parryTouchTwoPrev = false;
			parryTouchOne = false; //player 1 wins
			parryTouchTwo = false;
		}

		void PlayerTwoJustParry(){
			Debug.Log("JUST PARRY TWO!!!");
			Debug.Log(parryTouchTwoTime - parryTouchOneTime);
			justParry = true;
			parryTouchOnePrev = false;
			parryTouchTwoPrev = true;
			parryTouchTwo = false; //player 2 wins
			parryTouchOne = false;
		}
}
