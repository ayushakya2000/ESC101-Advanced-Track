using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PieceIdentity : MonoBehaviour
{
    public int currX { get; set; }//Auto implemented Property
    public int currY { get; set; }

    public bool hasEverMoved = false;//for castling
    public bool isWhitePiece;

    
    public static int[,] getKing(PieceIdentity[,] BoardPieces)//did this way coz returning chessPiece wasnt working for king
    {
        int[,] KingPos = new int[2,1];
        for (int i = 0; i < 8; i++)
        {
            bool k = false;
            for (int j = 0; j < 8; j++)
            {
                if (BoardPieces[i, j] != null)
                {
                    if (BoardPieces[i, j].isWhitePiece == BoardManager.isWhiteTurn && BoardPieces[i, j].GetType() == typeof(King))
                    {
                        KingPos[0, 0] = i;
                        KingPos[1, 0] = j;
                        k = true;
                        break;
                    }
                }
            }
            if (k)
                break;
        }

        return KingPos;
    }

    public int[,] getCheckArray(int[,] arr)
    {
        for(int i=0;i<8;i++)
        {
            for(int j=0;j<8;j++)
            {
                if (arr[i, j] > 0)
                {

                    PieceIdentity[,] nbd = getBoardAfter(BoardManager.BoardPieces, i, j);

                    if (isTileUnderDanger(nbd, getKing(nbd)[0, 0], getKing(nbd)[1, 0], BoardManager.isWhiteTurn))//kng is null, means, no king found, means king killed
                    {
                        arr[i, j] = -2;
                    }
                }
            }
        }

        return arr;
    }

    public bool movePossible(int[,] arr)//selection only done when a piece can actually move.
    {
        bool res = false;
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                if (arr[i, j] > 0)
                    res = true;
        return res;
    }

    public bool isTileUnderDanger(PieceIdentity[,] BoardPieces, int x, int y, bool isWhiteTurn)
    {
        bool danger = false;

        for(int i=0;i<8;i++)
        {
            for(int j=0;j<8;j++)
            {
                if (i == x && j == y) continue;

                if(BoardPieces[i,j]!=null)
                {
                    if (isWhiteTurn != BoardPieces[i, j].isWhitePiece)
                        if(BoardPieces[i, j].isAllowed(BoardPieces,false)[x, y]>0)
                        {
                            danger = true;
                            break;
                        }
                }
            }
            if (danger)
                break;
        }
        return danger;
    }

    public PieceIdentity[,] getBoardAfter(PieceIdentity[,] currBoard,int x1, int y1, int x2, int y2)//general function
    {
        PieceIdentity piece = currBoard[x1, y1];
        PieceIdentity[,] nbd = new PieceIdentity[8, 8];//create a copy, not assign currBoard to nbd
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                nbd[i, j] = currBoard[i, j];
        nbd[x1, y1] = null;
        nbd[x2, y2] = piece;

        return nbd;
    }

    public PieceIdentity[,] getBoardAfter(PieceIdentity[,] currBoard, int x2, int y2)//overloaded function for current piece
    {
        PieceIdentity piece = currBoard[currX, currY];
        PieceIdentity[,] nbd = new PieceIdentity[8, 8];//create a copy, not assign currBoard to nbd
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                nbd[i, j] = currBoard[i, j];
        nbd[currX, currY] = null;
        nbd[x2, y2] = piece;

        return nbd;
    }

    public void setXY(int x, int y)
    {
        currX = x;
        currY = y;
    }

    public bool isInsideBoard(int x,int y)//added after coding pawn
    {
        bool k = true;
        if (x < 0 || x > 7)
            k = false;
        if (y < 0 || y > 7)
            k = false;
        return k;
    }

    public virtual int[,] isAllowed(PieceIdentity[,] BoardPieces, bool Predict)//to override in derived classes, use virtual
    {
        int[,] validMoves = new int[8, 8];

        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                validMoves[i, j] = 0;

        return validMoves;
    }
}
