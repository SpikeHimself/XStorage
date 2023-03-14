using UnityEngine;

namespace XStorage.GUI
{
    public class GridSize
    {
        public int Columns { get; }
        public int Rows { get; }

        public GridSize(int columns, int rows)
        {
            Columns = columns;
            Rows = rows;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is GridSize))
            {
                return false;
            }

            GridSize other = obj as GridSize;
            return Columns == other.Columns && Rows == other.Rows;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Columns}x{Rows}";
        }

        public static GridSize Calculate(int maxCols, int maxRows, int cells)
        {
            int cols = Mathf.Min(maxCols, cells);
            int totalRows = Mathf.CeilToInt(cells / (float)cols);
            int rows = Mathf.Min(maxRows, totalRows);
            return new GridSize(cols, rows);
        }

    }
}
