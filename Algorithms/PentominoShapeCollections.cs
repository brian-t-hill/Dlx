using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

using static Pentomino.Algorithms.PentominoShapeCollections;

namespace Pentomino.Algorithms;


public static class PentominoShapeCollections
{
    public enum CommonPieceIds
    {
        None = 0,

        // negative numbers reserved for shapes that are not part of solutions.
        // 1-9999 reserved for common shapes

        F,
        L,
        I,
        P,
        S,
        T,
        U,
        V,
        W,
        X,
        Y,
        Z,

        Block_2x3,

        LastOfReservedRange = 9999
    }


    public record ShapeData(int PieceId, int PieceColumnIndex, Shape? Shape, Brush Brush);


    public static IEnumerable<ShapeData> Make10x6Shapes()
    {
        yield return new((int) CommonPieceIds.F,  0,  MakeShape((int) CommonPieceIds.F),  Brushes.Peru);
        yield return new((int) CommonPieceIds.L,  1,  MakeShape((int) CommonPieceIds.L),  Brushes.Red);
        yield return new((int) CommonPieceIds.I,  2,  MakeShape((int) CommonPieceIds.I),  Brushes.LightSteelBlue);
        yield return new((int) CommonPieceIds.P,  3,  MakeShape((int) CommonPieceIds.P),  Brushes.Yellow);
        yield return new((int) CommonPieceIds.S,  4,  MakeShape((int) CommonPieceIds.S),  Brushes.DarkGray);
        yield return new((int) CommonPieceIds.T,  5,  MakeShape((int) CommonPieceIds.T),  Brushes.Firebrick);
        yield return new((int) CommonPieceIds.U,  6,  MakeShape((int) CommonPieceIds.U),  Brushes.DarkOrchid);
        yield return new((int) CommonPieceIds.V,  7,  MakeShape((int) CommonPieceIds.V),  Brushes.RoyalBlue);
        yield return new((int) CommonPieceIds.W,  8,  MakeShape((int) CommonPieceIds.W),  Brushes.Lime);
        yield return new((int) CommonPieceIds.X,  9,  MakeShape((int) CommonPieceIds.X),  Brushes.NavajoWhite);
        yield return new((int) CommonPieceIds.Y, 10,  MakeShape((int) CommonPieceIds.Y), Brushes.Green);
        yield return new((int) CommonPieceIds.Z, 11,  MakeShape((int) CommonPieceIds.Z), Brushes.Aquamarine);
    }


    public static IEnumerable<ShapeData> MakeCalendarShapes()
    {
        yield return new(-1,   -1, MakeTextShape(LocalizableStrings.idsMonthName_January,   row: 0, column: 0), Brushes.DarkGray);
        yield return new(-2,   -1, MakeTextShape(LocalizableStrings.idsMonthName_February,  row: 0, column: 1), Brushes.DarkGray);
        yield return new(-3,   -1, MakeTextShape(LocalizableStrings.idsMonthName_March,     row: 0, column: 2), Brushes.DarkGray);
        yield return new(-4,   -1, MakeTextShape(LocalizableStrings.idsMonthName_April,     row: 0, column: 3), Brushes.DarkGray);
        yield return new(-5,   -1, MakeTextShape(LocalizableStrings.idsMonthName_May,       row: 0, column: 4), Brushes.DarkGray);
        yield return new(-6,   -1, MakeTextShape(LocalizableStrings.idsMonthName_June,      row: 0, column: 5), Brushes.DarkGray);
        yield return new(-7,   -1, MakeTextShape(LocalizableStrings.idsMonthName_July,      row: 1, column: 0), Brushes.DarkGray);
        yield return new(-8,   -1, MakeTextShape(LocalizableStrings.idsMonthName_August,    row: 1, column: 1), Brushes.DarkGray);
        yield return new(-9,   -1, MakeTextShape(LocalizableStrings.idsMonthName_September, row: 1, column: 2), Brushes.DarkGray);
        yield return new(-10,  -1, MakeTextShape(LocalizableStrings.idsMonthName_October,   row: 1, column: 3), Brushes.DarkGray);
        yield return new(-11,  -1, MakeTextShape(LocalizableStrings.idsMonthName_November,  row: 1, column: 4), Brushes.DarkGray);
        yield return new(-12,  -1, MakeTextShape(LocalizableStrings.idsMonthName_December,  row: 1, column: 5), Brushes.DarkGray);

        yield return new(-101,  -1, MakeTextShape("1",  row: 2, column: 0), Brushes.DarkGray);
        yield return new(-102,  -1, MakeTextShape("2",  row: 2, column: 1), Brushes.DarkGray);
        yield return new(-103,  -1, MakeTextShape("3",  row: 2, column: 2), Brushes.DarkGray);
        yield return new(-104,  -1, MakeTextShape("4",  row: 2, column: 3), Brushes.DarkGray);
        yield return new(-105,  -1, MakeTextShape("5",  row: 2, column: 4), Brushes.DarkGray);
        yield return new(-106,  -1, MakeTextShape("6",  row: 2, column: 5), Brushes.DarkGray);
        yield return new(-107,  -1, MakeTextShape("7",  row: 2, column: 6), Brushes.DarkGray);
        yield return new(-108,  -1, MakeTextShape("8",  row: 3, column: 0), Brushes.DarkGray);
        yield return new(-109,  -1, MakeTextShape("9",  row: 3, column: 1), Brushes.DarkGray);
        yield return new(-110,  -1, MakeTextShape("10", row: 3, column: 2), Brushes.DarkGray);
        yield return new(-111,  -1, MakeTextShape("11", row: 3, column: 3), Brushes.DarkGray);
        yield return new(-112,  -1, MakeTextShape("12", row: 3, column: 4), Brushes.DarkGray);
        yield return new(-113,  -1, MakeTextShape("13", row: 3, column: 5), Brushes.DarkGray);
        yield return new(-114,  -1, MakeTextShape("14", row: 3, column: 6), Brushes.DarkGray);
        yield return new(-115,  -1, MakeTextShape("15", row: 4, column: 0), Brushes.DarkGray);
        yield return new(-116,  -1, MakeTextShape("16", row: 4, column: 1), Brushes.DarkGray);
        yield return new(-117,  -1, MakeTextShape("17", row: 4, column: 2), Brushes.DarkGray);
        yield return new(-118,  -1, MakeTextShape("18", row: 4, column: 3), Brushes.DarkGray);
        yield return new(-119,  -1, MakeTextShape("19", row: 4, column: 4), Brushes.DarkGray);
        yield return new(-120,  -1, MakeTextShape("20", row: 4, column: 5), Brushes.DarkGray);
        yield return new(-121,  -1, MakeTextShape("21", row: 4, column: 6), Brushes.DarkGray);
        yield return new(-122,  -1, MakeTextShape("22", row: 5, column: 0), Brushes.DarkGray);
        yield return new(-123,  -1, MakeTextShape("23", row: 5, column: 1), Brushes.DarkGray);
        yield return new(-124,  -1, MakeTextShape("24", row: 5, column: 2), Brushes.DarkGray);
        yield return new(-125,  -1, MakeTextShape("25", row: 5, column: 3), Brushes.DarkGray);
        yield return new(-126,  -1, MakeTextShape("26", row: 5, column: 4), Brushes.DarkGray);
        yield return new(-127,  -1, MakeTextShape("27", row: 5, column: 5), Brushes.DarkGray);
        yield return new(-128,  -1, MakeTextShape("28", row: 5, column: 6), Brushes.DarkGray);
        yield return new(-129,  -1, MakeTextShape("29", row: 6, column: 0), Brushes.DarkGray);
        yield return new(-130,  -1, MakeTextShape("30", row: 6, column: 1), Brushes.DarkGray);
        yield return new(-131,  -1, MakeTextShape("31", row: 6, column: 2), Brushes.DarkGray);

        yield return new((int) CommonPieceIds.L, 0, MakeShape((int) CommonPieceIds.L), Brushes.Red);
        yield return new((int) CommonPieceIds.P, 1, MakeShape((int) CommonPieceIds.P), Brushes.Yellow);
        yield return new((int) CommonPieceIds.S, 2, MakeShape((int) CommonPieceIds.S), Brushes.DarkGray);
        yield return new((int) CommonPieceIds.U, 3, MakeShape((int) CommonPieceIds.U), Brushes.DarkOrchid);
        yield return new((int) CommonPieceIds.V, 4, MakeShape((int) CommonPieceIds.V), Brushes.RoyalBlue);
        yield return new((int) CommonPieceIds.Y, 5, MakeShape((int) CommonPieceIds.Y), Brushes.Green);
        yield return new((int) CommonPieceIds.Z, 6, MakeShape((int) CommonPieceIds.Z), Brushes.Aquamarine);
        yield return new((int) CommonPieceIds.Block_2x3, 7, MakeShape((int) CommonPieceIds.Block_2x3), Brushes.Lavender);
    }


    public static Shape? MakeTextShape(string text, int row, int column)
    {
        FormattedText formattedText = new FormattedText(text,
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface("Segoe UI"),
            20.0,
            Brushes.Black,
            1.0);

        formattedText.MaxTextWidth = 100;
        formattedText.MaxTextHeight = 100;
        formattedText.TextAlignment = TextAlignment.Center;
        formattedText.SetFontWeight(FontWeights.UltraLight);  // REVIEW$:  Why does the text look so bulky?  Might be related to DPI or scaling.

        Path path = new()
        {
            Data = formattedText.BuildGeometry(new Point(0, (100.0 - formattedText.Height) / 2.0)),
            Width = 100.0,
            Height = 100.0,
        };

        Canvas.SetLeft(path, column * 100.0);
        Canvas.SetTop(path, row * 100.0);

        return path;
    }


    public static Shape? MakeShape(int pieceId)
    {
        return pieceId switch
        {
            // . F F
            // F F .
            // . F .
            (int) CommonPieceIds.F => new Polygon() { Points = new() { new(100, 0), new(300, 0), new(300, 100), new(200, 100), new(200, 300), new(100, 300), new(100, 200), new(0, 200), new(0, 100), new(100, 100) } },

            // L .
            // L .
            // L .
            // L L
            (int) CommonPieceIds.L => new Polygon() { Points = new() { new(0, 0), new(100, 0), new(100, 300), new(200, 300), new(200, 400), new(0, 400) } },

            // I
            // I
            // I
            // I
            // I
            (int) CommonPieceIds.I => new Polygon() { Points = new() { new(0, 0), new(100, 0), new(100, 500), new(0, 500) } },

            // P P
            // P P
            // P .
            (int) CommonPieceIds.P => new Polygon() { Points = new() { new(0, 0), new(200, 0), new(200, 200), new(100, 200), new(100, 300), new(0, 300) } },

            // S
            // S S
            // . S
            // . S
            (int) CommonPieceIds.S => new Polygon() { Points = new() { new(0, 0), new(100, 0), new(100, 100), new(200, 100), new(200, 400), new(100, 400), new(100, 200), new(0, 200) } },

            // T T T
            // . T .
            // . T .
            (int) CommonPieceIds.T => new Polygon() { Points = new() { new(0, 0), new(300, 0), new(300, 100), new(200, 100), new(200, 300), new(100, 300), new(100, 100), new(0, 100) } },

            // U . U
            // U U U
            (int) CommonPieceIds.U => new Polygon() { Points = new() { new(0, 0), new(100, 0), new(100, 100), new(200, 100), new(200, 0), new(300, 0), new(300, 200), new(0, 200) } },

            // V . .
            // V . .
            // V V V
            (int) CommonPieceIds.V => new Polygon() { Points = new() { new(0, 0), new(100, 0), new(100, 200), new(300, 200), new(300, 300), new(0, 300) } },

            // W . .
            // W W .
            // . W W
            (int) CommonPieceIds.W => new Polygon() { Points = new() { new(0, 0), new(100, 0), new(100, 100), new(200, 100), new(200, 200), new(300, 200), new(300, 300), new(100, 300), new(100, 200), new(0, 200) } },

            // . X .
            // X X X
            // . X .
            (int) CommonPieceIds.X => new Polygon() { Points = new() { new(100, 0), new(200, 0), new(200, 100), new(300, 100), new(300, 200), new(200, 200), new(200, 300), new(100, 300), new(100, 200), new(0, 200), new(0, 100), new(100, 100) } },

            // . . Y .
            // Y Y Y Y
            (int) CommonPieceIds.Y => new Polygon() { Points = new() { new(200, 0), new(300, 0), new(300, 100), new(400, 100), new(400, 200), new(0, 200), new(0, 100), new(200, 100) } },

            // Z Z .
            // . Z .
            // . Z Z
            (int) CommonPieceIds.Z => new Polygon() { Points = new() { new(0, 0), new(200, 0), new(200, 200), new(300, 200), new(300, 300), new(100, 300), new(100, 100), new(0, 100) } },

            // O O
            // O O
            // O O
            (int) CommonPieceIds.Block_2x3 => new Polygon() { Points = new() { new(0, 0), new(200, 0), new(200, 300), new(0, 300) } },

            _ => null
        };
    }
}
