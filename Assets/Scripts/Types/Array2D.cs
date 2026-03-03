using System;

namespace ArrowFlow.Types
{   
    [Serializable]
    public class Array2D<T>
    {
        public Row<T>[] Rows;
        public int Width => _Width;
        public int Height => _Height;

        private int _Width;
        private int _Height;

        public Array2D(int width, int height)
        {
            _Width = width;
            _Height = height;
            Rows = new Row<T>[height];

            for (int i = 0; i < height; i++)
            {
                Rows[i] = new Row<T>(width);
            }
        }

        public T this[int x, int y]
        {
            get { return Rows[y].Values[x]; }
            set { Rows[y].Values[x] = value; }
        }
    }

    [Serializable]
    public class Row<T>
    {
        public T[] Values;

        public Row(int width)
        {
            Values = new T[width];
        }
    }
}