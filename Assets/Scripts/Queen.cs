using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : ChessPiece
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


        for (int i = currentX + 1; i < b_size; i++)
        {
            if(board[i, currentY] == null)
                avMoves.Add(new Vector2Int(i,currentY));
            if(board[i,currentY] != null)
            {
                if(board[i,currentY].team != team)
                    avMoves.Add(new Vector2Int(i,currentY));
                break;
            }
        }

        for (int i = currentX + 1, j = currentY + 1; i < b_size && j < b_size; i++, j ++)
        {
            if(board[i, j] == null)
                avMoves.Add(new Vector2Int(i,j));
            else
            {
            if(board[i,j].team != team)
                avMoves.Add(new Vector2Int(i,j));
            break; 
            }
        }
        for (int i = currentX - 1, j = currentY + 1; i >= 0 && j < b_size; i--, j++)
        {
            if(board[i, j] == null)
                avMoves.Add(new Vector2Int(i,j));
            else
            {
            if(board[i,j].team != team)
                avMoves.Add(new Vector2Int(i,j));
            break; 
            }
        }

        for (int i = currentX + 1, j = currentY -1; i < b_size && j >= 0; i++, j--)
        {
            if(board[i, j] == null)
                avMoves.Add(new Vector2Int(i,j));
            else
            {
            if(board[i,j].team != team)
                avMoves.Add(new Vector2Int(i,j));
            break; 
            }
        }

        for (int i = currentX - 1, j = currentY -1; i >=0 && j >= 0; i--, j--)
        {
            if(board[i, j] == null)
                avMoves.Add(new Vector2Int(i,j));
            else
            {
            if(board[i,j].team != team)
                avMoves.Add(new Vector2Int(i,j));
            break; 
            }
        }
        return avMoves;
    }
}
