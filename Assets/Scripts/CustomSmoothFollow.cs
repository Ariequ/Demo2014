﻿using UnityEngine;
using System.Collections;

public class CustomSmoothFollow : MonoBehaviour 
{
    public Transform target;

    public float distance = 10.0f;
    public float height = 5.0f;

    public float heightDamping = 2.0f;
    public float rotationDamping = 3.0f;
	
//    void LateUpdate () 
//	{
//        // Early out if we don't have a target
//        if (target != null) 
//		{
//			// Calculate the current rotation angles
////			var wantedRotationAngle = target.eulerAngles.y;
//			var wantedHeight = target.position.y + height;
//			
//			var currentRotationAngle = transform.eulerAngles.y;
//			var currentHeight = transform.position.y;
//			
//			// Damp the rotation around the y-axis
////			currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
//			
//			// Damp the height
//			currentHeight = heightDamping > 0 ? Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime) : wantedHeight;
//			
//			// Convert the angle into a rotation
//			var currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
//			
//			// Set the position of the camera on the x-z plane to:
//			// distance meters behind the target
//			//        transform.position = target.position;
//			//        transform.position -= currentRotation * Vector3.forward * distance;
//			//        
//			//        // Set the height of the camera
//			//        transform.position.y = currentHeight;
//			
//			Vector3 temp = target.position;
//			temp -= currentRotation * Vector3.forward * distance;
//			temp.y = currentHeight;
//			transform.position = temp;
//			
//			// Always look at the target
//			transform.LookAt (target);
//		}
//    }

	void LateUpdate () 
	{
		// Early out if we don't have a target
		if (target != null) 
		{
			// Calculate the current rotation angles
			float wantedRotationAngle = target.eulerAngles.y;
			float wantedHeight = target.position.y + height;
			
			float currentRotationAngle = transform.eulerAngles.y;
			float currentHeight = transform.position.y;
			
			// Damp the rotation around the y-axis
			currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
			
			// Damp the height
			currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
			
			// Convert the angle into a rotation
			var currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
			
			// Set the position of the camera on the x-z plane to:
			// distance meters behind the target
			Vector3 position = target.position;
			position.x = 0;
			position -= currentRotation * Vector3.forward * distance;
			
			// Set the height of the camera
			position.y = currentHeight;

			transform.position = position;
			
			// Always look at the target
//			transform.LookAt (target);
		}
	}
}