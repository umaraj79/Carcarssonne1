using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControllerScript : MonoBehaviour
{
    //Number of players
    public int players;
    //The points of each player where each index represents a player (index+1).
    // public int[] points;
    //The matrix of tiles (separated by 2.0f in all 2D directions)
    public GameObject[,] placedTiles;
    //Current tile held in hand
    public GameObject currentTile;

    public PlayerScript playerScript;

    public int currentPlayer;

    public int lastPlayer = 0;

    public StackScript stackScript;

    public Image invalidTile;

    public Image invalideMeeple;

    public Camera mainCamera;

    public GameObject tileMesh;

    public GameObject meepleMesh;

    public GameObject MeeplePrefab;

    // public GameObject[][] meeples;

    public int xs, zs;

    public float aimX, aimZ;

    private PointScript.Direction direction;

    private TileScript.geography meepleGeography;

    public TurnScript turnScript;

    public Borderscript borderscript;

    public RectTransform mPanelGameOver;

    public Text mTextGameOver;

    int NewTileRotation = 0;

    int VertexItterator;

    bool cityIsFinished;

    bool renderCurrentTile = false;

    float AimX;
    float AimZ;
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

    public enum CursorStates
    {
        Inside,
        Outside
    }

    public CursorStates CursorState;

    public GameStates state;

    //Startar nytt spel
    public void NewGame(int players)
    {
        placedTiles = new GameObject[170, 170];
        NewTileRotation = 0;
        stackScript = GetComponent<StackScript>().createStackScript();
        currentTile = stackScript.InstatiateTiles(7, 0, 0, 0);
        currentTile.name = "BaseTile";

        VertexItterator = 1;

        turnScript = GetComponent<TurnScript>();
        borderscript = GetComponent<Borderscript>();
        playerScript = GetComponent<PlayerScript>();

        playerScript.CreatePlayer(0, "Adam", new Color32(0, 255, 0, 255));
        playerScript.CreatePlayer(1, "Markus", new Color32(255, 0, 0, 255));
        playerScript.CreatePlayer(2, "Henrik", new Color32(255, 255, 0, 255));
        Debug.Log("Kommer hit");

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
        VertexItterator++;
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




    public GameObject[] generateMeeples(int player)
    {
        GameObject[] res = new GameObject[8];
        for (int i = 0; i < 8; i++)
        {
            GameObject meeple = GameObject.Instantiate(MeeplePrefab, new Vector3(20.0f, 20.0f, 20.0f), Quaternion.identity);
            meeple.GetComponent<MeepleScript>().create(player);
            res[i] = meeple;
        }
        return res;
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

    //Test City checker
    public bool CityIsFinished(int x, int y)
    {
        cityIsFinished = true;
        bool[,] visited = new bool[170, 170];
        RecursiveCityIsFinished(x, y, visited);

        return cityIsFinished;
    }

    public void RecursiveCityIsFinished(int x, int y, bool[,] visited)
    {
        visited[x, y] = true;
        if (placedTiles[x, y].GetComponent<TileScript>().North == TileScript.geography.City)
        {
            if (placedTiles[x, y].GetComponent<TileScript>().getCenter() != TileScript.geography.Grass || placedTiles[x, y].GetComponent<TileScript>().getCenter() != TileScript.geography.Stream)
            {
                if (placedTiles[x, y + 1] != null)
                {
                    if (!visited[x, y + 1])
                    {
                        RecursiveCityIsFinished(x, y + 1, visited);
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
            if (placedTiles[x, y].GetComponent<TileScript>().getCenter() != TileScript.geography.Grass && placedTiles[x, y].GetComponent<TileScript>().getCenter() != TileScript.geography.Stream)
            {
                if (placedTiles[x + 1, y] != null)
                {
                    if (!visited[x + 1, y])
                    {
                        RecursiveCityIsFinished(x + 1, y, visited);
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
            if (placedTiles[x, y].GetComponent<TileScript>().getCenter() != TileScript.geography.Grass && placedTiles[x, y].GetComponent<TileScript>().getCenter() != TileScript.geography.Stream)
            {
                if (placedTiles[x, y - 1] != null)
                {
                    if (!visited[x, y - 1])
                    {
                        RecursiveCityIsFinished(x, y - 1, visited);
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
            if (placedTiles[x, y].GetComponent<TileScript>().getCenter() != TileScript.geography.Grass && placedTiles[x, y].GetComponent<TileScript>().getCenter() != TileScript.geography.Stream)
            {
                if (placedTiles[x - 1, y] != null)
                {
                    if (!visited[x - 1, y])
                    {
                        RecursiveCityIsFinished(x - 1, y, visited);
                    }
                }
                else
                {
                    cityIsFinished = false;
                }
            }
        }
    }

    public bool cantTileBePlaced(GameObject tile)
    {
        TileScript script = tile.GetComponent<TileScript>();
        bool isNotAloneRemix = false;

        for (int i = 0; i < placedTiles.GetLength(0); i++)
        {
            for (int k = 0; k < placedTiles.GetLength(1); k++)
            {
                if (placedTiles[i, k] != null)
                {
                    if (placedTiles[i - 1, k] == null)
                    {

                        isNotAloneRemix = true;
                        for (int j = 0; j < 4; j++)
                        {
                            if (script.East == placedTiles[i, k].GetComponent<TileScript>().West) return CheckNeighborsIfTileCanBePlaced(tile, i - 1, k);
                            RotateTile();
                        }

                    }
                    if (placedTiles[i + 1, k] == null)
                    {
                        isNotAloneRemix = true;
                        for (int j = 0; j < 4; j++)
                        {
                            if (script.West == placedTiles[i, k].GetComponent<TileScript>().East) return CheckNeighborsIfTileCanBePlaced(tile, i + 1, k);
                            RotateTile();
                        }

                    }
                    if (placedTiles[i, k - 1] == null)
                    {
                        isNotAloneRemix = true;
                        for (int j = 0; j < 4; j++)
                        {
                            if (script.North == placedTiles[i, k].GetComponent<TileScript>().South) return CheckNeighborsIfTileCanBePlaced(tile, i, k - 1);
                            RotateTile();
                        }
                    }
                    if (placedTiles[i, k + 1] == null)
                    {
                        isNotAloneRemix = true;
                        for (int j = 0; j < 4; j++)
                        {
                            if (script.South == placedTiles[i, k].GetComponent<TileScript>().North) return CheckNeighborsIfTileCanBePlaced(tile, i, k + 1);
                            RotateTile();
                        }
                    }
                }
            }
        }
        return isNotAloneRemix;
    }

    /// <summary>
    /// AimTile doesn't do anything but visualize where in the tile grid the player is pointing.
    /// </summary>
    void AimTile()
    {
        if (state == GameStates.TileHeld && CursorState == CursorStates.Inside)
        {
            float planeY = 0;
            Plane plane = new Plane(Vector3.up, Vector3.up * planeY);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out float distance))
            {
                AimX = ray.GetPoint(distance).x;
                AimZ = ray.GetPoint(distance).z;
            }
            int xs;
            int zs;
            if (AimX > 0)
            {
                xs = (int)(AimX + 1f) / 2;

            }
            else
            {
                xs = (int)(AimX - 1f) / 2;
            }
            if (AimZ > 0)
            {
                zs = (int)(AimZ + 1f) / 2;
            }
            else
            {
                zs = (int)(AimZ - 1f) / 2;
            }
            AimX = xs + 85;
            AimZ = zs + 85;
            try
            {
                //If the player points their brick within the grid, the held brick is displayed at the grid point where the players cursor is pointing
                if (placedTiles[xs + 85, zs + 85] == null)
                {
                    Mesh mesh = tileMesh.GetComponentInChildren<MeshFilter>().sharedMesh;
                    Graphics.DrawMesh(mesh, new Vector3(xs * 2, 0.0f, zs * 2), Quaternion.Euler(0.0f, 180.0f + (90.0f * NewTileRotation), 0.0f), currentTile.GetComponentInChildren<Renderer>().material, 0);
                }
            }
            catch (IndexOutOfRangeException e)
            {
                Debug.Log(e);
            }
        }
    }

    public void AimMeeple()
    {

        aimX = 0;
        aimZ = 0;
        float planeY = 0;
        Plane plane = new Plane(Vector3.up, Vector3.up * planeY);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out float distance))
        {
            aimX = ray.GetPoint(distance).x;
            aimZ = ray.GetPoint(distance).z;
        }
        if (aimX > 0)
        {
            xs = (int)(aimX + 1f) / 2;
        }
        else
        {
            xs = (int)(aimX - 1f) / 2;
        }
        if (aimZ > 0)
        {
            zs = (int)(aimZ + 1f) / 2;
        }
        else
        {
            zs = (int)(aimZ - 1f) / 2;
        }
        try
        {
            if (placedTiles[xs + 85, zs + 85] != null && placedTiles[xs + 85, zs + 85] == currentTile)
            {
                GameObject tile = placedTiles[xs + 85, zs + 85];
                TileScript tileScript = tile.GetComponent<TileScript>();

                aimX -= xs * 2;
                aimZ -= zs * 2;

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
                        float tmpX = xs * 2 + aimX;
                        float tmpZ = zs * 2 + aimZ;
                        //Debug.Log("X: " + (tmpX) + " z: " + (tmpZ));
                        Mesh mesh = meepleMesh.GetComponentInChildren<MeshFilter>().sharedMesh;
                        // Material mat = meeples[0][0].GetComponent<MeepleScript>().materials[currentPlayer];
                        Material mat = playerScript.GetPlayer(currentPlayer).meeples[0].GetComponent<MeepleScript>().material;
                        Graphics.DrawMesh(mesh, new Vector3(tmpX, 0.175f, tmpZ), Quaternion.Euler(0.0f, 180.0f + (90.0f * NewTileRotation), 0.0f), mat, 0);
                    }
                }
            }
        }
        catch (IndexOutOfRangeException e)
        {
            Debug.Log(e);
        }
    }


    public void PlaceMeeple(GameObject meeple, int xs, int zs, PointScript.Direction direction, TileScript.geography meepleGeography, float tmpX, float tmpZ)
    {
        TileScript currentTileScript = placedTiles[xs + 85, zs + 85].GetComponent<TileScript>();
        if (!currentTileScript.checkIfOcupied(direction))
        {
            TileScript.geography geography = currentTileScript.getGeographyAt(direction);
            currentTileScript.occupy(direction);
            if (meepleGeography == TileScript.geography.Cityroad) meepleGeography = TileScript.geography.City;
            meeple.GetComponent<MeepleScript>().assignAttributes(xs + 85, zs + 85, direction, meepleGeography);
            meeple.GetComponentInChildren<MeshRenderer>().enabled = true;
            meeple.transform.position = new Vector3(tmpX, 0.175f, tmpZ);
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
        calculatePoints(false, false);
        state = GameStates.TileDown;
    }
    //Metod för att plocka upp en ny tile
    public void PickupBrick()
    {

        currentTile = stackScript.Pop();
        NewTileRotation = 0;

        if (cantTileBePlaced(currentTile))//Ska den göras för varje rotation eller räcker detta? osäker
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
            if (TilePlacementIsValid(currentTile, (int)AimX, (int)AimZ))
            {
                PlaceBrick(currentTile, (int)AimX, (int)AimZ);
                invalidTile.GetComponent<CardSlideScript>().InvalidTile(false);
            }
            else if (!TilePlacementIsValid(currentTile, (int)AimX, (int)AimZ))
            {
                invalidTile.GetComponent<CardSlideScript>().InvalidTile(true);

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
                    if (CityIsFinished(meeple.x, meeple.z))
                    {
                        if (placedTiles[meeple.x, meeple.z].GetComponent<TileScript>().getCenter() == TileScript.geography.Stream || placedTiles[meeple.x, meeple.z].GetComponent<TileScript>().getCenter() == TileScript.geography.Grass || placedTiles[meeple.x, meeple.z].GetComponent<TileScript>().getCenter() == TileScript.geography.Road)
                        {
                            finalscore = GetComponent<PointScript>().startDfsDirection(placedTiles[meeple.x, meeple.z].GetComponent<TileScript>().vIndex, meeple.geography, meeple.direction, GameEnd);
                            //Debug.Log("Finalscore: " + finalscore);
                        }
                        else
                        {
                            finalscore = GetComponent<PointScript>().startDfs(placedTiles[meeple.x, meeple.z].GetComponent<TileScript>().vIndex, meeple.geography, GameEnd);
                            //Debug.Log("Finalscore: " + finalscore);
                        }
                    }


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
        //NewTileRotation = 0;
        //currentTile.GetComponent<TileScript>().resetRotation();
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
        }

        if (renderCurrentTile)
        {
            AimTile();
        }

        int touches = Input.touchCount;


        if (touches > 0 && touches < 2)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                AimTile();
            }
            if (touch.phase == TouchPhase.Ended)
            {
                bool hasNoNeighbours = true;
                int[] neighbours = GetNeighbors((int)AimX, (int)AimZ);
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
        Debug.Log(CursorState);
        //If the player presses the R button they rotate the temporary "display" tile AND the currently held tile.
        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateTile();
        }
        if (state == GameStates.MeepleHeld)
        {
            AimMeeple();
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

}
