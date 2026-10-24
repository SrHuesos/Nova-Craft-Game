
using UnityEngine;
using UnityEngine.AI;

public class EnemyChase : MonoBehaviour
{
    [Header("NavMesh")]
    public NavMeshAgent navMeshAgent;

    [Header("Movement")]
    public float startWaitTime = 4f;
    public float timeToRotate = 2f;
    public float speedWalk = 3.5f;
    public float speedRun = 5f;

    [Header("Vision")]
    public float viewRadius = 10f;
    [Range(0f, 360f)]
    public float viewAngle = 90f;
    public LayerMask playerMask;
    public LayerMask obstacleMask;
    public float eyeHeight = 1.5f;

    [Header("Patrol")]
    public Transform[] waypoints;

    [Header("Chase")]
    public float catchDistance = 2.5f;

    private int m_CurrentWaypointIndex;
    private float m_WaitTime;
    private float m_TimeToRotate;

    private bool m_PlayerInRange;
    private bool m_IsPatrol = true;
    private bool m_CaughtPlayer;

    private Transform m_Player;
    private Vector3 m_PlayerPosition;
    private Vector3 playerLastPosition;

    void Start()
    {
        if (navMeshAgent == null)
            navMeshAgent = GetComponent<NavMeshAgent>();

        if (navMeshAgent == null)
        {
            Debug.LogError("EnemyChase necesita un NavMeshAgent.", this);
            enabled = false;
            return;
        }

        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("Asigna al menos un waypoint al enemigo.", this);
            enabled = false;
            return;
        }

        m_WaitTime = startWaitTime;
        m_TimeToRotate = timeToRotate;
        m_CurrentWaypointIndex = 0;

        m_PlayerInRange = false;
        m_IsPatrol = true;
        m_CaughtPlayer = false;

        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speedWalk;
        navMeshAgent.SetDestination(
            waypoints[m_CurrentWaypointIndex].position
        );
    }

    void Update()
    {
        if (m_CaughtPlayer || !navMeshAgent.isOnNavMesh)
            return;

        Environment();

        if (m_PlayerInRange)
        {
            m_IsPatrol = false;
            Chasing();
        }
        else
        {
            if (!m_IsPatrol)
            {
                // El jugador salió del campo de visión.
                m_IsPatrol = true;
                m_WaitTime = startWaitTime;
                m_TimeToRotate = timeToRotate;
                Move(speedWalk);
                navMeshAgent.SetDestination(
                    waypoints[m_CurrentWaypointIndex].position
                );
            }

            Patrolling();
        }
    }

    void Environment()
    {
        m_PlayerInRange = false;
        m_Player = null;

        Collider[] players = Physics.OverlapSphere(
            transform.position,
            viewRadius,
            playerMask
        );

        float bestDistance = float.MaxValue;

        foreach (Collider playerCollider in players)
        {
            Hideable hideable =
                playerCollider.GetComponentInParent<Hideable>();

            // Ignorar al jugador si está escondido.
            if (hideable != null && hideable.IsHidden)
                continue;

            Transform candidate = playerCollider.transform;

            Vector3 direction =
                candidate.position - transform.position;

            float distance = direction.magnitude;

            if (distance >= bestDistance || distance == 0f)
                continue;

            float angle = Vector3.Angle(
                transform.forward,
                direction
            );

            if (angle > viewAngle / 2f)
                continue;

            Vector3 eyePosition =
                transform.position + Vector3.up * eyeHeight;

            Vector3 targetPosition = candidate.position;

            Vector3 rayDirection = targetPosition - eyePosition;

            float rayDistance = rayDirection.magnitude;

            if (rayDistance <= 0f)
                continue;

            bool blocked = Physics.Raycast(
                eyePosition,
                rayDirection.normalized,
                rayDistance,
                obstacleMask,
                QueryTriggerInteraction.Ignore
            );

            if (blocked)
                continue;

            bestDistance = distance;
            m_Player = candidate;
        }

        if (m_Player != null)
        {
            m_PlayerInRange = true;
            m_PlayerPosition = m_Player.position;
            playerLastPosition = m_PlayerPosition;
        }
    }

    void Chasing()
    {
        if (m_Player == null)
            return;

        float distance = Vector3.Distance(
            transform.position,
            m_Player.position
        );

        if (distance > catchDistance)
        {
            Move(speedRun);
            navMeshAgent.SetDestination(m_Player.position);
        }
        else
        {
            Stop();

            // Mantener al enemigo orientado hacia el jugador.
            Vector3 direction =
                m_Player.position - transform.position;

            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion rotation = Quaternion.LookRotation(direction);

                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    rotation,
                    navMeshAgent.angularSpeed * Time.deltaTime
                );
            }

            // Detenerse a la distancia de captura.
            // Aquí puedes añadir la lógica de ataque.
        }
    }

    void Patrolling()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;

        if (navMeshAgent.isStopped)
            Move(speedWalk);

        Transform waypoint = waypoints[m_CurrentWaypointIndex];

        if (waypoint == null)
            return;

        navMeshAgent.SetDestination(waypoint.position);

        if (!navMeshAgent.pathPending &&
            navMeshAgent.remainingDistance <=
            navMeshAgent.stoppingDistance)
        {
            Stop();
            m_WaitTime -= Time.deltaTime;

            if (m_WaitTime <= 0f)
            {
                NextPoint();
                m_WaitTime = startWaitTime;
                m_TimeToRotate = timeToRotate;
                Move(speedWalk);
            }
        }
    }

    void Move(float speed)
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speed;
    }

    void Stop()
    {
        navMeshAgent.isStopped = true;
    }

    public void NextPoint()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;

        m_CurrentWaypointIndex =
            (m_CurrentWaypointIndex + 1) % waypoints.Length;

        if (waypoints[m_CurrentWaypointIndex] != null)
        {
            navMeshAgent.SetDestination(
                waypoints[m_CurrentWaypointIndex].position
            );
        }
    }

    public void CaughtPlayer()
    {
        m_CaughtPlayer = true;
        Stop();
    }

    void OnDrawGizmosSelected()
    {
        // Radio de detección.
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        // Límites del campo de visión.
        Vector3 leftBoundary = DirectionFromAngle(
            -viewAngle / 2f
        );

        Vector3 rightBoundary = DirectionFromAngle(
            viewAngle / 2f
        );

        Gizmos.color = Color.cyan;

        Gizmos.DrawRay(
            transform.position,
            leftBoundary * viewRadius
        );

        Gizmos.DrawRay(
            transform.position,
            rightBoundary * viewRadius
        );
    }

    Vector3 DirectionFromAngle(float angle)
    {
        angle += transform.eulerAngles.y;

        return new Vector3(
            Mathf.Sin(angle * Mathf.Deg2Rad),
            0f,
            Mathf.Cos(angle * Mathf.Deg2Rad)
        );
    }
}
