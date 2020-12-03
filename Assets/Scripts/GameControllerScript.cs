using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControllerScript : MonoBehaviour
{
    //Number of players
    private int players;
    //The points of each player where each index represents a player (index+1).
    // public int[] points;
    //The matrix of tiles (separated by 2.0f in all 2D directions)
    private GameObject[,] placedTiles;
    //Current tile held in hand
    private GameObject currentTile;

    private PlayerScript playerScript;

    private int currentPlayer;

    private int lastPlayer = 0;

    private StackScript stackScript;

    public Image invalidTile;

    public Image invalideMeeple;

    public Camera mainCamera;

    public GameObject tileMesh;

    public GameObject meepleMesh;

    public GameObject MeeplePrefab;

    public List<MeepleScript> MeeplesInCity;

    //private int xs, zs;

    private float aimX = 0, aimZ = 0;

    private PointScript.Direction direction;

    private TileScript.geography meepleGeography;

    private TurnScript turnScript;

    private Borderscript borderscript;

    public RectTransform mPanelGameOver;

    public Text mTextGameOver;

    int NewTileRotation = 0;

    int VertexItterator;

    bool cityIsFinished;

    bool renderCurrentTile = false;

    bool[,] visited;

    string errorOutput = "";

    public GameObject debugCluster;

    float fTileAimX = 0;
    float fTileAimZ = 0;

    int iTileAimX, iTileAimZ;

    public ErrorPlaneScript ErrorPlane;


    int tempX;
    int tempY;
    Color32 playerColor;

    //Add Meeple Down state functionality
    public enum GameStates
    {
        NewTurn,
        TileHeld,
        TileDown,
        MeepleHeld,
        MeepleDown,
        GameOver
    };
    public GameStates state;

    public enum CursorStates
    {
        Inside,
        Outside
    };

    private CursorStates CursorState;

    private enum PlatformStates
    {
        Computer,
        Tablet
    }

    private PlatformStates Platform;


    //Startar nytt spel
    public void NewGame(int players)
    {

        Platform = (PlatformStates)Enum.Parse(typeof(PlatformStates), PlayerPrefs.GetString("Platform"));


        placedTiles = new GameObject[170, 170];
        NewTileRotation = 0;
        stackScript = GetComponent<StackScript>().createStackScript();
        currentTile = stackScript.Pop();
        currentTile.name = "BaseTile";


        VertexItterator = 1;

        turnScript = GetComponent<TurnScript>();
        borderscript = GetComponent<Borderscript>();
        playerScript = GetComponent<PlayerScript>();

        playerScript.CreatePlayer(0, "Adam", new Color32(0, 255, 0, 255));
        playerScript.CreatePlayer(1, "Markus", new Color32(255, 0, 0, 255));
        playerScript.CreatePlayer(2, "Henrik", new Color32(255, 255, 0, 255));
        //Debug.Log("Kommer hit");
        /*
                this.players = players;
                points = new int[players];
                meeples = new GameObject[players][];

                for (int i = 0; i < players; i++)
                {
                    meeples[i] = (generateMeeples(i));
                }
        */
        PlaceBrick(currentTile, 85, 85);
        getInvalidTileImage();
        //VertexItterator++;
        state = GameStates.NewTurn;
        currentPlayer = turnScript.currentPlayer();
        borderscript.ChangeCurrentPlayer(playerScript.GetPlayer(currentPlayer).GetPlayerColor());
    }


    private void getInvalidTileImage()
    {
        GameObject image = GameObject.FindGameObjectWithTag("InvalidTile");
        if (image != null)
        {
            invalidTile = image.GetComponent<Image>();
        }
    }

    public void getInvalidMeeple()
    {
        GameObject image = GameObject.FindGameObjectWithTag("InvalidMeeple");
        if (image != null)
        {
            invalideMeeple = image.GetComponent<Image>();
        }
    }

    //Kontrollerar att tilen får placeras på angivna koordinater 
    public bool TilePlacementIsValid(GameObject tile, int x, int y)
    {
        TileScript script = tile.GetComponent<TileScript>();
        bool isNotAlone = false;
        if (placedTiles[x - 1, y] != null)
        {
            isNotAlone = true;
            if (script.West != placedTiles[x - 1, y].GetComponent<TileScript>().East) return false;
        }
        if (placedTiles[x + 1, y] != null)
        {
            isNotAlone = true;
            if (script.East != placedTiles[x + 1, y].GetComponent<TileScript>().West) return false;
        }
        if (placedTiles[x, y - 1] != null)
        {
            isNotAlone = true;
            if (script.South != placedTiles[x, y - 1].GetComponent<TileScript>().North) return false;
        }
        if (placedTiles[x, y + 1] != null)
        {
            isNotAlone = true;
            if (script.North != placedTiles[x, y + 1].GetComponent<TileScript>().South) return false;
        }
        if (placedTiles[x, y] != null)
        {
            return false;
        }
        return isNotAlone;
    }

    public bool CheckNeighborsIfTileCanBePlaced(GameObject tile, int x, int y)
    {
        TileScript script = tile.GetComponent<TileScript>();
        bool isNotAlone2 = false;

        if (placedTiles[x - 1, y] != null)
        {
            isNotAlone2 = true;
            if (script.West == placedTiles[x - 1, y].GetComponent<TileScript>().East) return false;
        }
        if (placedTiles[x + 1, y] != null)
        {
            isNotAlone2 = true;
            if (script.East == placedTiles[x + 1, y].GetComponent<TileScript>().West) return false;
        }
        if (placedTiles[x, y - 1] != null)
        {
            isNotAlone2 = true;
            if (script.South == placedTiles[x, y - 1].GetComponent<TileScript>().North) return false;
        }
        if (placedTiles[x, y + 1] != null)
        {
            isNotAlone2 = true;
            if (script.North == placedTiles[x, y + 1].GetComponent<TileScript>().South) return false;
        }

        return isNotAlone2;
    }

    public int CheckSurroundedCloister(int x, int z, bool endTurn)
    {
        int pts = 1;
        if (placedTiles[x - 1, z - 1] != null) pts++;
        if (placedTiles[x - 1, z] != null) pts++;
        if (placedTiles[x - 1, z + 1] != null) pts++;
        if (placedTiles[x, z - 1] != null) pts++;
        if (placedTiles[x, z + 1] != null) pts++;
        if (placedTiles[x + 1, z - 1] != null) pts++;
        if (placedTiles[x + 1, z] != null) pts++;
        if (placedTiles[x + 1, z + 1] != null) pts++;
        if (pts == 9 || endTurn)
        {
            return pts;
        }
        else
        {
            return 0;
        }
    }

    public PointScript.Direction[] getDirections(int x, int y)
    {
        PointScript.Direction[] directions = new PointScript.Direction[4];
        int itt = 0;
        if (placedTiles[x + 1, y] != null)
        {
            directions[itt] = PointScript.Direction.EAST;
            itt++;
        }
        if (placedTiles[x - 1, y] != null)
        {
            directions[itt] = PointScript.Direction.WEST;
            itt++;
        }
        if (placedTiles[x, y + 1] != null)
        {
            directions[itt] = PointScript.Direction.NORTH;
            itt++;
        }
        if (placedTiles[x, y - 1] != null)
        {
            directions[itt] = PointScript.Direction.SOUTH;
        }
        return directions;
    }

    public TileScript.geography[] getCenters(int x, int y)
    {
        TileScript.geography[] centers = new TileScript.geography[4];
        int itt = 0;
        if (placedTiles[x + 1, y] != null)
        {
            centers[itt] = placedTiles[x + 1, y].GetComponent<TileScript>().getCenter();
            itt++;
        }
        if (placedTiles[x - 1, y] != null)
        {
            centers[itt] = placedTiles[x - 1, y].GetComponent<TileScript>().getCenter();
            itt++;
        }
        if (placedTiles[x, y + 1] != null)
        {
            centers[itt] = placedTiles[x, y + 1].GetComponent<TileScript>().getCenter();
            itt++;
        }
        if (placedTiles[x, y - 1] != null)
        {
            centers[itt] = placedTiles[x, y - 1].GetComponent<TileScript>().getCenter();
        }
        return centers;
    }

    public TileScript.geography[] getWeights(int x, int y)
    {
        TileScript.geography[] weights = new TileScript.geography[4];
        int itt = 0;
        if (placedTiles[x + 1, y] != null)
        {
            weights[itt] = placedTiles[x + 1, y].GetComponent<TileScript>().West;
            itt++;
        }
        if (placedTiles[x - 1, y] != null)
        {
            weights[itt] = placedTiles[x - 1, y].GetComponent<TileScript>().East;
            itt++;
        }
        if (placedTiles[x, y + 1] != null)
        {
            weights[itt] = placedTiles[x, y + 1].GetComponent<TileScript>().South;
            itt++;
        }
        if (placedTiles[x, y - 1] != null)
        {
            weights[itt] = placedTiles[x, y - 1].GetComponent<TileScript>().North;
        }
        return weights;
    }
    //Hämtar grannarna till en specifik tile
    public int[] GetNeighbors(int x, int y)
    {
        int[] Neighbors = new int[4];
        int itt = 0;
        if (placedTiles[x + 1, y] != null)
        {
            Neighbors[itt] = placedTiles[x + 1, y].GetComponent<TileScript>().vIndex;
            itt++;
        }
        if (placedTiles[x - 1, y] != null)
        {
            Neighbors[itt] = placedTiles[x - 1, y].GetComponent<TileScript>().vIndex;
            itt++;
        }
        if (placedTiles[x, y + 1] != null)
        {
            Neighbors[itt] = placedTiles[x, y + 1].GetComponent<TileScript>().vIndex;
            itt++;
        }
        if (placedTiles[x, y - 1] != null)
        {
            Neighbors[itt] = placedTiles[x, y - 1].GetComponent<TileScript>().vIndex;
        }
        return Neighbors;
    }

    private MeepleScript FindMeeple(int x, int y, TileScript.geography geography)
    {
        MeepleScript res = null;

        foreach (PlayerScript.Player p in playerScript.players)
        {
            foreach (GameObject m in p.meeples)
            {
                MeepleScript tmp = m.GetComponent<MeepleScript>();

                if (tmp.geography == geography && tmp.x == x && tmp.z == y)
                {
                    return tmp;
                }
            }
        }
        return res;
    }

    private MeepleScript FindMeeple(int x, int y, TileScript.geography geography, PointScript.Direction direction)
    {
        MeepleScript res = null;

        foreach (PlayerScript.Player p in playerScript.players)
        {
            foreach (GameObject m in p.meeples)
            {
                MeepleScript tmp = m.GetComponent<MeepleScript>();

                if (tmp.geography == geography && tmp.x == x && tmp.z == y && tmp.direction == direction)
                {
                    return tmp;
                }
            }
        }
        return res;
    }

    public bool CityIsFinishedDirection(int x, int y, PointScript.Direction direction)
    {
        MeeplesInCity = new List<MeepleScript>();
        MeeplesInCity.Add(FindMeeple(x, y, TileScript.geography.City, direction));


        cityIsFinished = true;
        visited = new bool[170, 170];
        RecursiveCityIsFinishedDirection(x, y, direction);
        Debug.Log(direction);
        return cityIsFinished;
    }

    //Test City checker
    public bool CityIsFinished(int x, int y)
    {
        MeeplesInCity = new List<MeepleScript>();
        MeeplesInCity.Add(FindMeeple(x, y, TileScript.geography.City));


        cityIsFinished = true;
        visited = new bool[170, 170];
        RecursiveCityIsFinished(x, y);
        return cityIsFinished;
    }

    public void RecursiveCityIsFinishedDirection(int x, int y, PointScript.Direction direction)
    {
        visited[x, y] = true;
        if (direction == PointScript.Direction.NORTH)
        {
            if (placedTiles[x, y].GetComponent<TileScript>().North == TileScript.geography.City)
            {
                if (placedTiles[x, y + 1] != null)
                {
                    if (!visited[x, y + 1])
                    {
                        RecursiveCityIsFinished(x, y + 1);
                    }
                }
                else
                {
                    cityIsFinished = false;
                }
            }
        }
        if (direction == PointScript.Direction.EAST)
        {
            if (placedTiles[x, y].GetComponent<TileScript>().East == TileScript.geography.City)
            {
                if (placedTiles[x + 1, y] != null)
                {
                    if (!visited[x + 1, y])
                    {
                        RecursiveCityIsFinished(x + 1, y);
                    }
                }
                else
                {
                    cityIsFinished = false;
                }
            }
        }
        if (direction == PointScript.Direction.SOUTH)
        {
            if (placedTiles[x, y].GetComponent<TileScript>().South == TileScript.geography.City)
            {
                if (placedTiles[x, y - 1] != null)
                {
                    if (!visited[x, y - 1])
                    {
                        RecursiveCityIsFinished(x, y - 1);
                    }
                }
                else
                {
                    cityIsFinished = false;
                }
            }
        }
        if (direction == PointScript.Direction.WEST)
        {
            if (placedTiles[x, y].GetComponent<TileScript>().West == TileScript.geography.City)
            {
                if (placedTiles[x - 1, y] != null)
                {
                    if (!visited[x - 1, y])
                    {

                        RecursiveCityIsFinished(x - 1, y);
                    }
                }
                else
                {
                    cityIsFinished = false;
                }
            }
        }
    }

    private bool CityTileHasCityCenter(int x, int y)
    {
        return placedTiles[x, y].GetComponent<TileScript>().getCenter() == TileScript.geography.City || placedTiles[x, y].GetComponent<TileScript>().getCenter() == TileScript.geography.Cityroad;
    }

    private bool CityTileHasGrassOrStreamCenter(int x, int y)
    {
        return placedTiles[x, y].GetComponent<TileScript>().getCenter() == TileScript.geography.Grass || placedTiles[x, y].GetComponent<TileScript>().getCenter() != TileScript.geography.Stream;
    }

    public void RecursiveCityIsFinished(int x, int y)
    {
        visited[x, y] = true;

        if (placedTiles[x, y].GetComponent<TileScript>().North == TileScript.geography.City)
        {
            if (!CityTileHasGrassOrStreamCenter(x, y))
            {
                if (placedTiles[x, y + 1] != null)
                {
                    if (!visited[x, y + 1])
                    {
                        RecursiveCityIsFinished(x, y + 1);
                    }
                }
                else
                {
                    cityIsFinished = false;
                }
            }

        }
        if (placedTiles[x, y].GetComponent<TileScript>().East == TileScript.geography.City)
        {
            if (!CityTileHasGrassOrStreamCenter(x, y))
            {
                if (placedTiles[x + 1, y] != null)
                {
                    if (!visited[x + 1, y])
                    {
                        RecursiveCityIsFinished(x + 1, y);
                    }
                }
                else
                {
                    cityIsFinished = false;
                }
            }
        }
        if (placedTiles[x, y].GetComponent<TileScript>().South == TileScript.geography.City)
        {
            if (!CityTileHasGrassOrStreamCenter(x, y))
            {
                if (placedTiles[x, y - 1] != null)
                {
                    if (!visited[x, y - 1])
                    {
                        RecursiveCityIsFinished(x, y - 1);
                    }
                }
                else
                {
                    cityIsFinished = false;
                }
            }
        }
        if (placedTiles[x, y].GetComponent<TileScript>().West == TileScript.geography.City)
        {
            if (!CityTileHasGrassOrStreamCenter(x, y))
            {
                if (placedTiles[x - 1, y] != null)
                {
                    if (!visited[x - 1, y])
                    {

                        RecursiveCityIsFinished(x - 1, y);
                    }
                }
                else
                {
                    cityIsFinished = false;
                }
            }
        }
    }


    private bool HasNeighbor(int x, int z)
    {
        if (x + 1 < placedTiles.GetLength(0))
        {
            if (placedTiles[x + 1, z] != null) return true;
        }
        if (x - 1 >= 0)
        {
            if (placedTiles[x - 1, z] != null) return true;
        }
        if (z + 1 < placedTiles.GetLength(1))
        {
            if (placedTiles[x, z + 1] != null) return true;
        }
        if (z - 1 >= 0)
        {
            if (placedTiles[x, z - 1] != null) return true;
        }
        return false;
    }

    private bool MatchGeographyOrNull(int x, int y, PointScript.Direction dir, TileScript.geography geography)
    {
        if (placedTiles[x, y] == null)
        {
            return true;
        }
        else if (placedTiles[x, y].GetComponent<TileScript>().getGeographyAt(dir) == geography)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool TileCanBePlaced(TileScript script)
    {
        //Debug.Log(placedTiles.GetLength(0) + " : " + placedTiles.GetLength(1));
        for (int i = 0; i < placedTiles.GetLength(0); i++)
        {
            for (int j = 0; j < placedTiles.GetLength(1); j++)
            {
                if (HasNeighbor(i, j) && placedTiles[i, j] == null)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        if (MatchGeographyOrNull(i - 1, j, PointScript.Direction.EAST, script.West))
                        {
                            if (MatchGeographyOrNull(i + 1, j, PointScript.Direction.WEST, script.East))
                            {
                                if (MatchGeographyOrNull(i, j - 1, PointScript.Direction.NORTH, script.South))
                                {
                                    if (MatchGeographyOrNull(i, j + 1, PointScript.Direction.SOUTH, script.North))
                                    {
                                        ResetTileRotation();
                                        return true;
                                    }
                                }
                            }
                        }
                        RotateTile();
                    }
                }
            }
        }
        ResetTileRotation();
        return false;

    }

    /// <summary>
    /// AimTile doesn't do anything but visualize where in the tile grid the player is pointing.
    /// </summary>
    void RenderTempTile()
    {
        bool showMesh = true;
        if (state == GameStates.TileHeld)
        {
            try
            {
                //If the player points their brick within the grid, the held brick is displayed at the grid point where the players cursor is pointing
                if (placedTiles[iTileAimX, iTileAimZ] == null)
                {
                    showMesh = true;
                }
                else
                {
                    showMesh = false;
                }
            }
            catch (IndexOutOfRangeException e)
            {
                //Debug.Log(e);
                errorOutput = e.ToString();
            }
            if (showMesh)
            {
                Mesh mesh = tileMesh.GetComponentInChildren<MeshFilter>().sharedMesh;
                Graphics.DrawMesh(mesh, new Vector3((iTileAimX - 85) * 2, 0.0f, (iTileAimZ - 85) * 2), Quaternion.Euler(0.0f, 180.0f + (90.0f * NewTileRotation), 0.0f), currentTile.GetComponentInChildren<Renderer>().material, 0);
            }
        }
    }

    private void MouseAim()
    {
        float planeY = 0;
        Plane plane = new Plane(Vector3.up, Vector3.up * planeY);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out float distance))
        {
            fTileAimX = ray.GetPoint(distance).x;
            fTileAimZ = ray.GetPoint(distance).z;
        }

        if (fTileAimX > 0)
        {
            iTileAimX = ((int)(fTileAimX + 1f) / 2) + 85;
        }
        else
        {
            iTileAimX = ((int)(fTileAimX - 1f) / 2) + 85;
        }
        if (fTileAimZ > 0)
        {
            iTileAimZ = ((int)(fTileAimZ + 1f) / 2) + 85;
        }
        else
        {
            iTileAimZ = ((int)(fTileAimZ - 1f) / 2) + 85;
        }
        //TileAimX = xs + 85;
        //TileAimZ = zs + 85;
    }

    public void AimMeeple(bool TouchEnded)
    {

        aimX = 0;
        aimZ = 0;
        try
        {
            if (placedTiles[iTileAimX, iTileAimZ] == currentTile)
            {
                GameObject tile = placedTiles[iTileAimX, iTileAimZ];
                TileScript tileScript = tile.GetComponent<TileScript>();

                //aimX = TileAimX - (xs * 2);
                //aimZ = TileAimZ - (zs * 2);
                aimX = (fTileAimX - ((iTileAimX - 85) * 2));
                aimZ = (fTileAimZ - ((iTileAimZ - 85) * 2));

                //Debug.Log("x: " + aimX + " z: " + aimZ);
                int id = tile.GetComponent<TileScript>().id;

                meepleGeography = TileScript.geography.Grass;
                direction = PointScript.Direction.CENTER;

                if (aimX > .25f && aimZ < .25f && aimZ > -.25f)
                {
                    direction = PointScript.Direction.EAST;
                    meepleGeography = tileScript.East;
                }
                else if (aimX < -.25f && aimZ < .25f && aimZ > -.25f)
                {
                    direction = PointScript.Direction.WEST;
                    meepleGeography = tileScript.West;
                }
                if (aimZ > .25f && aimX < .25f && aimX > -.25f)
                {
                    direction = PointScript.Direction.NORTH;
                    meepleGeography = tileScript.North;
                }
                else if (aimZ < -.25f && aimX < .25f && aimX > -.25f)
                {
                    direction = PointScript.Direction.SOUTH;
                    meepleGeography = tileScript.South;
                }
                if (aimZ < .25f && aimZ > -.25f && aimX < .25f && aimX > -.25f)
                {
                    direction = PointScript.Direction.CENTER;
                    meepleGeography = tileScript.getCenter();
                }

                if (meepleGeography == TileScript.geography.City || meepleGeography == TileScript.geography.Cloister || meepleGeography == TileScript.geography.Road)
                {
                    if (state == GameStates.MeepleHeld)
                    {
                        //float tmpX = xs * 2 + aimX;
                        //float tmpZ = zs * 2 + aimZ;
                        //Debug.Log("X: " + (tmpX) + " z: " + (tmpZ));
                        //Material mat = meeples[0][0].GetComponent<MeepleScript>().materials[currentPlayer];



                        Mesh mesh = meepleMesh.GetComponentInChildren<MeshFilter>().sharedMesh;
                        Material mat = playerScript.GetPlayer(currentPlayer).meeples[0].GetComponent<MeepleScript>().material;
                        Graphics.DrawMesh(mesh, new Vector3(fTileAimX, 0.175f, fTileAimZ), Quaternion.Euler(0.0f, 180.0f + 0.0f, 0.0f), mat, 0);

                        if (TouchEnded && CursorState == CursorStates.Inside)
                        {
                            GameObject newMeeple = null;
                            foreach (GameObject meeple in playerScript.GetPlayer(currentPlayer).meeples)
                            {
                                if (meeple.GetComponent<MeepleScript>().free)
                                {
                                    newMeeple = meeple;
                                    break;
                                }
                            }
                            if (newMeeple != null)
                            {
                                PlaceMeeple(newMeeple, iTileAimX - 85, iTileAimZ - 85, direction, meepleGeography);
                            }
                        }
                    }
                }
            }
        }
        catch (IndexOutOfRangeException e)
        {
            Debug.Log(e);
            errorOutput = e.ToString();
        }
    }

    public void PlaceMeeple(GameObject meeple, int xs, int zs, PointScript.Direction direction, TileScript.geography meepleGeography)
    {
        //TileScript currentTileScript = placedTiles[xs + 85, zs + 85].GetComponent<TileScript>();
        TileScript currentTileScript = currentTile.GetComponent<TileScript>();

        bool res;
        if (currentTileScript.getCenter() == TileScript.geography.Village || currentTileScript.getCenter() == TileScript.geography.Grass || currentTileScript.getCenter() == TileScript.geography.Cloister && direction != PointScript.Direction.CENTER)
        {
            res = GetComponent<PointScript>().testIfMeepleCantBePlacedDirection(currentTileScript.vIndex, meepleGeography, direction);
        }
        else
        {
            res = GetComponent<PointScript>().testIfMeepleCantBePlaced(currentTileScript.vIndex, meepleGeography);
            Debug.Log(res);
        }

        if (!currentTileScript.checkIfOcupied(direction) && !res)
        {
            TileScript.geography geography = currentTileScript.getGeographyAt(direction);
            currentTileScript.occupy(direction);
            if (meepleGeography == TileScript.geography.Cityroad) meepleGeography = TileScript.geography.City;
            meeple.GetComponent<MeepleScript>().assignAttributes(xs + 85, zs + 85, direction, meepleGeography);
            meeple.GetComponentInChildren<MeshRenderer>().enabled = true;
            meeple.transform.position = new Vector3(fTileAimX, 0.175f, fTileAimZ);
            meeple.name = "MeepleDown";
            meeple.GetComponent<MeepleScript>().free = false;
            state = GameStates.MeepleDown;
        }
    }

    //Metod för att placera en tile på brädan
    public void PlaceBrick(GameObject tile, int x, int y)
    {
        tempX = x;
        tempY = y;
        tile.GetComponent<TileScript>().vIndex = VertexItterator;
        GetComponent<PointScript>().placeVertex(VertexItterator, GetNeighbors(tempX, tempY), getWeights(tempX, tempY), currentTile.GetComponent<TileScript>().getCenter(), getCenters(tempX, tempY), getDirections(tempX, tempY));
        VertexItterator++;
        placedTiles[x, y] = tile;
        tile.transform.position = new Vector3(2.0f * (x - 85), 0.0f, 2.0f * (y - 85));
        tile.GetComponentInChildren<MeshRenderer>().enabled = true;
        //Sätter alla HasMeeple();
        /*
        MeepleScript[] ms = GetMeepleInNeighbours(x, y);

        foreach (MeepleScript m in ms)
        {
            if(m != null) { 
                m.vertex = tile.GetComponent<TileScript>().vIndex;
                m.x = x;
                m.z = y;
            }
        }
        */
        calculatePoints(false, false);
        state = GameStates.TileDown;
    }

    //Metod för att plocka upp en ny tile
    public void PickupBrick()
    {
        currentTile = stackScript.Pop();
        ResetTileRotation();
        renderCurrentTile = false;
        if (!TileCanBePlaced(currentTile.GetComponent<TileScript>()))
        {
            Debug.Log("Tile not possible to place: discarding and drawing a new one. " + "Tile id: " + currentTile.GetComponent<TileScript>().id);
            currentTile = null;
            PickupBrick();
        }
        else
        {
            ResetTileRotation();
            state = GameStates.TileHeld;
        }
    }

    public void oneFingerDown()
    {
        int touches = Input.touchCount;
        if (touches > 0 && touches < 2)
        {
            if (state == GameStates.NewTurn)
            {
                PickupBrick();
            }
        }
    }

    public void tileConfirmedClick()
    {
        if (state == GameStates.TileHeld)
        {
            if (TilePlacementIsValid(currentTile, iTileAimX, iTileAimZ))
            {
                ErrorPlane.flashConfirm();
                PlaceBrick(currentTile, iTileAimX, iTileAimZ);
                //invalidTile.GetComponent<CardSlideScript>().InvalidTile(false);
                renderCurrentTile = false;
            }
            else if (!TilePlacementIsValid(currentTile, iTileAimX, iTileAimZ))
            {
                ErrorPlane.flashError();
                //invalidTile.GetComponent<CardSlideScript>().InvalidTile(true);
            }
        }
    }


    //Funktion för undo
    public void UndoAction()
    {
        if (state == GameStates.TileDown || state == GameStates.MeepleHeld)
        {
            placedTiles[(int)tempX, (int)tempY] = null;
            currentTile.GetComponentInChildren<MeshRenderer>().enabled = false;
            state = GameStates.TileHeld;

            /*
            for (int i = 0; i < meeples.Length; i++)
            {
                for (int j = 0; j < meeples[i].Length; j++)
                {
                    MeepleScript tmp = meeples[i][j].GetComponent<MeepleScript>();
                    if (tmp.x == (int)tempX && tmp.z == (int)tempY && tmp.free == false)
                    {
                        tmp.reset();
                        state = GameStates.TileHeld;
                    }
                }
            }
            */


            //Temp playerscript
            VertexItterator--;
            GetComponent<PointScript>().RemoveVertex(VertexItterator);
        }
    }

    //Avslutar nuvarande spelares runda
    public void EndTurn()
    {
        if (state == GameStates.TileDown || state == GameStates.MeepleDown || state == GameStates.MeepleHeld)
        {
            lastPlayer = currentPlayer;
            NewTileRotation = 0;
            calculatePoints(true, false);
            if (stackScript.GetTileCount() == -1)
            {
                GameOver();
            }
            else
            {
                state = GameStates.NewTurn;

                currentPlayer = turnScript.newTurn();
                playerColor = GetPlayerColor(currentPlayer);
                //borderscript.ChangeCurrentPlayer(playerColor);

                borderscript.ChangeCurrentPlayer(playerScript.GetPlayer(currentPlayer).GetPlayerColor());
            }
        }
    }

    public Color32 GetPlayerColor(int currPlayer)
    {
        if (currPlayer == 0)
        {
            playerColor = new Color32(255, 255, 0, 255);
        }
        else if (currPlayer == 1)
        {
            playerColor = new Color32(255, 0, 0, 255);
        }
        else if (currPlayer == 2)
        {
            playerColor = new Color32(0, 255, 0, 255);
        }
        else if (currPlayer == 3)
        {
            playerColor = new Color32(0, 0, 255, 255);
        }
        else if (currPlayer == 4)
        {
            playerColor = new Color32(0, 0, 0, 255);
        }
        return playerColor;
    }

    public void calculatePoints(bool RealCheck, bool GameEnd)
    {
        foreach (PlayerScript.Player p in playerScript.GetPlayers())
        {
            for (int j = 0; j < p.meeples.Length; j++)
            {
                MeepleScript meeple = p.meeples[j].GetComponent<MeepleScript>();
                if (!meeple.free)
                {
                    int tileID = placedTiles[meeple.x, meeple.z].GetComponent<TileScript>().id;
                    int finalscore = 0;
                    if (meeple.geography == TileScript.geography.City)
                    {
                        if (placedTiles[meeple.x, meeple.z].GetComponent<TileScript>().getCenter() == TileScript.geography.Stream || placedTiles[meeple.x, meeple.z].GetComponent<TileScript>().getCenter() == TileScript.geography.Grass || placedTiles[meeple.x, meeple.z].GetComponent<TileScript>().getCenter() == TileScript.geography.Road)
                        {
                            if (CityIsFinishedDirection(meeple.x, meeple.z, meeple.direction))
                            {

                                finalscore = GetComponent<PointScript>().startDfsDirection(placedTiles[meeple.x, meeple.z].GetComponent<TileScript>().vIndex, meeple.geography, meeple.direction, GameEnd);
                                Debug.Log("Finalscore: " + finalscore);

                            }
                            else
                            {
                                GetComponent<PointScript>().startDfsDirection(placedTiles[meeple.x, meeple.z].GetComponent<TileScript>().vIndex, meeple.geography, meeple.direction, GameEnd);
                            }
                        }
                        else
                        {
                            if (CityIsFinished(meeple.x, meeple.z))
                            {
                                finalscore = GetComponent<PointScript>().startDfs(placedTiles[meeple.x, meeple.z].GetComponent<TileScript>().vIndex, meeple.geography, GameEnd);
                                Debug.Log("Finalscore: " + finalscore);
                            }
                        }
                    }
                    else
                    {

                        if (placedTiles[meeple.x, meeple.z].GetComponent<TileScript>().getCenter() == TileScript.geography.Village || placedTiles[meeple.x, meeple.z].GetComponent<TileScript>().getCenter() == TileScript.geography.Grass)
                        {
                            finalscore = GetComponent<PointScript>().startDfsDirection(placedTiles[meeple.x, meeple.z].GetComponent<TileScript>().vIndex, meeple.geography, meeple.direction, GameEnd);
                            //Debug.Log("Finalscore: " + finalscore);
                        }
                        else
                        {
                            finalscore = GetComponent<PointScript>().startDfs(placedTiles[meeple.x, meeple.z].GetComponent<TileScript>().vIndex, meeple.geography, GameEnd);
                            //Debug.Log("Finalscore: " + finalscore);
                        }

                        if (placedTiles[meeple.x, meeple.z].GetComponent<TileScript>().getCenter() == TileScript.geography.Cloister)
                        {
                            finalscore = CheckSurroundedCloister(meeple.x, meeple.z, false);
                            // Debug.Log("Finalscore: " + finalscore);
                        }
                    }

                    if (finalscore > 0 && RealCheck)
                    {
                        Debug.Log("Player recieved " + finalscore + " points");
                        meeple.playerScriptPlayer.SetPlayerScore(meeple.playerScriptPlayer.GetPlayerScore() + finalscore);
                        meeple.free = true;
                        meeple.GetComponentInChildren<MeshRenderer>().enabled = false;
                    }
                }
            }
        }


    }



    public void PlaceMeeple()
    {
        if (state == GameStates.TileDown)
        {
            state = GameStates.MeepleHeld;
        }
        else if (state == GameStates.MeepleHeld)
        {
            state = GameStates.TileDown;
        }
    }

    void Start()
    {
        mainCamera = Camera.main;
        NewGame(PlayerPrefs.GetInt("PlayerCount"));
    }

    public void RotateTile()
    {
        if (state == GameStates.TileHeld)
        {
            NewTileRotation++;
            if (NewTileRotation > 3)
            {
                NewTileRotation = 0;
            }
            currentTile.GetComponent<TileScript>().Rotate();
        }
    }

    public void ResetTileRotation()
    {
        NewTileRotation = 0;
        currentTile.GetComponent<TileScript>().resetRotation();
    }

    void GameOver()
    {
        calculatePoints(true, true);
        mPanelGameOver.gameObject.SetActive(true);
        mTextGameOver.text = "Game Over Man, Player with most points won";
        state = GameStates.GameOver;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.mousePosition.x > 2149 || Input.mousePosition.x < 315 || Input.mousePosition.y > 1460 || Input.mousePosition.y < 220)
        {
            CursorState = CursorStates.Outside;
        }
        else
        {
            CursorState = CursorStates.Inside;
            MouseAim();
        }
        if (renderCurrentTile)
        {
            RenderTempTile();
        }

        if (Platform == PlatformStates.Tablet)
        {
            int touches = Input.touchCount;
            Touch touch = Input.GetTouch(0);
            if (touches > 0 && touches < 2 && CursorState == CursorStates.Inside)
            {
                renderCurrentTile = true;

                if (touch.phase == TouchPhase.Moved)
                {
                    RenderTempTile();
                }
                if (touch.phase == TouchPhase.Ended)
                {
                    bool hasNoNeighbours = true;
                    int[] neighbours = GetNeighbors(iTileAimX, iTileAimZ);
                    for (int i = 0; i < neighbours.Length; i++)
                    {
                        if (neighbours[i] != 0)
                        {
                            hasNoNeighbours = false;
                        }
                    }
                    if (hasNoNeighbours)
                    {
                        invalidTile.GetComponent<CardSlideScript>().InvalidTile(true);
                        renderCurrentTile = false;
                    }
                    if (!hasNoNeighbours)
                    {
                        renderCurrentTile = true;
                    }
                }
            }
            if (state == GameStates.MeepleHeld)
            {
                AimMeeple(touch.phase == TouchPhase.Ended);
            }
        }
        else
        {
            if (state == GameStates.MeepleHeld)
            {
                AimMeeple(Input.GetMouseButtonDown(0));
            }

            if (CursorState == CursorStates.Inside)
            {
                renderCurrentTile = true;
            }
            if (state == GameStates.TileHeld)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    tileConfirmedClick();
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                PickupBrick();
            }

            //If the player presses the R button they rotate the temporary "display" tile AND the currently held tile.
            if (Input.GetKeyDown(KeyCode.R))
            {
                RotateTile();
            }

            //AimMeeple();
            //För nästa runda spelares tur, kan ändras senare när vi har knapp
            if (Input.GetKeyDown(KeyCode.M))
            {
                EndTurn();
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                UndoAction();
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                GetComponent<PointScript>().printEverything();
            }
        }
        ErrorPlane.UpdatePosition(iTileAimX, iTileAimZ);
        updateDebug();
    }

    private void updateDebug()
    {
        TileScript currentTileScript = currentTile.GetComponent<TileScript>();
        if (currentTile != null) debugCluster.transform.Find("DebugText1").GetComponent<Text>().text = (currentTile.GetComponent<TileScript>().rotation == NewTileRotation).ToString();
        debugCluster.transform.Find("DebugText2").GetComponent<Text>().text = "";
        debugCluster.transform.Find("DebugText3").GetComponent<Text>().text = "";
        debugCluster.transform.Find("DebugText4").GetComponent<Text>().text = "";
        debugCluster.transform.Find("DebugText5").GetComponent<Text>().text = "";
        debugCluster.transform.Find("DebugText6").GetComponent<Text>().text = "";
        debugCluster.transform.Find("DebugText7").GetComponent<Text>().text = "";
        debugCluster.transform.Find("DebugText8").GetComponent<Text>().text = "";
        debugCluster.transform.Find("DebugText9").GetComponent<Text>().text = "";
        debugCluster.transform.Find("DebugText10").GetComponent<Text>().text = "";
    }

    public void toggleDebug()
    {
        debugCluster.SetActive(!debugCluster.activeSelf);
    }
}
