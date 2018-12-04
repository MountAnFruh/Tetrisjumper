using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group : MonoBehaviour
{
    public Spawner Spawner { get; set; }

    // Time since last gravity tick
    float lastFall = 0;
    float inputStart = 0;

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
        // Default position not valid? Then it's game over
        if (!isValidGridPos())
        {
            Debug.Log("GAME OVER");
            Destroy(gameObject);
        }   
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Move Left
        if (Input.GetKeyDown(KeyCode.LeftArrow) ||
            Input.GetKey(KeyCode.LeftArrow) && Time.time - inputStart >= 0.33)
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

            if (Input.GetKeyDown(KeyCode.LeftArrow))
                inputStart = Time.time;
        }

        // Move Right
        else if (Input.GetKeyDown(KeyCode.RightArrow) ||
                 Input.GetKey(KeyCode.RightArrow) && Time.time - inputStart >= 0.33)
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

            if (Input.GetKeyDown(KeyCode.RightArrow))
                inputStart = Time.time;
        }

        // Rotate
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            rotateTetromino(transform);
                
        }

        // Move Downwards and Fall
        else if (Input.GetKeyDown(KeyCode.DownArrow) ||
                 Input.GetKey(KeyCode.DownArrow) && Time.time - inputStart >= 0.33 ||
                 Time.time - lastFall >= 0.33)
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
                Grid.deleteFullRows();

                // Spawn next Group
                FindObjectOfType<Spawner>().spawnNext();

                // Disable script
                enabled = false;
            }

            lastFall = Time.time;

            if (Input.GetKeyDown(KeyCode.DownArrow))
                inputStart = Time.time;
        }
    }

    void rotateTetromino(Transform transform)
    {
        transform.Rotate(0, 0, -90);
        foreach (Transform child in transform)
        {
            child.Rotate(0, 0, 90);
        }

        // See if valid
        if (isValidGridPos())
            // It's valid. Update grid.
            updateGrid();
        else
        {
            // It's not valid. revert.
            transform.Rotate(0, 0, 90);
            foreach (Transform child in transform)
            {
                child.Rotate(0, 0, -90);
            }
        }
    }
}
