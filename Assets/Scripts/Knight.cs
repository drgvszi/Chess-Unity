using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : ChessPiece
{
    public override List<Vector2Int> AvailableMoves(ref ChessPiece[,] board, int b_size)
    {
        List<Vector2Int> avMoves = new List<Vector2Int>();

        int[] x_pos = { 2, 2, -2, -2, 1, 1, -1, -1 };
        int[] y_pos = { -1, 1, 1, -1, 2, -2, 2, -2 };

        for (int i = 0; i < b_size; i++)
        {
            int newX = currentX + x_pos[i];
            int newY = currentY + y_pos[i];

            if (IsWithinBounds(newX, newY, b_size))
            {
                ChessPiece targetPiece = board[newX, newY];
                if (targetPiece == null || targetPiece.team != team)
                {
                    avMoves.Add(new Vector2Int(newX, newY));
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
       