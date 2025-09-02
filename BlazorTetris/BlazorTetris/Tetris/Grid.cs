namespace BlazorTetris.Tetris {
    public class Grid {

        private readonly int rows = 20;
        private readonly int columns = 10;
        public Cell[,] cellGrid;
        public Grid() {
            cellGrid = new Cell[rows, columns];
            InitializeGrid();
        }
        private void InitializeGrid() {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    cellGrid[row, col] = new Cell{IsFilled=false, Color="#d1cbc7" };
                }
            }
        }

        public Cell this[int row, int col] {
            get { return cellGrid[row, col]; }
        }
    }
}
