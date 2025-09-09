namespace BlazorTetris.Tetris
{
    /// <summary>
    /// The 20x10 playfield of settled (locked) cells.
    /// Holds a 2D array of Cell and exposes a read-only indexer to read cells.
    /// </summary>
    public class Grid
    {
        // Fixed board size for standard Tetris.
        private readonly int rows = 20;
        private readonly int columns = 10;

        // The actual 2D storage for cells. (row 0 = top, row 19 = bottom)
        public Cell[,] cellGrid;

        /// <summary>
        /// Allocate the grid and fill every slot with an "empty" Cell.
        /// </summary>
        public Grid()
        {
            cellGrid = new Cell[rows, columns];
            InitializeGrid();
        }

        /// <summary>
        /// Set each cell to an empty/default state.
        /// Empty = IsFilled=false with a neutral background color.
        /// </summary>
        private void InitializeGrid()
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    cellGrid[row, col] = new Cell
                    {
                        IsFilled = false,
                        Color = "#d1cbc7"
                    };
                }
            }
        }

        /// <summary>
        /// Read-only indexer to access a cell by (row, col).
        /// Assumes caller passes valid indices: 0 ≤ row < 20, 0 ≤ col < 10.
        /// </summary>
        public Cell this[int row, int col]
        {
            get { return cellGrid[row, col]; }
        }
    }
}