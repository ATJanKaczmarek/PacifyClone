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
    public SphereCollider interactionZone;
    public bool canInteract;

    [Header("Sound")]
    public AudioClip sound;
    public AudioSource source;

    [Header("Animation")]
    public Animation animClip;
    public Animator animator;

    private void Start()
    {
        UI.SetActive(false);
    }

    private void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform != null)
            {
                if (hit.transform.gameObject.tag == "Interactable" && canInteract)
                {
                    interactionText.text = interactMessage;
                    UI.SetActive(true);

                    if(Input.GetKeyDown(interactionKey))
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

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            canInteract = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            canInteract = false;
        }
    }

}
