using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pentomino.Algorithms;

public static class PentominoMatrix
{
    public record PlacementMetadata(int Piece, int Angle, double LeftAdjust, double TopAdjust, int ScaleX, int ScaleY);

    public static bool[/* col */, /* row */] MakeMatrixFor10x6()
    {
        return MakeMatrixFor10x6WithMetadata().Matrix;
    }


    public static (bool[/* col */, /* row */] Matrix, PlacementMetadata[] Metadata) MakeMatrixFor10x6WithMetadata()
    {
        // The matrix will have one column for each pentomino (12).
        // There will be one matrix for each board position (60).
        // The board is 10 wide and 6 tall.  The positions run
        // in reading order (left to right, top to bottom).

        // The notation we use for the pentomino pieces are FLIP STUVWXYZ

        bool[,] matrix = new bool[k_NumberOfMatrixColumns_10x6, k_NumberOfMatrixRows_10x6];

        PlacementMetadata[] allPlacementMetadata = new PlacementMetadata[k_NumberOfMatrixRows_10x6];

        int rowsAdded = 0;

        int rowsAddedForShape = AddFToMatrix(10, 6, matrix, allPlacementMetadata, rowsAdded);
        Debug.Assert(rowsAddedForShape == k_F_Rows_10x6);
        rowsAdded += rowsAddedForShape;

        rowsAddedForShape = AddLToMatrix(10, 6, matrix, allPlacementMetadata, rowsAdded);
        Debug.Assert(rowsAddedForShape == k_L_Rows_10x6);
        rowsAdded += rowsAddedForShape;

        rowsAddedForShape = AddIToMatrix(10, 6, matrix, allPlacementMetadata, rowsAdded);
        Debug.Assert(rowsAddedForShape == k_I_Rows_10x6);
        rowsAdded += rowsAddedForShape;

        rowsAddedForShape = AddPToMatrix(10, 6, matrix, allPlacementMetadata, rowsAdded);
        Debug.Assert(rowsAddedForShape == k_P_Rows_10x6);
        rowsAdded += rowsAddedForShape;

        rowsAddedForShape = AddSToMatrix(10, 6, matrix, allPlacementMetadata, rowsAdded);
        Debug.Assert(rowsAddedForShape == k_S_Rows_10x6);
        rowsAdded += rowsAddedForShape;

        rowsAddedForShape = AddTToMatrix(10, 6, matrix, allPlacementMetadata, rowsAdded);
        Debug.Assert(rowsAddedForShape == k_T_Rows_10x6);
        rowsAdded += rowsAddedForShape;

        rowsAddedForShape = AddUToMatrix(10, 6, matrix, allPlacementMetadata, rowsAdded);
        Debug.Assert(rowsAddedForShape == k_U_Rows_10x6);
        rowsAdded += rowsAddedForShape;

        rowsAddedForShape = AddVToMatrix(10, 6, matrix, allPlacementMetadata, rowsAdded);
        Debug.Assert(rowsAddedForShape == k_V_Rows_10x6);
        rowsAdded += rowsAddedForShape;

        rowsAddedForShape = AddWToMatrix(10, 6, matrix, allPlacementMetadata, rowsAdded);
        Debug.Assert(rowsAddedForShape == k_W_Rows_10x6);
        rowsAdded += rowsAddedForShape;

        rowsAddedForShape = AddXToMatrix(10, 6, matrix, allPlacementMetadata, rowsAdded);
        Debug.Assert(rowsAddedForShape == k_X_Rows_10x6);
        rowsAdded += rowsAddedForShape;

        rowsAddedForShape = AddYToMatrix(10, 6, matrix, allPlacementMetadata, rowsAdded);
        Debug.Assert(rowsAddedForShape == k_Y_Rows_10x6);
        rowsAdded += rowsAddedForShape;

        rowsAddedForShape = AddZToMatrix(10, 6, matrix, allPlacementMetadata, rowsAdded);
        Debug.Assert(rowsAddedForShape == k_Z_Rows_10x6);
        rowsAdded += rowsAddedForShape;

        return (matrix, allPlacementMetadata);
    }


    private const int k_F_Index = 0;
    private const int k_L_Index = 1;
    private const int k_I_Index = 2;
    private const int k_P_Index = 3;
    private const int k_S_Index = 4;
    private const int k_T_Index = 5;
    private const int k_U_Index = 6;
    private const int k_V_Index = 7;
    private const int k_W_Index = 8;
    private const int k_X_Index = 9;
    private const int k_Y_Index = 10;
    private const int k_Z_Index = 11;
    private const int k_firstBoardPosition = 12;

    private const int k_F_Rows_10x6 = 256;
    private const int k_L_Rows_10x6 = 248;
    private const int k_I_Rows_10x6 = 56;
    private const int k_P_Rows_10x6 = 304;
    private const int k_S_Rows_10x6 = 248;
    private const int k_T_Rows_10x6 = 128;
    private const int k_U_Rows_10x6 = 152;
    private const int k_V_Rows_10x6 = 128;
    private const int k_W_Rows_10x6 = 128;
    private const int k_X_Rows_10x6 = 32;
    private const int k_Y_Rows_10x6 = 248;
    private const int k_Z_Rows_10x6 = 128;

    private const int k_NumberOfMatrixColumns_10x6 = 72;
    private const int k_NumberOfMatrixRows_10x6 = k_F_Rows_10x6 + k_L_Rows_10x6 + k_I_Rows_10x6 + k_P_Rows_10x6 + k_S_Rows_10x6 + k_T_Rows_10x6 +
                                            k_U_Rows_10x6 + k_V_Rows_10x6 + k_W_Rows_10x6 + k_X_Rows_10x6 + k_Y_Rows_10x6 + k_Z_Rows_10x6;


    private static int AddShapeToMatrix(
        int shapeWidth,
        int shapeHeight,
        int boardWidth,
        int boardHeight,
        int[] homePositions,
        bool[/* col */, /* row */] matrix,
        PlacementMetadata basePlacementMetadata,
        PlacementMetadata[] allPlacementMetadata,
        int startingRow)
    {
        int columnVariants = boardWidth - shapeWidth + 1;
        int rowVariants = boardHeight - shapeHeight + 1;

        for (int rowVariant = 0; rowVariant < rowVariants; ++rowVariant)
        {
            for (int columnVariant = 0; columnVariant < columnVariants; ++columnVariant)
            {
                allPlacementMetadata[startingRow + (rowVariant * columnVariants) + columnVariant] = new(
                    Piece: basePlacementMetadata.Piece,
                    Angle: basePlacementMetadata.Angle,
                    LeftAdjust: basePlacementMetadata.LeftAdjust + columnVariant,
                    TopAdjust: basePlacementMetadata.TopAdjust + rowVariant,
                    ScaleX: basePlacementMetadata.ScaleX,
                    ScaleY: basePlacementMetadata.ScaleY);

                matrix[basePlacementMetadata.Piece, startingRow + (rowVariant * columnVariants) + columnVariant] = true;  // Set the column for the shape itself.

                foreach (int homePosition in homePositions)
                {
                    int pos = homePosition + (rowVariant * boardWidth) + columnVariant;

                    matrix[pos, startingRow + (rowVariant * columnVariants) + columnVariant] = true;  // Set the column for the position covered by the shape's geometry.
                }
            }
        }

        return columnVariants * rowVariants;
    }


    private static int AddFToMatrix(int boardWidth, int boardHeight, bool[/* col */, /* row */] matrix, PlacementMetadata[] allPlacementMetadata, int startingRow)
    {
        int rowsAdded = 0;

        //     13, 14,
        // 22, 23,
        //     33

        int[] home = new[] { 13, 14, 22, 23, 33 };
        PlacementMetadata basePlacementMetadata = new(Piece: k_F_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: 1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(3, 3, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        // 12, 13,
        //     23, 24,
        //     33

        home = new[] { 12, 13, 23, 24, 33 };
        basePlacementMetadata = new(Piece: k_F_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: -1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(3, 3, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        //     13,
        // 22, 23, 24,
        //         34

        home = new[] { 13, 22, 23, 24, 34 };
        basePlacementMetadata = new(Piece: k_F_Index, Angle: 90, LeftAdjust: 0, TopAdjust: 0, ScaleX: 1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(3, 3, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        //     13,
        // 22, 23, 24,
        // 32

        home = new[] { 13, 22, 23, 24, 32 };
        basePlacementMetadata = new(Piece: k_F_Index, Angle: 90, LeftAdjust: 0, TopAdjust: 0, ScaleX: -1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(3, 3, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        //     13,
        //     23, 24,
        // 32, 33

        home = new[] { 13, 23, 24, 32, 33 };
        basePlacementMetadata = new(Piece: k_F_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: -1, ScaleY: -1);
        rowsAdded += AddShapeToMatrix(3, 3, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        //     13,
        // 22, 23,
        //     33, 34

        home = new[] { 13, 22, 23, 33, 34 };
        basePlacementMetadata = new(Piece: k_F_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: 1, ScaleY: -1);
        rowsAdded += AddShapeToMatrix(3, 3, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        // 12,
        // 22, 23, 24,
        //     33

        home = new[] { 12, 22, 23, 24, 33 };
        basePlacementMetadata = new(Piece: k_F_Index, Angle: -90, LeftAdjust: 0, TopAdjust: 0, ScaleX: 1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(3, 3, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        //         14,
        // 22, 23, 24,
        //     33

        home = new[] { 14, 22, 23, 24, 33 };
        basePlacementMetadata = new(Piece: k_F_Index, Angle: -90, LeftAdjust: 0, TopAdjust: 0, ScaleX: -1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(3, 3, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        return rowsAdded;
    }


    private static int AddLToMatrix(int boardWidth, int boardHeight, bool[/* col */, /* row */] matrix, PlacementMetadata[] allPlacementMetadata, int startingRow)
    {
        int rowsAdded = 0;

        // 12,
        // 22,
        // 32,
        // 42, 43

        int[] home = new[] { 12, 22, 32, 42, 43 };
        PlacementMetadata basePlacementMetadata = new(Piece: k_L_Index, Angle: 90, LeftAdjust: -1, TopAdjust: 1, ScaleX: 1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(2, 4, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        //     13,
        //     23,
        //     33,
        // 42, 43

        home = new[] { 13, 23, 33, 42, 43 };
        basePlacementMetadata = new(Piece: k_L_Index, Angle: 90, LeftAdjust: -1, TopAdjust: 1, ScaleX: -1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(2, 4, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        // 12, 13,
        // 22,
        // 32,
        // 42

        home = new[] { 12, 13, 22, 32, 42 };
        basePlacementMetadata = new(Piece: k_L_Index, Angle: -90, LeftAdjust: -1, TopAdjust: 1, ScaleX: -1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(2, 4, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        // 12, 13,
        //     23,
        //     33,
        //     43

        home = new[] { 12, 13, 23, 33, 43 };
        basePlacementMetadata = new(Piece: k_L_Index, Angle: -90, LeftAdjust: -1, TopAdjust: 1, ScaleX: 1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(2, 4, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        // 12, 13, 14, 15,
        // 22

        home = new[] { 12, 13, 14, 15, 22 };
        basePlacementMetadata = new(Piece: k_L_Index, Angle: 180, LeftAdjust: 0, TopAdjust: 0, ScaleX: 1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(4, 2, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        // 12,
        // 22, 23, 24, 25

        home = new[] { 12, 22, 23, 24, 25 };
        basePlacementMetadata = new(Piece: k_L_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: -1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(4, 2, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        //             15,
        // 22, 23, 24, 25

        home = new[] { 15, 22, 23, 24, 25 };
        basePlacementMetadata = new(Piece: k_L_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: 1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(4, 2, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        // 12, 13, 14, 15,
        //             25

        home = new[] { 12, 13, 14, 15, 25 };
        basePlacementMetadata = new(Piece: k_L_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: 1, ScaleY: -1);
        rowsAdded += AddShapeToMatrix(4, 2, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        return rowsAdded;
    }


    private static int AddIToMatrix(int boardWidth, int boardHeight, bool[/* col */, /* row */] matrix, PlacementMetadata[] allPlacementMetadata, int startingRow)
    {
        int rowsAdded = 0;

        // 12,
        // 22,
        // 32,
        // 42,
        // 52

        int[] home = new[] { 12, 22, 32, 42, 52 };
        PlacementMetadata basePlacementMetadata = new(Piece: k_I_Index, Angle: 90, LeftAdjust: -2, TopAdjust: 2, ScaleX: 1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(1, 5, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        // 12, 13, 14, 15, 16

        home = new[] { 12, 13, 14, 15, 16 };
        basePlacementMetadata = new(Piece: k_I_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: 1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(5, 1, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        return rowsAdded;
    }


    private static int AddPToMatrix(int boardWidth, int boardHeight, bool[/* col */, /* row */] matrix, PlacementMetadata[] allPlacementMetadata, int startingRow)
    {
        int rowsAdded = 0;

        // 12, 13,
        // 22, 23,
        // 32

        int[] home = new[] { 12, 13, 22, 23, 32 };
        PlacementMetadata basePlacementMetadata = new(Piece: k_P_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: 1, ScaleY: -1);
        rowsAdded += AddShapeToMatrix(2, 3, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        // 12, 13,
        // 22, 23,
        //     33

        home = new[] { 12, 13, 22, 23, 33 };
        basePlacementMetadata = new(Piece: k_P_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: -1, ScaleY: -1);
        rowsAdded += AddShapeToMatrix(2, 3, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        // 12, 13, 14,
        //     23, 24

        home = new[] { 12, 23, 14, 23, 24 };
        basePlacementMetadata = new(Piece: k_P_Index, Angle: 90, LeftAdjust: 0.5, TopAdjust: -0.5, ScaleX: -1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(3, 2, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        // 12, 13, 14,
        // 22, 23

        home = new[] { 12, 13, 14, 22, 23 };
        basePlacementMetadata = new(Piece: k_P_Index, Angle: 90, LeftAdjust: 0.5, TopAdjust: -0.5, ScaleX: 1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(3, 2, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        //     13,
        // 22, 23,
        // 32, 33

        home = new[] { 13, 22, 23, 32, 33 };
        basePlacementMetadata = new(Piece: k_P_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: -1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(2, 3, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        // 12,
        // 22, 23,
        // 32, 33

        home = new[] { 12, 22, 23, 32, 33 };
        basePlacementMetadata = new(Piece: k_P_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: 1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(2, 3, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        // 12, 13,
        // 22, 23, 24

        home = new[] { 12, 13, 22, 23, 24 };
        basePlacementMetadata = new(Piece: k_P_Index, Angle: 90, LeftAdjust: 0.5, TopAdjust: -0.5, ScaleX: 1, ScaleY: -1);
        rowsAdded += AddShapeToMatrix(3, 2, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        //     13, 14,
        // 22, 23, 24,

        home = new[] { 13, 14, 22, 23, 24 };
        basePlacementMetadata = new(Piece: k_P_Index, Angle: -90, LeftAdjust: 0.5, TopAdjust: -0.5, ScaleX: 1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(3, 2, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        return rowsAdded;
    }


    private static int AddSToMatrix(int boardWidth, int boardHeight, bool[/* col */, /* row */] matrix, PlacementMetadata[] allPlacementMetadata, int startingRow)
    {
        int rowsAdded = 0;

        // 12,
        // 22, 23,
        //     33,
        //     43

        int[] home = new[] { 12, 22, 23, 33, 43 };
        PlacementMetadata basePlacementMetadata = new(Piece: k_S_Index, Angle: -90, LeftAdjust: -1, TopAdjust: 1, ScaleX: 1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(2, 4, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        //     13,
        //     23,
        // 32, 33,
        // 42

        home = new[] { 13, 23, 32, 33, 42 };
        basePlacementMetadata = new(Piece: k_S_Index, Angle: 90, LeftAdjust: -1, TopAdjust: 1, ScaleX: -1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(2, 4, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        //     13,
        // 22, 23,
        // 32,
        // 42

        home = new[] { 13, 22, 23, 32, 42 };
        basePlacementMetadata = new(Piece: k_S_Index, Angle: -90, LeftAdjust: -1, TopAdjust: 1, ScaleX: -1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(2, 4, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        // 12,
        // 22,
        // 32, 33,
        //     43

        home = new[] { 12, 22, 32, 33, 43 };
        basePlacementMetadata = new(Piece: k_S_Index, Angle: 90, LeftAdjust: -1, TopAdjust: 1, ScaleX: 1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(2, 4, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        //         14, 15,
        // 22, 23, 24

        home = new[] { 14, 15, 22, 23, 24 };
        basePlacementMetadata = new(Piece: k_S_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: 1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(4, 2, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        // 12, 13,
        //     23, 24, 25

        home = new[] { 12, 13, 23, 24, 25 };
        basePlacementMetadata = new(Piece: k_S_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: -1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(4, 2, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        // 12, 13, 14,
        //         24, 25

        home = new[] { 12, 13, 14, 24, 25 };
        basePlacementMetadata = new(Piece: k_S_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: 1, ScaleY: -1);
        rowsAdded += AddShapeToMatrix(4, 2, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        //     13, 14, 15,
        // 22, 23

        home = new[] { 13, 14, 15, 22, 23 };
        basePlacementMetadata = new(Piece: k_S_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: -1, ScaleY: -1);
        rowsAdded += AddShapeToMatrix(4, 2, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        return rowsAdded;
    }


    private static int AddTToMatrix(int boardWidth, int boardHeight, bool[/* col */, /* row */] matrix, PlacementMetadata[] allPlacementMetadata, int startingRow)
    {
        int rowsAdded = 0;

        // 12, 13, 14,
        //     23,
        //     33

        int[] home = new[] { 12, 13, 14, 23, 33 };
        PlacementMetadata basePlacementMetadata = new(Piece: k_T_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: 1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(3, 3, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        //         14,
        // 22, 23, 24,
        //         34

        home = new[] { 14, 22, 23, 24, 34 };
        basePlacementMetadata = new(Piece: k_T_Index, Angle: 90, LeftAdjust: 0, TopAdjust: 0, ScaleX: 1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(3, 3, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        // 12,
        // 22, 23, 24,
        // 32

        home = new[] { 12, 22, 23, 24, 32 };
        basePlacementMetadata = new(Piece: k_T_Index, Angle: -90, LeftAdjust: 0, TopAdjust: 0, ScaleX: 1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(3, 3, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        //     13,
        //     23,
        // 32, 33, 34

        home = new[] { 13, 23, 32, 33, 34 };
        basePlacementMetadata = new(Piece: k_T_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: 1, ScaleY: -1);
        rowsAdded += AddShapeToMatrix(3, 3, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        return rowsAdded;
    }


    private static int AddUToMatrix(int boardWidth, int boardHeight, bool[/* col */, /* row */] matrix, PlacementMetadata[] allPlacementMetadata, int startingRow)
    {
        int rowsAdded = 0;

        // 12,     14,
        // 22, 23, 24

        int[] home = new[] { 12, 14, 22, 23, 24 };
        PlacementMetadata basePlacementMetadata = new(Piece: k_U_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: 1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(3, 2, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        // 12, 13,
        // 22,
        // 32, 33

        home = new[] { 12, 13, 22, 32, 33 };
        basePlacementMetadata = new(Piece: k_U_Index, Angle: 90, LeftAdjust: -0.5, TopAdjust: 0.5, ScaleX: 1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(2, 3, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        // 12, 13, 14,
        // 22,     24

        home = new[] { 12, 13, 14, 22, 24 };
        basePlacementMetadata = new(Piece: k_U_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: 1, ScaleY: -1);
        rowsAdded += AddShapeToMatrix(3, 2, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        // 12, 13,
        //     23,
        // 32, 33

        home = new[] { 12, 13, 23, 32, 33 };
        basePlacementMetadata = new(Piece: k_U_Index, Angle: -90, LeftAdjust: -0.5, TopAdjust: 0.5, ScaleX: 1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(2, 3, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        return rowsAdded;
    }


    private static int AddVToMatrix(int boardWidth, int boardHeight, bool[/* col */, /* row */] matrix, PlacementMetadata[] allPlacementMetadata, int startingRow)
    {
        int rowsAdded = 0;

        // 12,
        // 22,
        // 32, 33, 34

        int[] home = new[] { 12, 22, 32, 33, 34 };
        PlacementMetadata basePlacementMetadata = new(Piece: k_V_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: 1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(3, 3, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        // 12, 13, 14
        // 22,
        // 32

        home = new[] { 12, 13, 14, 22, 32 };
        basePlacementMetadata = new(Piece: k_V_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: 1, ScaleY: -1);
        rowsAdded += AddShapeToMatrix(3, 3, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        // 12, 13, 14,
        //         24,
        //         34

        home = new[] { 12, 13, 14, 24, 34 };
        basePlacementMetadata = new(Piece: k_V_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: -1, ScaleY: -1);
        rowsAdded += AddShapeToMatrix(3, 3, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        //         14,
        //         24,
        // 32, 33, 34

        home = new[] { 14, 24, 32, 33, 34 };
        basePlacementMetadata = new(Piece: k_V_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: -1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(3, 3, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        return rowsAdded;
    }


    private static int AddWToMatrix(int boardWidth, int boardHeight, bool[/* col */, /* row */] matrix, PlacementMetadata[] allPlacementMetadata, int startingRow)
    {
        int rowsAdded = 0;

        // 12,
        // 22, 23,
        //     33, 34

        int[] home = new[] { 12, 22, 23, 33, 34 };
        PlacementMetadata basePlacementMetadata = new(Piece: k_W_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: 1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(3, 3, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        //     13, 14,
        // 22, 23,
        // 32

        home = new[] { 13, 14, 22, 23, 32 };
        basePlacementMetadata = new(Piece: k_W_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: 1, ScaleY: -1);
        rowsAdded += AddShapeToMatrix(3, 3, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        // 12, 13,
        //     23, 24,
        //         34

        home = new[] { 12, 13, 23, 24, 34 };
        basePlacementMetadata = new(Piece: k_W_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: -1, ScaleY: -1);
        rowsAdded += AddShapeToMatrix(3, 3, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        //         14,
        //     23, 24,
        // 32, 33

        home = new[] { 14, 23, 24, 32, 33 };
        basePlacementMetadata = new(Piece: k_W_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: -1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(3, 3, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        return rowsAdded;
    }


    private static int AddXToMatrix(int boardWidth, int boardHeight, bool[/* col */, /* row */] matrix, PlacementMetadata[] allPlacementMetadata, int startingRow)
    {
        int rowsAdded = 0;

        //     13,
        // 22, 23, 24
        //     33

        int[] home = new[] { 13, 22, 23, 24, 33 };
        PlacementMetadata basePlacementMetadata = new(Piece: k_X_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: 1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(3, 3, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        return rowsAdded;
    }


    private static int AddYToMatrix(int boardWidth, int boardHeight, bool[/* col */, /* row */] matrix, PlacementMetadata[] allPlacementMetadata, int startingRow)
    {
        int rowsAdded = 0;

        //     13,
        // 22, 23,
        //     33,
        //     43

        int[] home = new[] { 13, 22, 23, 33, 43 };
        PlacementMetadata basePlacementMetadata = new(Piece: k_Y_Index, Angle: -90, LeftAdjust: -1, TopAdjust: 1, ScaleX: 1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(2, 4, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        // 12,
        // 22, 23,
        // 32,
        // 42

        home = new[] { 12, 22, 23, 32, 42 };
        basePlacementMetadata = new(Piece: k_Y_Index, Angle: -90, LeftAdjust: -1, TopAdjust: 1, ScaleX: -1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(2, 4, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        //     13,
        //     23,
        // 32, 33,
        //     43

        home = new[] { 13, 23, 32, 33, 43 };
        basePlacementMetadata = new(Piece: k_Y_Index, Angle: -90, LeftAdjust: -1, TopAdjust: 1, ScaleX: 1, ScaleY: -1);
        rowsAdded += AddShapeToMatrix(2, 4, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        //     13,
        // 22, 23,
        //     33,
        //     43

        home = new[] { 13, 22, 23, 33, 43 };
        basePlacementMetadata = new(Piece: k_Y_Index, Angle: -90, LeftAdjust: -1, TopAdjust: 1, ScaleX: 1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(2, 4, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        //         14,
        // 22, 23, 24, 25

        home = new[] { 14, 22, 23, 24, 25 };
        basePlacementMetadata = new(Piece: k_Y_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: 1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(4, 2, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        //     13,
        // 22, 23, 24, 25

        home = new[] { 13, 22, 23, 24, 25 };
        basePlacementMetadata = new(Piece: k_Y_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: -1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(4, 2, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        // 12, 13, 14, 15,
        //         24

        home = new[] { 12, 13, 14, 15, 24 };
        basePlacementMetadata = new(Piece: k_Y_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: 1, ScaleY: -1);
        rowsAdded += AddShapeToMatrix(4, 2, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        // 12, 13, 14, 15,
        //     23

        home = new[] { 12, 13, 14, 15, 23 };
        basePlacementMetadata = new(Piece: k_Y_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: -1, ScaleY: -1);
        rowsAdded += AddShapeToMatrix(4, 2, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        return rowsAdded;
    }


    private static int AddZToMatrix(int boardWidth, int boardHeight, bool[/* col */, /* row */] matrix, PlacementMetadata[] allPlacementMetadata, int startingRow)
    {
        int rowsAdded = 0;

        // 12, 13,
        //     23,
        //     33, 34

        int[] home = new[] { 12, 13, 23, 33, 34 };
        PlacementMetadata basePlacementMetadata = new(Piece: k_Z_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: 1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(3, 3, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        //     13, 14,
        //     23,
        // 32, 33

        home = new[] { 13, 14, 23, 32, 33 };
        basePlacementMetadata = new(Piece: k_Z_Index, Angle: 0, LeftAdjust: 0, TopAdjust: 0, ScaleX: -1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(3, 3, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        //         14,
        // 22, 23, 24,
        // 32

        home = new[] { 14, 22, 23, 24, 32 };
        basePlacementMetadata = new(Piece: k_Z_Index, Angle: 90, LeftAdjust: 0, TopAdjust: 0, ScaleX: 1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(3, 3, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        // 12,
        // 22, 23, 24,
        //         34

        home = new[] { 12, 22, 23, 24, 34 };
        basePlacementMetadata = new(Piece: k_Z_Index, Angle: 90, LeftAdjust: 0, TopAdjust: 0, ScaleX: -1, ScaleY: 1);
        rowsAdded += AddShapeToMatrix(3, 3, boardWidth, boardHeight, home, matrix, basePlacementMetadata, allPlacementMetadata, startingRow + rowsAdded);

        return rowsAdded;
    }

    private static char GetShape(int row, bool[/* col */, /* row */] matrix)
    {
        if (matrix[k_F_Index, row])
            return 'F';

        if (matrix[k_L_Index, row])
            return 'L';

        if (matrix[k_I_Index, row])
            return 'I';

        if (matrix[k_P_Index, row])
            return 'P';

        if (matrix[k_S_Index, row])
            return 'S';

        if (matrix[k_T_Index, row])
            return 'T';

        if (matrix[k_U_Index, row])
            return 'U';

        if (matrix[k_V_Index, row])
            return 'V';

        if (matrix[k_W_Index, row])
            return 'W';

        if (matrix[k_X_Index, row])
            return 'X';

        if (matrix[k_Y_Index, row])
            return 'Y';

        if (matrix[k_Z_Index, row])
            return 'Z';

        return ' ';
    }


    public static char[,] ComposeBoard(int boardWidth, int boardHeight, HashSet<int> rows, bool[/* col */, /* row */] matrix)
    {
        char[,] board = new char[boardWidth, boardHeight];

        foreach (int row in rows)
        {
            char shape = GetShape(row, matrix);

            for (int pos = k_firstBoardPosition; pos < matrix.GetLength(0); ++pos)
            {
                if (matrix[pos, row])
                {
                    int x = (pos - k_firstBoardPosition) % boardWidth;
                    int y = (pos - k_firstBoardPosition) / boardWidth;

                    board[x, y] = shape;
                }
            }
        }

        return board;
    }

}
