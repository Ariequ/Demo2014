using UnityEngine;
using System.Collections;

public class CameraDistanceController : MonoBehaviour {

	public float distance = 5f;

	public float height = 3f;

	public float smoothSpeed = 1.5f;

	private CustomSmoothFollow smoothFollow;

	private Animator charactorAnimator;

	void Start () 
	{
		this.smoothFollow = GetComponent<CustomSmoothFollow>();
		this.charactorAnimator = smoothFollow.target.GetComponent<Animator>();
	}

	void LateUpdate () 
	{
		float charactorAnimatorSpeed = charactorAnimator.GetFloat("Speed");

		float speed = Time.deltaTime * smoothSpeed;
//		float distance = 4f;
//		float height = 2f;

		if (charactorAnimatorSpeed > 0)
		{
			distance = this.distance * charactorAnimatorSpeed;
			height = this.height * charactorAnimatorSpeed;
		}

		smoothFollow.distance = Mathf.Lerp(smoothFollow.distance, distance, speed);
		smoothFollow.height = Mathf.Lerp(smoothFollow.height, height, speed);
	}

	void OnMouseDown()
	{
		charactorAnimator.SetBool("Jump", true);

		Debug.Log("OnMouseDown");
	}
}
