using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    float speed = 3f;
    bool isPlayerMoving = false;
    Vector3 targetPos;
    Animator animator;
    int runHash;

    // Use this for initialization
    private void Start()
    {
        animator = GetComponent<Animator>();
        runHash = Animator.StringToHash("isWalking");
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            isPlayerMoving = true;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                targetPos = raycastHit.point;
            }
        }

        if (isPlayerMoving)
        {
            animator.SetBool(runHash, true);
            Vector3 targetLook = new Vector3(targetPos.x, 0f, targetPos.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            transform.LookAt(targetLook);
            if (transform.position == targetPos)
            {
                isPlayerMoving = false;
                animator.SetBool(runHash, false);
            }

        }
    }
}
