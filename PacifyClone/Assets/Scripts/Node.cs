using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Node : MonoBehaviour
{
    private int probability = 25;
    private TextMeshPro text;
    private MeshRenderer sphere;
    public Material[] materials;

    public Vector3 position;

    private void Start()
    {
        text = gameObject.transform.GetChild(0).GetChild(1).GetComponent<TextMeshPro>();
        sphere = gameObject.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        position = gameObject.transform.position;

        ChangeNodeGraphics();
        FaceToPlayer();
    }

    private void FaceToPlayer()
    {
        transform.rotation = Camera.main.transform.rotation;
    }

    private void ChangeNodeGraphics()
    {
        switch (probability)
        {
            case 25:
                sphere.material = materials[0];
                break;
            case 50:
                sphere.material = materials[1];
                break;
            case 75:
                sphere.material = materials[2];
                break;
            case 100:
                sphere.material = materials[3];
                break;
            default:
                break;
        }

        text.text = probability.ToString() + "%";
    }

}
