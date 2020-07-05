using System.Collections.Generic;
using System.Linq;
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
        searchingSound,
    }

    #region Variables
    #region public

    public float fovAngle = 110f;
    public bool playerInSight;
    public Vector3 personalLastSighting;
    public float roamRadius;
    public float soundSourceShutdownRadius = 5f;

    #endregion public
    #region private

    private NavMeshAgent nav;
    private SphereCollider col;
    private GameObject player;
    private Vector3 previousSighting;
    private Vector3 soundSourcePosition;
    private enemyStates states;
    private Node[] n;
    private Queue<Node> q;
    private bool isWalking;
    private bool soundSourcePlaying;
    private Animator aiAnimator;
    private List<AudioSource> soundSourcePool;
    private Interaction interaction;

    #endregion private
    #endregion Variables

    private void Awake()
    {
        // Assining necessary values & components to variables in beginning
        nav = GetComponent<NavMeshAgent>();
        col = GetComponent<SphereCollider>();
        GameObject[] playerArray = GameObject.FindGameObjectsWithTag("Player");
        player = playerArray[0];
        aiAnimator = gameObject.transform.GetChild(0).GetComponent<Animator>();

        // Getting all sound sources, then filter every gameobject with tag: "PlayerAudioSource" out.
        soundSourcePool = new List<AudioSource>();
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource sourceUnfiltered in allAudioSources)
        {
            if (sourceUnfiltered.gameObject.tag != "PlayerAudioSource")
            {
                soundSourcePool.Add(sourceUnfiltered);
                Debug.Log("Added: " + sourceUnfiltered.gameObject.name);
            }
            else
            {
                Debug.Log("Filtered: " + sourceUnfiltered.gameObject.name);
            }
        }
        Debug.Log("Total list: " + soundSourcePool.Count);

        // Getting all Node-Objects and put them into a queue -> Waypoints for 'enemyStates.roaming'
        n = FindObjectsOfType<Node>();
        q = new Queue<Node>();
        foreach (Node node in n)
        {
            q.Enqueue(node);
        }
    }

    private void Update()
    {
        CheckSoundSources();
        AnimationHandler();
        ChangeNodePercentage();
        Node node = GetClosestNode(transform.position);
        Debug.Log("==== NODE ====: " + node.name);

        if (personalLastSighting != previousSighting)
        {
            states = enemyStates.hunting;
        }
        else if (personalLastSighting == previousSighting && !soundSourcePlaying)
        {
            states = enemyStates.roaming;
        }
        else if (soundSourcePlaying)
        {
            states = enemyStates.searchingSound;
        }

        // Checking for states
        switch (states)
        {
            case enemyStates.hunting:
                isWalking = true;
                nav.SetDestination(personalLastSighting);
                previousSighting = personalLastSighting;
                Debug.Log("Hunting Player");
                break;
            case enemyStates.attacking:
                break;
            case enemyStates.roaming:
                isWalking = true;
                Roaming();
                Debug.Log("Roaming");
                break;
            case enemyStates.patroling:
                break;
            case enemyStates.searchingSound:
                nav.SetDestination(soundSourcePosition);
                Debug.Log("Searching Sound Source");
                break;
            default:
                break;
        }
    }

    // Checking if AI sees player and calculating distance to player's position
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
                if (Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, col.radius))
                {
                    if (hit.collider.gameObject == player)
                    {
                        playerInSight = true;
                    }
                }
            }

            if (CalculatePathLength(player.transform.position) <= col.radius)
            {
                personalLastSighting = player.transform.position;
            }
        }
    }

    // Checking if player leaves Trigger of AI and setting it's value to false if yes
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            playerInSight = false;
        }
    }

    // Method for calculating Corners
    private float CalculatePathLength(Vector3 targetPosition)
    {
        NavMeshPath path = new NavMeshPath();
        if (nav.enabled)
            nav.CalculatePath(targetPosition, path);
        Vector3[] allWayPoints = new Vector3[path.corners.Length + 2];
        allWayPoints[0] = transform.position;
        allWayPoints[allWayPoints.Length - 1] = targetPosition;

        for (int i = 0; i < path.corners.Length; i++)
        {
            allWayPoints[i + 1] = path.corners[i];
        }

        float pathLength = 0f;
        for (int i = 0; i < allWayPoints.Length - 1; i++)
        {
            pathLength += Vector3.Distance(allWayPoints[i], allWayPoints[i + 1]);
        }
        return pathLength;
    }

    // Methon for Roaming state
    private void Roaming()
    {
        if (!nav.pathPending)
        {
            if (nav.remainingDistance <= nav.stoppingDistance)
            {
                if (!nav.hasPath || nav.velocity.sqrMagnitude == 0f)
                {                    
                    if (q.Count > 0)
                    {
                        Vector3 pos = q.Peek().position;
                        q.Dequeue();
                        nav.SetDestination(pos);
                    }
                    else
                    {
                        foreach (Node node in n)
                        {
                            q.Enqueue(node);
                        }
                        nav.SetDestination(transform.position);
                    }
                }
            }
        }
    }

    // Methond for handling animations of AI
    private void AnimationHandler()
    {
        if (isWalking)
        {
            aiAnimator.SetBool("isWalking", true);
        }
        else if (!isWalking)
        {
            aiAnimator.SetBool("isWalking", false);
        }
    }

    // Method for checking is SoundSources in pool
    private void CheckSoundSources()
    {
        foreach (AudioSource source in soundSourcePool)
        {
            if(source.isPlaying == true)
            {
                if(CalculatePathLength(source.transform.position) <= col.radius)
                {
                    soundSourcePlaying = true;
                    soundSourcePosition = source.transform.position;
                }

                if (Vector3.Distance(transform.position, soundSourcePosition) <= soundSourceShutdownRadius && states == enemyStates.searchingSound)
                {
                    interaction = source.gameObject.GetComponent<Interaction>();
                    interaction.Interact();
                    soundSourcePlaying = false;
                }
            }            
        }
    }


    // Gets closest Node GO
    private Node GetClosestNode(Vector3 pos)
    {
        Node nodery = new Node();
        float[] distances = new float[n.Length];
        for (int i = 0; i < n.Length; i++)
        {
            distances[i] = Vector3.Distance(n[i].transform.position, pos);
        }
        float smallest = distances.Min();
        for (int i = 0; i < n.Length; i++)
        {
            if(smallest == Vector3.Distance(n[i].transform.position, pos))
            {
                nodery = n[i];
            }
        }
        return nodery;
    }

    private void ChangeNodePercentage()
    {
        Node playerNearest = GetClosestNode(player.transform.position);
        Node aiNearest = GetClosestNode(transform.position);

        if (playerNearest.gameObject == aiNearest.gameObject && playerInSight)
        {
            aiNearest.probability = 100;
        }
        else if (playerNearest.gameObject != aiNearest)
        {
            aiNearest.probability = 25;
        }
    }
}
