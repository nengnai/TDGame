using UnityEngine;
using UnityEngine.AI;
public class AnimNavMeshControlle : MonoBehaviour
{
    Animator animator;
    private MoveController moveController;
    void Awake()
    {
        animator = GetComponent<Animator>();
        moveController = GetComponent<MoveController>();
    }
    void Update()
    {
        bool isMoving = moveController.IsMoving();
        animator.SetBool("isWalking", isMoving);
    }
}