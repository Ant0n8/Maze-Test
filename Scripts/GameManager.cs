using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum GameMode
{
    // Set the avaliable gamemodes
    View,
    Play
}

public class GameManager : MonoBehaviour
{
    public GameMode gameMode { get; private set; }

    public Slider zoomSlider;

    [SerializeField]
    private CameraController cameraController;

    [SerializeField]
    private Button generateMazeButton;

    [SerializeField]
    private Button generateMazeAnimatedButton;

    [SerializeField]
    private TMP_InputField widthInputField;

    [SerializeField]
    private TMP_InputField heightInputField;

    [SerializeField]
    private Button changeModeButton;

    [SerializeField]
    private MazeGenerator mazeGenerator;

    // Start is called before the first frame update
    void Start()
    {
        // Set the gamemode to view at the start
        gameMode = GameMode.View;

        // Add listeners to the generate maze buttons which will listen if the buttons has been pressed
        // If the generate maze animated button has been pressed use true instead of false as parameter for the function call to make it animated
        // If a button has been pressed call the corresponding function to generate a new maze
        generateMazeButton.onClick.AddListener(() => mazeGenerator.GenerateNewMaze(false));
        generateMazeAnimatedButton.onClick.AddListener(() => mazeGenerator.GenerateNewMaze(true));

        // Add listeners to the width and height inputfields which will listen if the inputfields have been filled in after clicking away from the inputfields
        // If a inputfield has been filled in call the corresponding function to set the new width/height
        widthInputField.onEndEdit.AddListener(mazeGenerator.SetWidth);
        heightInputField.onEndEdit.AddListener(mazeGenerator.SetHeight);

        // Add a listener to the zoom slider which will listen if the value of the slider has changed
        // If the slider has been adjusted call the corresponding function to set the new camera position
        zoomSlider.onValueChanged.AddListener((float zoomValue) => cameraController.SetCameraPosition((int)zoomValue));

        // Add a listener to the change mode button which will listen if the button has been pressed
        // If the button has been pressed call the corresponding method to change the gamemode
        changeModeButton.onClick.AddListener(ChangeGameMode);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeGameMode()
    {
        if (gameMode == GameMode.View)
        {
            // If the current gamemode is view set it to play
            gameMode = GameMode.Play;

            // Set the following UI objects interactability to false to prevent the player from using them in play mode
            widthInputField.interactable = false;
            heightInputField.interactable = false;
            zoomSlider.interactable = false;
        }

        else if (gameMode == GameMode.Play)
        {
            // If the current gamemode is play set it to view
            gameMode = GameMode.View;

            // Set the following UI objects interactability to true to allow the player to use them in view mode
            widthInputField.interactable = true;
            heightInputField.interactable = true;
            zoomSlider.interactable = true;
        }

        // Set the new camera position
        cameraController.SetCameraPosition();
    }

    public void SetGeneratingUI(bool isGenerating)
    {
        if (isGenerating)
        {
            // While the maze is being generated set the following UI objects interactability to false to prevent the player from using
            generateMazeButton.interactable = false;
            generateMazeAnimatedButton.interactable = false;
            changeModeButton.interactable = false;
        }

        else if (!isGenerating)
        {
            // While the maze is not being generated set the following UI objects interactability to true to allow the player to use them
            generateMazeButton.interactable = true;
            generateMazeAnimatedButton.interactable = true;
            changeModeButton.interactable = true;
        }
    }
}