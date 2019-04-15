using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : PieceIdentity //inheriting PieceIdentity
{
    public override int[,] isAllowed(PieceIdentity[,] BoardPieces, bool Predict)
    {
        int[,] arr = new int[8, 8];
        int i = 1;
        while (isInsideBoard(currX+i, currY + i))//up right
        {
            if ( BoardPieces[currX+i, currY + i] == null)
                arr[currX+i, currY + i] = 1;
            else if ( BoardPieces[currX+i, currY + i].isWhitePiece == isWhitePiece)
                break;
            else
            {
                arr[currX+i, currY + i] = 2;//kill
                break;
            }
            i++;
        }

        i = 1;

        while (isInsideBoard(currX+i, currY - i))//down right
        {
            if ( BoardPieces[currX+i, currY - i] == null)
                arr[currX+i, currY - i] = 1;
            else if ( BoardPieces[currX+i, currY - i].isWhitePiece == isWhitePiece)
                break;
            else
            {
                arr[currX+i, currY - i] = 2;//kill
                break;
            }
            i++;
        }

        i = 1;

        while (isInsideBoard(currX - i, currY+i))//up left
        {
            if ( BoardPieces[currX - i, currY+i] == null)
                arr[currX - i, currY+i] = 1;
            else if ( BoardPieces[currX - i, currY+i].isWhitePiece == isWhitePiece)
                break;
            else
            {
                arr[currX - i, currY+i] = 2;//kill
                break;
            }
            i++;
        }

        i = 1;

        while (isInsideBoard(currX - i, currY-i))//down left
        {
            if ( BoardPieces[currX - i, currY-i] == null)
                arr[currX - i, currY-i] = 1;
            else if ( BoardPieces[currX - i, currY-i].isWhitePiece == isWhitePiece)
                break;
            else
            {
                arr[currX - i, currY-i] = 2;//kill
                break;
            }
            i++;
        }

        return arr;
    }
}
