namespace BlazorTetris.Tetris
{
    /// <summary>
    /// Movement is a tiny helper around the Board's current piece.
    /// It turns player intents (left/right/down/up/rotate) into changes,
    /// but only after asking the Board if that move is legal.
    /// </summary>
    public class Movement
    {
        // The game board we operate on.
        private Board board;

        // Convenience: the currently falling piece on the board.
        private Tetromino piece => board.currentPiece;

        /// <summary>
        /// Keep a reference to the Board so we can read the current piece
        /// and call Board.CanPlacePiece(...) to validate moves.
        /// </summary>
        public Movement(Board board)
        {
            this.board = board;
        }

        /// <summary>
        /// Create a NEW rotated copy of the current piece's shape (does not modify the piece).
        /// direction:
        ///   - "clockwise" → rotate 90° clockwise
        ///   - anything else → rotate 90° counter-clockwise
        ///
        /// Rotation mapping for a bool[,] matrix:
        ///   CW:  (r, c) → (c, R - 1 - r)
        ///   CCW: (r, c) → (C - 1 - c, r)
        /// where R = rows, C = cols of the original shape.
        /// </summary>
        public bool[,] RotatePiece(String direction)
        {
            var original = piece.Shape;
            int originalRows = original.GetLength(0);
            int originalCols = original.GetLength(1);

            // Rotated dimensions are flipped: cols x rows
            bool[,] rotated = new bool[originalCols, originalRows];

            for (int row = 0; row < originalRows; row++)
            {
                for (int col = 0; col < originalCols; col++)
                {
                    if (direction == "clockwise")
                    {
                        // (row, col) → (col, rows - 1 - row)
                        rotated[col, originalRows - 1 - row] = original[row, col];
                    }
                    else
                    {
                        // (row, col) → (cols - 1 - col, row)
                        rotated[originalCols - 1 - col, row] = original[row, col];
                    }
                }
            }

            return rotated;
        }

        /// <summary>
        /// Try to rotate the piece in place.
        /// We build a temporary Tetromino with the rotated shape at the same position,
        /// ask the Board if that placement is legal, and only then apply the rotation.
        /// (No wall-kicks here; simple accept/deny.)
        /// </summary>
        public void Rotate(String direction)
        {
            bool[,] rotated = RotatePiece(direction);

            // Temporary piece at the same X/Y, used only for the collision/bounds check.
            Tetromino hello = new Tetromino(rotated, piece.Type)
            {
                xPosition = piece.xPosition,
                yPosition = piece.yPosition
            };

            // "rotation" direction tells CanPlacePiece to check in place (dx=dy=0).
            if (board.CanPlacePiece(hello, "rotation"))
                piece.Shape = rotated;
        }

        /// <summary>
        /// Move one cell left if allowed.
        /// </summary>
        public void MoveLeft()
        {
            if (board.CanPlacePiece(piece, "left"))
                piece.xPosition--;
        }

        /// <summary>
        /// Move one cell right if allowed.
        /// </summary>
        public void MoveRight()
        {
            if (board.CanPlacePiece(piece, "right"))
                piece.xPosition++;
        }

        /// <summary>
        /// Move one cell down if allowed (soft drop step).
        /// </summary>
        public void MoveDown()
        {
            if (board.CanPlacePiece(piece, "down"))
                piece.yPosition++;
        }

        /// <summary>
        /// Move one cell up if allowed.
        /// </summary>
        public void MoveUp()
        {
            if (board.CanPlacePiece(piece, "up"))
                piece.yPosition--;
        }
    }
}
