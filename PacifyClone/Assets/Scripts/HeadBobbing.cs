using UnityEngine;

public class HeadBobbing : MonoBehaviour
{
    private enum bobbingStates { breathing, walking, running, jump }

    public Vector3 restPosition;
    public Vector3 camPos;

    public float runSpeed = 12.0f;
    public float walkSpeed = 6.0f;
    public float breathingSpeed = 1.5f;
    public float bobAmount = 0.05f;
    public float transitionSpeed = 20f;

    private PlayerMovement _playerMovement;
    private bobbingStates states;
    private float timer = Mathf.PI / 2;

    private void Start()
    {
        _playerMovement = GetComponentInParent<PlayerMovement>();
        restPosition = Camera.main.transform.position;
        camPos = Camera.main.transform.position;
    }

    private void Update()
    {
        // Set states of bobbing
        if(_playerMovement.isGrounded && _playerMovement.isWalking)
        {
            states = bobbingStates.walking;
        }
        else if(!_playerMovement.isGrounded)
        {
            states = bobbingStates.jump;
        }
        else if(_playerMovement.isGrounded && _playerMovement.isRunning)
        {
            states = bobbingStates.running;
        }
        else
        {
            states = bobbingStates.breathing;
        }

        Bobbing();
    }

    private void Bobbing()
    {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) //moving
        {
            timer += walkSpeed * Time.deltaTime;

            //use the timer value to set the position
            Vector3 newPosition = new Vector3(Mathf.Cos(timer) * bobAmount, restPosition.y + Mathf.Abs((Mathf.Sin(timer) * bobAmount)), restPosition.z); //abs val of y for a parabolic path
            camPos = newPosition;
        }
        else
        {
            timer = Mathf.PI / 2; //reinitialize

            Vector3 newPosition = new Vector3(Mathf.Lerp(camPos.x, restPosition.x, transitionSpeed * Time.deltaTime), Mathf.Lerp(camPos.y, restPosition.y, transitionSpeed * Time.deltaTime), Mathf.Lerp(camPos.z, restPosition.z, transitionSpeed * Time.deltaTime)); //transition smoothly from walking to stopping.
            camPos = newPosition;
        }

        if (timer > Mathf.PI * 2) //completed a full cycle on the unit circle. Reset to 0 to avoid bloated values.
            timer = 0;
    }
}
