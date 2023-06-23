using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : ChessPiece
{
    public override List<Vector2Int> AvailableMoves(ref ChessPiece[,] board, int b_size)
    {
        List<Vector2Int> avMoves = new List<Vector2Int>();

        int[] directions = { 1, -1 };

        foreach (int xDir in directions)
        {
            foreach (int yDir in directions)
            {
                int i = currentX + xDir;
                int j = currentY + yDir;

                while (i >= 0 && i < b_size && j >= 0 && j < b_size)
                {
                    if (board[i, j] == null)
                    {
                        avMoves.Add(new Vector2Int(i, j));
                    }
                    else
                    {
                        if (board[i, j].team != team)
                        {
                            avMoves.Add(new Vector2Int(i, j));
                        }
                        break;
                    }
                    i += xDir;
                    j += yDir;
                }
            }
        }

        return avMoves;
    }
    public override List<Vector2Int> SpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int> avMoves, ref List<Vector2Int> movesHistory, int b_size)
    {
        List<Vector2Int> spMoves = new List<Vector2Int>();
        return spMoves;
    }
}
