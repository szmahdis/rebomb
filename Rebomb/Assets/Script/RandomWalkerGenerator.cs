using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class RandomWalkerGenerator : MonoBehaviour
{
    public static int dimension = 8;
    private int[,] map;
    private int maxTunnel = 100; // number of times we will run the algorithm
    private int maxLength = 5; // max length the walker takes
    private float startingBreakableWallProbability = 0.1f; // the probability of a position being breakablewall
    private float startingItemProbability = 0.04f; // the probability of position having an item
    private bool mapAccepted = false;

    [SerializeField] private GameObject FloorPrefab1;
    [SerializeField] private GameObject FloorPrefab2;
    [SerializeField] private GameObject BreakableWallPrefab;
    [SerializeField] private GameObject UnbreakableWallPrefab;
    [SerializeField] private GameObject BorderPrefab;
    [SerializeField] private GameObject HourglassPrefab;
    [SerializeField] private GameObject CoinPrefab;
    [SerializeField] private GameObject BootPrefab;
    [SerializeField] Transform MapParent;

    // Constants for map generation
    private int Floor = 0;
    private int UnbreakableWall = 1;
    private int BreakableWall = 2;
    private int Border = 3;
    //private int BorderCorner = 4; // To clean border corners
    private int ItemOnMap = 5;
    private int AttemptCount = 0; // to avoid infinite loop
    private int ItemTypeCount;

    public static Vector3[] initial_positions = new Vector3[] {
        new Vector3(1, 0.5f, 1),
        new Vector3(dimension, 0.5f, dimension),
        new Vector3(1, 0.5f, dimension),
        new Vector3(dimension, 0.5f, 1)
    };

    public void GenerateMapButton()
    {
        CleanMap();
        GenerateMap();
    }

    private void CleanMap()
    {
        CleanElement("Bombs");
        CleanElement("UnbreakableWall");
        CleanElement("BreakableWall");
        CleanElement("Floor");
        CleanElement("ItemWorld");
        CleanElement("Border");
        mapAccepted = false;
    }

    private void GenerateMap()
    {
        Debug.Log("Max tunnel: " + maxTunnel);
        Debug.Log("Max length: " + maxLength);
        if (AttemptCount > 50)
        {
            Debug.Log("Change TunnelMax Parameters");
            // TODO: exit game in case of infinite loop
        }
        while (mapAccepted == false)
        {
            map = new int[dimension, dimension];
            int[] walkerposition;

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    map[i, j] = UnbreakableWall; // Generate the map initially with UnbreakableWall and "dig" tunnels

                }
            }


            walkerposition = GenerateStartingWalkerPosition();

            map[walkerposition[0], walkerposition[1]] = Floor; // Initial Walker position
            //Debug.Log("Walker starting position is " + walkerposition[0] + ", " + walkerposition[1]);

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

            // We only accept the maps that has corners free
            // so we can place players in the corners
            if (map[0, 0] + map[0, dimension - 1] + map[dimension - 1, 0] + map[dimension - 1, dimension - 1] == Floor) // Floor has to be 0
            {
                mapAccepted = true;
            }
            else
            {
                AttemptCount++; // infinite generation of maps if TunnelMax is too low;
                //TODO: fix it
            }
        }

        map = PlaceBreakableWalls(map, startingBreakableWallProbability);
        map = PlaceItems(map, startingItemProbability);
        map = ClearCorners(map);
        map = AddBorders(map);

        InstantiateMap(map);

    }

    private int[,] ClearCorners(int[,] map)
    {
        // top left
        map[0, 0] = Floor;
        map[0, 1] = Floor;
        map[1, 0] = Floor;
        map[1, 1] = Floor;

        // bottom left
        map[0, dimension - 1] = Floor;
        map[0, dimension - 2] = Floor;
        map[1, dimension - 1] = Floor;
        map[1, dimension - 2] = Floor;

        // top right
        map[dimension - 1, 0] = Floor;
        map[dimension - 1, 1] = Floor;
        map[dimension - 2, 0] = Floor;
        map[dimension - 2, 1] = Floor;

        // bottom right
        map[dimension - 1, dimension - 1] = Floor;
        map[dimension - 1, dimension - 2] = Floor;
        map[dimension - 2, dimension - 1] = Floor;
        map[dimension - 2, dimension - 2] = Floor;

        return map;

    }
    private void InstantiateMap(int[,] map)
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                // Create alternating floor tiles for better visualization
                if ((i + j) % 2 == 0 && i != 0 && j != 0 && i != dimension + 1 && j != dimension + 1) // even and not borders
                {
                    GameObject floor = Instantiate(FloorPrefab1, new Vector3(i, 0.0f, j), Quaternion.identity);
                    floor.transform.parent = MapParent.Find("Floor");
                    floor.layer = 2;  // Ignore Raycast
                }
                else if ((i + j) % 2 != 0 && i != 0 && j != 0 && i != dimension + 1 && j != dimension + 1) // odd and not borders
                {
                    GameObject floor = Instantiate(FloorPrefab2, new Vector3(i, 0.0f, j), Quaternion.identity);
                    floor.transform.parent = MapParent.Find("Floor");
                    floor.layer = 2;  // Ignore Raycast
                }

                if (map[i, j] == UnbreakableWall)
                {
                    GameObject unbreakableWall = Instantiate(UnbreakableWallPrefab, new Vector3(i, 1.0f, j), Quaternion.identity);
                    unbreakableWall.transform.parent = MapParent.Find("UnbreakableWall");
                }
                else if (map[i, j] == BreakableWall)
                {
                    GameObject breakableWall = Instantiate(BreakableWallPrefab, new Vector3(i, 1.0f, j), Quaternion.identity);
                    breakableWall.transform.parent = MapParent.Find("BreakableWall");
                }
                else if (map[i, j] == Border)
                {
                    GameObject borderWall = Instantiate(BorderPrefab, new Vector3(i, 1.0f, j), Quaternion.identity);
                    borderWall.transform.parent = MapParent.Find("Border");
                }
            }
        }

        int[] hourglassPos = HourglassPosition(map);
        GameObject hourglass = Instantiate(HourglassPrefab, new Vector3(hourglassPos[0], 0.5f, hourglassPos[1]), Quaternion.identity);
        hourglass.transform.parent = MapParent.Find("ItemWorld");


        ItemTypeCount = Item.ItemType.GetNames(typeof(Item.ItemType)).Length;


        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j] == ItemOnMap)
                {
                    int rnd = Random.Range(1, ItemTypeCount);

                    if (rnd == (int)Item.ItemType.Coin)
                    {
                        GameObject coin = Instantiate(CoinPrefab, new Vector3(i, 1.0f, j), Quaternion.identity);
                        coin.transform.parent = MapParent.Find("ItemWorld");
                    }
                    else if (rnd == (int)Item.ItemType.Boot)
                    {
                        GameObject boot = Instantiate(BootPrefab, new Vector3(i, 1.0f, j), Quaternion.identity);
                        boot.transform.parent = MapParent.Find("ItemWorld");
                    }

                }
            }
        }
    }

    // Help function to print the map for debugging purposes
    private void PrintMap(int[,] map)
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

    private int[] GenerateStartingWalkerPosition()
    {
        int[] WalkerPosition = new int[2];
        WalkerPosition[0] = Random.Range(0, dimension);
        WalkerPosition[1] = Random.Range(0, dimension);

        return WalkerPosition;
    }

    private int[] UpdateWalkerPosition(int[] currentPosition, int direction)
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


    private int[,] UpdateMap(int[,] map, int[] currentPosition)
    {

        // update the map by setting the position of the walker to 0
        map[currentPosition[0], currentPosition[1]] = Floor;
        return map;
    }

    private int[,] PlaceBreakableWalls(int[,] map, float probability)
    {

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j] == Floor)
                {
                    float rnd = Random.Range(0.0f, 1.0f);
                    if (rnd <= probability)
                    {
                        map[i, j] = BreakableWall;
                        probability = startingBreakableWallProbability;

                    }
                    else if (rnd > probability)
                    {
                        probability = probability + startingBreakableWallProbability;
                    }
                }
            }
        }
        return map;
    }


    private int[,] PlaceItems(int[,] map, float probability)
    {

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j] == Floor)
                {

                    float rnd = Random.Range(0.0f, 1.0f);
                    //Debug.Log("Probability is " + rnd + "/" + probability + " for " + i + " , " + j);
                    if (rnd <= probability)
                    {
                        map[i, j] = ItemOnMap;
                        probability = startingItemProbability;
                    }
                    else if (rnd > probability)
                    {
                        probability = probability + startingItemProbability;
                    }
                }
            }
        }

        return map;
    }

    private int[] HourglassPosition(int[,] map)
    {

        List<int[]> breakableWallList = new List<int[]>();
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j] == BreakableWall)
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

    private int[,] AddBorders(int[,] map)
    {

        int[,] mapWithBorders = new int[dimension + 2, dimension + 2];

        for (int i = 0; i < mapWithBorders.GetLength(0); i++)
        {
            for (int j = 0; j < mapWithBorders.GetLength(1); j++)
            {
                mapWithBorders[i, j] = Border; //Map full of borders

            }
        }

        // Copy map to mapWithBorders
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                mapWithBorders[i + 1, j + 1] = map[i, j];
            }
        }
        return mapWithBorders;
    }

    private void CleanElement(string name)
    {
        Transform element_parent = MapParent.Find(name);
        if (element_parent != null)
        {
            foreach (Transform child in element_parent.transform)
            {
                Destroy(child.gameObject);
            }
            Debug.Log("All contents inside " + name + "have been deleted.");
        }
        else
        {
            Debug.Log("No element found with the name " + name);
        }
    }
}
