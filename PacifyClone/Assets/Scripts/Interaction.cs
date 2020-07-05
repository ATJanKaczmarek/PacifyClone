using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum ActivationTypes
{
    SoundActivation, AnimationActivation, //...
}

public class Interaction : MonoBehaviour
{
    [Header("General")]
    public KeyCode interactionKey = KeyCode.E;
    public bool activated = false;
    public string interactMessage = "Use 'E' to interact";
    public TMP_Text interactionText;
    public GameObject UI;
    public ActivationTypes type;
    //public SphereCollider interactionRange;
    public bool playerInInteractionRange;
    public int maxRange;
    public int minRange;
    private Vector3 playerPos;
    private GameObject player;

    [Header("Sound")]
    public AudioClip sound;
    public AudioSource source;

    [Header("Animation")]
    public Animation animClip;
    public Animator animator;

    private void Start()
    {
        UI.SetActive(false);
        player = GameObject.FindWithTag("Player");
        playerPos = player.transform.position;
    }

    private void Update()
    {
        if ((Vector3.Distance(transform.position, player.transform.position) < maxRange) && (Vector3.Distance(transform.position, player.transform.position) > minRange))
        {
            playerInInteractionRange = true;
        }
        else
        {
            playerInInteractionRange = false;
        }

        
        if(Camera.main != null)
        {
            RaycastHit hit1;
            Ray ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray1, out hit1))
            {
                if (hit1.transform != null)
                {
                    if (hit1.collider.gameObject.tag == "Interactable" && playerInInteractionRange)
                    {
                        interactionText.text = interactMessage;
                        UI.SetActive(true);

                        if (Input.GetKeyDown(interactionKey))
                            Interact();
                    }
                    else
                    {
                        interactionText.text = interactMessage;
                        UI.SetActive(false);

                    }
                }
            }
        }
    }

    public void Interact()
    {
        if (activated)
        {
            activated = false;
        }
        else if(!activated)
        {
            activated = true;
        }

        if(type == ActivationTypes.SoundActivation && activated)
        {
            source.clip = sound;
            source.Play();
        }
        else if(type == ActivationTypes.SoundActivation && !activated)
        {
            source.Stop();
        }
    }
}
