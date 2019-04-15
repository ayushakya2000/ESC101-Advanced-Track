using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : PieceIdentity //inheriting PieceIdentity
{
    public static bool isCheck;

    public override int[,] isAllowed(PieceIdentity[,] BoardPieces, bool Predict)
    {
        int[,] arr = new int[8, 8];
        int i = 1;

        if (isInsideBoard(currX, currY + i))//up
        {
            if ( BoardPieces[currX, currY + i] == null)
                arr[currX, currY + i] = 1;
            else if ( BoardPieces[currX, currY + i].isWhitePiece != isWhitePiece)
                arr[currX, currY + i] = 2;//kill
        }

        if (isInsideBoard(currX, currY - i))//down
        {
            if ( BoardPieces[currX, currY - i] == null)
                arr[currX, currY - i] = 1;
            else if ( BoardPieces[currX, currY - i].isWhitePiece != isWhitePiece)
                arr[currX, currY - i] = 2;//kill
        }

        if (isInsideBoard(currX + i, currY))//right
        {
            if ( BoardPieces[currX + i, currY] == null)
                arr[currX + i, currY] = 1;
            else if ( BoardPieces[currX + i, currY].isWhitePiece != isWhitePiece)
                arr[currX + i, currY] = 2;//kill
        }

        

        if (isInsideBoard(currX - i, currY))//left
        {
            if ( BoardPieces[currX - i, currY] == null)
                arr[currX - i, currY] = 1;
            else if ( BoardPieces[currX - i, currY].isWhitePiece != isWhitePiece)
                arr[currX - i, currY] = 2;//kill
        }

        

        if (isInsideBoard(currX + i, currY + i))//up right
        {
            if ( BoardPieces[currX + i, currY + i] == null)
                arr[currX + i, currY + i] = 1;
            else if ( BoardPieces[currX + i, currY + i].isWhitePiece != isWhitePiece)
                arr[currX + i, currY + i] = 2;//kill
        }

        

        if (isInsideBoard(currX + i, currY - i))//down right
        {
            if ( BoardPieces[currX + i, currY - i] == null)
                arr[currX + i, currY - i] = 1;
            else if ( BoardPieces[currX + i, currY - i].isWhitePiece != isWhitePiece)
                arr[currX + i, currY - i] = 2;//kill
        }

        

        if (isInsideBoard(currX - i, currY + i))//up left
        {
            if ( BoardPieces[currX - i, currY + i] == null)
                arr[currX - i, currY + i] = 1;
            else if ( BoardPieces[currX - i, currY + i].isWhitePiece != isWhitePiece)
                arr[currX - i, currY + i] = 2;//kill
        }

        

        if (isInsideBoard(currX - i, currY - i))//down left
        {
            if ( BoardPieces[currX - i, currY - i] == null)
                arr[currX - i, currY - i] = 1;
            else if ( BoardPieces[currX - i, currY - i].isWhitePiece != isWhitePiece)
                arr[currX - i, currY - i] = 2;//kill
        }

        if (Predict)//if Predict is true everytime, might get stuck in infinite loop
        {
            for (int i1 = currX - 1; i1 <= currX + 1; i1++)//king cant move such that he falls under check and direct kill
            {
                for (int j1 = currY - 1; j1 <= currY + 1; j1++)
                {
                    if (i1 == currX && j1 == currY)
                        continue;
                    if (isInsideBoard(i1, j1) && arr[i1, j1]>0)
                    {
                        Debug.Log("--1-king=" + BoardPieces[currX, currY]);
                        PieceIdentity[,] nbd = getBoardAfter(BoardPieces, i1, j1);
                        Debug.Log("--2-king=" + BoardPieces[currX, currY]);
                        if (nbd[i1,j1].isTileUnderDanger(nbd,i1,j1,BoardManager.isWhiteTurn))
                        {
                            arr[i1, j1] = -1;//if moved will be under check. so blocked this move.
                        }
                    }
                }
            }
        }

        //Castling Move
        int end = (isWhitePiece) ? 0 : 7;
        if(BoardPieces[7,end]!=null)//means moved
        if (hasEverMoved==false&&BoardPieces[7,end].hasEverMoved==false&&Predict==true)//King and rook have never moved
        {
            //if 4,5,6 arent under direct attack
            if (BoardPieces[5, end] == null && BoardPieces[6, end]==null)
            {
                if (!isTileUnderDanger(BoardPieces, 4, end, BoardManager.isWhiteTurn) && !isTileUnderDanger(BoardPieces, 5, end, BoardManager.isWhiteTurn) && !isTileUnderDanger(BoardPieces, 6, end, BoardManager.isWhiteTurn))
                {
                    Debug.Log("In3");
                    arr[6, end] = 6;//castling
                }
            }
        }
        return arr;
    }
}
