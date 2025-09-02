namespace BlazorTetris.Tetris {
    public class Movement {

        private Board board;
        private Tetromino piece => board.currentPiece;

        public Movement(Board board) {
            this.board = board;
        }

        public bool[,] RotatePiece(String direction) {
            var original = piece.Shape;
            int originalRows = original.GetLength(0);
            int originalCols = original.GetLength(1);

            // Rotated dimensions: cols x rows (flipped)
            bool[,] rotated = new bool[originalCols, originalRows];

            for (int row = 0; row < originalRows; row++) {
                for (int col = 0; col < originalCols; col++) {
                    if (direction == "clockwise") {
                        rotated[col, originalRows - 1 - row] = original[row, col];
                    }
                    else {
                        rotated[originalCols - 1 - col, row] = original[row, col];
                    }
                }
            }

            return rotated;
        }
        public void Rotate(String direction) {
            bool[,] rotated = RotatePiece(direction);
            Tetromino hello = new Tetromino(rotated, piece.Type)
            {
                xPosition = piece.xPosition,
                yPosition = piece.yPosition
            };
            if (board.CanPlacePiece(hello, "rotation"))
                piece.Shape = rotated;
        }


        public void MoveLeft() {
            if (board.CanPlacePiece(piece, "left"))
                piece.xPosition--;
        }

        public void MoveRight() {
            if (board.CanPlacePiece(piece, "right"))
                piece.xPosition++;
        }

        public void MoveDown() {
            if (board.CanPlacePiece(piece, "down"))
                piece.yPosition++;
        }

        public void MoveUp() {
            if (board.CanPlacePiece(piece, "up"))
                piece.yPosition--;
        }
    }
}
