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
    private bool canRoam = true;

    #endregion private
    #endregion Variables

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        col = GetComponent<SphereCollider>();
        GameObject[] playerArray = GameObject.FindGameObjectsWithTag("Player");
        player = playerArray[0];

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
                StartCoroutine(Roaming());
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

    private Vector3 GetNextRoamVector(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

    private IEnumerator Roaming()
    {
        while(states == enemyStates.roaming && canRoam)
        {
            canRoam = false;
            yield return new WaitForSeconds(5f);
            nav.SetDestination(GetNextRoamVector(100f));
            canRoam = true;
        }  
    }
}
