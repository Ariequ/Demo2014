using UnityEngine;
using System.Collections;

public class FollowingController : MonoBehaviour {
	public float timeOffset = 0.1f;

	private Transform movingTarget;
	private Animator animator;
	private static string ANIMATOR_PARAM_SPEED = "Speed";
	public int index;

	private Transform attackingTarget;
	private float targetSpeed;
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (this.attackingTarget != null && Vector3.Distance(attackingTarget.position, transform.position) < 4)
		{
			animator.SetBool("Attacking", true);
			animator.SetFloat(ANIMATOR_PARAM_SPEED, -1f);
			attackingTarget.GetComponent<Animator>().SetBool("Attacking", true);
		}
	}

	IEnumerator ChangeMovingTarget(Transform target)
	{
		yield return new WaitForSeconds(timeOffset);
		movingTarget = target;
		ChangeDirection();
	}

	IEnumerator StartMoving()
	{
		yield return new WaitForSeconds(timeOffset);
		ChangeDirection();
		animator.SetFloat(ANIMATOR_PARAM_SPEED, 0.5f);
	}

	private void ChangeDirection()
	{
		Vector3 direction = movingTarget.position - transform.position;
		direction.y = 0;
		transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
		
		this.targetSpeed = movingTarget.GetComponent<TerrainSpeedInfo>().speed;
	}

	IEnumerator Jump(int jumpTpye)
	{
		yield return new WaitForSeconds(timeOffset);
		animator.SetTrigger("Jump " + jumpTpye);
	}

	IEnumerator ChangeSpeed(float speed)
	{
		yield return new WaitForSeconds(timeOffset);
		animator.SetFloat(ANIMATOR_PARAM_SPEED, speed);
	}

	IEnumerator EnterAttack()
	{
		yield return new WaitForSeconds(timeOffset);
		animator.SetBool("Attacking", true);
		animator.SetFloat(ANIMATOR_PARAM_SPEED, -1f);
		attackingTarget.GetComponent<Animator>().SetBool("Attacking", true);
	}

	IEnumerator StopAttacking(Transform movingTarget)
	{
		this.attackingTarget = null;
		this.targetSpeed = -1f;		
		this.movingTarget = movingTarget;
		animator.SetBool("Attacking", false);
		yield return null;
	}

	IEnumerator Stop()
	{
		yield return new WaitForSeconds(timeOffset);
		animator.SetFloat(ANIMATOR_PARAM_SPEED, -1f);
	}

	IEnumerator Swipe(int direction)
	{
		yield return new WaitForSeconds(timeOffset);

		iTween.MoveBy(gameObject, iTween.Hash("x", direction * 5));
		this.movingTarget.Translate(new Vector3(direction*1, 0, 0));
	}

	void OnTriggerEnter(Collider collider)
	{
		if (collider.CompareTag("Battlefield"))
		{
			this.movingTarget = collider.transform.FindChild("Anchors").GetChild(this.index);
			this.attackingTarget = collider.transform.FindChild("Monsters").GetChild(this.index);
			
			ChangeDirection();
			
			animator.SetTrigger("AttackRun");
		}
	}

	public void StopAttacking()
	{
		this.attackingTarget = null;

		this.targetSpeed = -1f;
		
		animator.SetBool("Attacking", false);
	}
}
