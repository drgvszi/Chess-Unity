using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChessPieceType{
    None = 0,
    Pawn = 1,
    Rook = 2,
    Knight = 3,
    Bishop = 4,
    Queen = 5,
    King = 6
}

public abstract class ChessPiece : MonoBehaviour
{

    public int team;
    public int currentX;
    public int currentY;
    public ChessPieceType type;
    private Vector3 desiredPosition;
    
    public abstract List<Vector2Int> AvailableMoves(ref ChessPiece[,] board, int b_size);
    public abstract  List<Vector2Int> SpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int> avMoves, ref List<Vector2Int> movesHistory, int b_size);
    protected bool IsWithinBounds(int x, int y, int b_size)
    {
        return x >= 0 && x < b_size && y >= 0 && y < b_size;
    }
    
}
