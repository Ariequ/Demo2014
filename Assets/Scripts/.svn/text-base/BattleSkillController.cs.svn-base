using UnityEngine;
using System.Collections;

public class BattleSkillController : MonoBehaviour {

	public GameObject effect_1;

	public GameObject effect_2;

	public GameObject joinAttack;

	public Transform uiRoot;

	public Transform monsters;

	public Transform swordmans;

	private TerrainData terrainData;

	private bool needDisappearBattlefield;

	void Start()
	{
		this.terrainData = Terrain.activeTerrain.terrainData;

		StartCoroutine(HideAttackControlUI());
	}

	void Update()
	{
		if(this.needDisappearBattlefield)
		{
			Vector3 position = transform.position;
			position.y = Mathf.Lerp(position.y, -5, Time.deltaTime * 0.1f);
			transform.position = position;

			if(position.y < -0.3f)
			{
				CharactorController[] charactorControllers = swordmans.GetComponentsInChildren<CharactorController>();

				foreach(CharactorController charactorController in charactorControllers)
				{
					charactorController.StartMoving();
				}

				Camera.main.GetComponent<CameraComponentManager>().StartFollowMode();

				uiRoot.FindChild("Movement").gameObject.SetActive(true);

				Destroy(gameObject);
			}
		}
	}

	void OnTriggerEnter(Collider collider)			// TODO zhi xing le 3 ci
	{
		uiRoot.FindChild("FinalAttack").gameObject.SetActive(true);
		uiRoot.FindChild("Movement").gameObject.SetActive(false);

		Camera.main.GetComponent<CameraComponentManager>().targetAnchor = transform.FindChild("CameraAnchor");
	}
	
	public void StartSkill_1()
	{
		GameObject skillInstance = Instantiate(effect_1, GetTerrainPosition(), Quaternion.identity) as GameObject;

		StartCoroutine(OnGetHit("GetHit", 1.0f));
	}

	public void StartSkill_2()
	{
		GameObject skillInstance = Instantiate(effect_2, GetTerrainPosition(), Quaternion.identity) as GameObject;

		StartCoroutine(OnGetHit("GetHit", 1.0f));
	}

	public void JoinAttack()
	{
		GameObject skillInstance = Instantiate(joinAttack, GetTerrainPosition(), Quaternion.identity) as GameObject;

		CharactorController[] charactorControllers = swordmans.GetComponentsInChildren<CharactorController>();
		
		foreach(CharactorController charactorController in charactorControllers)
		{
			charactorController.StopAttacking();
		}

		StartCoroutine(OnGetHit("Die", 1.0f, -1));
		StartCoroutine(DisappearMonsters());
	}

	private IEnumerator HideAttackControlUI()
	{
		yield return new WaitForSeconds(0.5f);

		uiRoot.FindChild("FinalAttack").gameObject.SetActive(false);
		uiRoot.FindChild("Movement").gameObject.SetActive(true);
	}

	private IEnumerator OnGetHit(string animatorParam, float delay = 1.0f, int random = 3)
	{
		yield return new WaitForSeconds(delay);

		for(int i = 0; i < monsters.childCount; i++)
		{
			if(Random.Range(0, 10) > random)
			{
				Animator animator = monsters.GetChild(i).GetComponent<Animator>();
				animator.SetTrigger(animatorParam);
			}
		}
	}

	private IEnumerator DisappearMonsters()
	{
		uiRoot.FindChild("FinalAttack").gameObject.SetActive(false);

		yield return new WaitForSeconds(5f);

		this.needDisappearBattlefield = true;
	}

	private Vector3 GetTerrainPosition()
	{
		Vector3 position = transform.position;
		position.y = terrainData.GetInterpolatedHeight(position.x / terrainData.size.x, position.z / terrainData.size.z) - 0.1f;

		return position;
	}
}
