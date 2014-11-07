using UnityEngine;
using System.Collections;

public class CameraComponentManager : MonoBehaviour {

	public Transform targetAnchor;

	private CustomSmoothFollow customSmoothFollow;
	private CameraDistanceController cameraDistanceController;

	// Use this for initialization
	void Start () {
	
		this.customSmoothFollow = GetComponent<CustomSmoothFollow>();
		this.cameraDistanceController = GetComponent<CameraDistanceController>();
	}

	void Update ()
	{
		if (this.targetAnchor != null && ! customSmoothFollow.enabled)
		{
			transform.position = Vector3.Lerp(transform.position, targetAnchor.position, Time.deltaTime);
			transform.LookAt(customSmoothFollow.target);
		}
	}
	
	public void StartFollowMode()
	{
		customSmoothFollow.enabled = true;
		cameraDistanceController.enabled = true;

		this.targetAnchor = null;
	}

	public void StartBattleMode()
	{
		customSmoothFollow.enabled = false;
		cameraDistanceController.enabled = false;
	}
}
