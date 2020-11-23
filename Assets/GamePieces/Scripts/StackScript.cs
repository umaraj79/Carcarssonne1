using UnityEngine;

/// <summary>
/// The Stack of tiles.
/// </summary>
public class StackScript : MonoBehaviour{

    /// <summary>
    /// A reference to the prefab Tile, to be used later.
    /// </summary>
    public UnityEngine.GameObject tile;

    /// <summary>
    /// The array of tiles
    /// </summary>
    UnityEngine.GameObject[] tileArray;
    /// <summary>
    /// An array of ID's.
    /// </summary>
    int[] tiles;
    /// <summary>
    /// The next tile
    /// </summary>
    int nextTile;

    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="array"></param>
    public void setArray(UnityEngine.GameObject[] array)
    {
        this.tileArray = array;
        this.nextTile = array.Length;
        
        if (array != null)
        {
            setAll();
        }
    }

    /// <summary>
    /// Creates a new tile by giving it an ID. The ID corresponds to a category of tiles with distinct attributes that only it is aware of.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public UnityEngine.GameObject createTiles(int id)
    {   
        
        GameObject res = Instantiate(tile, new Vector3(2.0f, 0.0f, 0.0f), Quaternion.identity);
        res.GetComponent<TileScript>().AssignAttributes(id + 1);

        return res;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tiles"></param>
    /// <returns></returns>
    public int[] generateIDs(int[] tiles)
    {
        tiles = new int[84];
        int counter = 0;
        int[] array = new int[33];

        array[0] = 4;  array[1] = 2;  array[2] = 8;  array[3] = 9;  array[4] = 4;
        array[5] = 1;  array[6] = 5;  array[7] = 4;  array[8] = 3;  array[9] = 3;
        array[10] = 3;  array[11] = 1;  array[12] = 3;  array[13] = 3;  array[14] = 2;
        array[15] = 3;  array[16] = 2;  array[17] = 2;  array[18] = 2;  array[19] = 3;
        array[20] = 1;  array[21] = 1;  array[22] = 2;  array[23] = 1;  array[24] = 2;
        array[25] = 2;  array[26] = 2;  array[27] = 1;  array[28] = 1;  array[29] = 1;
        array[30] = 1;  array[31] = 1;  array[32] = 1;

        for (int i = 0; i < array.Length; i++)
        {
            for (int j = 0; j < array[i]; j++)
            {
                tiles[counter] = i;
                counter++;
            }
        }

        return tiles;
    }
       
    /// <summary>
    /// Creates all of the tiles.
    /// </summary>
    public void setAll()
    {
        tileArray = new UnityEngine.GameObject[85];
        this.tiles = generateIDs(tiles);
        nextTile = tileArray.Length-1;

        this.tiles = shuffle();
        for (int j = 0; j < tiles.Length; j++)
        {
            UnityEngine.GameObject tmp = InstatiateTiles(tiles[j], 0.0f, 0.05f * j, 0.0f);
            tileArray[j] = tmp;
        }
        tileArray[tileArray.Length-1] = InstatiateTiles(7, 0.0f, 0.0f, 0.0f);
        for(int j = 0; j < tileArray.Length; j++)
        {
            tileArray[j].transform.parent = tileArray[tileArray.Length - 1].transform;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public UnityEngine.GameObject InstatiateTiles(int id, float x, float y, float z)
    {
        GameObject res = Instantiate(tile, new Vector3(x, y, z), Quaternion.identity);
        res.GetComponent<TileScript>().AssignAttributes(id + 1);
        res.GetComponentInChildren<MeshRenderer>().enabled = false;

        return res;
    }

    /// <summary>
    /// Shuffles the array of tiles.
    /// </summary>
    /// <returns></returns>
    private int[] shuffle()
    {
        System.Random rand = new System.Random();

        for (int i = tiles.Length - 1; i > 0; i--)
        {
            int randomIndex = rand.Next(0, i + 1);

            int temp = tiles[i];
            tiles[i] = tiles[randomIndex];
            tiles[randomIndex] = temp;
        }
        return this.tiles;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameObj"></param>
    public void Push(GameObject gameObj)
    {
        tileArray[nextTile+1] = gameObj;
        nextTile++;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public UnityEngine.GameObject Pop()
    {
        UnityEngine.GameObject tile = tileArray[nextTile];
        nextTile--;

        return tile;
    }

    public int GetTileCount()
    {
        return nextTile;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public StackScript createStackScript()
    {
        setAll();
        return this;
    }

}
