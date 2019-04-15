using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : PieceIdentity //inheriting PieceIdentity
{
    public override int[,] isAllowed(PieceIdentity[,] BoardPieces, bool Predict)
    {
        int[,] arr = new int[8, 8];
        int[,] knightMoves = new int[,]{ {currX+1 ,currY+2 }, { currX+2, currY+1 }, { currX+2, currY-1 }, { currX+1, currY-2 }, { currX-1, currY-2 }, { currX-2, currY-1 }, { currX-2, currY+1 }, { currX-1, currY+2 }};
        
        for(int i=0; i<8;i++)
        {
            if (isInsideBoard(knightMoves[i,0], knightMoves[i, 1]))
            {
                if ( BoardPieces[knightMoves[i, 0], knightMoves[i, 1]] == null)//normal move
                    arr[knightMoves[i, 0], knightMoves[i, 1]] = 1;
                else if( BoardPieces[knightMoves[i, 0], knightMoves[i, 1]].isWhitePiece != isWhitePiece)//opposite color check
                    arr[knightMoves[i, 0], knightMoves[i, 1]] = 2;//kill
            }
        }

        return arr;
    }
}
