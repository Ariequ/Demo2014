using UnityEngine;
using System.Collections;

public class Follow : MonoBehaviour
{
    public float timeOffset;
    public int AttackingIndex;
    private Vector3 moveDir = Vector3.zero;
    private CharacterController characterController;
    private Animator animator;
    private Controller.Position positionStand;

    // Use this for initialization
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        positionStand = Controller.Position.Middle;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (moveDir != Vector3.zero)
        {
            CheckLane();
            characterController.Move(moveDir * Time.deltaTime);
            animator.SetFloat("Speed", 5);
        }
        else
        {
            animator.SetFloat("Speed", -1);
        }

        characterController.Move(Physics.gravity * Time.deltaTime);
    }

    IEnumerator ChangeMoveDir(Vector3 moveDir)
    {
        yield return new WaitForSeconds(timeOffset);
        this.moveDir = moveDir;
    }

    IEnumerator ChangeJumpType(int type)
    {
        yield return new WaitForSeconds(timeOffset);
        animator.SetTrigger("Jump " + type);
    }

    IEnumerator MoveToEnemy(Collider collider)
    {
        Vector3 movingTarget = collider.transform.FindChild("Anchors").GetChild(this.AttackingIndex).position;

        Transform attackingTarget = collider.transform.FindChild("Monsters").GetChild(this.AttackingIndex);

        while (Mathf.Abs(transform.position.z - movingTarget.z) > 0.1)
        {
            iTween.MoveTo(gameObject, iTween.Hash("position", movingTarget, "time", 2f));
            animator.SetFloat("Speed", 5f);
            yield return 1;
        }

        animator.SetFloat("Speed", -1f);
        animator.SetBool("Attacking", true);
        transform.rotation = Quaternion.LookRotation(attackingTarget.position - transform.position);
        
        attackingTarget.GetComponent<Animator>().SetBool("Attacking", true);
    }

    IEnumerator BackToLine(Vector3 leaderPostion)
    {
        yield return new WaitForSeconds(timeOffset);
        Vector3 movingTarget = leaderPostion;
        movingTarget.z -= this.AttackingIndex * 0.8f;

        iTween.MoveTo(gameObject, iTween.Hash("position", movingTarget, "time", 0.5f));
        transform.rotation = Quaternion.identity;

        animator.SetBool("Attacking", false);
    }

    IEnumerator ChangeStandPostion(Controller.Position standPos)
    {
        yield return new WaitForSeconds(timeOffset);
        positionStand = standPos;
    }

    private void CheckLane()
    {
        if (positionStand == Controller.Position.Middle)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(0, transform.position.y, transform.position.z), 6 * Time.deltaTime);
        }
        else
        if (positionStand == Controller.Position.Left)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(-1.8f, transform.position.y, transform.position.z), 6 * Time.deltaTime);
        }
        else
        if (positionStand == Controller.Position.Right)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(1.8f, transform.position.y, transform.position.z), 6 * Time.deltaTime);
        }
    }


}
