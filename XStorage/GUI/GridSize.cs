using System;
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

        public static GridSize Calculate(int maxColumns, int maxRows, int cells)
        {
            var sqrt = Mathf.Sqrt(cells);

            var rounded = Mathf.RoundToInt(sqrt);
            var ceiled = Mathf.CeilToInt(sqrt);

            int cols = Math.Min(maxColumns, ceiled);
            int rows = Math.Min(maxRows, rounded);

            return new GridSize(cols, rows);
        }

    }
}
