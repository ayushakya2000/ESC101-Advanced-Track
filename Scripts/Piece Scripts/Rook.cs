using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : PieceIdentity //inheriting PieceIdentity
{
    public override int[,] isAllowed(PieceIdentity[,] BoardPieces, bool Predict)
    {
        int[,] arr = new int[8, 8];
        int i = 1;
        while(isInsideBoard(currX, currY + i))//up
        {
            if ( BoardPieces[currX, currY + i] == null)
                arr[currX, currY + i] = 1;
            else if ( BoardPieces[currX, currY + i].isWhitePiece == isWhitePiece)
                break;
            else
            {
                arr[currX, currY + i] = 2;//kill
                break;
            }
            i++;
        }

        i = 1;

        while (isInsideBoard(currX, currY - i))//down
        {
            if ( BoardPieces[currX, currY - i] == null)
                arr[currX, currY - i] = 1;
            else if ( BoardPieces[currX, currY - i].isWhitePiece == isWhitePiece)
                break;
            else
            {
                arr[currX, currY - i] = 2;//kill
                break;
            }
            i++;
        }

        i = 1;

        while (isInsideBoard(currX+i, currY))//right
        {
            if ( BoardPieces[currX+i, currY] == null)
                arr[currX+i, currY] = 1;
            else if ( BoardPieces[currX+i, currY].isWhitePiece == isWhitePiece)
                break;
            else
            {
                arr[currX+i, currY] = 2;//kill
                break;
            }
            i++;
        }

        i = 1;

        while (isInsideBoard(currX-i, currY))//left
        {
            if ( BoardPieces[currX-i, currY] == null)
                arr[currX-i, currY] = 1;
            else if ( BoardPieces[currX-i, currY].isWhitePiece == isWhitePiece)
                break;
            else
            {
                arr[currX-i, currY] = 2;//kill
                break;
            }
            i++;
        }

        return arr;
    }
}
