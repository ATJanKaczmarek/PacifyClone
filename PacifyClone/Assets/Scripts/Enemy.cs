using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private enum enemyStates
    {
        hunting,
        attacking,
        roaming,
        patroling,
    }

    #region Variables
    #region public

    public float fovAngle = 110f;
    public bool playerInSight;
    public Vector3 personalLastSighting;
    public float roamRadius;

    #endregion public
    #region private

    private NavMeshAgent nav;
    private SphereCollider col;
    private GameObject player;
    private Vector3 previousSighting;
    private enemyStates states;
    private Node[] n;
    private Queue<Node> q;

    #endregion private
    #endregion Variables

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        col = GetComponent<SphereCollider>();
        GameObject[] playerArray = GameObject.FindGameObjectsWithTag("Player");
        player = playerArray[0];

        n = FindObjectsOfType<Node>();
        q = new Queue<Node>();
        foreach (Node node in n)
        {
            q.Enqueue(node);
        }
    }



    private void Update()
    {
        if(personalLastSighting != previousSighting)
        {
            states = enemyStates.hunting;
        }
        else if (personalLastSighting == previousSighting)
        { 
            states = enemyStates.roaming;
        }
        //TODO: roaming
        switch (states)
        {
            case enemyStates.hunting:
                nav.SetDestination(personalLastSighting);
                previousSighting = personalLastSighting;
                Debug.Log("Hunting Player");
                break;
            case enemyStates.attacking:
                break;
            case enemyStates.roaming:
                Roaming();
                Debug.Log("Roaming");
                break;
            case enemyStates.patroling:
                break;
            default:
                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == player)
        {
            playerInSight = false;
            Vector3 direction = other.transform.position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward);
            if (angle < (fovAngle / 2))
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, col.radius)) {
                    if (hit.collider.gameObject == player)
                    {
                        playerInSight = true;
                    }
                }
            }

            if(CalculatePathLength(player.transform.position) <= col.radius)
            {
                personalLastSighting = player.transform.position;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            playerInSight = false;
        }
    }

    private float CalculatePathLength(Vector3 targetPosition)
    {
        NavMeshPath path = new NavMeshPath();
        if(nav.enabled)
            nav.CalculatePath(targetPosition, path);
        Vector3[] allWayPoints = new Vector3[path.corners.Length + 2];
        allWayPoints[0] = transform.position;
        allWayPoints[allWayPoints.Length - 1] = targetPosition;

        for (int i = 0; i < path.corners.Length; i++)
        {
            allWayPoints[i + 1] = path.corners[i];
        }

        float pathLength = 0f;
        for (int i = 0; i < allWayPoints.Length-1; i++)
        {
            pathLength += Vector3.Distance(allWayPoints[i], allWayPoints[i + 1]);
        }
        return pathLength;
    }

    private Vector3 GetNextRoamVector()
    {
        if(q.Count > 0)
        {
            Vector3 pos = q.Peek().position;
            q.Dequeue();
            return pos;
        }
        else
        {
            foreach (Node node in n)
            {
                q.Enqueue(node);
            }
            return transform.position;
        }
    }

    private void Roaming()
    {
        if (!nav.pathPending)
        {
            if (nav.remainingDistance <= nav.stoppingDistance)
            {
                if (!nav.hasPath || nav.velocity.sqrMagnitude == 0f)
                {
                    nav.SetDestination(GetNextRoamVector());
                }
            }
        }
    }
}
