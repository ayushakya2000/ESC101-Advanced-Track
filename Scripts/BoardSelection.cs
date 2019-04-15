using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSelection : MonoBehaviour// add this script to board and put tile under prefabs.
{
    const float tileSize = 1.0f;
    public static BoardSelection BoardSelectionObject { get; set; }//prefer to use color over colour :)
    BoardManager bm = new BoardManager();// to use givePos function
    public List<GameObject> coloredTiles=new List<GameObject>();//list of all colored tiles
    public List<GameObject> tilePrefabs = new List<GameObject>();//later on convert this to list and make kill and move tiles different

    Quaternion tileblackCorrection = Quaternion.Euler(270, 270, 0);
    Quaternion tilewhiteCorrection = Quaternion.Euler(270, 180, 0);
    // Start is called before the first frame update
    void Start()
    {
        BoardSelectionObject = this;//refers to the gameObject to whom this script is attatched to. In this case, the plane white tile we had.
        createActualBoard();
    }

    void createActualBoard()
    {
        //create board
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if ((i + j) % 2 == 1)
                    getColorTile(3, i, j);//light tile
                else
                    getColorTile(4, i, j);//dark tile
            }
        }
    }

    public Vector3 givePos(int x, int y, int col)//gives position at middle of x,y
    {
        if(col>=3&&col<5)//<5 to exclude yellow tile
            return (new Vector3((x * tileSize) + (tileSize / 2), -0.14f, (y * tileSize) + (tileSize / 2)));
        else//elevated for colored tiles
            return (new Vector3((x * tileSize) + (tileSize / 2), -0.05f, (y * tileSize) + (tileSize / 2)));
    }

    GameObject getColorTile(int col, int x,int y)//col is color,
    {
        GameObject newTile;
        if (col!=3)
            newTile = Instantiate(tilePrefabs[col], givePos(x, y, col), tileblackCorrection);
        else
            newTile = Instantiate(tilePrefabs[col], givePos(x, y, col), tilewhiteCorrection);
        return newTile;
    }

    public void createColorSelection(int[,] validMoves, int x, int y)
    {
        GameObject selTile = getColorTile(5, x, y);//yellow, selection tile.
        coloredTiles.Add(selTile);

        for (int i=0;i<8;i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (validMoves[i, j]==1 || validMoves[i, j] == 6)//blue for castling too
                {
                    GameObject tile = getColorTile(0, i, j);//blue
                    coloredTiles.Add(tile);
                }
                if (validMoves[i, j] == 4 || validMoves[i, j] == 5)
                {
                    GameObject tile = getColorTile(2, i, j);//green
                    coloredTiles.Add(tile);
                }
                if(validMoves[i, j] == 2 || validMoves[i, j] == 3)
                {
                    GameObject tile = getColorTile(1, i, j);//red
                    coloredTiles.Add(tile);
                }
            }
        }
    }

    public void removeColorSelection()
    {
        foreach (GameObject obj in coloredTiles)
            Destroy(obj);
        coloredTiles.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
