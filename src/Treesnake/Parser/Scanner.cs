/*
 * Scanner class.
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
using System.Collections.Generic;
using NLog;

namespace Treesnake.Parser
{
    /// <summary>
    /// Used for tokenizing strings.
    /// </summary>
    public class Scanner
    {
        /// <summary>
        /// Class logger.
        /// </summary>
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Number of spaces a tab occupies.
        /// </summary>
        private const int TabSize = 4;

        /// <summary>
        /// The input string to extract tokens from.
        /// </summary>
        private readonly string input;

        /// <summary>
        /// Current character position in the input string.
        /// </summary>
        private int currentOffset;

        /// <summary>
        /// Current line number in the input string.
        /// </summary>
        private int currentLine;

        /// <summary>
        /// Current column number on the current line of the input string.
        /// </summary>
        private int currentColumn;

        /// <summary>
        /// Current indent size.
        /// </summary>
        private int currentIndentSize;

        /// <summary>
        /// Stack of indent sizes to make sure indent levels are consistent.
        /// </summary>
        private Stack<int> indentSizes;

        /// <summary>
        /// Type of the previous token.
        /// Used for checking end of statement tokens.
        /// </summary>
        private TokenType previousTokenType;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="input">The input string to extract tokens from.</param>
        public Scanner(string input)
        {
            this.input = input;
            Reset();
        }

        /// <summary>
        /// Resets the scanner.
        /// </summary>
        public void Reset()
        {
            currentOffset = 0;
            currentLine = 1;
            currentColumn = 1;

            indentSizes = new Stack<int>();
            indentSizes.Push(0);
        }


        /// <summary>
        /// Gets the next token in the input string.
        /// </summary>
        /// <returns>A token.</returns>
        /// <exception cref="IndentationError">Raised when an unindent level doesn't match any previous indent level.</exception>
        public Token GetNextToken()
        {
            var token = DoIndent();
            if (token != null)
            {
                logger.Debug("Token: " + token.ToString());
                return token;
            }

            while (currentOffset < input.Length)
            {
                char c = input[currentOffset];
                switch (c)
                {
                    case ' ':
                    case '\t':
                        // Speed up on spaces and tabs.
                        break;

                    case '#':
                        // Skip to end of line or end of input.
                        DoComment();
                        break;

                    case '\n':
                        if (previousTokenType != TokenType.Newline)
                        {
                            // This ends the statement
                            token = CreateToken(TokenType.Newline);
                        }

                        AddLine();
                        break;

                    case '\\':
                        if (PeekAhead() != '\n')
                        {
                            token = CreateToken(TokenType.Error, "\\");
                            break;
                        }

                        Advance();
                        AddLine();
                        break;

                    case '"':
                    case '\'':
                        token = DoStringLiteral(c);
                        break;

                    case '(':
                        token = CreateToken(TokenType.LeftParenthesis);
                        break;

                    case ')':
                        token = CreateToken(TokenType.RightParenthesis);
                        break;

                    case '[':
                        token = CreateToken(TokenType.LeftSquareBracket);
                        break;

                    case ']':
                        token = CreateToken(TokenType.RightSquareBracket);
                        break;

                    case '{':
                        token = CreateToken(TokenType.LeftBrace);
                        break;

                    case '}':
                        token = CreateToken(TokenType.RightBrace);
                        break;

                    case '+':
                        if (PeekAhead() == '=')
                        {
                            token = CreateToken(TokenType.PlusEqual);
                            Advance();
                        }
                        else
                        {
                            token = CreateToken(TokenType.Plus);
                        }
                        break;

                    case '-':
                        if (PeekAhead() == '=')
                        {
                            token = CreateToken(TokenType.MinusEqual);
                            Advance();
                        }
                        else
                        {
                            token = CreateToken(TokenType.Minus);
                        }
                        break;

                    case '*':
                        if (PeekAhead(2) == "*=")
                        {
                            token = CreateToken(TokenType.DoubleStarEqual);
                            Advance(2);
                        }
                        else if (PeekAhead() == '=')
                        {
                            token = CreateToken(TokenType.StarEqual);
                            Advance();
                        }
                        else if (PeekAhead() == '*')
                        {
                            token = CreateToken(TokenType.DoubleStar);
                            Advance();
                        }
                        else
                        {
                            token = CreateToken(TokenType.Star);
                        }
                        break;

                    case '/':
                        if (PeekAhead(2) == "/=")
                        {
                            token = CreateToken(TokenType.DoubleSlashEqual);
                            Advance(2);
                        }
                        else if (PeekAhead() == '=')
                        {
                            token = CreateToken(TokenType.SlashEqual);
                            Advance();
                        }
                        else if (PeekAhead() == '/')
                        {
                            token = CreateToken(TokenType.DoubleSlash);
                            Advance();
                        }
                        else
                        {
                            token = CreateToken(TokenType.Slash);
                        }
                        break;

                    case ':':
                        token = CreateToken(TokenType.Colon);
                        break;

                    case ';':
                        token = CreateToken(TokenType.Semicolon);
                        break;

                    case ',':
                        token = CreateToken(TokenType.Comma);
                        break;

                    case '.':
                        if (Char.IsDigit(PeekAhead()))
                        {
                            token = DoNumberLiteral();
                        }
                        else
                        {
                            token = CreateToken(TokenType.Dot);
                        }
                        break;

                    case '&':
                        if (PeekAhead() == '=')
                        {
                            token = CreateToken(TokenType.AmpersandEqual);
                            Advance();
                        }
                        else
                        {
                            token = CreateToken(TokenType.Ampersand);
                        }
                        break;

                    case '<':
                        if (PeekAhead(2) == "<=")
                        {
                            token = CreateToken(TokenType.LeftShiftEqual);
                            Advance(2);
                        }
                        else if (PeekAhead() == '<')
                        {
                            token = CreateToken(TokenType.LeftShift);
                            Advance();
                        }
                        else if (PeekAhead() == '=')
                        {
                            token = CreateToken(TokenType.LessThanEqual);
                            Advance();
                        }
                        else
                        {
                            token = CreateToken(TokenType.LessThan);
                        }
                        break;

                    case '>':
                        if (PeekAhead(2) == ">=")
                        {
                            token = CreateToken(TokenType.RightShiftEqual);
                            Advance(2);
                        }
                        else if (PeekAhead() == '>')
                        {
                            token = CreateToken(TokenType.RightShift);
                            Advance();
                        }
                        else if (PeekAhead() == '=')
                        {
                            token = CreateToken(TokenType.GreaterThanEqual);
                            Advance();
                        }
                        else
                        {
                            token = CreateToken(TokenType.GreaterThan);
                        }
                        break;

                    case '%':
                        if (PeekAhead() == '=')
                        {
                            token = CreateToken(TokenType.PercentEqual);
                            Advance();
                        }
                        else
                        {
                            token = CreateToken(TokenType.Percent);
                        }
                        break;

                    case '~':
                        token = CreateToken(TokenType.Tilde);
                        break;

                    case '^':
                        if (PeekAhead() == '=')
                        {
                            token = CreateToken(TokenType.CircumflexEqual);
                            Advance();
                        }
                        else
                        {
                            token = CreateToken(TokenType.Circumflex);
                        }
                        break;

                    case '|':
                        if (PeekAhead() == '=')
                        {
                            token = CreateToken(TokenType.VerticalBarEqual);
                            Advance();
                        }
                        else
                        {
                            token = CreateToken(TokenType.VerticalBar);
                        }
                        break;

                    case '=':
                        if (PeekAhead() == '=')
                        {
                            token = CreateToken(TokenType.EqualEqual);
                            Advance();
                        }
                        else
                        {
                            token = CreateToken(TokenType.Equal);
                        }
                        break;

                    case '!':
                        if (PeekAhead() == '=')
                        {
                            token = CreateToken(TokenType.NotEqual);
                            Advance();
                        }
                        break;

                    default:
                    {
                        if (Char.IsLetter(c) || c == '_')
                        {
                            token = DoName();
                        }
                        else if (Char.IsDigit(c))
                        {
                            token = DoNumberLiteral();
                        }
                        else
                        {
                            token = CreateToken(TokenType.Error);
                        }
                        break;
                    }
                }

                if (currentOffset < input.Length)
                {
                    // Safe guard so it doesn't skip too far.
                    Advance();
                }

                if (token != null)
                {
                    previousTokenType = token.Type;
                    break;
                }
            }

            if (token == null)
            {
                // At end of input.
                token = CreateToken(TokenType.EndMarker);
            }

            logger.Debug("Token: " + token.ToString());

            return token;
        }

        /// <summary>
        /// Method for creating tokens.
        /// </summary>
        /// <param name="type">Token type.</param>
        /// <returns>A token.</returns>
        private Token CreateToken(TokenType type)
        {
            return new Token(type, currentOffset, currentLine, currentColumn);
        }

        /// <summary>
        /// Method for creating tokens.
        /// </summary>
        /// <param name="type">Token type.</param>
        /// <param name="literalValue">Literal value of the token.</param>
        /// <returns>A token.</returns>
        private Token CreateToken(TokenType type, string literalValue)
        {
            return new Token(type, currentOffset, currentLine, currentColumn, literalValue);
        }

        /// <summary>
        /// Method for creating tokens.
        /// </summary>
        /// <param name="type">Token type.</param>
        /// <param name="offset">Starting offset in the input string.</param>
        /// <param name="line">Line number.</param>
        /// <param name="column">Column number.</param>
        /// <param name="literalValue">Literal value of the token.</param>
        /// <returns>A token.</returns>
        private Token CreateToken(TokenType type, int offset, int line, int column, string literalValue = null)
        {
            return new Token(type, offset, line, column, literalValue);
        }

        /// <summary>
        /// Increases the current line number and resets the column number.
        /// </summary>
        private void AddLine()
        {
            ++currentLine;
            currentColumn = 0;      // Set to zero because it increments at the end of the token loop.
        }

        /// <summary>
        /// Returns the next character from input.
        /// </summary>
        /// <returns>
        /// The next character from input, or a null character if there is
        /// no more characters left.
        /// </returns>
        private char PeekAhead()
        {
            int endOffset = currentOffset + 1;
            if (endOffset >= input.Length)
            {
                return '\0';
            }

            return input[endOffset];
        }

        /// <summary>
        /// Returns the next number of characters from input.
        /// </summary>
        /// <param name="numOfChars">Number of characters to retrieve.</param>
        /// <returns>
        /// The next number of characters, or an empty string if numOfChars
        /// is greater than input length.
        /// </returns>
        private string PeekAhead(int numOfChars)
        {
            int startOffset = currentOffset + 1;
            int endOffset = currentOffset + numOfChars;
            if (endOffset >= input.Length)
            {
                if (startOffset < input.Length)
                {
                    return input.Substring(startOffset);
                }

                return "";
            }

            return input.Substring(startOffset, numOfChars);
        }

        /// <summary>
        /// Sets the offset ahead by a number of characters.
        /// </summary>
        /// <param name="numOfChars">How far ahead to advance.</param>
        private void Advance(int numOfChars = 1)
        {
            currentOffset += numOfChars;
            currentColumn += numOfChars;
        }

        /// <summary>
        /// Consumes an indent and gets the next indent or dedent token.
        /// </summary>
        /// <returns>An indent or dedent token, or null if none.</returns>
        /// <exception cref="IndentationError">Raised when an unindent doesn't match any previous indent level.</exception>
        private Token DoIndent()
        {
            int startOffset = currentOffset;
            int startColumn = currentColumn;
            if (currentColumn == 1)
            {
                currentIndentSize = 0;
                while (currentOffset < input.Length)
                {
                    bool isWhitespace = true;
                    char c = input[currentOffset];
                    switch (c)
                    {
                        case ' ':
                            // Increase indent size.
                            ++currentIndentSize;
                            break;

                        case '\t':
                            // Increase indent size.
                            currentIndentSize += TabSize;
                            break;

                        case '\n':
                            // Ignore blank line and do the indent on the next line.
                            AddLine();
                            currentIndentSize = 0;
                            startOffset = currentOffset;
                            startColumn = currentColumn;
                            break;

                        case '#':
                            // Stop and let GetNextToken() to consume the comment.
                            currentIndentSize = indentSizes.Peek();
                            isWhitespace = false;
                            break;

                        default:
                            // Indent ends.
                            isWhitespace = false;
                            break;
                    }

                    if (!isWhitespace)
                    {
                        break;
                    }

                    Advance();
                }
            }

            int prevIndentSize = indentSizes.Peek();
            if (currentIndentSize > prevIndentSize)
            {
                // Indent
                indentSizes.Push(currentIndentSize);
                return CreateToken(TokenType.Indent, startOffset, currentLine, startColumn);
            }
            else if (currentIndentSize < prevIndentSize)
            {
                // Dedent
                indentSizes.Pop();
                if (indentSizes.Count == 1 && currentIndentSize > indentSizes.Peek())
                {
                    throw new IndentationError(startOffset, currentLine, startColumn, "Unindent doesn't match any previous indent level.");
                }

                return CreateToken(TokenType.Dedent, startOffset, currentLine, startColumn);
            }

            return null;
        }

                /// <summary>
        /// Consumes a comment.
        /// </summary>
        /// <returns>Comment token.</returns>
        private Token DoComment()
        {
            int startOffset = currentOffset;
            int startLine = currentLine;
            int startColumn = currentColumn;
            bool addOne = false;        // For adding 1 to the offset difference when a newline isn't consumed.
            while (currentOffset < input.Length)
            {
                if (PeekAhead() == '\n')
                {
                    switch (previousTokenType)
                    {
                        case TokenType.Newline:
                        case TokenType.Comment:
                        case TokenType.Error:
                            // Consume the newline.
                            Advance();
                            AddLine();
                            break;
                        default:
                            addOne = true;
                            break;
                    }
                    
                    break;
                }

                Advance();
            }
            
            return CreateToken(TokenType.Comment, startOffset, startLine, startColumn, input.Substring(startOffset, currentOffset - startOffset + (addOne ? 1 : 0)) );
        }

        /// <summary>
        /// Consumes and returns a name token.
        /// </summary>
        /// <returns>A name token.</returns>
        private Token DoName()
        {
            int startOffset = currentOffset;
            int startColumn = currentColumn;
            while (currentOffset < input.Length)
            {
                char c = PeekAhead();
                if (!Char.IsLetter(c) && !Char.IsDigit(c) && c != '_')
                {
                    // End of the token.
                    break;
                }

                Advance();
            }

            return CreateToken(TokenType.Name, startOffset, currentLine, startColumn, input.Substring(startOffset, currentOffset - startOffset + 1));
        }


        /// <summary>
        /// Consumes a number literal.
        /// </summary>
        /// <returns>Float or integer token.</returns>
        private Token DoNumberLiteral()
        {
            TokenType type;
            char c = input[currentOffset];
            if (c == '.')
            {
                type = TokenType.FloatLiteral;
            }
            else if (c == '0')
            {
                char nextChar = PeekAhead();
                switch (nextChar)
                {
                    case 'x':
                        return DoHexadecimalLiteral();

                    case 'b':
                        return DoBinaryLiteral();

                    default:
                        if (nextChar >= '0' && nextChar <= '7')
                        {
                            return DoOctalLiteral();
                        }

                        return CreateToken(TokenType.IntegerLiteral, "0");
                }
            }
            else
            {
                type = TokenType.IntegerLiteral;
            }

            int startOffset = currentOffset;
            int startColumn = currentColumn;
            while (currentOffset < input.Length)
            {
                c = PeekAhead();
                if (c == '.')
                {
                    if (type != TokenType.FloatLiteral)
                    {
                        // Must be a float literal.
                        type = TokenType.FloatLiteral;
                    }
                    else
                    {
                        // End of token.
                        break;
                    }
                }
                else if (!Char.IsDigit(c))
                {
                    // End of token.
                    break;
                }

                Advance();
            }

            return CreateToken(type, startOffset, currentLine, startColumn, input.Substring(startOffset, currentOffset - startOffset + 1));
        }

        /// <summary>
        /// Consumes a hexadecimal literal.
        /// </summary>
        /// <returns>Integer token.</returns>
        private Token DoHexadecimalLiteral()
        {
            string nextChars = PeekAhead(2);
            if (nextChars.Length < 2 || !IsHexadecimalCharacter(nextChars[1]))
            {
                // Reached the end of input, or not valid hex character.
                return CreateToken(TokenType.IntegerLiteral, "0");
            }

            int startOffset = currentOffset;
            int startColumn = currentColumn;
            while (currentOffset < input.Length)
            {
                Advance();
                if (!IsHexadecimalCharacter(PeekAhead()))
                {
                    break;
                }
            }

            return CreateToken(TokenType.IntegerLiteral, startOffset, currentLine, startColumn, input.Substring(startOffset, currentOffset - startOffset + 1));
        }

        /// <summary>
        /// Consumes a binary literal.
        /// </summary>
        /// <returns>Integer token.</returns>
        private Token DoBinaryLiteral()
        {
            string nextChars = PeekAhead(2);
            if (nextChars.Length < 2 || !IsBinaryCharacter(nextChars[1]))
            {
                // Reached the end of input, or not valid binary character.
                return CreateToken(TokenType.IntegerLiteral, "0");
            }

            int startOffset = currentOffset;
            int startColumn = currentColumn;
            while (currentOffset < input.Length)
            {
                Advance();
                if (!IsBinaryCharacter(PeekAhead()))
                {
                    break;
                }
            }

            return CreateToken(TokenType.IntegerLiteral, startOffset, currentLine, startColumn, input.Substring(startOffset, currentOffset - startOffset + 1));
        }

        /// <summary>
        /// Consumes an octal literal.
        /// </summary>
        /// <returns>Integer token.</returns>
        private Token DoOctalLiteral()
        {
            int startOffset = currentOffset;
            int startColumn = currentColumn;
            while (currentOffset < input.Length)
            {
                if (!IsOctalCharacter(PeekAhead()))
                {
                    break;
                }

                Advance();
            }

            return CreateToken(TokenType.IntegerLiteral, startOffset, currentLine, startColumn, input.Substring(startOffset, currentOffset - startOffset + 1));
        }

        /// <summary>
        /// Returns a string literal token.
        /// </summary>
        /// <param name="quote">The quote character to match.</param>
        /// <returns>A string literal token.</returns>
        private Token DoStringLiteral(char quote)
        {
            bool isThreeQuoted = false;
            string quoteStr = quote.ToString();
            string threeQuotes = quoteStr + quoteStr + quoteStr;
            if (quoteStr + PeekAhead(2) == threeQuotes)
            {
                isThreeQuoted = true;
            }

            int startOffset = currentOffset;
            int startLine = currentLine;
            int startColumn = currentColumn;
            Advance(isThreeQuoted ? 3 : 1);
            int stringStartOffset = currentOffset;
            bool isEscaping = false;
            while (currentOffset < input.Length)
            {
                char c = input[currentOffset];
                if (isEscaping)
                {
                    isEscaping = false;
                    if (c == '\n')
                    {
                        // Continued on next line.
                        AddLine();
                    }
                }
                else
                {
                    if (c == '\\')
                    {
                        isEscaping = true;
                    }
                    else if (c == '\n')
                    {
                        if (isThreeQuoted)
                        {
                            AddLine();
                        }
                        else
                        {
                            // Unexpected end of line.
                            break;
                        }
                    }
                    else if (c == quote && (!isThreeQuoted || (isThreeQuoted && (quoteStr + PeekAhead(2)) == threeQuotes)))
                    {
                        // End of string literal.
                        string value = input.Substring(stringStartOffset, currentOffset - stringStartOffset)
                            .Replace("\\\n", "")
                            .Replace("\\'", "'");

                        if (isThreeQuoted)
                        {
                            // Necessary to reach the end of the three quoted literal.
                            Advance(2);
                        }
                        return CreateToken(TokenType.StringLiteral, startOffset, startLine, startColumn, value);
                    }
                }

                Advance();
            }

            if (isThreeQuoted)
            {
                // Unterminated three quote string.
                return CreateToken(TokenType.Error, startOffset, startLine, startColumn, "\"\"\"");
            }

            // Unterminated string.
            return CreateToken(TokenType.Error, startOffset, startLine, startColumn, "\"");
        }

        /// <summary>
        /// Returns whether a character is a hexadecimal character.
        /// </summary>
        /// <param name="c">Character to test.</param>
        /// <returns>True if the character is hexadecimal, or false otherwise.</returns>
        private bool IsHexadecimalCharacter(char c)
        {
            char lower = Char.ToLower(c);
            return ((lower >= '0' && lower <= '9') || (lower >= 'a' && lower <= 'f'));
        }

        /// <summary>
        /// Returns whether a character is a binary character.
        /// </summary>
        /// <param name="c">Character to test.</param>
        /// <returns>True if the character is binary, or false otherwise.</returns>
        private bool IsBinaryCharacter(char c)
        {
            return (c == '0' || c == '1');
        }

        /// <summary>
        /// Returns whether a character is an octal character.
        /// </summary>
        /// <param name="c">Character to test.</param>
        /// <returns>True if the character is octal, or false otherwise.</returns>
        private bool IsOctalCharacter(char c)
        {
            return (c >= '0' && c <= '7');
        }
    }
}
