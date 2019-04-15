using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : PieceIdentity //inheriting PieceIdentity
{
    public override int[,] isAllowed(PieceIdentity[,] BoardPieces, bool Predict)
    {
        int[,] arr = new int[8, 8];

        int y = (isWhitePiece) ? 1 : -1;
        int start = (isWhitePiece) ? 1 : 6;
        int end = (isWhitePiece) ? 7 : 0;
        /*Vector2 forward = new Vector2(0, i);//forward wrt piece
        Vector2 right = new Vector2(i, 0); //x+y for right
        Vector2 left = new Vector2(-i, 0); //x-y for left


        Use above reference to understand why and when is y used...
        */
        //int end = start + (6 * y);

        if (end-1 > (y * currY))//end not reached yet
            if ( BoardPieces[currX, currY + y] == null)
                 arr[currX, currY + y] = 1;//1 for normal move

        if (currY == start)
            if (( BoardPieces[currX, currY + y] == null) && ( BoardPieces[currX, currY + 2 * y] == null))
                arr[currX, currY + 2 * y] = 1;

        if (currX!=(end)&&currY!=(end-y)&& BoardPieces[currX + y, currY + y] != null &&  BoardPieces[currX + y, currY + y].isWhitePiece != isWhitePiece)
            arr[currX + y, currY + y] = 2;//2 for normal kill/capture move

        if (currX!=(start-y) && currY != (end - y) &&  BoardPieces[currX - y, currY + y] != null &&  BoardPieces[currX - y, currY + y].isWhitePiece != isWhitePiece)
            arr[currX - y, currY + y] = 2;

        //en passant
        if(currY==(start+(y*3)))//en passant position
        {
            if ( BoardManager.historyOfMoves[0,0] == 6 &&  BoardManager.historyOfMoves[0, 4] == (start + (y * 3)))//pawn at same y position
            {
                if(currX<7)
                {
                    if ( BoardManager.historyOfMoves[0, 3] == currX + 1)
                        arr[currX + 1, currY + y] = 3;//3 for en passant kill
                }
                if(currX>0)
                {
                    if ( BoardManager.historyOfMoves[0, 3] == currX - 1)
                        arr[currX - 1, currY + y] = 3;
                }
            }
        }

        if(currY+y==end)//promotion
        {
            if ( BoardPieces[currX, currY + y] == null)
                arr[currX, currY + y] = 4;//4 for promotion
            if(currX != (end))
                if ( BoardPieces[currX+y, currY + y] != null)
                    if( BoardPieces[currX+y, currY + y].isWhitePiece!=isWhitePiece)
                        arr[currX+y, currY + y] = 5;//5 for promotion kill
            if (currX != (start - y))
            {
                if ( BoardPieces[currX - y, currY + y] != null)
                {
                    if ( BoardPieces[currX - y, currY + y].isWhitePiece != isWhitePiece)
                    {
                        arr[currX - y, currY + y] = 5;//5 for promotion kill
                    }
                }
            }
        }

        return arr;
    }
}
