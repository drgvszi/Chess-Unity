    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;

    public class ChessBoard : MonoBehaviour
    {   
        //Pieces and board appearance
        public GameObject tile;
        public GameObject[] piecesPref;
        public Material highlightedCell;
        public Material highlightedKingCell;
        public Material highlightedCellUnderPiece;
        public Material tileCellMat;
        public Material[] teamMat;
        
        //Game Logic
        const int white = 0;
        const int black = 1;
        const int boardSize = 8;
        const float tileSize = 5.0f;
        private bool isWhiteTurn;
        private bool winCondition;
        private bool gameStarted;
        private ChessPiece currentMove;
        private List<ChessPiece> deadW;
        private List<ChessPiece> deadB;
        private Vector3 DeathWhitePosition;
        private Vector3 DeathBlackPosition;
        private GameObject[,] boardArray;
        private ChessPiece[,] chessPieces;
        private List<Vector2Int> availableMoves;
        private List<Vector2Int> specialMoves;
        private List<Vector2Int> filteredMoves;
        private List<Vector2Int> movesHistory;
        public TextMeshProUGUI resultText;
        public TextMeshProUGUI statusText;
        public TextMeshProUGUI updateTurnText;
        public TextMeshProUGUI movesHistoryText;
        string[] letters = new string[]{"A", "B", "C", "D", "E", "F", "G", "H" };
        //Camera movement
        private bool isCameraRotated;
        private bool autoChangeCam;

        //Base Unity methods
        void Start()
        {   
            gameStarted = false;
            isCameraRotated = false;
            autoChangeCam = false;
            deadW = new List<ChessPiece>();
            deadB = new List<ChessPiece>();
            DeathWhitePosition = new Vector3(-1.5f, 1f, -4.5f);
            DeathBlackPosition = new Vector3(36.5f, 1f, 39.5f);
            boardArray= new GameObject[boardSize, boardSize];
            chessPieces = new ChessPiece[boardSize, boardSize];
            availableMoves = new List<Vector2Int>();
            specialMoves = new List<Vector2Int>();
            filteredMoves = new List<Vector2Int>();
            movesHistory = new List<Vector2Int>();
        }

        void Update()
        {
            
            if (gameStarted)
            {   
                if (Input.GetMouseButtonDown(0))
                {
                    HandleMouseDown();
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    HandleMouseUp();
                }
            }
        }

        public void CreateNewGame()
        {
            isWhiteTurn = true;
            winCondition = false;
            gameStarted = true;
            createTiles();
            spawnAllPieces();
            piecesPosition();
            UpdateUI();
        }

        // Mouse Handler
        private Vector2Int TileIndex( GameObject hit )
        {
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    if (boardArray[x,y] == hit)
                        return new Vector2Int(x,y);  
                }
            }
            return -Vector2Int.one;
        }

        void HandleMouseDown()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                Vector2Int hitInfo = TileIndex(hit.transform.gameObject);

                if (hitInfo.x >= 0 && hitInfo.y >= 0 && hitInfo.x < boardSize && hitInfo.y < boardSize)
                {
                    if (chessPieces[hitInfo.x, hitInfo.y] != null)
                    {
                        HandlePieceSelection(hitInfo);
                    }
                    else
                    {
                        currentMove = null;
                        RemoveHighlightTiles();
                    }
                }
            }
        }

        void HandleMouseUp()
        {
            if (currentMove != null)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, 100.0f))
                {
                    Vector2Int hitInfo = TileIndex(hit.transform.gameObject);
                    Vector2Int lastPosition = new Vector2Int(currentMove.currentX, currentMove.currentY);
                    bool validMove = MakeMove(currentMove, hitInfo.x, hitInfo.y);

                    if (!validMove)
                    {
                        RevertNonValidMove(currentMove, lastPosition.x, lastPosition.y);
                    }

                    currentMove = null;
                    RemoveHighlightTiles();
                }
            }
        }

        //Board and pieces management
        void HandlePieceSelection(Vector2Int selectedTile)
        {
            if (!winCondition)
            {
                if ((chessPieces[selectedTile.x, selectedTile.y].team == white && isWhiteTurn) || (chessPieces[selectedTile.x, selectedTile.y].team == black && !isWhiteTurn))
                {
                    currentMove = chessPieces[selectedTile.x, selectedTile.y];
                    availableMoves = currentMove.AvailableMoves(ref chessPieces, boardSize);
                    int team = isWhiteTurn ? white : black;
                    PreventCheck();
                    HighlightTiles();
                    if (IsCheck(team))
                    {
                        Vector2Int kingPosition = FindKingPosition(team);
                        HighlightKing(kingPosition);
                    }
                }
                else
                {
                    UpdateUI();

                }
            }
        }

        private void createTiles()
        {
            for( int x = 0; x < boardSize; x++ )   
            {
                for(int y = 0; y < boardSize; y++ )
                {
                    boardArray[x, y] = Instantiate(tile, new Vector3(x * tileSize, 1.01f, y * tileSize), Quaternion.identity);
                    boardArray[x, y].name = "[ X:" + x + ", Y:" + y + " ]";
                    boardArray[x, y].tag = "Tile";
                    boardArray[x,y].transform.parent = gameObject.transform;
                }
            }      
        }

        public void ClearTheTable()
        {
            gameStarted = false;
            winCondition = false;
            deadW.Clear();
            deadB.Clear();
            DeathWhitePosition = new Vector3(-1.5f, 1f, -5f);
            DeathBlackPosition = new Vector3(36.5f, 1f, 40f);
            availableMoves.Clear();
            specialMoves.Clear();
            resultText.gameObject.SetActive(false);
            currentMove = null;
            ClearBoardArray();
            DestroyChessPieces();
        }

        private void ClearBoardArray()
        {
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    Destroy(boardArray[x, y]);
                    boardArray[x, y] = null;
                }
            }
        }

        private void DestroyChessPieces()
        {
            ChessPiece[] chessPieces = FindObjectsOfType<ChessPiece>();
            foreach (ChessPiece chessPiece in chessPieces)
            {
                Destroy(chessPiece.gameObject);
            }
        }

        public ChessPiece spawnOnePiece(ChessPieceType type, int team)
        {
            ChessPiece cp = Instantiate(piecesPref[(int)type -1 ], transform).GetComponent<ChessPiece>();
            cp.GetComponent<MeshRenderer>().material = teamMat[team];
            cp.type = type;
            cp.team = team;
            return cp;
        }

        public void spawnAllPieces()
        {
            chessPieces = new ChessPiece[boardSize, boardSize];
            for(int i = 0; i < boardSize; i++)
            {
                chessPieces[i, 1] = spawnOnePiece(ChessPieceType.Pawn, white);
                chessPieces[i, 6] = spawnOnePiece(ChessPieceType.Pawn, black); 
            }

            chessPieces[0, 0] = spawnOnePiece(ChessPieceType.Rook, white);
            chessPieces[1, 0] = spawnOnePiece(ChessPieceType.Knight, white);
            chessPieces[2, 0] = spawnOnePiece(ChessPieceType.Bishop, white);
            chessPieces[3, 0] = spawnOnePiece(ChessPieceType.Queen, white);
            chessPieces[4, 0] = spawnOnePiece(ChessPieceType.King, white);
            chessPieces[5, 0] = spawnOnePiece(ChessPieceType.Bishop, white);
            chessPieces[6, 0] = spawnOnePiece(ChessPieceType.Knight, white);
            chessPieces[7, 0] = spawnOnePiece(ChessPieceType.Rook, white);

            chessPieces[0, 7] = spawnOnePiece(ChessPieceType.Rook, black);
            chessPieces[1, 7] = spawnOnePiece(ChessPieceType.Knight, black);
            chessPieces[2, 7] = spawnOnePiece(ChessPieceType.Bishop, black);
            chessPieces[3, 7] = spawnOnePiece(ChessPieceType.Queen, black);
            chessPieces[4, 7] = spawnOnePiece(ChessPieceType.King, black);
            chessPieces[5, 7] = spawnOnePiece(ChessPieceType.Bishop, black);
            chessPieces[6, 7] = spawnOnePiece(ChessPieceType.Knight, black);
            chessPieces[7, 7] = spawnOnePiece(ChessPieceType.Rook, black);
        
        }

        public void piecesPosition()
        {
            for (int x = 0; x < boardSize; x++)
                for (int y = 0; y < boardSize; y++)
                    if(chessPieces[x,y]!= null)
                    {
                        chessPieces[x,y].currentX = x;
                        chessPieces[x,y].currentY = y;
                        chessPieces[x,y].transform.position = new Vector3(x*tileSize, 1, y*tileSize);
                        chessPieces[x,y].tag = "Piece";

                        if( chessPieces[x,y].type == ChessPieceType.Knight && chessPieces[x,y].team == black )
                            chessPieces[x,y].transform.Rotate(0f, 0f, 180f);   

                        if( chessPieces[x,y].type == ChessPieceType.Pawn )
                            chessPieces[x,y].transform.position = chessPieces[x,y].transform.position + new Vector3(0, 1, 0);
                    }  
        }

        // Game Logic
        public bool IsCheck(int team)
        {
            // Find the position of the king for the specified team
            Vector2Int kingPosition = FindKingPosition(team);

            // Check if any opposing piece can attack the king
            foreach (ChessPiece piece in chessPieces)
            {
                if (piece != null && piece.team != team)
                {
                    List<Vector2Int> moves = piece.AvailableMoves(ref chessPieces, boardSize);
                    if (moves.Contains(kingPosition))
                    {
                        return true;
                    }
                    
                }
            }

            return false;
        }
        
        public bool IsCheckmate()
        {
            int team = isWhiteTurn ? white : black;
            if (!IsCheck(team))
                return false; // King is not in check, so not in checkmate

            // Check if any of the player's pieces have a valid move to get out of check
            foreach (ChessPiece piece in chessPieces)
            {
                if (piece != null && piece.team == team)
                {
                    List<Vector2Int> moves = piece.AvailableMoves(ref chessPieces, boardSize);
                    foreach (Vector2Int move in moves)
                    {
                        // Simulate the move
                        ChessPiece targetPiece = chessPieces[move.x, move.y];
                        MovePiece(piece, move.x, move.y);
                        bool isCheck = IsCheck(team);
                        // Revert the move
                        RevertNonValidMove(piece, piece.currentX, piece.currentY);
                        chessPieces[move.x, move.y] = targetPiece;

                        if (!isCheck)
                            return false; // There is a valid move to escape check
                    }
                }
            }

            return true; // No valid moves to escape check, so in checkmate
        }

        private void PreventCheck()
        {
            ChessPiece targetKing = null;
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    if (chessPieces[x,y] != null)
                    {
                        if (chessPieces[x, y].type == ChessPieceType.King)
                        {
                            if (chessPieces[x, y].team == currentMove.team)
                            {
                                targetKing = chessPieces[x,y];
                            }                       
                        }
                    }
                }
            }
            SimulateMove(currentMove, ref availableMoves, targetKing);
        }

        private void SimulateMove(ChessPiece cp, ref List<Vector2Int> moves, ChessPiece targetKing)
        {
            int actualX = cp.currentX;
            int actualY = cp.currentY;

            List<Vector2Int> movesToRemove = new List<Vector2Int>();

            foreach (var move in moves)
            {
                int simX = move.x;
                int simY = move.y;

                Vector2Int kingSimPosition = cp.type == ChessPieceType.King ? new Vector2Int(simX, simY) : new Vector2Int(targetKing.currentX, targetKing.currentY);

                ChessPiece[,] simulation = new ChessPiece[boardSize, boardSize];
                List<ChessPiece> simAttackingPieces = new List<ChessPiece>();

                for (int x = 0; x < boardSize; x++)
                {
                    for (int y = 0; y < boardSize; y++)
                    {
                        ChessPiece piece = chessPieces[x, y];
                        if (piece != null)
                        {
                            simulation[x, y] = piece;
                            if (piece.team != cp.team)
                            {
                                simAttackingPieces.Add(piece);
                            }
                        }
                    }
                }

                simulation[actualX, actualY] = null;
                cp.currentX = simX;
                cp.currentY = simY;
                simulation[simX, simY] = cp;

                var deadPiece = simAttackingPieces.Find(c => c.currentX == simX && c.currentY == simY);
                if (deadPiece != null)
                {
                    simAttackingPieces.Remove(deadPiece);
                }

                List<Vector2Int> simMoves = new List<Vector2Int>();

                foreach (var piece in simAttackingPieces)
                {
                    var pieceMoves = piece.AvailableMoves(ref simulation, boardSize);
                    simMoves.AddRange(pieceMoves);
                }

                if (ContainsValidMove(ref simMoves, kingSimPosition))
                {
                    movesToRemove.Add(move);
                }

                cp.currentX = actualX;
                cp.currentY = actualY;
            }

            foreach (var moveToRemove in movesToRemove)
            {
                moves.Remove(moveToRemove);
            }
        }

        private Vector2Int FindKingPosition(int team)
        {
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    ChessPiece piece = chessPieces[x, y];
                    if (piece != null && piece.type == ChessPieceType.King && piece.team == team)
                    {
                        return new Vector2Int(x, y);
                    }
                }
            }

            return new Vector2Int(-1, -1); 
        }
        
        public void AddToHistory(Vector2Int startPos, Vector2Int endPos)
        {
            movesHistory.Add(startPos);
            movesHistory.Add(endPos);
            movesHistoryText.text = $"{letters[startPos.x]}{startPos.y + 1} -> {letters[endPos.x]}{endPos.y + 1}";
            Debug.Log($"{letters[startPos.x]}{startPos.y + 1} -> {letters[endPos.x]}{endPos.y + 1}");
        }
        public bool MakeMove(ChessPiece chessPiece, int targetX, int targetY)
        {
            
            Vector2Int previousPosition = new Vector2Int(chessPiece.currentX, chessPiece.currentY);
            Vector2Int nextPosition = new Vector2Int(targetX,targetY);
            if (winCondition)
            {
                Debug.Log("The game is already over. No more moves can be made.");
                return false;
            }
            
            if (!ContainsValidMove(ref availableMoves, nextPosition))
                return false;

            if (!IsValidPosition(targetX, targetY))
                return false;

           
            ChessPiece targetPiece = chessPieces[targetX, targetY];
            if (targetPiece != null)
            {
                if (chessPiece.team == targetPiece.team)
                    return false;
                KillThePiece(targetPiece);
            }
            MovePiece(chessPiece, targetX, targetY);
            UpdatePiecePosition(chessPiece, targetX, targetY);
            AddToHistory(previousPosition, nextPosition);
            UpdateTurn();
            if (autoChangeCam)
                ChangeTheView();
            return true;
        }

        private bool ContainsValidMove(ref List<Vector2Int> moves, Vector2Int position)
        {
            return moves.Contains(position);
        }

        private bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < boardSize && y >= 0 && y < boardSize;
        }

        private void MovePiece(ChessPiece piece, int targetX, int targetY)
        {
            chessPieces[targetX, targetY] = piece;
            chessPieces[piece.currentX, piece.currentY] = null;
        }

        private void KillThePiece( ChessPiece enemypiece )
        {
            if (enemypiece.team == white)
            {   
                deadW.Add(enemypiece);
                
                if (enemypiece.type == ChessPieceType.Pawn)
                    enemypiece.transform.position = DeathWhitePosition + new Vector3(0f,0.35f,0f);
                else
                    enemypiece.transform.position = DeathWhitePosition;
                DeathWhitePosition.x = DeathWhitePosition.x + 2.5f;
            }
            else
            {
                deadB.Add(enemypiece);
                
                if (enemypiece.type == ChessPieceType.Pawn)
                    enemypiece.transform.position = DeathBlackPosition + new Vector3(0f,0.35f,0f);
                else
                    enemypiece.transform.position = DeathBlackPosition;
                DeathBlackPosition.x = DeathBlackPosition.x - 2.5f;
            }

            enemypiece.transform.localScale = Vector3.one * 35f;
        }

        private void RevertNonValidMove(ChessPiece piece, int targetX, int targetY)
        {
            chessPieces[piece.currentX, piece.currentY] = null;
            chessPieces[targetX, targetY] = piece;
            piece.currentX = targetX;
            piece.currentY = targetY;
            UpdatePiecePosition(piece, piece.currentX, piece.currentY);
        }

        private void UpdatePiecePosition(ChessPiece piece, int targetX, int targetY)
        {
            piece.currentX = targetX;
            piece.currentY = targetY;

            if (piece.type == ChessPieceType.Pawn)
                piece.transform.position = new Vector3(targetX * tileSize, 2, targetY * tileSize);
            else
                piece.transform.position = new Vector3(targetX * tileSize, 1, targetY * tileSize);
        }

        private void UpdateTurn()
        {
            isWhiteTurn = !isWhiteTurn;
            
            UpdateUI();
        }

        //Visual
        private void UpdateUI()
        {   
            
            int opponentTeam = isWhiteTurn ? white : black;
            bool isCheck = IsCheck(opponentTeam);
            bool isCheckmate = IsCheckmate();
            statusText.text = "";
            if (!isWhiteTurn)
            {
                updateTurnText.color = new Color(0, 0, 0);
                statusText.color = new Color(0, 0, 0);
                updateTurnText.text = "Black turn";
            }    
            else
            {
                updateTurnText.color = new Color(1, 1, 1);
                statusText.color = new Color(1, 1, 1);
                updateTurnText.text = "White turn";
            }

            if (isCheck)
            {   
                statusText.text = "Check!";
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
        
        private void HighlightTiles()
        {
            for (int i = 0; i < availableMoves.Count; i++)
            {
                int x = availableMoves[i].x;
                int y = availableMoves[i].y;

                if (IsValidPosition(x,y))
                {
                    if (boardArray[x, y].tag == "Tile")
                    {   
                        boardArray[x, y].tag = "Highlighted";
                        if( chessPieces[x,y] != null )
                        {   
                            boardArray[x, y].GetComponent<MeshRenderer>().material = highlightedCellUnderPiece; 
                        }
                        else
                        {
                            boardArray[x, y].GetComponent<MeshRenderer>().material = highlightedCell;
                        }
                            
                    }
                }
            }
        }

        private void HighlightKing(Vector2Int kingPosition)
        {
            if (boardArray[kingPosition.x, kingPosition.y].tag == "Tile")
            {
                boardArray[kingPosition.x, kingPosition.y].tag = "Highlighted";
                boardArray[kingPosition.x, kingPosition.y].GetComponent<MeshRenderer>().material = highlightedKingCell;
            }
        }

        private void RemoveHighlightTiles()
        {
            for (int i = 0; i < boardSize; i++)
            {
                for(int j = 0; j< boardSize; j++)
                {
                    if (IsValidPosition(i,j))
                    {
                        if (boardArray[i, j].tag == "Highlighted")
                        {
                            boardArray[i, j].tag = "Tile";
                            boardArray[i, j].GetComponent<MeshRenderer>().material = tileCellMat;
                        }
                    }
                }   
            }
        }


        // Camera management
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
    
    }

