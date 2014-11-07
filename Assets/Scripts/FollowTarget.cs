using UnityEngine;
using System.Collections;

public class FollowTarget : MonoBehaviour
{

	public Transform fowllowingTarget;
	private Vector3 fowllowOffset;


	// Use this for initialization
	void Start ()
	{
		fowllowOffset = transform.position - fowllowingTarget.position;
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.position = fowllowingTarget.position + fowllowOffset;
	}
}
