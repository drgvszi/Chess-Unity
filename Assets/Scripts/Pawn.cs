using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece
{
    public override List<Vector2Int> AvailableMoves(ref ChessPiece[,] board, int b_size)
    {
        List<Vector2Int> avMoves = new List<Vector2Int>();
        int direction = (team == 0) ? 1 : -1;
        int nextY = currentY + direction;

        if (IsWithinBounds(currentX, nextY, b_size) && board[currentX, nextY] == null)
        {
            avMoves.Add(new Vector2Int(currentX, nextY));

            int doubleStepY = currentY + 2 * direction;
            if ((currentY == 1 || currentY == 6) && IsWithinBounds(currentX, doubleStepY, b_size) && board[currentX, doubleStepY] == null)
            {
                avMoves.Add(new Vector2Int(currentX, doubleStepY));
            }
        }

        int attackX1 = currentX + 1;
        int attackX2 = currentX - 1;

        if (IsWithinBounds(attackX1, nextY, b_size) && board[attackX1, nextY] != null && board[attackX1, nextY].team != team)
        {
            avMoves.Add(new Vector2Int(attackX1, nextY));
        }

        if (IsWithinBounds(attackX2, nextY, b_size) && board[attackX2, nextY] != null && board[attackX2, nextY].team != team)
        {
            avMoves.Add(new Vector2Int(attackX2, nextY));
        }
        
        return avMoves;
    }
    
    public override List<Vector2Int> SpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int> avMoves, ref List<Vector2Int> movesHistory, int b_size)
    {
        List<Vector2Int> svMoves = new List<Vector2Int>();

        // Check if the pawn is eligible for en passant
        int enemyDirection = (team == 0) ? -1 : 1;
        int prevY = currentY - enemyDirection;
        int prev2Y = currentY - 2 * enemyDirection;

        if (IsWithinBounds(currentX + 1, currentY, b_size) && board[currentX + 1, currentY] != null &&
            board[currentX + 1, currentY].team != team && board[currentX + 1, currentY] is Pawn &&
            movesHistory.Contains(new Vector2Int(currentX + 1, prev2Y)))
        {
            svMoves.Add(new Vector2Int(currentX + 1, currentY + enemyDirection));
        }

        if (IsWithinBounds(currentX - 1, currentY, b_size) && board[currentX - 1, currentY] != null &&
            board[currentX - 1, currentY].team != team && board[currentX - 1, currentY] is Pawn &&
            movesHistory.Contains(new Vector2Int(currentX - 1, prev2Y)))
        {
            svMoves.Add(new Vector2Int(currentX - 1, currentY + enemyDirection));
        }

        return svMoves;
    }
    
}
