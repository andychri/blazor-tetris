namespace BlazorTetris.Tetris {
    /// <summary>
    /// A factory class that creates tetromino pieces with their default shapes and types.
    /// </summary>
    public class TetrominoFactory {
        public static Tetromino CreateT() {
            return new Tetromino(new bool[,] { 
                { false, true, false }, 
                { true, true, true } 
            },BlockType.T);
        }

        public static Tetromino CreateI() {
            return new Tetromino(new bool[,]
            {
                { true },
                { true },
                { true },
                { true }
            }, BlockType.I);
        }

        public static Tetromino CreateO() {
            return new Tetromino(new bool[,]
            {
                { true, true },
                { true, true }
            }, BlockType.O);
        }

        public static Tetromino CreateL() {
            return new Tetromino(new bool[,]
            {
                { false, false, true },
                { true, true, true }
            }, BlockType.L);
        }

        public static Tetromino CreateJ() {
            return new Tetromino(new bool[,]
            {
                { true, false, false },
                { true, true, true }
            }, BlockType.J);
        }

        public static Tetromino CreateS() {
            return new Tetromino(new bool[,]
            {
                { false, true, true },
                { true, true, false }
            }, BlockType.S);
        }

        public static Tetromino CreateZ() {
            return new Tetromino(new bool[,]
            {
                { true, true, false },
                { false, true, true }
            }, BlockType.Z);
        }

        /// <summary>
        /// Creates a tetromino of a random type.
        /// </summary>
        /// <returns>A Tetromino piece of type I, O, T, S, Z, J, or L.</returns>
        public static Tetromino CreateRandomPiece() {
            var random = new Random();
            switch (random.Next(0,7))
            {
                case 0: return CreateT();
                case 1: return CreateI();
                case 2: return CreateO();
                case 3: return CreateL();
                case 4: return CreateJ();
                case 5: return CreateS();
                case 6: return CreateZ();
                default: return CreateT();
            }

        }
    }


}
