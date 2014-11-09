/// <summary>
/// Controller.
/// this script use for control a character
/// </summary>

using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]
//[RequireComponent (typeof (AnimationManager))]
public class Controller : MonoBehaviour {
	
	public enum DirectionInput{
	   Null, Left, Right, Up, Down
	}
	
	public enum Position{
		Middle, Left, Right	
	}
	
	public CoinRotation coinRotate;
	public GameObject magnet;
	public float speedMove = 5; 
	public float gravity;
	public float jumpValue;
	
	[HideInInspector]
	public bool isRoll;
	[HideInInspector]
	public bool isDoubleJump;
	[HideInInspector]
	public bool isMultiply;
	[HideInInspector]
	public CharacterController characterController;
	
	[HideInInspector]
	public float timeSprint;
	[HideInInspector]
	public float timeMagnet;
	[HideInInspector]
	public float timeMultiply;
	[HideInInspector]
	public float timeJump;
	
	private bool activeInput;
	private bool jumpSecond;
	
	private Vector3 moveDir;
	private Vector2 currentPos;
	
	public bool keyInput;
	public bool touchInput;
	
	private Position positionStand;
	private DirectionInput directInput;
//	private AnimationManager animationManager;

	private Vector3 direction;
	private Vector3 currectPosCharacter;
	
	public static Controller instace;

	private Animator animator;

	public int AttackingIndex;

	public Follow[] follows;
	
	
	//Check item collider
//	void OnTriggerEnter(Collider col){
//		if(col.tag == "Item"){
//			if(col.GetComponent<Item>().useAbsorb){
//				col.GetComponent<Item>().useAbsorb = false;	
//				col.GetComponent<Item>().StopAllCoroutines();
//			}
//			col.GetComponent<Item>().ItemGet();	
//		}
//	}
	
	void Start(){
		//Set state character
		instace = this;
		characterController = this.GetComponent<CharacterController>();
//		animationManager = this.GetComponent<AnimationManager>();
		speedMove = GameAttribute.gameAttribute.speed;
		jumpSecond = false;
		magnet.SetActive(false);
		GameAttribute.gameAttribute.isPlaying = false;
		animator = GetComponent<Animator>();
		Invoke("WaitStart",0.2f);
	}

	public void StartMoving()
	{
		GameAttribute.gameAttribute.isPlaying = true;
	}

	//Reset state,variable when character die
	public void Reset(){
		transform.position = new Vector3(0, transform.position.y, -5);
//		animationManager.animationState = animationManager.Run;
		positionStand = Position.Middle;
		jumpSecond = false;
		isRoll = false;
		isDoubleJump = false;
		isMultiply = false;
		magnet.SetActive(false);
		StopAllCoroutines();
		StartCoroutine(UpdateAction());
	}
	
	void WaitStart(){
		StartCoroutine(UpdateAction());
	}	
	
	//Update Loop
	IEnumerator UpdateAction(){
		while(GameAttribute.gameAttribute.life > 0){
			if(GameAttribute.gameAttribute.pause == false && GameAttribute.gameAttribute.isPlaying && !GameAttribute.gameAttribute.isAttacking){
				
				if(keyInput)
				KeyInput();
				
				if(touchInput){
					//TouchInput();
					DirectionAngleInput();
				}
				CheckLane();
				MoveForward();

                foreach(Follow follow in follows)
                {
                    follow.StartCoroutine("ChangeStandPostion", positionStand);
                }
			}else{
//				animation.Stop();
			}
			yield return 0;	
		}
		StartCoroutine(MoveBack());
//		animationManager.animationState = animationManager.Dead;

		yield return new WaitForSeconds (2);

		GameController.instace.StartCoroutine(GameController.instace.ResetGame());
	}
	
	IEnumerator MoveBack(){
		float z = transform.position.z-0.5f;
		bool complete = false;
		while(complete == false){
			transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x,transform.position.y,z),2*Time.deltaTime);
			if((transform.position.z - z) < 0.05f){
				complete = true;
			}
			yield return 0;
		}
		
		yield return 0;
	}
	
	private void MoveForward(){
		speedMove = GameAttribute.gameAttribute.speed;
		
		if(characterController.isGrounded){
			moveDir = Vector3.zero;
			if(directInput == DirectionInput.Up){
				Jump();
				animator.SetTrigger ("Jump " + 2);
				if(isDoubleJump){
					jumpSecond = true;	
					animator.SetTrigger ("Jump " + 2);
				}

				foreach(Follow follow in follows)
				{
					follow.StartCoroutine("ChangeJumpType", 2);
				}
			}
		}else{
			if(directInput == DirectionInput.Down){
				QuickGround();
			}
			if(directInput == DirectionInput.Up){
				if(jumpSecond){
					JumpSeccond();
					jumpSecond = false;
				}
			}
//			
//			if(animationManager.animationState != animationManager.Jump
//				&& animationManager.animationState != animationManager.JumpSecond
//				&& animationManager.animationState != animationManager.Roll){
//				animationManager.animationState = animationManager.JumpLoop;
//			}
		}
		moveDir.z = 0;
		moveDir += this.transform.TransformDirection(Vector3.forward*speedMove);
		moveDir.y -= gravity * Time.deltaTime;

		CheckSideCollision ();
		characterController.Move((moveDir + direction)*Time.deltaTime);

		foreach(Follow follow in follows)
		{
			follow.StartCoroutine("ChangeMoveDir", moveDir + direction);
		}


		animator.SetFloat("Speed", speedMove);
	}
	
	private bool checkSideCollision;
	private float countDeleyInput;

	private void CheckSideCollision(){
			if (positionStand == Position.Right) {
				if((int)characterController.collisionFlags == 5 || characterController.collisionFlags == CollisionFlags.Sides){
					if(transform.position.x < 1.75f && checkSideCollision == false){
						Debug.Log("Hit");
						CameraFollow.instace.ActiveShake();
						positionStand = Position.Middle;
						checkSideCollision = true;
					}
				}
			}

			if (positionStand == Position.Left) {
				if((int)characterController.collisionFlags == 5 || characterController.collisionFlags == CollisionFlags.Sides){
					if(transform.position.x > -1.75f && checkSideCollision == false){
						Debug.Log("Hit");
						CameraFollow.instace.ActiveShake();
						positionStand = Position.Middle;
						checkSideCollision = true;
					}
				}
			}

			if(positionStand == Position.Middle){
				if((int)characterController.collisionFlags == 5 || characterController.collisionFlags == CollisionFlags.Sides){
					if(transform.position.x < -0.05f && checkSideCollision == false){
						Debug.Log("Hit");
						CameraFollow.instace.ActiveShake();
						positionStand = Position.Left;
						
						checkSideCollision = true;
					}else if(transform.position.x > 0.05f && checkSideCollision == false){
						Debug.Log("Hit");
						CameraFollow.instace.ActiveShake();
						positionStand = Position.Right;
						checkSideCollision = true;
					}
				}
			}

		if (checkSideCollision == true) {
			countDeleyInput += Time.deltaTime;
			if(countDeleyInput >= 1f){
				checkSideCollision = false;
				countDeleyInput = 0;
			}
		}
	}
	
	private void QuickGround(){
		moveDir.y -= jumpValue*3;
	}
	
	
	//Jump State
	private void Jump(){
		//Play sfx when jump
		SoundManager.instance.PlayingSound("Jump");
		
//		animationManager.animationState = animationManager.Jump;
		moveDir.y += jumpValue;
	}
	
	private void JumpSeccond(){
//		animationManager.animationState = animationManager.JumpSecond;
		moveDir.y += jumpValue*1.15f;
	}
	
	private void CheckLane(){
		if(positionStand == Position.Middle){
			if(directInput == DirectionInput.Right){
				if(characterController.isGrounded){
//					animation.Stop();
//					animationManager.animationState = animationManager.TurnRight;
				}
				positionStand = Position.Right;	
				//Play sfx when step
				SoundManager.instance.PlayingSound("Step");
			}else if(directInput == DirectionInput.Left){
				if(characterController.isGrounded){
//					animation.Stop();
//					animationManager.animationState = animationManager.TurnLeft;
				}
				positionStand = Position.Left;	
				//Play sfx when step
				SoundManager.instance.PlayingSound("Step");
			}

			//transform.position = Vector3.Lerp(transform.position, new Vector3(0,transform.position.y,transform.position.z), 6 * Time.deltaTime);
			if(transform.position.x > 0.05f){
				direction = Vector3.Lerp(direction, Vector3.left*6, Time.deltaTime * 500);
			}else if(transform.position.x < -0.05f){
				direction = Vector3.Lerp(direction, Vector3.right*6, Time.deltaTime * 500);
			}else{

				Debug.Log("in ===============");
				direction = Vector3.zero;
				checkSideCollision = false;
				transform.position = Vector3.Lerp(transform.position, new Vector3(0,transform.position.y,transform.position.z), 6 * Time.deltaTime);
			}
		}else if(positionStand == Position.Left){
			if(directInput == DirectionInput.Right){
				if(characterController.isGrounded){
//					animation.Stop();
//					animationManager.animationState = animationManager.TurnRight;
				}
				positionStand = Position.Middle;	
				//Play sfx when step
				SoundManager.instance.PlayingSound("Step");
			}
			//transform.position = Vector3.Lerp(transform.position, new Vector3(-1.8f,transform.position.y,transform.position.z), 6 * Time.deltaTime);
			if(transform.position.x > -1.8f){
				direction = Vector3.Lerp(direction, Vector3.left*6, Time.deltaTime * 500);
			}else{
				Debug.Log("in =============== 311");
				direction = Vector3.zero;
				checkSideCollision = false;
				transform.position = Vector3.Lerp(transform.position, new Vector3(-1.8f,transform.position.y,transform.position.z), 6 * Time.deltaTime);
			}
		}else if(positionStand == Position.Right){
			if(directInput == DirectionInput.Left){
				if(characterController.isGrounded){
//					animation.Stop();
//					animationManager.animationState = animationManager.TurnLeft;
				}
				positionStand = Position.Middle;
				//Play sfx when step
				SoundManager.instance.PlayingSound("Step");
			}
			//transform.position = Vector3.Lerp(transform.position, new Vector3(1.8f,transform.position.y,transform.position.z), 6 * Time.deltaTime);
			if(transform.position.x < 1.8f){
				direction = Vector3.Lerp(direction, Vector3.right*6, Time.deltaTime * 500);
			}else{
				direction = Vector3.zero;
				checkSideCollision = false;
				transform.position = Vector3.Lerp(transform.position, new Vector3(1.8f,transform.position.y,transform.position.z), 6 * Time.deltaTime);
			}
		}
		
		if(directInput == DirectionInput.Down){
//			animation.Stop();
//			animationManager.animationState = animationManager.Roll;
			//Play sfx when roll
			SoundManager.instance.PlayingSound("Roll");
		}
	}
	
	//Key input method
	private void KeyInput()
	{
		if(Input.anyKeyDown)
		{
			activeInput = true;
		}
		
		if(activeInput && checkSideCollision == false)
		{
			if(Input.GetKey(KeyCode.A))
			{
				directInput = DirectionInput.Left;
				activeInput = false;
			}else
			
			if(Input.GetKey(KeyCode.D))
			{
				directInput = DirectionInput.Right;
				activeInput = false;
			}else
			
			if(Input.GetKey(KeyCode.W))
			{
				directInput = DirectionInput.Up;
				activeInput = false;
			}else
				
			if(Input.GetKey(KeyCode.S))
			{
				directInput = DirectionInput.Down;
				activeInput = false;
			}
		}else{
			directInput = DirectionInput.Null;	
		}
		
		
	}
	
	//Touch input method
	private void TouchInput(){
		if(Input.GetMouseButtonDown(0)){
			currentPos = Input.mousePosition;	
			activeInput = true;
		}
		if(Input.GetMouseButton(0) && checkSideCollision == false){
			if(activeInput){
				if((Input.mousePosition.x - currentPos.x) > 40){
					directInput = DirectionInput.Right;
					activeInput = false;
				}else if((Input.mousePosition.x - currentPos.x) < -40){
					directInput = DirectionInput.Left;
					activeInput = false;
				}else if((Input.mousePosition.y - currentPos.y) > 40){
					directInput = DirectionInput.Up;
					activeInput = false;
				}else if((Input.mousePosition.y - currentPos.y) < -40){
					directInput = DirectionInput.Down;
					activeInput = false;
				}
			}else{
				directInput = DirectionInput.Null;
			}
			
		}
		if(Input.GetMouseButtonUp(0)){
			directInput = DirectionInput.Null;	
		}
		currentPos = Input.mousePosition;
	}
	
	private void DirectionAngleInput(){
		if(Input.GetMouseButtonDown(0)){
			currentPos = Input.mousePosition;
			activeInput = true;
		}
		
		if(Input.GetMouseButton(0) && checkSideCollision == false){
			if(activeInput){
				float ang = CalculateAngle.GetAngle(currentPos, Input.mousePosition);
				if((Input.mousePosition.x - currentPos.x) > 20){
					if(ang < 45 && ang > -45){
						directInput = DirectionInput.Right;
						activeInput = false;
					}else if(ang >= 45){
						directInput = DirectionInput.Up;
						activeInput = false;
					}else if(ang <= -45){
						directInput = DirectionInput.Down;
						activeInput = false;
					}
				}else if((Input.mousePosition.x - currentPos.x) < -20){
					if(ang < 45 && ang > -45){
						directInput = DirectionInput.Left;
						activeInput = false;
					}else if(ang >= 45){
						directInput = DirectionInput.Down;
						activeInput = false;
					}else if(ang <= -45){
						directInput = DirectionInput.Up;
						activeInput = false;
					}
				}else if((Input.mousePosition.y - currentPos.y) > 20){
					if((Input.mousePosition.x - currentPos.x) > 0){
						if(ang > 45 && ang <= 90){
							directInput = DirectionInput.Up;
							activeInput = false;
						}else if(ang <= 45 && ang >= -45){
							directInput = DirectionInput.Right;
							activeInput = false;
						}
					}else if((Input.mousePosition.x - currentPos.x) < 0){
						if(ang < -45 && ang >= -89){
							directInput = DirectionInput.Up;
							activeInput = false;
						}else if(ang >= -45){
							directInput = DirectionInput.Left;
							activeInput = false;
						}
					}
				}else if((Input.mousePosition.y - currentPos.y) < -20){
					if((Input.mousePosition.x - currentPos.x) > 0){
						if(ang > -89 && ang < -45){
							directInput = DirectionInput.Down;
							activeInput = false;
						}else if(ang >= -45){
							directInput = DirectionInput.Right;
							activeInput = false;
						}
					}else if((Input.mousePosition.x - currentPos.x) < 0){
						if(ang > 45 && ang < 89){
							directInput = DirectionInput.Down;
							activeInput = false;
						}else if(ang <= 45){
							directInput = DirectionInput.Left;
							activeInput = false;
						}
					}
					
				}
			}
		}
		
		if(Input.GetMouseButtonUp(0)){
			directInput = DirectionInput.Null;	
			activeInput = false;
		}
		
	}
	
	//Sprint Item
	public void Sprint(float speed, float time){
		StopCoroutine("CancelSprint");
		GameAttribute.gameAttribute.speed = speed;
		timeSprint = time;
		StartCoroutine(CancelSprint());
	}
	
	IEnumerator CancelSprint(){
		while(timeSprint > 0){
			timeSprint -= 1 * Time.deltaTime;
			yield return 0;
		}
		int i = 0;
		GameAttribute.gameAttribute.speed = GameAttribute.gameAttribute.starterSpeed;
		while(i < GameController.instace.countAddSpeed+1){
			GameAttribute.gameAttribute.speed += GameController.instace.speedAdd;	
			i++;
		}
	}
	
	//Magnet Item
	public void Magnet(float time){
		StopCoroutine("CancleMagnet");
		magnet.SetActive(true);
		timeMagnet = time;
		StartCoroutine(CancleMagnet());
	}
			
	IEnumerator CancleMagnet(){
		while(timeMagnet > 0){
			timeMagnet -= 1 * Time.deltaTime;
			yield return 0;
		}
		magnet.SetActive(false);
	}
	
	//Double jump Item
	public void JumpDouble(float time){
		StopCoroutine("CancleJumpDouble");
		isDoubleJump = true;
		timeJump = time;
		StartCoroutine(CancleJumpDouble());
	}
	
	IEnumerator CancleJumpDouble(){
		while(timeJump > 0){
			timeJump -= 1 * Time.deltaTime;
			yield return 0;
		}
		isDoubleJump = false;
	}
	
	//Multiply Item
	public void Multiply(float time){
		StopCoroutine("CancleMultiply");
		isMultiply = true;
		timeMultiply = time;
		StartCoroutine(CancleMultiply());
	}
	
	IEnumerator CancleMultiply(){
		while(timeMultiply > 0){
			timeMultiply -= 1 * Time.deltaTime;
			yield return 0;
		}
		isMultiply = false;
	}

	void OnTriggerEnter (Collider collider)
	{
		Debug.Log(collider.tag);

		if (collider.CompareTag ("Battlefield")) {
			animator.SetTrigger ("AttackRun");
			GameAttribute.gameAttribute.isAttacking = true;
			StartCoroutine(MoveToEnemy(collider));

			foreach(Follow follow in follows)
			{
				follow.StartCoroutine("ChangeMoveDir", Vector3.zero);
				follow.StartCoroutine("MoveToEnemy", collider);
			}
		}
		else if (collider.CompareTag ("GameOverField"))
		{
			GameAttribute.gameAttribute.isPlaying = false;

			animator.SetFloat ("Speed", -1f);
			
			GameObject uiCanvas = GameObject.Find ("UI Canvas");
			
			if (uiCanvas != null) {
				uiCanvas.SetActive (false);
				
				Debug.Log ("Stopped !");
			}

			foreach(Follow follow in follows)
			{
				follow.StartCoroutine("ChangeMoveDir", Vector3.zero);
			}
		}
	}

	IEnumerator MoveToEnemy(Collider collider)
	{
		Vector3 movingTarget = collider.transform.FindChild ("Anchors").GetChild (this.AttackingIndex).position;
		movingTarget.y = transform.position.y;
		Transform attackingTarget = collider.transform.FindChild ("Monsters").GetChild (this.AttackingIndex);

		while (Mathf.Abs(transform.position.z - movingTarget.z) > 0.1)
		{
			iTween.MoveTo(gameObject, iTween.Hash("position", movingTarget, "time", 2f));
			yield return 1;
		}

		animator.SetFloat ("Speed", -1f);
		animator.SetBool ("Attacking", true);

		transform.rotation = Quaternion.LookRotation(attackingTarget.position - transform.position);

		attackingTarget.GetComponent<Animator> ().SetBool ("Attacking", true);

		GameAttribute.gameAttribute.isPlaying = false;
	}

	public void StopAttacking ()
	{
		animator.SetBool ("Attacking", false);

		Vector3 movingTarget = transform.position;
		movingTarget.x = 0;
		iTween.MoveTo(gameObject, iTween.Hash("position", movingTarget, "time", 0.5f));
		GameAttribute.gameAttribute.isAttacking = false;
		transform.rotation = Quaternion.identity;
//		StartCoroutine(ContinueRun());

		foreach(Follow follow in follows)
		{
			follow.StartCoroutine("BackToLine", movingTarget);
		}
	}

	IEnumerator ContinueRun()
	{
		yield return new WaitForSeconds(5f);

		transform.rotation = Quaternion.identity;

		GameAttribute.gameAttribute.isAttacking = false;
	}
}
