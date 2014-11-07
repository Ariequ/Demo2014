using UnityEngine;
using System.Collections;

public class SwordmanMovementController : MonoBehaviour {

	public void SwitchMove()
	{
		CharactorController[] charactorControllers = GetComponentsInChildren<CharactorController>();

		foreach (CharactorController charactorController in charactorControllers)
		{
			charactorController.SwitchMoving();
		}
	}
}
