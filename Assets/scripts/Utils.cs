using UnityEngine;
using System.Collections;

public class Utils : MonoBehaviour {
    public static int width;
    public static int height;
    public static int GetID(int row, int col)
    {
        return row * width + col;
    }
    public static void GetRowCol(int index, out int row, out int col)
    {
        row = index / width;
        col = index % width;
    }
    public static int GetValidGem()
    {
        return Random.Range(1, 6);
    }
}
