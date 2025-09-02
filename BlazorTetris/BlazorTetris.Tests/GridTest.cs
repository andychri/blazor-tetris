using BlazorTetris.Tetris;

namespace BlazorTetris.Tests {
    [TestClass]
    public sealed class GridTest {
        [TestMethod]
        public void GridHasCorrectDimensions() {
            var grid = new Grid();

            Assert.AreEqual(20, grid.cellGrid.GetLength(0));
            Assert.AreEqual(10, grid.cellGrid.GetLength(1));

        }
    }
}
