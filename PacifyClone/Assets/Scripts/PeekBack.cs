using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeekBack : MonoBehaviour
{
    public GameObject backCam;
    public GameObject mainCam;
    private void Start()
    {
        backCam.SetActive(false);
        mainCam.SetActive(true);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            mainCam.SetActive(false);
            backCam.SetActive(true);
        }
        
        if(Input.GetKeyUp(KeyCode.Q))
        {
            mainCam.SetActive(true);
            backCam.SetActive(false);
        }
    }
}
