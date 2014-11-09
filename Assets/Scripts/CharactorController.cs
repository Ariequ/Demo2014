using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharactorController : MonoBehaviour
{

	private static float NODE_NEARBY_SQU_DISTANCE = 1f;
	private static float SPEED_LERP_SCALE = 0.6f;
	private static string ANIMATOR_PARAM_SPEED = "Speed";
	public Transform runningPath;
	public int index;
	private Animator animator;
	private Transform[] pathNodes;
	private int targetNodeIndex;
	private float targetSpeed;
	private Transform movingTarget;
	private Transform attackingTarget;
	public FollowingController[] fowllowers;

	void Start ()
	{
		this.animator = GetComponent<Animator> (); 
		this.pathNodes = new Transform[runningPath.childCount];

		for (int i = 0; i < runningPath.childCount; i++) {
			pathNodes [i] = runningPath.GetChild (i);
		}

		this.targetNodeIndex = 0;
		this.movingTarget = pathNodes [this.targetNodeIndex];

		foreach (FollowingController follower in fowllowers) {
			follower.StartCoroutine ("ChangeMovingTarget", this.movingTarget);
		}
	}

	void Update ()
	{
		if (Input.GetMouseButtonDown (0) && animator.GetFloat (ANIMATOR_PARAM_SPEED) > 0) {
//			Vector3 mousePosition = Input.mousePosition;
//
//			if (mousePosition.x > Screen.currentResolution.width / 2 && mousePosition.y < Screen.currentResolution.height / 2)
//			{
//				int jumpTpye = Random.Range(1, 3);
//				animator.SetTrigger("Jump " + jumpTpye);
//
//				foreach(FollowingController follower in fowllowers)
//				{
//					follower.StartCoroutine("Jump", jumpTpye);
//				}
//			}

//			Swipe (1);
		}

		if (this.targetNodeIndex < pathNodes.Length) {
			Vector3 targetPosition = movingTarget.position;
			Vector3 charactorPosition = transform.position;
			
			if (this.targetSpeed >= 0 && Mathf.Pow (targetPosition.x - charactorPosition.x, 2) + Mathf.Pow (targetPosition.z - charactorPosition.z, 2) < NODE_NEARBY_SQU_DISTANCE) {
				if (this.attackingTarget == null) {
					this.targetNodeIndex += 1;
					
					if (this.targetNodeIndex < pathNodes.Length) {
						this.movingTarget = pathNodes [this.targetNodeIndex];

						ChangeDirection ();

						foreach (FollowingController follower in fowllowers) {
							follower.StartCoroutine ("ChangeMovingTarget", this.movingTarget);
						}
					} else {
						animator.SetFloat (ANIMATOR_PARAM_SPEED, -1f);

						GameObject uiCanvas = GameObject.Find ("UI Canvas");

						if (uiCanvas != null) {
							uiCanvas.SetActive (false);
							
							Debug.Log ("Stopped !");
						}

						foreach (FollowingController follower in fowllowers) {
							follower.StartCoroutine ("Stop");
						}
					}
				} else {
					animator.SetFloat (ANIMATOR_PARAM_SPEED, -1f);
					animator.SetBool ("Attacking", true);

					attackingTarget.GetComponent<Animator> ().SetBool ("Attacking", true);

					Camera.main.GetComponent<CameraComponentManager> ().StartBattleMode ();
				}
			} else {
				float speed = Mathf.Lerp (animator.GetFloat (ANIMATOR_PARAM_SPEED), this.targetSpeed, Time.deltaTime * SPEED_LERP_SCALE);
				animator.SetFloat (ANIMATOR_PARAM_SPEED, speed);

				foreach (FollowingController follower in fowllowers) {
					follower.StartCoroutine ("ChangeSpeed", speed);
				}
			}
		}
	}

	void OnTriggerEnter (Collider collider)
	{
		if (collider.CompareTag ("Battlefield")) {
			this.movingTarget = collider.transform.FindChild ("Anchors").GetChild (this.index);
			this.attackingTarget = collider.transform.FindChild ("Monsters").GetChild (this.index);

			ChangeDirection ();

			animator.SetTrigger ("AttackRun");
		}
	}

	public void StopAttacking ()
	{
		this.attackingTarget = null;
		this.movingTarget = pathNodes [this.targetNodeIndex];
		this.targetSpeed = -1f;

		animator.SetBool ("Attacking", false);

		foreach (FollowingController follower in fowllowers) {
			follower.StartCoroutine ("StopAttacking", this.movingTarget);
		}
	}

	public void StartMoving ()
	{
		ChangeDirection ();

		animator.SetFloat (ANIMATOR_PARAM_SPEED, 0.5f);

		foreach (FollowingController follower in fowllowers) {
			follower.StartCoroutine ("StartMoving");
		}


	}

	public void SwitchMoving ()
	{
		if (animator.GetFloat (ANIMATOR_PARAM_SPEED) < 0) {
			StartMoving ();
		} else {
			this.targetSpeed = -1f;
		}
	}

	private void ChangeDirection ()
	{
		Vector3 direction = movingTarget.position - transform.position;
		direction.y = 0;
		transform.rotation = Quaternion.LookRotation (direction, Vector3.up);

		this.targetSpeed = movingTarget.GetComponent<TerrainSpeedInfo> ().speed;
	}

	public void Jump ()
	{
		if (animator.GetFloat (ANIMATOR_PARAM_SPEED) > 0) {
			int jumpTpye = Random.Range (1, 3);
			animator.SetTrigger ("Jump " + jumpTpye);
		
			foreach (FollowingController follower in fowllowers) {
				follower.StartCoroutine ("Jump", jumpTpye);
			}
		}
	}

	public void Swipe (int direction)
	{
//		Camera.main.transform.rotation.eulerAngles.x
//		transform.Translate (new Vector3 (direction * 1, 0, 0));

		iTween.MoveBy(gameObject, iTween.Hash("x", direction * 5));

		this.movingTarget.Translate (new Vector3 (direction * 1, 0, 0));

		foreach (FollowingController follower in fowllowers) {
			follower.StartCoroutine ("Swipe", direction);
		}

		for (int i = 0; i < runningPath.childCount; i++) {
			pathNodes [i].position = pathNodes [i].position + new Vector3 (direction * 1, 0, 0);
		}
	}
}
