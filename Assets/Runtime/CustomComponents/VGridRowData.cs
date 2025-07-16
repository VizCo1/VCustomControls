namespace VCustomComponents
{
    public readonly struct VGridRowData
    {
        public readonly int Row;
        public readonly int[,] Grid;

        public VGridRowData(int row, int[,] grid)
        {
            Row = row;
            Grid = grid;
        }

        public int GetWidth() => Grid.GetLength(1);
    }
}