using System.Net.NetworkInformation;

namespace BlazorTetris.Tetris {
    public class Board {

        public Grid gameGrid = new Grid();
        public Tetromino currentPiece = null!;
        public bool GameOver; // defaults to false
        public int score = 0;
        public Board() {
            SpawnPiece();
        }

        public void SpawnPiece() {
            if (!gameGrid.cellGrid[0, 5].IsFilled)
                currentPiece = TetrominoFactory.CreateRandomPiece();
        }

        private bool TopRowOccupied()
        {
            for (int c = 0; c < 10; c++)
                if (gameGrid.cellGrid[0, c].IsFilled) return true;
            return false;
        }

        public Cell GetCell(int row, int col) {
            // 1. Check if the current piece exists
            if (currentPiece != null)
            {
                var shape = currentPiece.Shape;
                int pieceHeight = shape.GetLength(0);
                int pieceWidth = shape.GetLength(1);

                for (int y = 0; y < pieceHeight; y++)
                {
                    for (int x = 0; x < pieceWidth; x++)
                    {
                        // Only care about the active blocks of the shape
                        if (shape[y, x])
                        {
                            int boardRow = currentPiece.yPosition + y;
                            int boardCol = currentPiece.xPosition + x;

                            // If this shape cell maps to the board cell we're checking
                            if (boardRow == row && boardCol == col)
                            {
                                return new Cell
                                {
                                    IsFilled = true,
                                    Color = currentPiece.Color
                                };
                            }
                        }
                    }
                }
            }

            // 2. Return the existing cell on the board
            return gameGrid[row, col];
        }

        public bool CanPlacePiece(Tetromino piece, String direction) {
            int dx = 0, dy = 0;

            switch (direction)
            {
                case "left": dx = -1; break;
                case "right": dx = 1; break;
                case "down": dy = 1; break;
                case "up": dy = -1; break;
                case "rotation": break;
            }

            for (int row = 0; row < piece.Shape.GetLength(0); row++)
            {
                for (int col = 0; col < piece.Shape.GetLength(1); col++)
                {
                    if (!piece.Shape[row, col])
                        continue;

                    int boardRow = piece.yPosition + row + dy;
                    int boardCol = piece.xPosition + col + dx;
                    

                    if (boardRow < 0 || boardRow >= 20 || boardCol < 0 || boardCol >= 10)
                        return false;

                    if (gameGrid.cellGrid[boardRow, boardCol].IsFilled)
                        return false;
                }
            }
            return true;
        }

        public void PlacePiece() {

            while (CanPlacePiece(currentPiece, "down"))
            {
                currentPiece.yPosition++;
            }

            for (int row = 0; row < currentPiece.Shape.GetLength(0); row++)
            {
                for (int col = 0; col < currentPiece.Shape.GetLength(1); col++)
                {
                    int boardRow = currentPiece.yPosition + row;
                    int boardCol = currentPiece.xPosition + col;

                    if (currentPiece.Shape[row, col])
                    {
                        gameGrid.cellGrid[boardRow, boardCol].IsFilled = true;
                        gameGrid.cellGrid[boardRow, boardCol].Color = currentPiece.Color;
                    }
                }
            }
            RemoveRow();
            if (TopRowOccupied())
            {
                GameOver = true;
                return;                                      // stop here: no new spawn
            }
            SpawnPiece();
        }

        public void RemoveRow() {

            for (int row = 0; row < 20; row++) {

                int countFilled = 0;

                for (int col = 0; col <10; col++) {

                    if (gameGrid.cellGrid[row, col].IsFilled) {
                        countFilled++;
                    }
                }
                if (countFilled == 10) {

                    for (int col2 = 0; col2 < 10; col2++) {
                        gameGrid.cellGrid[row, col2] = new Cell { IsFilled = false, Color = "#d1cbc7" };
                    }

                    for (int r = row; r > 0; r--) {
                        for (int col2 = 0; col2 < 10; col2++) {
                            gameGrid.cellGrid[r, col2] = gameGrid.cellGrid[r - 1, col2]; // ✅ use r, not row
                        }
                    }
                    for (int col = 0; col < 10; col++) {
                        gameGrid.cellGrid[0, col] = new Cell { IsFilled = false, Color = "#d1cbc7" };
                    }
                }
            }
        }
    }
}
