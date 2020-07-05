using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    // Light
    private Light _light;
    // Mesh Renderer
    private Renderer flashlightGraphics;
    // Sound
    private AudioSource audioSource;
    // Animation
    private Animator animator;
    public float waitTime;
    public float animTimer = 0f; // used to wait for animation til the player can use the flashlight again
    private void Start()
    {
        _light = gameObject.transform.GetChild(0).GetChild(0).GetComponent<Light>();
        flashlightGraphics = gameObject.transform.GetChild(0).GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();  
        animator = GetComponent<Animator>();

        _light.enabled = false;
        flashlightGraphics.enabled = false;
    }

    private void Update()
    {
        if (animTimer > 0.0f)
        {
            animTimer -= Time.deltaTime;
            if (animTimer < 0.0f)
            {
                animTimer = 0.0f;
            }
        }

        if (Input.GetKeyDown(KeyCode.F) && !_light.enabled) // if player presses F and the light is disabled
        {
            flashlightGraphics.enabled = true;
            animator.SetBool("FlashLightOn", true);
            Invoke("ActivateFlashLight", 0.15f);
        }
        else if (Input.GetKeyDown(KeyCode.F) && _light.enabled) // if player presses F and the light is enabled
        {
            _light.enabled = false;
            audioSource.Play();
            animator.SetBool("FlashLightOn", false);
            Invoke("DeactivateFlashlight", 0.15f);
        }
    }

    private void ActivateFlashLight()
    {
        audioSource.Play();
        _light.enabled = true;
    }

    private void DeactivateFlashlight()
    {
        flashlightGraphics.enabled = false;
    }
}
