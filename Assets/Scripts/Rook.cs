using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : ChessPiece
{
    public override List<Vector2Int> AvailableMoves(ref ChessPiece[,] board, int b_size)
    {
        List<Vector2Int> avMoves = new List<Vector2Int>();

        for (int i = currentY-1; i >= 0; i--)
        {
            if(board[currentX, i] == null)
                avMoves.Add(new Vector2Int(currentX,i));
            if(board[currentX,i] != null)
            {
                if(board[currentX,i].team != team)
                    avMoves.Add(new Vector2Int(currentX,i));
                break;
            }
        }

        for (int i = currentX - 1; i >= 0; i--)
        {
            if(board[i, currentY] == null)
                avMoves.Add(new Vector2Int(i,currentY));
            if(board[i, currentY] != null)
            {
                if(board[i,currentY].team != team)
                    avMoves.Add(new Vector2Int(i,currentY));
                break;
            }
        }

        for (int i = currentY+1; i < b_size; i++)
        {
            if(board[currentX, i] == null)
                avMoves.Add(new Vector2Int(currentX,i));
            if(board[currentX,i] != null)
            {
                if(board[currentX,i].team != team)
                    avMoves.Add(new Vector2Int(currentX,i));
                break;
            }
        }

        for (int i = currentX + 1; i < b_size; i++)
        {
            if(board[i, currentY] == null)
                avMoves.Add(new Vector2Int(i,currentY));
            if(board[i, currentY] != null)
            {
                if(board[i,currentY].team != team)
                    avMoves.Add(new Vector2Int(i,currentY));
                break;
            }
        }


        return avMoves;
    }
}
