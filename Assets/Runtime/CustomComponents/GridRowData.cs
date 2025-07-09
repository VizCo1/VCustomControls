namespace VCustomComponents
{
    public readonly struct GridRowData
    {
        public readonly int Row;
        public readonly int[,] Grid;

        public GridRowData(int row, int[,] grid)
        {
            Row = row;
            Grid = grid;
        }

        public int GetWidth() => Grid.GetLength(1);
    }
}