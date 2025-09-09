namespace BlazorTetris.Tetris
{
    /// <summary>
    /// The Board holds all game state and rules:
    /// - Settled grid (20x10)
    /// - Current falling piece
    /// - GameOver flag
    /// - Score
    /// It also implements the core steps: spawn, movement checks, lock, clear, score.
    /// </summary>
    public class Board
    {
        // The 20x10 playfield of settled (locked) cells.
        public Grid gameGrid = new Grid();

        // The currently falling tetromino (set by SpawnPiece()).
        public Tetromino currentPiece = null!;

        // Set to true when blocks reach the top (or you decide game should end).
        public bool GameOver; // defaults to false

        // Player score. Updated when rows are cleared.
        public int score = 0;

        /// <summary>
        /// New board: spawn the first piece immediately.
        /// </summary>
        public Board()
        {
            SpawnPiece();
        }

        /// <summary>
        /// Spawn a new random piece if the spawn spot is free.
        /// (Simple check at [row 0, col 5].)
        /// </summary>
        public void SpawnPiece()
        {
            if (!gameGrid.cellGrid[0, 5].IsFilled)
                currentPiece = TetrominoFactory.CreateRandomPiece();
            // else: spawn is blocked; you could set GameOver = true here if you want.
        }

        /// <summary>
        /// Returns true if any cell in the top row is filled.
        /// Used after locking to decide if the game has ended.
        /// </summary>
        private bool TopRowOccupied()
        {
            for (int c = 0; c < 10; c++)
                if (gameGrid.cellGrid[0, c].IsFilled) return true;
            return false;
        }

        /// <summary>
        /// Returns what should be drawn at (row, col):
        /// - If the current falling piece covers that cell, return the piece cell.
        /// - Otherwise return the settled grid cell.
        /// </summary>
        public Cell GetCell(int row, int col)
        {
            // 1) Overlay the active piece on top of the settled grid.
            if (currentPiece != null)
            {
                var shape = currentPiece.Shape;
                int pieceHeight = shape.GetLength(0);
                int pieceWidth = shape.GetLength(1);

                for (int y = 0; y < pieceHeight; y++)
                {
                    for (int x = 0; x < pieceWidth; x++)
                    {
                        // Only consider squares that are "on" in the shape.
                        if (shape[y, x])
                        {
                            int boardRow = currentPiece.yPosition + y;
                            int boardCol = currentPiece.xPosition + x;

                            // If this shape cell maps exactly to (row, col), draw the piece here.
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

            // 2) If the active piece doesn't cover this cell, show the settled cell.
            return gameGrid[row, col];
        }

        /// <summary>
        /// Checks if a piece could move one step in the given direction (or rotate in place).
        /// Directions: "left", "right", "down", "up", "rotation" (rotation = no XY shift).
        /// Returns false if any shape cell would go out of bounds or collide with a filled cell.
        /// </summary>
        public bool CanPlacePiece(Tetromino piece, String direction)
        {
            int dx = 0, dy = 0;

            // Convert direction to a delta movement (dx, dy).
            switch (direction)
            {
                case "left": dx = -1; break;
                case "right": dx = 1; break;
                case "down": dy = 1; break;
                case "up": dy = -1; break;
                case "rotation": break; // rotation is checked in-place
            }

            // Check every "on" cell in the piece shape.
            for (int row = 0; row < piece.Shape.GetLength(0); row++)
            {
                for (int col = 0; col < piece.Shape.GetLength(1); col++)
                {
                    if (!piece.Shape[row, col])
                        continue;

                    int boardRow = piece.yPosition + row + dy;
                    int boardCol = piece.xPosition + col + dx;

                    // Bounds check. Board is 20 rows x 10 columns.
                    if (boardRow < 0 || boardRow >= 20 || boardCol < 0 || boardCol >= 10)
                        return false;

                    // Collision with a settled block?
                    if (gameGrid.cellGrid[boardRow, boardCol].IsFilled)
                        return false;
                }
            }
            return true; // All shape cells fit safely.
        }

        /// <summary>
        /// Lock the current piece:
        /// 1) Drop straight down as far as possible.
        /// 2) Merge the piece into the settled grid.
        /// 3) Clear any full rows (which also updates score).
        /// 4) If the top row is now occupied → GameOver; otherwise spawn the next piece.
        /// </summary>
        public void PlacePiece()
        {
            // 1) Hard drop: move down until we can't.
            while (CanPlacePiece(currentPiece, "down"))
            {
                currentPiece.yPosition++;
            }

            // 2) Merge the piece blocks into the settled grid.
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

            // 3) Clear full rows and award points for the number cleared.
            RemoveRow();

            // 4) If blocks reached the top, end the game; otherwise spawn next.
            if (TopRowOccupied())
            {
                GameOver = true;
                return; // stop here: no new spawn
            }
            SpawnPiece();
        }

        /// <summary>
        /// Add points for the number of lines cleared in a single lock.
        /// 1→100, 2→300, 3→500, 4→800, 5+ scaled.
        /// </summary>
        private void ScoreLines(int lines)
        {
            if (lines <= 0) return;

            int gained;
            switch (lines)
            {
                case 1: gained = 100; break;
                case 2: gained = 300; break;
                case 3: gained = 500; break;
                case 4: gained = 800; break;
                default: gained = 1000 + (lines - 4) * 400; break; // safety
            }
            score += gained;
        }

        /// <summary>
        /// Scan for full rows:
        /// - For each full row: clear it, shift everything above down by 1, clear the new top row,
        ///   increment lines-cleared, and re-check the same row index (since new content dropped in).
        /// - After the pass, award score once based on how many lines were cleared together.
        /// </summary>
        public void RemoveRow()
        {
            int linesCleared = 0;

            for (int row = 0; row < 20; row++)
            {
                int countFilled = 0;

                // Count filled cells in this row.
                for (int col = 0; col < 10; col++)
                {
                    if (gameGrid.cellGrid[row, col].IsFilled)
                        countFilled++;
                }

                if (countFilled == 10)
                {
                    // a) Clear this row.
                    for (int col2 = 0; col2 < 10; col2++)
                        gameGrid.cellGrid[row, col2] = new Cell { IsFilled = false, Color = "#d1cbc7" };

                    // b) Shift everything above down by one row.
                    for (int r = row; r > 0; r--)
                        for (int col2 = 0; col2 < 10; col2++)
                            gameGrid.cellGrid[r, col2] = gameGrid.cellGrid[r - 1, col2];

                    // c) Clear the new top row after the shift.
                    for (int col = 0; col < 10; col++)
                        gameGrid.cellGrid[0, col] = new Cell { IsFilled = false, Color = "#d1cbc7" };

                    linesCleared++;

                    // d) Re-check this same row index next loop,
                    //    because a new row just fell into this position.
                    row--;
                }
            }

            // Award points once per piece lock based on how many lines were cleared.
            ScoreLines(linesCleared);
        }
    }
}