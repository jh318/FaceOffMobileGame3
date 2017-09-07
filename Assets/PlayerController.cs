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

		Touch t;
		bool touched = false;
		bool firstTouch = false;
		bool parryTouchOne = false;
		bool parryTouchTwo = false;
		bool parryTouchOnePrev = false;
		bool parryTouchTwoPrev = false;

		Color playerObjectOneColor;
		Color playerObjectTwoColor;

		Coroutine inputCheck;

		void Start(){
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
				StartCoroutine("ParryTimer", playerObjectTwo);
			}
			else if(touchedObject.name == "Player2"){
				firstTouch = true;
				Debug.Log("Player2 Touch");
				playerObjectOne.GetComponentInChildren<SpriteRenderer>().color = Color.white;
				parryTouchTwo = true;
				parryTouchTwoPrev = true;
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
				StartCoroutine("WinAndReset", playerObjectOne);
			}
				
			else if(parryTouchTwoPrev == true){
				Debug.Log("Player2 wins");
				playerObjectOne.GetComponentInChildren<SpriteRenderer>().color = Color.black;
				StartCoroutine("WinAndReset", playerObjectTwo);
			}
		}

		IEnumerator ParryTouch(GameObject otherObject){
			parryTouchOne = false;
			parryTouchTwo = false;

			for(float time = 0.0f; time < parryWindow; time += Time.deltaTime){
				if(t.phase == TouchPhase.Began){
					Ray r = Camera.main.ScreenPointToRay(t.position);
					RaycastHit2D hit = Physics2D.GetRayIntersection(r);
					if(hit.collider != null && hit.collider.gameObject.name == "Player1" && parryTouchOnePrev){
						Debug.Log("Player1 Cheat");
						hit.collider.gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.black;
						parryTouchOne = false;
						parryTouchTwo = true;
						yield break;
					}
					else if(hit.collider != null && hit.collider.gameObject.name == "Player2" && parryTouchTwoPrev){
						Debug.Log("Player2 Cheat");
						hit.collider.gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.black;
						parryTouchOne = true;
						parryTouchTwo = false;
						yield break;
					}
					if(!parryTouchOne && !parryTouchTwo){
						if(hit.collider != null && hit.collider.gameObject.name == "Player1"){
							Debug.Log("Player 1 parry");
							hit.collider.gameObject.GetComponentInChildren<SpriteRenderer>().color = playerObjectOneColor;
							playerObjectTwo.GetComponentInChildren<SpriteRenderer>().color = Color.white;
							parryTouchOne = true;
							parryTouchOnePrev = true; //Previous parry is player 1
							parryTouchTwoPrev = false;
							//StartCoroutine("ParryTimer", playerObjectTwo);
							yield break;
					}
						else if(hit.collider != null && hit.collider.gameObject.name == "Player2"){
							Debug.Log("Player 2 parry");
							hit.collider.gameObject.GetComponentInChildren<SpriteRenderer>().color = playerObjectTwoColor;
							playerObjectOne.GetComponentInChildren<SpriteRenderer>().color = Color.white;
							parryTouchTwo = true;
							parryTouchOnePrev = false;
							parryTouchTwoPrev = true;
							//StartCoroutine("ParryTimer", playerObjectOne);
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
			yield return new WaitForSeconds(3.0f);
			SceneManager.LoadScene("GameScene");
		}
}
