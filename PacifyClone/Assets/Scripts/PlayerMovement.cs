using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 6.0f;
    public float gravity = -9.81f;
    public float groundDistance = 0.4f;
    public float jumpHeight = 3f;
    public Transform groundCheck;
    public LayerMask groundMask;
    public float runSpeed = 12.0f;

    public AudioClip[] footsteps;
    public AudioSource source;
    private bool soundFinished = true;

    private Vector3 velocity;
    private bool isGrounded;
    private bool isRunning;
    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if (!isRunning)
            controller.Move(move * speed * Time.deltaTime);
        else if (isRunning)
            controller.Move(move * runSpeed * Time.deltaTime);
        
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if (isGrounded && soundFinished)
            {
                int randomNumber = Random.Range(0, footsteps.Length);
                source.clip = footsteps[randomNumber];
                source.Play();
                StartCoroutine(WaitForEndOfSound());
            }
        }

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private IEnumerator WaitForEndOfSound()
    {
        soundFinished = false;
        yield return new WaitWhile(() => source.isPlaying);
        yield return new WaitForSeconds(0.125f);
        soundFinished = true;
    }
}
