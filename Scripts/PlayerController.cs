using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float velocity = 1.0f;
    private float sensitivity = 2.0f;

    [SerializeField]
    private MazeGenerator mazeGenerator;

    [SerializeField]
    private GameManager gameManager;

    private Vector3 movement;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.gameMode == GameMode.Play)
        {
            // Repeatedly call the following methods if the gamemode is play
            Move();
            Rotate();
            PreventOutOfBounds();
        }
    }

    private void Move()
    {
        // Get the horizontal and vertical input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Get the movement and normalize it to make sure the player has the same velocity in every direction
        // Move the player based on the movement value and velocity
        movement = new Vector3(horizontalInput, 0, verticalInput);
        movement.Normalize();
        transform.Translate(movement * Time.deltaTime * velocity);
    }

    private void Rotate()
    {
        // Get the horizontal mouse input
        float horizontalMouseInput = Input.GetAxis("Mouse X");

        // Rotate the view based on the horizontal mouse input and sensitivity
        transform.Rotate(Vector3.up * horizontalMouseInput * sensitivity);        
    }

    private void PreventOutOfBounds()
    {
        // Set the horizontal and vertical boundaries
        // The position of the player can't be less than the second value which is 0 and can't be greater than the third value which is width/height - 1
        float horizontalBoundary = Mathf.Clamp(transform.position.x, 0, mazeGenerator.width - 1);
        float verticalBoundary = Mathf.Clamp(transform.position.z, 0, mazeGenerator.height - 1);

        // Move the player to the edge of the boundary if the player tries to go past the boundary
        transform.position = new Vector3(horizontalBoundary, 0, verticalBoundary);
    }

    private void OnTriggerEnter(Collider other)
    {
        // When the player hits a wall the movement will be reversed to prevent the player from going throught the wall
        transform.Translate(-movement * Time.deltaTime * velocity);
    }

    private void OnTriggerStay(Collider other)
    {
        // If the player gets inside the wall the movement will be reversed to prevent the player from going through the wall
        transform.Translate(-movement * Time.deltaTime * velocity);
    }
}