using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject mazeCellPrefab;

    [SerializeField]
    private CameraController cameraController;

    [SerializeField]
    private GameManager gameManager;

    public int width { get; private set; }

    public int height { get; private set; }

    private int[,] maze;
    private Dictionary<string, GameObject> CreatedMazeCellsDictionary = new Dictionary<string, GameObject>();
    private bool mazeCellsCreated = false;

    public Vector3 entrance;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator CreatePaths(bool isAnimated)
    {
        // Create a stack to keep track of the current and previous mazecells.
        // Start mazecell is a random mazecell at the bottom of the maze.
        // Make the value of the mazecell 1 instead of 0 to mark as visited.
        Stack<Vector2Int> MazeCellStack = new Stack<Vector2Int>();
        Vector2Int currentMazeCell = new Vector2Int(UnityEngine.Random.Range(0, width - 1), 0);
        MazeCellStack.Push(currentMazeCell);
        maze[currentMazeCell.x, currentMazeCell.y] = 1;

        // Look for the start mazecell in the dictionary then loop between the child objects to get the bottom wall and remove it to create the entrance.
        entrance = new Vector3(currentMazeCell.x, 0, currentMazeCell.y);
        GameObject entranceMazeCell = CreatedMazeCellsDictionary[$"MazeCell_{currentMazeCell.x}_{currentMazeCell.y}"];
        foreach (Transform wall in entranceMazeCell.transform)
        {
            if (wall.name == "Bottom Wall")
            {
                GameObject wallObject = wall.gameObject;
                Destroy(wallObject);
                break;
            }
        }

        // Keep repeating until all mazecells have been visited.
        while (MazeCellStack.Count > 0)
        {
            // Get the neighbour/adjacent mazecells which haven't been visited yet.
            List<Vector2Int> unvisitedNeighborsList = GetUnvisitedNeighbors(currentMazeCell);

            if (unvisitedNeighborsList.Count > 0)
            {
                // Save the current mazecell to the stack.
                // Get a random neigbor/adjacent mazecell from the unvisitedneighbourlist and remove the walls between the current mazecell and neighbour.
                // Make the chosen neighbour the currentcell
                MazeCellStack.Push(currentMazeCell);
                Vector2Int chosenNeighbor = unvisitedNeighborsList[UnityEngine.Random.Range(0, unvisitedNeighborsList.Count)];
                RemoveWalls(currentMazeCell, chosenNeighbor);
                currentMazeCell = chosenNeighbor;
                maze[currentMazeCell.x, currentMazeCell.y] = 1;
            }
            else if (MazeCellStack.Count > 0)
            {
                // If there are no unvisited neighbours remove the current mazecell from the stack.
                // Repeat until it arrives at a mazecell which does have unvisited neighbours.
                currentMazeCell = MazeCellStack.Pop();
            }

            if (isAnimated)
            {
                // If the condition is true it will create the paths by removing the walls with pauses included which makes the generation animated.
                yield return null;
            }
        }

        // Look for a random mazecell at the top in the dictionary then loop between the child objects to get the top wall and remove it to create the exit.
        GameObject exitMazeCell = CreatedMazeCellsDictionary[$"MazeCell_{UnityEngine.Random.Range(0, width - 1)}_{height - 1}"];
        foreach (Transform wall in exitMazeCell.transform)
        {
            if (wall.name == "Top Wall")
            {
                GameObject wallObject = wall.gameObject;
                Destroy(wallObject);
                break;
            }
        }

        // Enable the generate buttons
        gameManager.SetGeneratingUI(false);
    }

    private List<Vector2Int> GetUnvisitedNeighbors(Vector2Int currentMazeCell)
    {
        // Create a list to save the unvisited neighbours/adjacent mazecells
        List<Vector2Int> unvisitedNeighborsList = new List<Vector2Int>();

        // Check if there is a mazecell to the left of the current mazecell and check if it is unvisited.
        // If both are true save it to the list.
        if (currentMazeCell.x > 0 && maze[currentMazeCell.x - 1, currentMazeCell.y] == 0)
            unvisitedNeighborsList.Add(new Vector2Int(currentMazeCell.x - 1, currentMazeCell.y));

        // Check if there is a mazecell to the right of the current mazecell and check if it is unvisited.
        // If both are true save it to the list.
        if (currentMazeCell.x < width - 1 && maze[currentMazeCell.x + 1, currentMazeCell.y] == 0)
            unvisitedNeighborsList.Add(new Vector2Int(currentMazeCell.x + 1, currentMazeCell.y));

        // Check if there is a mazecell above the current mazecell and check if it is unvisited.
        // If both are true save it to the list.
        if (currentMazeCell.y > 0 && maze[currentMazeCell.x, currentMazeCell.y - 1] == 0)
            unvisitedNeighborsList.Add(new Vector2Int(currentMazeCell.x, currentMazeCell.y - 1));

        // Check if there is a mazecell under the current mazecell and check if it is unvisited.
        // If both are true save it to the list.
        if (currentMazeCell.y < height - 1 && maze[currentMazeCell.x, currentMazeCell.y + 1] == 0)
            unvisitedNeighborsList.Add(new Vector2Int(currentMazeCell.x, currentMazeCell.y + 1));

        return unvisitedNeighborsList;
    }

    private void RemoveWalls(Vector2Int currentMazeCell, Vector2Int neighbor)
    {
        // Values to save the walls that will be deleted.
        string wallCurrent = "";
        string wallNeighbor = "";

        // If the neighbour x position is greater than the current mazecell x position it means that the neighbour is to the right of the current mazecell.
        // Save the right wall of the current mazecell and the left wall of the neighbour which will be removed to create a path.
        if (neighbor.x > currentMazeCell.x)
        {
            wallCurrent = "Right Wall";
            wallNeighbor = "Left Wall";
        }

        // If the neighbour x position is less than the current mazecell x position it means that the neighbour is to the left of the current mazecell.
        // Save the left wall of the current mazecell and the right wall of the neighbour which will be removed to create a path.
        else if (neighbor.x < currentMazeCell.x)
        {
            wallCurrent = "Left Wall";
            wallNeighbor = "Right Wall";
        }

        // If the neighbour y position is greater than the current mazecell y position it means that the neighbour is to the above the current mazecell.
        // Save the top wall of the current mazecell and the bottom wall of the neighbour which will be removed to create a path.
        else if (neighbor.y > currentMazeCell.y)
        {          
            wallCurrent = "Top Wall";
            wallNeighbor = "Bottom Wall";
        }

        // If the neighbour y position is less than the current mazecell y position it means that the neighbour is to the below the current mazecell.
        // Save the bottom wall of the current mazecell and the top wall of the neighbour which will be removed to create a path.
        else if (neighbor.y < currentMazeCell.y)
        {         
            wallCurrent = "Bottom Wall";
            wallNeighbor = "Top Wall";
        }

        // Look for the current mazecell and the neighbour then look loop through the child objects to get the wall that has to be removed
        GameObject currentMazeCellObject = CreatedMazeCellsDictionary[$"MazeCell_{currentMazeCell.x}_{currentMazeCell.y}"];
        GameObject neighborObject = CreatedMazeCellsDictionary[$"MazeCell_{neighbor.x}_{neighbor.y}"];

        foreach (Transform wall in currentMazeCellObject.transform)
        {
            if (wall.name == wallCurrent)
            {
                GameObject wallObject = wall.gameObject;
                Destroy(wallObject);
                break;
            }
        }

        foreach (Transform wall in neighborObject.transform)
        {
            if (wall.name == wallNeighbor)
            {
                GameObject wallObject = wall.gameObject;
                Destroy(wallObject);
                break;
            }
        }
    }

    private void CreateMazeCells()
    {
        // Create the right amount of mazecells by increasing the x and y values.
        // The positions of the mazecells is based on the x and y values
        // Rename the mazecells so it will be easier to see in the hierarchy where the mazecells are located.
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject mazeCell = Instantiate(mazeCellPrefab, new Vector3(x, 0, y), Quaternion.identity);
                mazeCell.name = $"MazeCell_{x}_{y}";
                CreatedMazeCellsDictionary.Add($"{mazeCell.name}", mazeCell);
            }
        }

        // Make mazeCellsCreated true after creating the mazecells so the program knows when it is done creating the mazecells.
        mazeCellsCreated = true;
    }

    private void ClearMaze()
    {
        // Save all the objects with the specified tag to a list.
        // Loop through the list and delete all objects.
        GameObject[] mazeObjects = GameObject.FindGameObjectsWithTag("Maze");
        foreach (GameObject item in mazeObjects)
        {
            Destroy(item);
        }

        // Clear the dictionary in which the mazecells were saved and make mazeCellsCreated false.
        CreatedMazeCellsDictionary.Clear();
        mazeCellsCreated = false;
    }

    public void GenerateNewMaze(bool isAnimated)
    {
        // Check if the width and height are within the specified boundaries 
        if ((width >= 10 && width <= 250) && (height >= 10 && height <= 250))
        {
            // Disable the generate buttons.
            // Clear the maze and create a new maze.
            gameManager.SetGeneratingUI(true);
            ClearMaze();
            maze = new int[width, height];

            // Change the gamemode to view if the current gamemode is play
            if (gameManager.gameMode == GameMode.Play)
            {
                gameManager.ChangeGameMode();
            }

            // Reset the camera position
            cameraController.SetCameraPosition();

            // Create the mazecells for the new maze and create the paths after
            CreateMazeCells();
            StartCoroutine(CreatePathsAfterCells(isAnimated));
        }
    }

    private IEnumerator CreatePathsAfterCells(bool isAnimated)
    {
        // Create the paths after the mazecells have been created and make it animated if isAnimated is true
        yield return new WaitUntil(() => mazeCellsCreated);
        StartCoroutine(CreatePaths(isAnimated));
    }

    public void SetWidth(string input)
    {
        // Save the entered width if it is a valid number and within the specified boundaries
        try
        {
            int widthInput = int.Parse(input);

            if (widthInput >= 10 && widthInput <= 250)
            {
                width = widthInput;
            }

            else
            {
                Debug.LogWarning("Width must be a number from 10 to 250.");
            }
        }

        catch (FormatException)
        {
            Debug.LogWarning("Invalid input. Please enter a valid number.");
        }
    }

    public void SetHeight(string input)
    {
        // Save the entered height if it is a valid number and within the specified boundaries
        try
        {
            int heightInput = int.Parse(input);

            if (heightInput >= 10 && heightInput <= 250)
            {
                height = heightInput;
            }

            else
            {
                Debug.LogWarning("Height must be a number from 10 to 250.");
            }
        }

        catch (FormatException)
        {
            Debug.LogWarning("Invalid input. Please enter a valid number.");
        }
    }
}