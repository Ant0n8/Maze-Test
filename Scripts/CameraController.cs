using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private MazeGenerator mazeGenerator;

    [SerializeField]
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetCameraPosition(int zoomValue = 0)
    {
        if (gameManager.gameMode == GameMode.View)
        {           
            // Set the min and max values of the slider to double the maze size
            gameManager.zoomSlider.minValue = Mathf.Min(mazeGenerator.width, mazeGenerator.height) * -2.0f;
            gameManager.zoomSlider.maxValue = Mathf.Max(mazeGenerator.width, mazeGenerator.height) * 2.0f;

            // Get the center of the maze width and height
            float centerMazeWidth = (mazeGenerator.width) / 2.0f;
            float centerMazeHeight = (mazeGenerator.height) / 2.0f;

            // Set the position of the camera to the center of the maze
            if (zoomValue == 0)
            {
                mainCamera.transform.position = new Vector3(centerMazeWidth, Mathf.Max(mazeGenerator.width, mazeGenerator.height), centerMazeHeight);
            }

            // Set the position of the camera to the center of the maze and zoom in based on the zoomValue
            else if (zoomValue > 0)
            {
                mainCamera.transform.position = new Vector3(centerMazeWidth, Mathf.Max(mazeGenerator.width, mazeGenerator.height) - zoomValue, centerMazeHeight);
            }

            // Set the position of the camera to the center of the maze and zoom out based on the zoomValue
            else if (zoomValue < 0)
            {
                mainCamera.transform.position = new Vector3(centerMazeWidth, Mathf.Max(mazeGenerator.width, mazeGenerator.height) - zoomValue, centerMazeHeight);
            }

            // Rotate camera to see the top view of the maze
            mainCamera.transform.rotation = Quaternion.Euler(90, 0, 0);
        }

        else if (gameManager.gameMode == GameMode.Play)
        {
            // Set the camera position to the entrance of the maze
            mainCamera.transform.position = mazeGenerator.entrance;

            // Reset the rotation
            mainCamera.transform.rotation = Quaternion.identity;

            // Make the nearClipPlane value smaller to prevent the camera from seeing through walls
            mainCamera.nearClipPlane = 0.01f;
        }
    }
}