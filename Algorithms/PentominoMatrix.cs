using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Pentomino.Algorithms;

public static class PentominoMatrix
{
    public record PlacementMetadata(int Piece, int Angle, double LeftAdjust, double TopAdjust, bool Flip);

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

        // The notation we use for the pentomino pieces is FLIP STUVWXYZ

        bool[,] matrix = new bool[k_NumberOfMatrixColumns_10x6, k_NumberOfMatrixRows_10x6];

        PlacementMetadata[] allPlacementMetadata = new PlacementMetadata[k_NumberOfMatrixRows_10x6];

        int rowsAdded = AddAllShapesToMatrix(10, 6, matrix, allPlacementMetadata);
        Debug.Assert(rowsAdded == k_NumberOfMatrixRows_10x6);

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
                    Flip: basePlacementMetadata.Flip);

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


    private static int[] GetHomePositions10x6(bool[][] shape)
    {
        int[] homePositions = new int[5];
        int nextPosition = 0;

        for (int y = 0; y < shape.Length; ++y)
        {
            for (int x = 0; x < shape[y].Length; ++x)
            {
                if (shape[y][x])
                {
                    homePositions[nextPosition++] = y * 10 + x + k_firstBoardPosition;
                }
            }
        }

        return homePositions;
    }


    private static (bool[][] FlippedShape, PlacementMetadata FlippedMetadata) FlipShape(bool[][] shape, PlacementMetadata metadata)
    {
        bool[][] flipShape = new bool[shape.Length][];

        for (int y = 0; y < shape.Length; ++y)
        {
            int width = shape[y].Length;

            flipShape[y] = new bool[width];

            for (int x = 0; x < width; ++x)
            {
                flipShape[y][x] = shape[y][width - x - 1];
            }
        }

        PlacementMetadata flippedMetadata = new(metadata.Piece, metadata.Angle, metadata.LeftAdjust, metadata.TopAdjust, !metadata.Flip);

        return (flipShape, flippedMetadata);
    }


    private static (bool[][] RotatedShape, PlacementMetadata rotatedMetadata) RotateShape(bool[][] shape, PlacementMetadata metadata)  // rotate clockwise 90 degrees
    {
        int originalHeight = shape.Length;
        int newWidth = originalHeight;

        int originalWidth = shape[0].Length;
        int newHeight = originalWidth;

        bool[][] rotatedShape = new bool[newHeight][];

        for (int y = 0; y < newHeight; ++y)
        {
            rotatedShape[y] = new bool[newWidth];

            for (int x = 0; x < newWidth; ++x)
            {
                rotatedShape[y][x] = shape[originalHeight - x - 1][y];
            }
        }

        (double oldCenterX, double oldCenterY) = (originalWidth / 2.0, originalHeight / 2.0);
        (double newCenterX, double newCenterY) = (newWidth / 2.0, newHeight / 2.0);

        PlacementMetadata rotatedMetadata = new(metadata.Piece, (metadata.Angle + 90) % 360, metadata.LeftAdjust + newCenterX - oldCenterX, metadata.TopAdjust + newCenterY - oldCenterY, metadata.Flip);

        return (rotatedShape, rotatedMetadata);
    }


    private struct ShapeOrientationTraits
    {
        public int Shape { get; set; }

        public bool Rotate90 { get; set; }

        public bool Rotate180 { get; set; }

        public bool Flip { get; set; }

        public bool[][] Geometry { get; set; }
    }

    private static readonly ShapeOrientationTraits[] ShapeOrientations = new ShapeOrientationTraits[]
    {
        new() { Shape = k_F_Index,  Rotate90 = true,  Rotate180 = true,  Flip = true,  Geometry = new bool[][] { new bool[] { false, true, true }, new bool[] { true, true, false }, new bool[] { false, true, false } } },
        new() { Shape = k_L_Index,  Rotate90 = true,  Rotate180 = true,  Flip = true,  Geometry = new bool[][] { new bool[] { true, false }, new bool[] { true, false }, new bool[] { true, false }, new bool[] { true, true } } },
        new() { Shape = k_I_Index,  Rotate90 = true,  Rotate180 = false, Flip = false, Geometry = new bool[][] { new bool[] { true }, new bool[] { true }, new bool[] { true }, new bool[] { true }, new bool[] { true } } },
        new() { Shape = k_P_Index,  Rotate90 = true,  Rotate180 = true,  Flip = true,  Geometry = new bool[][] { new bool[] { true, true }, new bool[] { true, true }, new bool[] { true, false } } },
        new() { Shape = k_S_Index,  Rotate90 = true,  Rotate180 = true,  Flip = true,  Geometry = new bool[][] { new bool[] { true, false }, new bool[] { true, true }, new bool[] { false, true }, new bool[] { false, true } } },
        new() { Shape = k_T_Index,  Rotate90 = true,  Rotate180 = true,  Flip = false, Geometry = new bool[][] { new bool[] { true, true, true }, new bool[] { false, true, false }, new bool[] { false, true, false } } },
        new() { Shape = k_U_Index,  Rotate90 = true,  Rotate180 = true,  Flip = false, Geometry = new bool[][] { new bool[] { true, false, true }, new bool[] { true, true, true } } },
        new() { Shape = k_V_Index,  Rotate90 = true,  Rotate180 = true,  Flip = false, Geometry = new bool[][] { new bool[] { true, false, false}, new bool[] { true, false, false }, new bool[] { true, true, true } } },
        new() { Shape = k_W_Index,  Rotate90 = true,  Rotate180 = true,  Flip = false, Geometry = new bool[][] { new bool[] { true, false, false}, new bool[] { true, true, false }, new bool[] { false, true, true } } },
        new() { Shape = k_X_Index,  Rotate90 = false, Rotate180 = false, Flip = false, Geometry = new bool[][] { new bool[] { false, true, false}, new bool[] { true, true, true }, new bool[] { false, true, false } } },
        new() { Shape = k_Y_Index,  Rotate90 = true,  Rotate180 = true,  Flip = true,  Geometry = new bool[][] { new bool[] { false, false, true, false }, new bool[] { true, true, true, true } } },
        new() { Shape = k_Z_Index,  Rotate90 = true,  Rotate180 = false, Flip = true,  Geometry = new bool[][] { new bool[] { true, true, false }, new bool[] { false, true, false }, new bool[] { false, true, true } } },
    };


    private static int AddAllShapesToMatrix(int boardWidth, int boardHeight, bool[/* col */, /* row */] matrix, PlacementMetadata[] allPlacementMetadata)
    {
        int rowsAdded = 0;
        int startingRow = 0;

        for (int shapeIndex = k_F_Index; shapeIndex <= k_Z_Index; ++shapeIndex)
        {
            bool[][] shapeGeometry = ShapeOrientations[shapeIndex].Geometry;
            PlacementMetadata placementMetadata = new(Piece: shapeIndex, Angle: 0, LeftAdjust: 0, TopAdjust: 0, Flip: false);

            for (int twoPass = 0; twoPass < 2; ++twoPass)
            {

                if (twoPass > 0)
                {
                    if (!ShapeOrientations[shapeIndex].Flip)
                        break;

                    (shapeGeometry, placementMetadata) = FlipShape(shapeGeometry, placementMetadata);
                }

                int[] home = GetHomePositions10x6(shapeGeometry);
                rowsAdded += AddShapeToMatrix(shapeGeometry[0].Length, shapeGeometry.Length, boardWidth, boardHeight, home, matrix, placementMetadata, allPlacementMetadata, startingRow + rowsAdded);

                if (ShapeOrientations[shapeIndex].Rotate90)
                {
                    (bool[][] rotatedGeometry, PlacementMetadata rotatedMetadata) = RotateShape(shapeGeometry, placementMetadata);
                    home = GetHomePositions10x6(rotatedGeometry);
                    // TODO:  rotate basePlacementMetadata
                    rowsAdded += AddShapeToMatrix(rotatedGeometry[0].Length, rotatedGeometry.Length, boardWidth, boardHeight, home, matrix, rotatedMetadata, allPlacementMetadata, startingRow + rowsAdded);

                    if (ShapeOrientations[shapeIndex].Rotate180)
                    {
                        // 180 degrees

                        (rotatedGeometry, rotatedMetadata) = RotateShape(rotatedGeometry, rotatedMetadata);
                        home = GetHomePositions10x6(rotatedGeometry);
                        // TODO:  rotate basePlacementMetadata
                        rowsAdded += AddShapeToMatrix(rotatedGeometry[0].Length, rotatedGeometry.Length, boardWidth, boardHeight, home, matrix, rotatedMetadata, allPlacementMetadata, startingRow + rowsAdded);

                        // 270 degrees

                        (rotatedGeometry, rotatedMetadata) = RotateShape(rotatedGeometry, rotatedMetadata);
                        home = GetHomePositions10x6(rotatedGeometry);
                        // TODO:  rotate basePlacementMetadata
                        rowsAdded += AddShapeToMatrix(rotatedGeometry[0].Length, rotatedGeometry.Length, boardWidth, boardHeight, home, matrix, rotatedMetadata, allPlacementMetadata, startingRow + rowsAdded);
                    }
                }
            }
        }

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
