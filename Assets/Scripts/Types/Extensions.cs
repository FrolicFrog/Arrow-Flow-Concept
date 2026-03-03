using System.Collections.Generic;

public static class ListExtensions
{
    public static List<List<T>> Transpose<T>(this List<List<T>> matrix)
    {
        if (matrix.Count == 0) return new List<List<T>>();
        
        int rows = matrix.Count;
        int cols = matrix[0].Count;
        var result = new List<List<T>>();
        
        for (int col = 0; col < cols; col++)
        {
            var newRow = new List<T>();
            for (int row = 0; row < rows; row++)
            {
                newRow.Add(matrix[row][col]);
            }
            result.Add(newRow);
        }
        
        return result;
    }
}