using UnityEngine;

namespace XStorage.GUI
{
    public class GridSize
    {

        public enum ExpandPreference
        {
            ColumnsFirst,
            RowsFirst
        }

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

        public static GridSize CalculateSquare(int MaxColumns, int MaxRows, int panels, ExpandPreference pref)
        {
            var sqrt = Mathf.Sqrt(panels);
            var floor = Mathf.FloorToInt(sqrt);
            var ceil = Mathf.CeilToInt(sqrt);

            var maxColsCeiled = Mathf.Min(MaxColumns, ceil);
            var maxRowsCeiled = Mathf.Min(MaxRows, ceil);

            if (pref == ExpandPreference.RowsFirst)
            {
                var cols = Mathf.CeilToInt(panels / maxRowsCeiled);
                cols = Mathf.Min(MaxColumns, cols);
                return new GridSize(cols, maxRowsCeiled);
            }
            else
            {
                var rows = Mathf.CeilToInt(panels / maxColsCeiled);
                rows = Mathf.Min(MaxColumns, rows);
                return new GridSize(maxColsCeiled, rows);
            }

        }
    }
}
