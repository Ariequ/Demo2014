using UnityEngine;
using System.Collections;

public class RootMotionController : MonoBehaviour {

	private Animator animator;

	private CharacterController characterController;

	void Start()
	{
		this.animator = GetComponent<Animator>(); 
		this.characterController = GetComponent<CharacterController>();
	}

	void OnAnimatorMove()
	{
		Vector3 distance = transform.forward * animator.GetFloat("Runspeed") * Time.deltaTime;
		characterController.Move(distance + Physics.gravity);
	}
}
