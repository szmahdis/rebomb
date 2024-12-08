using System.Collections.Generic;
using System.Text;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class RandomWalkerGenerator : MonoBehaviour
{
    [SerializeField] private int dimension = 8;
    [SerializeField] private int[,] map;
    [SerializeField] private int maxTunnel = 1; // number of times we will run the algorithm
    [SerializeField] private int maxLength = 3; // max length the walker takes
    [SerializeField] private float startingProbability = 0.1f; // the probability of a position being breakablewall
    private bool mapAccepted = false;

    [SerializeField] private GameObject FloorPrefab;
    [SerializeField] private GameObject BreakableWallPrefab;
    [SerializeField] private GameObject UnbreakableWallPrefab;
    [SerializeField] private GameObject HourglassPrefab;
    [SerializeField] Transform MapParent;

    public void GenerateMap()
    {
        Debug.Log("Max tunnel: " + maxTunnel); 
        Debug.Log("Max length: " + maxLength);

        while (mapAccepted == false)
        {
            map = new int[dimension, dimension];
            int[] walkerposition;

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    map[i, j] = 1;

                }
            }


            walkerposition = GenerateStartingWalkerPosition();

            map[walkerposition[0], walkerposition[1]] = 0;
            //Debug.Log("Walker starting position is " + walkerposition[0] + ", " + walkerposition[1]);
            //PrintMap(map);


            for (int turn = 0; turn < maxTunnel; turn++)
            {
                int randomLength = Random.Range(1, maxLength);
                int randomDirection = Random.Range(0, 4);

                for (int len = 0; len < randomLength; len++)
                {
                    walkerposition = UpdateWalkerPosition(walkerposition, randomDirection);
                    map = UpdateMap(map, walkerposition);

                }

            }

            // placing player in the corners
            if (map[0, 0] + map[0, dimension - 1] + map[dimension - 1, 0] + map[dimension - 1, dimension - 1] == 0)
            {
                mapAccepted = true;
            }
        }

        map = PlaceBreakableWalls(map, startingProbability);
        map = ClearCorners(map);

        InstantiateMap(map);

    }

    public int[,] ClearCorners(int[,] map)
    {
        // top left
        map[0, 0] = 0;
        map[0, 1] = 0;
        map[1, 0] = 0;
        map[1, 1] = 0;

        // bottom left
        map[0, dimension - 1] = 0;
        map[0, dimension - 2] = 0;
        map[1, dimension - 1] = 0;
        map[1, dimension - 2] = 0;

        // top right
        map[dimension - 1, 0] = 0;
        map[dimension - 1, 1] = 0;
        map[dimension - 2, 0] = 0;
        map[dimension - 2, 1] = 0;

        // bottom right
        map[dimension - 1, dimension - 1] = 0;
        map[dimension - 1, dimension - 2] = 0;
        map[dimension - 2, dimension - 1] = 0;
        map[dimension - 2, dimension - 2] = 0;

        return map;

    }
    public void InstantiateMap(int[,] map)
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                // Floors are defined as 0 
                GameObject floor = Instantiate(FloorPrefab, new Vector3(i, 0.0f, j), Quaternion.identity);
                floor.transform.parent = MapParent.Find("Floor");

                // Unbreakable walls are defined as 1
                // Breakable walls are defined as 2
                if (map[i, j] == 1)
                {
                    GameObject unbreakableWall = Instantiate(UnbreakableWallPrefab, new Vector3(i, 1.0f, j), Quaternion.identity);
                    unbreakableWall.transform.parent = MapParent.Find("UnbreakableWall");
                }
                else if (map[i, j] == 2)
                {
                    GameObject breakableWall = Instantiate(BreakableWallPrefab, new Vector3(i, 1.0f, j), Quaternion.identity);
                    breakableWall.transform.parent = MapParent.Find("BreakableWall");
                }
            }
        }

        int[] hourglassPos = HourglassPosition(map);
        GameObject hourglass = Instantiate(HourglassPrefab, new Vector3(hourglassPos[0],0.0f, hourglassPos[1]), Quaternion.identity);
        hourglass.transform.parent = MapParent.Find("ItemWorld");
    }
    public void PrintMap(int[,] map)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                sb.Append(map[i, j]);
                sb.Append(' ');
            }
            sb.AppendLine();
        }
        Debug.Log("Generated map: \n" + sb.ToString());
    }

    public int[] GenerateStartingWalkerPosition()
    {
        int[] WalkerPosition = new int[2];
        WalkerPosition[0] = Random.Range(0, dimension);
        WalkerPosition[1] = Random.Range(0, dimension);

        return WalkerPosition;
    }

    public int[] UpdateWalkerPosition(int[] currentPosition, int direction)
    {
        if (direction == 0)
        {
            //go right
            //Debug.Log("Moving right from " + currentPosition[0] + ", " + currentPosition[1]);
            currentPosition[1]++;
            currentPosition[1] = Mathf.Min(currentPosition[1], dimension - 1);

        }
        else if (direction == 1)
        {
            //go left
            //Debug.Log("Moving left from " + currentPosition[0] + ", " + currentPosition[1]);
            currentPosition[1]--;
            currentPosition[1] = Mathf.Max(currentPosition[1], 0);
        }
        else if (direction == 2)
        {
            //go up
            //Debug.Log("Moving up from " + currentPosition[0] + ", " + currentPosition[1]);
            currentPosition[0]--;
            currentPosition[0] = Mathf.Max(currentPosition[0], 0);
        }
        else if (direction == 3)
        {
            //go down
            //Debug.Log("Moving down from " + currentPosition[0] + ", " + currentPosition[1]);
            currentPosition[0]++;
            currentPosition[0] = Mathf.Min(currentPosition[0], dimension - 1);
        }

        return currentPosition;

    }


    public int[,] UpdateMap(int[,] map, int[] currentPosition)
    {

        // update the map by setting the position of the walker to 0
        map[currentPosition[0], currentPosition[1]] = 0;
        return map;
    }

    public int[,] PlaceBreakableWalls(int[,] map, float probability)
    {

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j] == 0)
                {
                    float rnd = Random.Range(0.0f, 1.0f);
                    if (rnd < probability)
                    {
                        map[i, j] = 2; // 2 is breakable wall
                        probability = startingProbability;

                    }
                    else if (rnd > probability)
                    {
                        probability = probability + startingProbability;
                    }
                }
            }
        }
        return map;
    }

    public int[] HourglassPosition(int [,] map)
    {

        List<int[]> breakableWallList = new List<int[]>();
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j] == 2)
                {
                     // Make a list of breakable wall to place hourglass under it
                     breakableWallList.Add(new int[2] { i, j });
                   
                }
            }
        }

        int num = Random.Range(0, breakableWallList.Count);
        int[] hourGlassLocation = breakableWallList[num];

        return hourGlassLocation;

    }
    public void GenerateMapButton()
    {
        mapAccepted = false;
        // Clear existing generated map
        Transform unbreakableWall = MapParent.Find("UnbreakableWall");
        Transform breakableWall = MapParent.Find("BreakableWall");
        Transform floor = MapParent.Find("Floor");
        Transform items = MapParent.Find("ItemWorld");

        if (floor != null)
        {
            foreach (Transform child in floor)
            {
                //Destroy each child object
                Destroy(child.gameObject);
            }
            Debug.Log("All contents inside 'Floor' have been deleted.");
        }
        if (unbreakableWall != null)
        {
            // Iterate through all children of the "UnbreakableWall" subfolder
            foreach (Transform child in unbreakableWall)
            {
                // Destroy each child object
                Destroy(child.gameObject);
            }

            Debug.Log("All contents inside 'UnbreakableWall' have been deleted.");
        }
        if (breakableWall != null)
        {
            foreach (Transform child in breakableWall)
            {
                // Destroy each child object
                Destroy(child.gameObject);
            }

            Debug.Log("All contents inside 'Breakablewall' have been deleted.");
        }

        if (items != null)
        {
            foreach (Transform child in items)
            {
                // Destroy each child object
                Destroy(child.gameObject);
            }

            Debug.Log("All contents inside 'ItemWorld' have been deleted.");
        }


        //Generate New Map
        GenerateMap();

    }

    public void Start()
    {
        GenerateMap();
    }

}
