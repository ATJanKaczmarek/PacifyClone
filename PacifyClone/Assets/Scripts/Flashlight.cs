using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public GameObject lightSource;
    private bool flashlightOn;

    private void Start()
    {
        lightSource.SetActive(false);
        flashlightOn = false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F) && !flashlightOn)
        {
            lightSource.SetActive(true);
            flashlightOn = true;
        }
        else if(Input.GetKeyDown(KeyCode.F) && flashlightOn)
        {
            lightSource.SetActive(false);
            flashlightOn = false;
        }
    }
}
