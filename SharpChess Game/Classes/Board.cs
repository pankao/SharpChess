// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Board.cs" company="SharpChess">
//   Peter Hughes
// </copyright>
// <summary>
//   The board.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region License

// SharpChess
// Copyright (C) 2011 Peter Hughes
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
#endregion

namespace SharpChess
{
    #region Using

    using System;
    using System.Diagnostics;
    using System.Text;

    #endregion

    /// <summary>
    /// The board.
    /// </summary>
    public class Board
    {
        #region Constants and Fields

        /// <summary>
        ///   The fil e_ count.
        /// </summary>
        public const byte FILE_COUNT = 8;

        /// <summary>
        ///   The matri x_ width.
        /// </summary>
        public const byte MATRIX_WIDTH = 16;

        /// <summary>
        ///   The ran k_ count.
        /// </summary>
        public const byte RANK_COUNT = 8;

        /// <summary>
        ///   The squar e_ count.
        /// </summary>
        public const byte SQUARE_COUNT = 128;

        /// <summary>
        ///   The hash code a.
        /// </summary>
        public static ulong HashCodeA;

        /// <summary>
        ///   The hash code b.
        /// </summary>
        public static ulong HashCodeB;

        /// <summary>
        ///   The pawn hash code a.
        /// </summary>
        public static ulong PawnHashCodeA;

        /// <summary>
        ///   The pawn hash code b.
        /// </summary>
        public static ulong PawnHashCodeB;

        /// <summary>
        ///   The m_arr square.
        /// </summary>
        private static readonly Square[] m_arrSquare = new Square[RANK_COUNT * MATRIX_WIDTH];

        /// <summary>
        ///   The m_ orientation.
        /// </summary>
        private static enmOrientation m_Orientation = enmOrientation.White;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes static members of the <see cref = "Board" /> class.
        /// </summary>
        static Board()
        {
            for (int intOrdinal = 0; intOrdinal < SQUARE_COUNT; intOrdinal++)
            {
                m_arrSquare[intOrdinal] = new Square(intOrdinal);
            }
        }

        #endregion

        #region Enums

        /// <summary>
        /// The enm orientation.
        /// </summary>
        public enum enmOrientation
        {
            /// <summary>
            ///   The white.
            /// </summary>
            White, 

            /// <summary>
            ///   The black.
            /// </summary>
            Black
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets DebugString.
        /// </summary>
        public static string DebugString
        {
            get
            {
                Square square;
                Piece piece;
                string strOutput = string.Empty;
                int intOrdinal = SQUARE_COUNT - 1;

                for (int intRank = 0; intRank < RANK_COUNT; intRank++)
                {
                    for (int intFile = 0; intFile < FILE_COUNT; intFile++)
                    {
                        square = GetSquare(intOrdinal);
                        if (square != null)
                        {
                            if ((piece = square.Piece) != null)
                            {
                                strOutput += piece.Abbreviation;
                            }
                            else
                            {
                                strOutput += square.Colour == Square.enmColour.White ? "." : "#";
                            }
                        }

                        strOutput += Convert.ToChar(13) + Convert.ToChar(10);

                        intOrdinal--;
                    }
                }

                return strOutput;
            }
        }

        /// <summary>
        ///   Gets or sets Orientation.
        /// </summary>
        public static enmOrientation Orientation
        {
            get
            {
                return m_Orientation;
            }

            set
            {
                m_Orientation = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The append piece path.
        /// </summary>
        /// <param name="moves">
        /// The moves.
        /// </param>
        /// <param name="piece">
        /// The piece.
        /// </param>
        /// <param name="player">
        /// The player.
        /// </param>
        /// <param name="Offset">
        /// The offset.
        /// </param>
        /// <param name="movesType">
        /// The moves type.
        /// </param>
        public static void AppendPiecePath(
            Moves moves, Piece piece, Player player, int Offset, Moves.enmMovesType movesType)
        {
            int intOrdinal = piece.Square.Ordinal;
            Square square;

            intOrdinal += Offset;
            while ((square = GetSquare(intOrdinal)) != null)
            {
                if (square.Piece == null)
                {
                    if (movesType == Moves.enmMovesType.All)
                    {
                        moves.Add(0, 0, Move.enmName.Standard, piece, piece.Square, square, null, 0, 0);
                    }
                }
                else if (square.Piece.Player.Colour != player.Colour && square.Piece.IsCapturable)
                {
                    moves.Add(0, 0, Move.enmName.Standard, piece, piece.Square, square, square.Piece, 0, 0);
                    break;
                }
                else
                {
                    break;
                }

                intOrdinal += Offset;
            }
        }

        /// <summary>
        /// Display the chessboard in the Immediate Windows
        /// </summary>
        /// <remarks>
        /// VS.NET menu Debug/Windows/Immediate
        /// </remarks>
        /// <example>
        /// An example is Board.DebugDisplay()
        /// </example>
        /// <returns>
        /// A string representation of the current board position.
        /// </returns>
        public static string DebugDisplay()
        {
            Debug.Write(DebugGetBoard());
            Debug.Write(". ");
            return " ";
        }

        /// <summary>
        /// The debug get board.
        /// </summary>
        /// <returns>
        /// The debug get board.
        /// </returns>
        public static string DebugGetBoard()
        {
            StringBuilder strbBoard = new StringBuilder(160);
            Square square;
            strbBoard.Append("  0 1 2 3 4 5 6 7 :PlayerToPlay = ");
            strbBoard.Append((Game.PlayerToPlay.Colour == Player.enmColour.White) ? "White\n" : "Black\n");
            for (int indRank = 7; indRank >= 0; indRank--)
            {
                strbBoard.Append(indRank + 1);
                strbBoard.Append(":");
                for (int indFile = 0; indFile < 8; indFile++)
                {
                    square = GetSquare(indFile, indRank);
                    if (square != null)
                    {
                        if (square.Piece == null)
                        {
                            strbBoard.Append(". ");
                        }
                        else
                        {
                            if (square.Piece.Player.Colour == Player.enmColour.White)
                            {
                                strbBoard.Append(square.Piece.Abbreviation);
                            }
                            else
                            {
                                strbBoard.Append(square.Piece.Abbreviation.ToLower());
                            }

                            strbBoard.Append(" ");
                        }
                    }
                }

                DebugGameInfo(indRank, ref strbBoard);
            }

            strbBoard.Append("  a b c d e f g h :TurnNo = ");
            strbBoard.Append(Game.TurnNo);
            return strbBoard.ToString();
        }

        /// <summary>
        /// The establish hash key.
        /// </summary>
        public static void EstablishHashKey()
        {
            Piece piece;
            HashCodeA = 0UL;
            HashCodeB = 0UL;
            PawnHashCodeA = 0UL;
            PawnHashCodeB = 0UL;
            for (int intOrdinal = 0; intOrdinal < SQUARE_COUNT; intOrdinal++)
            {
                piece = GetPiece(intOrdinal);
                if (piece != null)
                {
                    HashCodeA ^= piece.HashCodeAForSquareOrdinal(intOrdinal);
                    HashCodeB ^= piece.HashCodeBForSquareOrdinal(intOrdinal);
                    if (piece.Name == Piece.enmName.Pawn)
                    {
                        PawnHashCodeA ^= piece.HashCodeAForSquareOrdinal(intOrdinal);
                        PawnHashCodeB ^= piece.HashCodeBForSquareOrdinal(intOrdinal);
                    }
                }
            }
        }

        /// <summary>
        /// The file from name.
        /// </summary>
        /// <param name="FileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The file from name.
        /// </returns>
        public static int FileFromName(string FileName)
        {
            switch (FileName)
            {
                case "a":
                    return 0;
                case "b":
                    return 1;
                case "c":
                    return 2;
                case "d":
                    return 3;
                case "e":
                    return 4;
                case "f":
                    return 5;
                case "g":
                    return 6;
                case "h":
                    return 7;
            }

            return -1;
        }

        /// <summary>
        /// The flip.
        /// </summary>
        public static void Flip()
        {
            m_Orientation = m_Orientation == enmOrientation.White ? enmOrientation.Black : enmOrientation.White;
        }

        /// <summary>
        /// The get piece.
        /// </summary>
        /// <param name="Ordinal">
        /// The ordinal.
        /// </param>
        /// <returns>
        /// </returns>
        public static Piece GetPiece(int Ordinal)
        {
            return (Ordinal & 0x88) == 0 ? m_arrSquare[Ordinal].Piece : null;
        }

        /// <summary>
        /// The get piece.
        /// </summary>
        /// <param name="File">
        /// The file.
        /// </param>
        /// <param name="Rank">
        /// The rank.
        /// </param>
        /// <returns>
        /// </returns>
        public static Piece GetPiece(int File, int Rank)
        {
            return (OrdinalFromFileRank(File, Rank) & 0x88) == 0
                       ? m_arrSquare[OrdinalFromFileRank(File, Rank)].Piece
                       : null;
        }

        /// <summary>
        /// The get square.
        /// </summary>
        /// <param name="Ordinal">
        /// The ordinal.
        /// </param>
        /// <returns>
        /// </returns>
        public static Square GetSquare(int Ordinal)
        {
            return (Ordinal & 0x88) == 0 ? m_arrSquare[Ordinal] : null;
        }

        /// <summary>
        /// The get square.
        /// </summary>
        /// <param name="File">
        /// The file.
        /// </param>
        /// <param name="Rank">
        /// The rank.
        /// </param>
        /// <returns>
        /// </returns>
        public static Square GetSquare(int File, int Rank)
        {
            return (OrdinalFromFileRank(File, Rank) & 0x88) == 0 ? m_arrSquare[OrdinalFromFileRank(File, Rank)] : null;
        }

        /// <summary>
        /// The get square.
        /// </summary>
        /// <param name="Label">
        /// The label.
        /// </param>
        /// <returns>
        /// </returns>
        public static Square GetSquare(string Label)
        {
            return m_arrSquare[OrdinalFromFileRank(FileFromName(Label.Substring(0, 1)), int.Parse(Label.Substring(1, 1)) - 1)];
        }

        /// <summary>
        /// The line is open.
        /// </summary>
        /// <param name="colour">
        /// The colour.
        /// </param>
        /// <param name="squareStart">
        /// The square start.
        /// </param>
        /// <param name="Offset">
        /// The offset.
        /// </param>
        /// <returns>
        /// The line is open.
        /// </returns>
        public static int LineIsOpen(Player.enmColour colour, Square squareStart, int Offset)
        {
            int intOrdinal = squareStart.Ordinal;
            int intSquareCount = 0;
            int intPenalty = 0;
            Square square;

            intOrdinal += Offset;

            while (intSquareCount <= 2
                   &&
                   ((square = GetSquare(intOrdinal)) != null
                    &&
                    (square.Piece == null
                     || (square.Piece.Name != Piece.enmName.Pawn && square.Piece.Name != Piece.enmName.Rook)
                     || square.Piece.Player.Colour != colour)))
            {
                intPenalty += 75;
                intSquareCount++;
                intOrdinal += Offset;
            }

            return intPenalty;
        }

        /// <summary>
        /// The line threatened by.
        /// </summary>
        /// <param name="player">
        /// The player.
        /// </param>
        /// <param name="squares">
        /// The squares.
        /// </param>
        /// <param name="squareStart">
        /// The square start.
        /// </param>
        /// <param name="Offset">
        /// The offset.
        /// </param>
        public static void LineThreatenedBy(Player player, Squares squares, Square squareStart, int Offset)
        {
            int intOrdinal = squareStart.Ordinal;
            Square square;

            intOrdinal += Offset;
            while ((square = GetSquare(intOrdinal)) != null)
            {
                if (square.Piece == null)
                {
                    squares.Add(square);
                }
                else if (square.Piece.Player.Colour != player.Colour && square.Piece.IsCapturable)
                {
                    squares.Add(square);
                    break;
                }
                else
                {
                    break;
                }

                intOrdinal += Offset;
            }
        }

        /// <summary>
        /// The lines first piece.
        /// </summary>
        /// <param name="colour">
        /// The colour.
        /// </param>
        /// <param name="PieceName">
        /// The piece name.
        /// </param>
        /// <param name="squareStart">
        /// The square start.
        /// </param>
        /// <param name="Offset">
        /// The offset.
        /// </param>
        /// <returns>
        /// </returns>
        public static Piece LinesFirstPiece(
            Player.enmColour colour, Piece.enmName PieceName, Square squareStart, int Offset)
        {
            int intOrdinal = squareStart.Ordinal;
            Square square;

            intOrdinal += Offset;
            while ((square = GetSquare(intOrdinal)) != null)
            {
                if (square.Piece == null)
                {
                }
                else if (square.Piece.Player.Colour != colour)
                {
                    return null;
                }
                else if (square.Piece.Name == PieceName || square.Piece.Name == Piece.enmName.Queen)
                {
                    return square.Piece;
                }
                else
                {
                    return null;
                }

                intOrdinal += Offset;
            }

            return null;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Display info on the game at the right of the chessboard
        /// </summary>
        /// <param name="indRank">
        /// the rank in the chessboard
        /// </param>
        /// <param name="strbBoard">
        /// output buffer
        /// </param>
        /// <remarks>
        /// Display the captured pieces and the MoveHistory
        /// </remarks>
        private static void DebugGameInfo(int indRank, ref StringBuilder strbBoard)
        {
            strbBoard.Append(":");
            strbBoard.Append(indRank);
            strbBoard.Append(" ");
            switch (indRank)
            {
                case 0:
                case 7:
                    Pieces piecesCaptureList = (indRank == 7)
                                                   ? Game.PlayerWhite.CapturedEnemyPieces
                                                   : Game.PlayerBlack.CapturedEnemyPieces;
                    if (piecesCaptureList.Count > 1)
                    {
                        strbBoard.Append("x ");
                        foreach (Piece pieceCaptured in piecesCaptureList)
                        {
                            strbBoard.Append(
                                (pieceCaptured.Name == Piece.enmName.Pawn)
                                    ? string.Empty
                                    : pieceCaptured.Abbreviation + pieceCaptured.Square.Name + " ");
                        }
                    }

                    break;

                case 5:
                    int iTurNoSave = Game.TurnNo; // Backup TurNo
                    Game.TurnNo -= Game.PlayerToPlay.SearchDepth;
                    for (int indMov = Math.Max(1, Game.MoveHistory.Count - Game.PlayerToPlay.MaxSearchDepth);
                         indMov < Game.MoveHistory.Count;
                         indMov++)
                    {
                        Move moveThis = Game.MoveHistory[indMov];
                        if (moveThis.Piece.Player.Colour == Player.enmColour.White)
                        {
                            strbBoard.Append(indMov >> 1);
                            strbBoard.Append(". ");
                        }

                        // moveThis.PgnSanFormat(false); // Contextual to Game.TurNo
                        strbBoard.Append(moveThis.Description + " ");
                        Game.TurnNo++;
                    }

                    Game.TurnNo = iTurNoSave; // Restore TurNo
                    break;
            }

            strbBoard.Append("\n");
        }

        /// <summary>
        /// The ordinal from file rank.
        /// </summary>
        /// <param name="File">
        /// The file.
        /// </param>
        /// <param name="Rank">
        /// The rank.
        /// </param>
        /// <returns>
        /// The ordinal from file rank.
        /// </returns>
        private static int OrdinalFromFileRank(int File, int Rank)
        {
            return (Rank << 4) | File;
        }

        #endregion

        // end DebugGameInfo
    }
}