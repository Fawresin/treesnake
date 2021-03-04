/*
 * Parser related exceptions.
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
    /// A parser error.
    /// </summary>
    public abstract class ParserError : Exception
    {
        /// <summary>
        /// Starting offset in the input string.
        /// </summary>
        public int Offset { get; }

        /// <summary>
        /// Starting line where the error occurred.
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// Starting column where the error occurred.
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="offset">Starting offset in the input string</param>
        /// <param name="line">Starting line where the error occurred.</param>
        /// <param name="column">Starting column where the error occurred.</param>
        /// <param name="message">Error message.</param>
        public ParserError(int offset, int line, int column, string message) : base(message)
        {
            Offset = offset;
            Line = line;
            Column = column;
        }
    }

    /// <summary>
    /// Raised when there is an unexpected indent or when an unindent level
    /// doesn't match any outer indent level.
    /// </summary>
    public class IndentationError : ParserError
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="offset">Starting offset in the input string</param>
        /// <param name="line">Starting line where the error occurred.</param>
        /// <param name="column">Starting column where the error occurred.</param>
        /// <param name="message">Exception message.</param>
        public IndentationError(int offset, int line, int column, string message) : base(offset, line, column, message) { }
    }

    public class SyntaxError : ParserError
    {
        public SyntaxError(int offset, int line, int column, string message) : base(offset, line, column, message) { }
    }
}
