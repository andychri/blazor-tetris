namespace BlazorTetris.Tetris {
    /// <summary>
    /// A blueprint class describing the type, shape and starting position of a single tetromino
    /// </summary>
    public class Tetromino {
        public BlockType Type;
        public bool[,] Shape;
        public int xPosition ; public int yPosition;
        public String Color { get; private set; }

        public Tetromino (bool[,] shape, BlockType type) {
            Type = type;
            Color = AssignColor(type);
            Shape = shape;
            xPosition = 5;
            yPosition = 0;
        }

        private String AssignColor(BlockType type) {
            switch (type)
            {
                case BlockType.I: return "cyan";
                case BlockType.O: return "yellow";
                case BlockType.T: return "purple";
                case BlockType.S: return "green";
                case BlockType.Z: return "red";
                case BlockType.J: return "blue";
                case BlockType.L: return "orange";
                default: return "White";
            }

        }
    }
}
