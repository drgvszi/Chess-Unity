using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    public override List<Vector2Int> AvailableMoves(ref ChessPiece[,] board, int b_size)
    {
        List<Vector2Int> avMoves = new List<Vector2Int>();
        int[] x_pos = { 0, 1, 1, 1, 0, -1, -1, -1 }; // -> Possible moves (x + x_pos, y + y_pos)
        int[] y_pos = { 1, 1, 0, -1, -1, -1, 0, 1 };

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

}