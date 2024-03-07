using System.Collections.Generic;

namespace Pentomino.Algorithms;


public static class PentominoMatrix
{
    public record PlacementMetadata(int PieceId, int PieceIndex, int Angle, double LeftAdjust, double TopAdjust, bool Flip);

    public static List<bool[]> MakeMatrixFor10x6(IEnumerable<PentominoShapeCollections.ShapeData> shapes)
    {
        (List<bool[]> matrixRows, List<PlacementMetadata> _) = MakeMatrixFor10x6WithMetadata(shapes);

        return matrixRows;
    }


    public static (List<bool[]> MatrixRows, List<PlacementMetadata> MetadataRows) MakeMatrixFor10x6WithMetadata(IEnumerable<PentominoShapeCollections.ShapeData> shapes)
    {
        // The matrix will have one column for each pentomino (12).
        // There will be one matrix for each board position (60).
        // The board is 10 wide and 6 tall.  The positions run
        // in reading order (left to right, top to bottom).

        // The notation we use for the pentomino pieces is FLIP STUVWXYZ

        return AddAllShapesToMatrix(10, 6, k_NumberOfMatrixColumns_10x6, shapes);
     }


    private const int k_FirstPieceColumn = 0;
    private const int k_LastPieceColumn = 11;

    private const int k_FirstBoardColumn = 12;
    private const int k_LastBoardColumn = 71;

    private const int k_NumberOfMatrixColumns_10x6 = 72;

    private const int k_pieceSize = 5;


    internal static void AddShapeToMatrix(
        int shapeWidth,
        int shapeHeight,
        int boardWidth,
        int boardHeight,
        int numberOfMatrixColumns,
        int[] homePositions,
        List<bool[]> matrixRows,
        PlacementMetadata basePlacementMetadata,
        List<PlacementMetadata> metadataRows)
    {
        int columnVariants = boardWidth - shapeWidth + 1;
        int rowVariants = boardHeight - shapeHeight + 1;

        for (int rowVariant = 0; rowVariant < rowVariants; ++rowVariant)
        {
            for (int columnVariant = 0; columnVariant < columnVariants; ++columnVariant)
            {
                metadataRows.Add(new(
                    PieceId: basePlacementMetadata.PieceId,
                    PieceIndex: basePlacementMetadata.PieceIndex,
                    Angle: basePlacementMetadata.Angle,
                    LeftAdjust: basePlacementMetadata.LeftAdjust + columnVariant,
                    TopAdjust: basePlacementMetadata.TopAdjust + rowVariant,
                    Flip: basePlacementMetadata.Flip));
                
                bool[] matrixRow = new bool[numberOfMatrixColumns];
                matrixRow[basePlacementMetadata.PieceIndex] = true;  // Set the column for the shape itself.

                foreach (int homePosition in homePositions)
                {
                    int pos = homePosition + (rowVariant * boardWidth) + columnVariant;

                    matrixRow[pos] = true;  // Set the column for the position covered by the shape's geometry.
                }

                matrixRows.Add(matrixRow);
            }
        }
    }


    internal static int[] GetHomePositions(bool[][] shape, int pieceSize, int boardWidth, int firstBoardColumnIndex)
    {
        int[] homePositions = new int[pieceSize];
        int nextPosition = 0;

        for (int y = 0; y < shape.Length; ++y)
        {
            for (int x = 0; x < shape[y].Length; ++x)
            {
                if (shape[y][x])
                {
                    homePositions[nextPosition++] = y * boardWidth + x + firstBoardColumnIndex;
                }
            }
        }

        return homePositions;
    }


    internal static (bool[][] FlippedShape, PlacementMetadata FlippedMetadata) FlipShape(bool[][] shape, PlacementMetadata metadata)
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

        PlacementMetadata flippedMetadata = new(metadata.PieceId, metadata.PieceIndex, metadata.Angle, metadata.LeftAdjust, metadata.TopAdjust, !metadata.Flip);

        return (flipShape, flippedMetadata);
    }


    internal static (bool[][] RotatedShape, PlacementMetadata rotatedMetadata) RotateShape(bool[][] shape, PlacementMetadata metadata)  // rotate clockwise 90 degrees
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

        PlacementMetadata rotatedMetadata = new(metadata.PieceId, metadata.PieceIndex, (metadata.Angle + 90) % 360, metadata.LeftAdjust + newCenterX - oldCenterX, metadata.TopAdjust + newCenterY - oldCenterY, metadata.Flip);

        return (rotatedShape, rotatedMetadata);
    }


    internal struct ShapeOrientationTraits
    {
        public bool Rotate90 { get; set; }

        public bool Rotate180 { get; set; }

        public bool Flip { get; set; }

        public bool[][] Geometry { get; set; }
    }

    private static readonly Dictionary<int, ShapeOrientationTraits> ShapeOrientations = new()
    {
        [(int) PentominoShapeCollections.CommonPieceIds.F] = new() { Rotate90 = true,  Rotate180 = true,  Flip = true,  Geometry = new bool[][] { new bool[] { false, true, true }, new bool[] { true, true, false }, new bool[] { false, true, false } } },
        [(int) PentominoShapeCollections.CommonPieceIds.L] = new() { Rotate90 = true,  Rotate180 = true,  Flip = true,  Geometry = new bool[][] { new bool[] { true, false }, new bool[] { true, false }, new bool[] { true, false }, new bool[] { true, true } } },
        [(int) PentominoShapeCollections.CommonPieceIds.I] = new() { Rotate90 = true,  Rotate180 = false, Flip = false, Geometry = new bool[][] { new bool[] { true }, new bool[] { true }, new bool[] { true }, new bool[] { true }, new bool[] { true } } },
        [(int) PentominoShapeCollections.CommonPieceIds.P] = new() { Rotate90 = true,  Rotate180 = true,  Flip = true,  Geometry = new bool[][] { new bool[] { true, true }, new bool[] { true, true }, new bool[] { true, false } } },
        [(int) PentominoShapeCollections.CommonPieceIds.S] = new() { Rotate90 = true,  Rotate180 = true,  Flip = true,  Geometry = new bool[][] { new bool[] { true, false }, new bool[] { true, true }, new bool[] { false, true }, new bool[] { false, true } } },
        [(int) PentominoShapeCollections.CommonPieceIds.T] = new() { Rotate90 = true,  Rotate180 = true,  Flip = false, Geometry = new bool[][] { new bool[] { true, true, true }, new bool[] { false, true, false }, new bool[] { false, true, false } } },
        [(int) PentominoShapeCollections.CommonPieceIds.U] = new() { Rotate90 = true,  Rotate180 = true,  Flip = false, Geometry = new bool[][] { new bool[] { true, false, true }, new bool[] { true, true, true } } },
        [(int) PentominoShapeCollections.CommonPieceIds.V] = new() { Rotate90 = true,  Rotate180 = true,  Flip = false, Geometry = new bool[][] { new bool[] { true, false, false}, new bool[] { true, false, false }, new bool[] { true, true, true } } },
        [(int) PentominoShapeCollections.CommonPieceIds.W] = new() { Rotate90 = true,  Rotate180 = true,  Flip = false, Geometry = new bool[][] { new bool[] { true, false, false}, new bool[] { true, true, false }, new bool[] { false, true, true } } },
        [(int) PentominoShapeCollections.CommonPieceIds.X] = new() { Rotate90 = false, Rotate180 = false, Flip = false, Geometry = new bool[][] { new bool[] { false, true, false}, new bool[] { true, true, true }, new bool[] { false, true, false } } },
        [(int) PentominoShapeCollections.CommonPieceIds.Y] = new() { Rotate90 = true,  Rotate180 = true,  Flip = true,  Geometry = new bool[][] { new bool[] { false, false, true, false }, new bool[] { true, true, true, true } } },
        [(int) PentominoShapeCollections.CommonPieceIds.Z] = new() { Rotate90 = true,  Rotate180 = false, Flip = true,  Geometry = new bool[][] { new bool[] { true, true, false }, new bool[] { false, true, false }, new bool[] { false, true, true } } },
    };


    private static (List<bool[]> MatrixRows, List<PlacementMetadata> MetadataRows) AddAllShapesToMatrix(int boardWidth, int boardHeight, int numberOfMatrixColumns, IEnumerable<PentominoShapeCollections.ShapeData> shapes)
    {
        List<bool[]> matrixRows = new();
        List<PlacementMetadata> metadataRows = new();

        foreach (PentominoShapeCollections.ShapeData shapeData in shapes)
        {
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

                int[] home = GetHomePositions(shapeGeometry, k_pieceSize, boardWidth, k_FirstBoardColumn);
                AddShapeToMatrix(shapeGeometry[0].Length, shapeGeometry.Length, boardWidth, boardHeight, numberOfMatrixColumns, home, matrixRows, placementMetadata, metadataRows);

                if (orientationTraits.Rotate90)
                {
                    // 90 degrees

                    (bool[][] rotatedGeometry, PlacementMetadata rotatedMetadata) = RotateShape(shapeGeometry, placementMetadata);
                    home = GetHomePositions(rotatedGeometry, k_pieceSize, boardWidth, k_FirstBoardColumn);
                    AddShapeToMatrix(rotatedGeometry[0].Length, rotatedGeometry.Length, boardWidth, boardHeight, numberOfMatrixColumns, home, matrixRows, rotatedMetadata, metadataRows);

                    if (orientationTraits.Rotate180)
                    {
                        // 180 degrees

                        (rotatedGeometry, rotatedMetadata) = RotateShape(rotatedGeometry, rotatedMetadata);
                        home = GetHomePositions(rotatedGeometry, k_pieceSize, boardWidth, k_FirstBoardColumn);
                        AddShapeToMatrix(rotatedGeometry[0].Length, rotatedGeometry.Length, boardWidth, boardHeight, numberOfMatrixColumns, home, matrixRows, rotatedMetadata, metadataRows);

                        // 270 degrees

                        (rotatedGeometry, rotatedMetadata) = RotateShape(rotatedGeometry, rotatedMetadata);
                        home = GetHomePositions(rotatedGeometry, k_pieceSize, boardWidth, k_FirstBoardColumn);
                        AddShapeToMatrix(rotatedGeometry[0].Length, rotatedGeometry.Length, boardWidth, boardHeight, numberOfMatrixColumns, home, matrixRows, rotatedMetadata, metadataRows);
                    }
                }
            }
        }

        return (matrixRows, metadataRows);
    }


}
