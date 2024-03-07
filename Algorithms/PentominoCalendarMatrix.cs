using System;
using System.Collections.Generic;

namespace Pentomino.Algorithms;
using static Pentomino.Algorithms.PentominoMatrix;


public static class PentominoCalendarMatrix
{
    public static List<bool[]> MakeMatrixForPentominoCalendar(IEnumerable<PentominoShapeCollections.ShapeData> shapes, int month, int date)
    {
        (List<bool[]> matrixRows, List<PlacementMetadata> _) = MakeMatrixForPentominoCalendarWithMetadata(shapes, month, date);

        return matrixRows;
    }


    public static (List<bool[]> MatrixRows, List<PlacementMetadata> MetadataRows) MakeMatrixForPentominoCalendarWithMetadata(IEnumerable<PentominoShapeCollections.ShapeData> shapes, int month, int date)
    {
        // The matrix will have one column for each pentomino (8).
        // There will be one matrix for each board position (43).
        // The board is 7 rows tall.  The first two rows are 6 wide, then four rows are seven wide, then the last row is 3 wide.
        //
        // We will simply use a 7x7 matrix and ignore the spaces that aren't used.  The spaces unused are:
        // (6, 0), (6, 1), (3, 6), (4, 6), (5, 6), (6, 6) = { 6, 13, 45, 46, 47, 48 }

        // The notation we use for the pentomino pieces is LP SUVXZ O

        (List<bool[]> matrixRows, List<PlacementMetadata> metadataRows) = AddAllShapesToCalendarMatrix(7, 7, k_NumberOfMatrixColumns_Calendar, shapes);

        bool[] boundsMatrixRow = new bool[k_NumberOfMatrixColumns_Calendar];

        boundsMatrixRow[k_Bounds_Cal_Index] = true;
        boundsMatrixRow[k_FirstCalBoardPosition + 6] = true;
        boundsMatrixRow[k_FirstCalBoardPosition + 13] = true;
        boundsMatrixRow[k_FirstCalBoardPosition + 45] = true;
        boundsMatrixRow[k_FirstCalBoardPosition + 46] = true;
        boundsMatrixRow[k_FirstCalBoardPosition + 47] = true;
        boundsMatrixRow[k_FirstCalBoardPosition + 48] = true;

        month = Math.Max(0, Math.Min(11, month));
        int monthColumn = k_FirstCalBoardPosition + month + (month > 5 ? 1 : 0);
        boundsMatrixRow[monthColumn] = true;

        date = Math.Max(0, Math.Min(30, date));
        int dateColumn = k_FirstCalBoardPosition + 14 + date;
        boundsMatrixRow[dateColumn] = true;

        matrixRows.Add(boundsMatrixRow);

        metadataRows.Add(new(PieceId: (int) PentominoShapeCollections.CommonPieceIds.None, PieceIndex: -1, Angle: 0, LeftAdjust: 0, TopAdjust: 0, Flip: false));

        return (matrixRows, metadataRows);
    }


    private const int k_FirstPieceColumn = 0;
    private const int k_LastPieceColumn = 7;
    private const int k_Bounds_Cal_Index = 8;

    private const int k_FirstCalBoardPosition = 9;
    private const int k_LastCalBoardColumn = 57;

    private const int k_NumberOfMatrixColumns_Calendar = 58;

    private const int k_CalBoardWidth = 7;
    private const int k_CalBoardHeight = 7; 



    private static readonly Dictionary<int, ShapeOrientationTraits> ShapeOrientations = new()
    {
        [(int) PentominoShapeCollections.CommonPieceIds.L]         = new() { Rotate90 = true,  Rotate180 = true,  Flip = true,  Geometry = new bool[][] { new bool[] { true, false }, new bool[] { true, false }, new bool[] { true, false }, new bool[] { true, true } } },
        [(int) PentominoShapeCollections.CommonPieceIds.P]         = new() { Rotate90 = true,  Rotate180 = true,  Flip = true,  Geometry = new bool[][] { new bool[] { true, true }, new bool[] { true, true }, new bool[] { true, false } } },
        [(int) PentominoShapeCollections.CommonPieceIds.S]         = new() { Rotate90 = true,  Rotate180 = true,  Flip = true,  Geometry = new bool[][] { new bool[] { true, false }, new bool[] { true, true }, new bool[] { false, true }, new bool[] { false, true } } },
        [(int) PentominoShapeCollections.CommonPieceIds.U]         = new() { Rotate90 = true,  Rotate180 = true,  Flip = false, Geometry = new bool[][] { new bool[] { true, false, true }, new bool[] { true, true, true } } },
        [(int) PentominoShapeCollections.CommonPieceIds.V]         = new() { Rotate90 = true,  Rotate180 = true,  Flip = false, Geometry = new bool[][] { new bool[] { true, false, false}, new bool[] { true, false, false }, new bool[] { true, true, true } } },
        [(int) PentominoShapeCollections.CommonPieceIds.Y]         = new() { Rotate90 = true,  Rotate180 = true,  Flip = true,  Geometry = new bool[][] { new bool[] { false, false, true, false }, new bool[] { true, true, true, true } } },
        [(int) PentominoShapeCollections.CommonPieceIds.Z]         = new() { Rotate90 = true,  Rotate180 = false, Flip = true,  Geometry = new bool[][] { new bool[] { true, true, false }, new bool[] { false, true, false }, new bool[] { false, true, true } } },
        [(int) PentominoShapeCollections.CommonPieceIds.Block_2x3] = new() { Rotate90 = true,  Rotate180 = false, Flip = false, Geometry = new bool[][] { new bool[] { true, true }, new bool[] { true, true }, new bool[] { true, true } } },
    };


    private static (List<bool[]> MatrixRows, List<PlacementMetadata> MetadataRows) AddAllShapesToCalendarMatrix(int boardWidth, int boardHeight, int numberOfMatrixColumns, IEnumerable<PentominoShapeCollections.ShapeData> shapes)
    {
        List<bool[]> matrixRows = new();
        List<PlacementMetadata> metadataRows = new();

        foreach (PentominoShapeCollections.ShapeData shapeData in shapes)
        {
            if (shapeData.PieceId == (int) PentominoShapeCollections.CommonPieceIds.None || shapeData.PieceColumnIndex < 0)
                continue;

            ShapeOrientationTraits orientationTraits = ShapeOrientations[shapeData.PieceId];
            bool[][] shapeGeometry = orientationTraits.Geometry;
            PlacementMetadata placementMetadata = new(PieceId: shapeData.PieceId, PieceIndex: shapeData.PieceColumnIndex, Angle: 0, LeftAdjust: 0, TopAdjust: 0, Flip: false);

            for (int twoPass = 0; twoPass < 2; ++twoPass)
            {
                if (twoPass > 0)
                {
                    if (!orientationTraits.Flip)
                        break;

                    (shapeGeometry, placementMetadata) = FlipShape(shapeGeometry, placementMetadata);
                }

                int pieceSize = (shapeData.PieceId == (int) PentominoShapeCollections.CommonPieceIds.Block_2x3 ? 6 : 5);
                int[] home = GetHomePositions(shapeGeometry, pieceSize, k_CalBoardWidth, k_FirstCalBoardPosition);
                AddShapeToMatrix(shapeGeometry[0].Length, shapeGeometry.Length, k_CalBoardWidth, k_CalBoardHeight, k_NumberOfMatrixColumns_Calendar, home, matrixRows, placementMetadata, metadataRows);

                if (orientationTraits.Rotate90)
                {
                    // 90 degrees

                    (bool[][] rotatedGeometry, PlacementMetadata rotatedMetadata) = RotateShape(shapeGeometry, placementMetadata);
                    home = GetHomePositions(rotatedGeometry, pieceSize, k_CalBoardWidth, k_FirstCalBoardPosition);
                    AddShapeToMatrix(rotatedGeometry[0].Length, rotatedGeometry.Length, k_CalBoardWidth, k_CalBoardHeight, k_NumberOfMatrixColumns_Calendar, home, matrixRows, rotatedMetadata, metadataRows);

                    if (orientationTraits.Rotate180)
                    {
                        // 180 degrees

                        (rotatedGeometry, rotatedMetadata) = RotateShape(rotatedGeometry, rotatedMetadata);
                        home = GetHomePositions(rotatedGeometry, pieceSize, k_CalBoardWidth, k_FirstCalBoardPosition);
                        AddShapeToMatrix(rotatedGeometry[0].Length, rotatedGeometry.Length, k_CalBoardWidth, k_CalBoardHeight, k_NumberOfMatrixColumns_Calendar, home, matrixRows, rotatedMetadata, metadataRows);

                        // 270 degrees

                        (rotatedGeometry, rotatedMetadata) = RotateShape(rotatedGeometry, rotatedMetadata);
                        home = GetHomePositions(rotatedGeometry, pieceSize, k_CalBoardWidth, k_FirstCalBoardPosition);
                        AddShapeToMatrix(rotatedGeometry[0].Length, rotatedGeometry.Length, k_CalBoardWidth, k_CalBoardHeight, k_NumberOfMatrixColumns_Calendar, home, matrixRows, rotatedMetadata, metadataRows);
                    }
                }
            }
        }

        return (matrixRows, metadataRows);
    }


}
