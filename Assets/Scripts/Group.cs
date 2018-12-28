using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group : MonoBehaviour
{
    public Spawner Spawner { get; set; }

    // Time since last gravity tick
    float lastFall = 0;
    float lastSoftDrop = 0;
    float inputStart = 0;
    bool groundSnap = false;
    int counter = 0;


    bool isValidGridPos()
    {
        foreach (Transform child in transform)
        {
            Vector2 v = Grid.getRelativeToSpawner(Grid.roundVec2(child.position), Spawner);

            // Not inside Border?
            if (!Grid.insideBorder(v))
                return false;

            // Block in grid cell (and not part of same group)?
            if (Grid.grid[(int)v.x, (int)v.y] != null &&
                Grid.grid[(int)v.x, (int)v.y].parent != transform)
                return false;
        }
        return true;
    }

    void updateGrid()
    {
        // Remove old children from grid
        for (int y = 0; y < Grid.h; ++y)
            for (int x = 0; x < Grid.w; ++x)
                if (Grid.grid[x, y] != null)
                    if (Grid.grid[x, y].parent == transform)
                        Grid.grid[x, y] = null;

        // Add new children to grid
        foreach (Transform child in transform)
        {
            Vector2 v = Grid.getRelativeToSpawner(Grid.roundVec2(child.position), Spawner);
            Grid.grid[(int)v.x, (int)v.y] = child;
        }
    }

    // Use this for initialization
    void Start()
    {

        if (transform.name.Contains("GroupI"))
        {
            transform.Rotate(0, 0, 180);
            foreach (Transform child in transform)
            {
                child.Rotate(0, 0, 180);
            }
            transform.position += new Vector3(1, 0, 0);
        }
        // Default position not valid? Then it's game over
        if (!isValidGridPos())
        {
            Debug.Log("GAME OVER");
            Destroy(gameObject);
        }   
    }

    // Update is called once per frame
    void Update()
    {
        // Move Left
        if ((Input.GetKeyDown(KeyCode.J) ||
            Input.GetKey(KeyCode.J) && Time.time - inputStart >= 0.33) && !Input.GetKey(KeyCode.L))
        {
            // Modify position
            transform.position += new Vector3(-1, 0, 0);

            // See if valid
            if (isValidGridPos())
                // It's valid. Update grid.
                updateGrid();
            else
                // It's not valid. revert.
                transform.position += new Vector3(1, 0, 0);

            if (Input.GetKeyDown(KeyCode.J))
                inputStart = Time.time;
        }

        // Move Right
        else if ((Input.GetKeyDown(KeyCode.L) ||
                 Input.GetKey(KeyCode.L) && Time.time - inputStart >= 0.33) && !Input.GetKey(KeyCode.J))
        {
            // Modify position
            transform.position += new Vector3(1, 0, 0);

            // See if valid
            if (isValidGridPos())
                // It's valid. Update grid.
                updateGrid();
            else
                // It's not valid. revert.
                transform.position += new Vector3(-1, 0, 0);

            if (Input.GetKeyDown(KeyCode.L))
                inputStart = Time.time;
        }

        // Rotate
        if (Input.GetKeyDown(KeyCode.Y))
        {
            rotateTetromino(transform, 90);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            rotateTetromino(transform, -90);
        }

        // Hard Drop
        if (Input.GetKeyDown(KeyCode.I))
        {
            while (isValidGridPos())
            {
                transform.position += new Vector3(0, -1, 0);
            }
            transform.position += new Vector3(0, 1, 0);
            updateGrid();
        }

        // Soft Drop and Fall
        if (Input.GetKey(KeyCode.K) && Time.time - lastSoftDrop >= 0.2 ||
                 Time.time - lastFall >= 1)
        {
            // Modify position
            transform.position += new Vector3(0, -1, 0);

            // See if valid
            if (isValidGridPos())
            {
                // It's valid. Update grid.
                updateGrid();
            }
            else
            {
                // It's not valid. revert.
                transform.position += new Vector3(0, 1, 0);

                // Clear filled horizontal lines
                Grid.deleteFullRows(Spawner.barrel);

                // Spawn next Group
                FindObjectOfType<Spawner>().spawnNext();
                counter = 0;

                // Disable script
                enabled = false;
            }

            lastFall = Time.time;
            if (Input.GetKey(KeyCode.K))
                lastSoftDrop = Time.time;
        }
    }

    private bool checkForWallKicks(Transform transfrom)
    {
        // Modify position
        transform.position += new Vector3(1, 0, 0);

        // See if valid
        if (isValidGridPos())
        {
            // It's valid. Update grid.
            updateGrid();
        }
        else
        {
            // It's not valid. revert.
            transform.position += new Vector3(-2, 0, 0);
            // See if valid
            if (isValidGridPos())
            {
                // It's valid. Update grid.
                updateGrid();
            }
            else
            {
                transform.position += new Vector3(1, 0, 0);
                return false;
            }
        }
        return true;
    }

    void rotateTetromino(Transform transform, int rotation)
    {

        string tetrominoName = transform.name;
        bool allowRotation = true;

        if (tetrominoName.Contains("GroupZ"))
        {
            if (counter % 2 == 0)
                rotation = 90;
            else
                rotation = -90;
            counter++;
        }
        if (tetrominoName.Contains("GroupS"))
        {
            if (counter % 2 == 0)
                rotation = -90;
            else
                rotation = 90;
            counter++;
        }
        if (tetrominoName.Contains("GroupI"))
        {
            if (counter % 2 == 0)
                rotation = 90;
            else
                rotation =-90;
            counter++;
        }
        if (tetrominoName.Contains("GroupO"))
        {
            rotation = 0;
        }

        transform.Rotate(0, 0, rotation);
        foreach (Transform child in transform)
        {
            child.Rotate(0, 0, -rotation);
        }

        if (tetrominoName.Contains("GroupL") || tetrominoName.Contains("GroupJ") || tetrominoName.Contains("GroupT"))
        {
            if (groundSnap == true)
            {
                // Modify position
                transform.position += new Vector3(0, 1, 0);

                // See if valid
                if (isValidGridPos())
                {
                    // It's valid. Update grid.
                    updateGrid();
                    groundSnap = false;
                }
                else if (checkForWallKicks(transform))
                    groundSnap = false;
                else
                {
                    // It's not valid. revert.
                    transform.position += new Vector3(0, -1, 0);
                    transform.Rotate(0, 0, -rotation);
                    foreach (Transform child in transform)
                    {
                        child.Rotate(0, 0, rotation);
                    }

                    allowRotation = false;
                }
            }
            if (rotation == 90 && allowRotation)
                counter++;
            else if (allowRotation)
                counter--;
            if (counter%4 == 2 || counter%4 == -2)
            {
                // Modify position
                transform.position += new Vector3(0, -1, 0);

                // See if valid
                if (isValidGridPos())
                {
                    // It's valid. Update grid.
                    updateGrid();
                    groundSnap = true;
                }
                else if (checkForWallKicks(transform))
                    groundSnap = true;
                else
                {
                    // It's not valid. revert.
                    transform.position += new Vector3(0, 1, 0);
                    transform.Rotate(0, 0, -rotation);
                    foreach (Transform child in transform)
                    {
                        child.Rotate(0, 0, rotation);
                    }
                    if (counter > 0)
                        counter--;
                    else
                        counter++;
                }
            }
            else
            {
                // See if valid
                if (isValidGridPos())
                {
                    // It's valid. Update grid.
                    updateGrid();
                }
                else if (checkForWallKicks(transform)) ;
                else
                {
                    transform.Rotate(0, 0, -rotation);
                    foreach (Transform child in transform)
                    {
                        child.Rotate(0, 0, rotation);
                    }
                }
            }
        }

        if (tetrominoName.Contains("GroupS") || tetrominoName.Contains("GroupZ") || tetrominoName.Contains("GroupI"))
        {
            // See if valid
            if (isValidGridPos())
            {
                // It's valid. Update grid.
                updateGrid();
            }
            else if (checkForWallKicks(transform)) ;
            else
            {
                counter--;
                transform.Rotate(0, 0, -rotation);
                foreach (Transform child in transform)
                {
                    child.Rotate(0, 0, rotation);
                }
            }
        }
        
    }
}
