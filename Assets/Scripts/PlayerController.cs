/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerController : MonoBehaviour
{
    
    public bool isCameraRotated = false;
    public bool autoChangeCam = false;
    private bool isWhiteTurn;
    public TextMeshProUGUI updateText;
    public TextMeshProUGUI resultText;
    private ChessBoard cboard;

    void Start()
    {
        ChessBoard chessBoard = FindObjectOfType<ChessBoard>();
        if (chessBoard != null)
        {
            SetChessBoard(chessBoard);
        }
        else
        {
            Debug.LogError("ChessBoard object not found!");
        }
    }
     public void SetChessBoard(ChessBoard chessBoard)
    {
        cboard = chessBoard;
    }
    public void ChangeTheAutomaticView(){
        autoChangeCam = !autoChangeCam;
    }

    public void ChangeTheView()
    {
        isCameraRotated = !isCameraRotated;
        Vector3 currentRotation = Camera.main.transform.rotation.eulerAngles;
        float newYRotation = isCameraRotated ? 180.0f : 0.0f;
        float newZPosition = isCameraRotated ? 35.0f : 0.0f;
        Quaternion newRotation = Quaternion.Euler(currentRotation.x, newYRotation, currentRotation.z);
        Vector3 newPosition = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, newZPosition);
        Camera.main.transform.rotation = newRotation;
        Camera.main.transform.position = newPosition;
    }

    public void PerformTurn(){
        UpdateTurn();
    }

    private void UpdateTurn()
    {
        isWhiteTurn = !isWhiteTurn;
        int opponentTeam = isWhiteTurn ? 0 : 1;
        bool isCheck = cboard.IsCheck(opponentTeam);
        bool isCheckmate = cboard.IsCheckmate();
        updateText.text = "";

        UpdateUI(isCheck, isCheckmate);
    }

    public void UpdateUI(bool isCheck, bool isCheckmate)
    {
        if (isCheck)
        {
            if (!isWhiteTurn)
            {
                updateText.color = new Color(0, 0, 0);
            }
            else
            {
                updateText.color = new Color(1, 1, 1);
            }
            updateText.text = "Check!";
        }

        if (isCheckmate)
        {
            resultText.gameObject.SetActive(true);
            if (!isWhiteTurn)
                resultText.text = "Checkmate! White wins";
            else
                resultText.text = "Checkmate! Black wins";
        }
    }
    public void UpdateUI(bool whiteTurn)
    {
         if (whiteTurn)
        {
            updateText.color = new Color(0, 0, 0);
        }
        else
        {
            updateText.color = new Color(1, 1, 1);
        }
        updateText.text = "Not your turn";
    }
    public void HandleMouseDown(ChessPiece cMove, ChessPiece[,] cPcs)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            Vector2Int hitInfo = cboard.TileIndex(hit.transform.gameObject);

            if (cboard.IsValidPosition(hitInfo.x, hitInfo.y))
            {
                if (cPcs[hitInfo.x, hitInfo.y] != null)
                {   
                    cboard.HandlePieceSelection(hitInfo);
                }
                else
                {
                    cMove = null;
                    cboard.RemoveHighlightTiles();
                }
            }
        }
    }

    public void HandleMouseUp(ChessPiece cMove)
    {
        if (cMove != null)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                Vector2Int hitInfo = cboard.TileIndex(hit.transform.gameObject);
                Vector2Int lastPosition = new Vector2Int(cMove.currentX, cMove.currentY);
                bool validMove = cboard.MakeMove(cMove, hitInfo.x, hitInfo.y);

                if (!validMove)
                {
                    cboard.RevertMove(cMove, lastPosition.x, lastPosition.y);
                }

                cMove = null;
                cboard.RemoveHighlightTiles();
            }
        }
    }

}*/
