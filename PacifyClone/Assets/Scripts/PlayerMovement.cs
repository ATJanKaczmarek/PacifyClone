using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    #region public

    public CharacterController controller;
    public float speed = 6.0f;
    public float gravity = -9.81f;
    public float groundDistance = 0.4f;
    public float jumpHeight = 3f;
    public Transform groundCheck;
    public LayerMask groundMask;
    public float runSpeed = 12.0f;

    public AudioClip[] footsteps;
    public AudioClip jumpSound;
    public AudioSource source;

    #region Used in other Classes
    public bool isGrounded;
    public bool isRunning;
    public bool isWalking;
    #endregion Used in other Classes
    #endregion public

    #region private
    private bool soundFinished = true;
    private Vector3 velocity;
    private AudioClip oldFootstepSound;
    #endregion private
    #endregion Variables
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
                isWalking = true;
                bool done = false;
                while (!done)
                {
                    int randomNumber = Random.Range(0, footsteps.Length);
                    if (oldFootstepSound != footsteps[randomNumber])
                    {
                        source.clip = footsteps[randomNumber];
                        source.Play();
                        oldFootstepSound = footsteps[randomNumber];
                        StartCoroutine(WaitForEndOfSound());
                        done = true;
                    }
                    else
                    {
                        randomNumber = Random.Range(0, footsteps.Length);
                        done = false;
                    }
                }
            }
        }

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            source.clip = jumpSound;
            source.Play();
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
