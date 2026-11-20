
using UnityEngine;
using UnityEngine.AI;

public class MovementAnimation : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    [SerializeField] private float movementThreshold = 0.1f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (agent == null || animator == null) return;

        bool isMoving = agent.velocity.magnitude > movementThreshold;

        animator.SetBool("IsMoving", isMoving);
    }
}
