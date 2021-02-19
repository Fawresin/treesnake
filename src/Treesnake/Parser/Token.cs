/*
 * Token class and token types.
 * Copyright (c) 2021 Fawresin
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;

namespace Treesnake.Parser
{
    /// <summary>
    /// A type of token.
    /// </summary>
    public enum TokenType
    {
        Error,
        Comment,
        Name,
        Indent,
        Dedent,
        Newline,
        EndMarker,
        IntegerLiteral,
        FloatLiteral,
        StringLiteral,
        LeftParenthesis,    // (
        RightParenthesis,   // )
        LeftSquareBracket,  // [
        RightSquareBracket, // ]
        LeftBrace,          // {
        RightBrace,         // }
        Plus,               // +
        PlusEqual,          // +=
        Minus,              // -
        MinusEqual,         // -=
        Star,               // *
        StarEqual,          // *=
        DoubleStar,         // **
        DoubleStarEqual,    // **=
        Slash,              // /
        SlashEqual,         // /=
        DoubleSlash,        // //
        DoubleSlashEqual,   // //=
        Colon,              // :
        Semicolon,          // ;
        Comma,              // ,
        Dot,                // .
        Ampersand,          // &
        AmpersandEqual,     // &=
        LessThan,           // < 
        LessThanEqual,      // <=
        LeftShift,          // <<
        LeftShiftEqual,     // <<=
        GreaterThan,        // >
        GreaterThanEqual,   // >=
        RightShift,         // >>
        RightShiftEqual,    // >>=
        Percent,            // %
        PercentEqual,       // %=
        Tilde,              // ~
        Circumflex,         // ^
        CircumflexEqual,    // ^=
        VerticalBar,        // |
        VerticalBarEqual,   // |=
        Equal,              // =
        EqualEqual,         // ==
        NotEqual,           // !=
    }

    /// <summary>
    /// A scanner token.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// The type of token.
        /// </summary>
        public TokenType Type { get; }

        /// <summary>
        /// Starting offset in the input string.
        /// </summary>
        public int Offset { get; }

        /// <summary>
        /// Line number.
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// Column number.
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// Literal value.
        /// </summary>
        public string Literal { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">The type of token.</param>
        /// <param name="offset">Starting offset in the input string.</param>
        /// <param name="line">Line number.</param>
        /// <param name="column">Column number.</param>
        /// <param name="literal">Literal value.</param>
        public Token(TokenType type, int offset, int line, int column, string literal = null)
        {
            Type = type;
            Offset = offset;
            Line = line;
            Column = column;
            Literal = literal;
        }

        public override string ToString()
        {
            return $"[{Type} Token '{Literal}']";
        }
    }
}
