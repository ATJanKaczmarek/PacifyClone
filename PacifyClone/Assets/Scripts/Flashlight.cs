using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public GameObject lightSource;
    private bool flashlightOn;

    public AudioSource flashlightTrigger;

    private Light lightComponent;
    private Animator animator;
    private void Start()
    {
        lightComponent = lightSource.GetComponent<Light>();
        animator = GetComponent<Animator>();
        lightSource.SetActive(false);
        flashlightOn = false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F) && !flashlightOn)
        {
            animator.SetBool("FlashLightOn", true);  
            flashlightOn = true;
            StartCoroutine(lightDelayed());
            StartCoroutine(flickering());
        }
        else if(Input.GetKeyDown(KeyCode.F) && flashlightOn)
        {
            flashlightOn = false;
            StartCoroutine(lightDelayed());
        }
    }

    private IEnumerator lightDelayed()
    {
        if(flashlightOn)
        {
            yield return new WaitForSeconds(0.2f);
            flashlightTrigger.Play();
            lightSource.SetActive(true);
        }
        else
        {
            flashlightTrigger.Play();
            lightSource.SetActive(false);
            yield return new WaitForSeconds(0.15f);
            animator.SetBool("FlashLightOn", false);
        }
    }

    private IEnumerator flickering()
    {
        if(flashlightOn)
        {
            yield return new WaitForSeconds(10f);
            lightComponent.intensity = 999999f;
            yield return new WaitForSeconds(0.1f);
            lightComponent.intensity = 9999999f;
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(flickering());
        }
    }
}
