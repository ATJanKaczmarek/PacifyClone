using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letter : MonoBehaviour
{
    public GameObject letterUI;
    private GameObject player;
    private GameObject cam;
    private bool letterOpen = false;

    private void Start()
    {
        letterUI.SetActive(false);
        player = GameObject.Find("Player");
        cam = Camera.main.gameObject;
    }

    private void Update()
    {
        if (letterOpen && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0)))
        {
            letterOpen = CloseLetter();
        }
        else if (!letterOpen && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E)))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {
                    Debug.Log(hit.collider.gameObject);
                    if (hit.collider.gameObject.tag == "Letter")
                    {
                        letterOpen = OpenLetter();
                    }
                }
            }
        }
    }

    private bool OpenLetter()
    {
        letterUI.SetActive(true);
        cam.GetComponent<MouseLook>().enabled = false;
        player.GetComponent<PlayerMovement>().enabled = false;
        return true;
    }

    private bool CloseLetter()
    {
        letterUI.SetActive(false);
        cam.GetComponent<MouseLook>().enabled = true;
        player.GetComponent<PlayerMovement>().enabled = true;
        return false;
    }
}
