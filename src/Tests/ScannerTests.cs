/*
 * Scanner tests.
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
using NUnit.Framework;
using Treesnake.Parser;

namespace Tests
{
    class ScannerTests
    {
        private Scanner AssertTokens(string input, Token[] expectedTokens)
        {
            var scanner = new Scanner(input);
            foreach (var expectedToken in expectedTokens)
            {
                var token = scanner.GetNextToken();
                Assert.AreEqual(expectedToken.Type, token.Type);
                Assert.AreEqual(expectedToken.Offset, token.Offset);
                Assert.AreEqual(expectedToken.Line, token.Line);
                Assert.AreEqual(expectedToken.Column, token.Column);
                Assert.AreEqual(expectedToken.Literal, token.Literal);
            }

            return scanner;
        }

        private Scanner AssertTokenTypes(string input, TokenType[] expectedTypes)
        {
            var scanner = new Scanner(input);
            foreach (var expectedType in expectedTypes)
            {
                Assert.AreEqual(expectedType, scanner.GetNextToken().Type);
            }

            return scanner;
        }

        [Test]
        public void WhitespaceTest()
        {
            string input = "\ta = 20";
            Token[] expected =
            {
                new Token(TokenType.Indent, 0, 1, 1),
                new Token(TokenType.Name, 1, 1, 2, "a"),
                new Token(TokenType.Equal, 3, 1, 4),
                new Token(TokenType.IntegerLiteral, 5, 1, 6, "20"),
                new Token(TokenType.EndMarker, 7, 1, 8)
            };

            AssertTokens(input, expected);
        }

        [Test]
        public void IntegerTest()
        {
            // Normal integer test.
            string input = "1234 0";
            Token[] expected =
            {
                new Token(TokenType.IntegerLiteral, 0, 1, 1, "1234"),
                new Token(TokenType.IntegerLiteral, 5, 1, 6, "0"),
                new Token(TokenType.EndMarker, 6, 1, 7)
            };

            AssertTokens(input, expected);
        }

        [Test]
        public void HexTest()
        {
            // Normal hex value.
            string input = "0xff";
            Token[] expected =
            {
                new Token(TokenType.IntegerLiteral, 0, 1, 1, "0xff")
            };

            // Hex mixed with non-hex character.
            input = "0x9x";
            expected = new Token[]
            {
                new Token(TokenType.IntegerLiteral, 0, 1, 1, "0x9"),
                new Token(TokenType.Name, 3, 1, 4, "x"),
                new Token(TokenType.EndMarker, 4, 1, 5)
            };

            AssertTokens(input, expected);
        }

        [Test]
        public void OctalTest()
        {
            string input = "077";
            Token[] expected =
            {
                new Token(TokenType.IntegerLiteral, 0, 1, 1, "077"),
            };

            AssertTokens(input, expected);

            // Invalid octal
            input = "08";
            expected = new Token[]
            {
                new Token(TokenType.IntegerLiteral, 0, 1, 1, "0"),
                new Token(TokenType.IntegerLiteral, 1, 1, 2, "8")
            };

            AssertTokens(input, expected);
        }

        [Test]
        public void BinaryTest()
        {
            // Normal binary.
            string input = "0b1111";
            Token[] expected =
            {
                new Token(TokenType.IntegerLiteral, 0, 1, 1, "0b1111")
            };

            AssertTokens(input, expected);


            // Invalid binary.
            input = "0b2";
            expected = new Token[]
            {
                new Token(TokenType.IntegerLiteral, 0, 1, 1, "0"),
                new Token(TokenType.Name, 1, 1, 2, "b2"),
            };

            AssertTokens(input, expected);
        }

        [Test]
        public void OpTest()
        {
            string input = "+-*/%&^<><<>>~**//|";
            TokenType[] expected =
            {
                TokenType.Plus,
                TokenType.Minus,
                TokenType.Star,
                TokenType.Slash,
                TokenType.Percent,
                TokenType.Ampersand,
                TokenType.Circumflex,
                TokenType.LessThan,
                TokenType.GreaterThan,
                TokenType.LeftShift,
                TokenType.RightShift,
                TokenType.Tilde,
                TokenType.DoubleStar,
                TokenType.DoubleSlash,
                TokenType.VerticalBar,
                TokenType.EndMarker
            };

            AssertTokenTypes(input, expected);

            // Assignment operators.
            input = "=+=-=*=/=%=&=|=<=>=<<=>>===!=^=**=//=";
            expected = new TokenType[]
            {
                TokenType.Equal,
                TokenType.PlusEqual,
                TokenType.MinusEqual,
                TokenType.StarEqual,
                TokenType.SlashEqual,
                TokenType.PercentEqual,
                TokenType.AmpersandEqual,
                TokenType.VerticalBarEqual,
                TokenType.LessThanEqual,
                TokenType.GreaterThanEqual,
                TokenType.LeftShiftEqual,
                TokenType.RightShiftEqual,
                TokenType.EqualEqual,
                TokenType.NotEqual,
                TokenType.CircumflexEqual,
                TokenType.DoubleStarEqual,
                TokenType.DoubleSlashEqual,
                TokenType.EndMarker
            };

            AssertTokenTypes(input, expected);
        }

        [Test]
        public void IndentTest()
        {
            // Identning and dedenting.
            string input = " a\n  b\nc";
            Token[] expected =
            {
                new Token(TokenType.Indent, 0, 1, 1),
                new Token(TokenType.Name, 1, 1, 2, "a"),
                new Token(TokenType.Newline, 2, 1, 3),
                new Token(TokenType.Indent, 3, 2, 1),
                new Token(TokenType.Name, 5, 2, 3, "b"),
                new Token(TokenType.Newline, 6, 2, 4),
                new Token(TokenType.Dedent, 7, 3, 1),
                new Token(TokenType.Dedent, 7, 3, 1),
                new Token(TokenType.Name, 7, 3, 1, "c"),
                new Token(TokenType.EndMarker, 8, 3, 2)
            };

            AssertTokens(input, expected);

            // Matching indents.
            input = "a\n b\nc\nd";
            expected = new Token[]
            {
                new Token(TokenType.Name, 0, 1, 1, "a"),
                new Token(TokenType.Newline, 1, 1, 2),
                new Token(TokenType.Indent, 2, 2, 1),
                new Token(TokenType.Name, 3, 2, 2, "b"),
                new Token(TokenType.Newline, 4, 2, 3),
                new Token(TokenType.Dedent, 5, 3, 1),
                new Token(TokenType.Name, 5, 3, 1, "c"),
                new Token(TokenType.Newline, 6, 3, 2),
                new Token(TokenType.Name, 7, 4, 1, "d"),
                new Token(TokenType.EndMarker, 8, 4, 2)
            };

            AssertTokens(input, expected);

            // Matching indents with tabs and spaces.
            input = "\t a\n     b";
            expected = new Token[]
            {
                new Token(TokenType.Indent, 0, 1, 1),
                new Token(TokenType.Name, 2, 1, 3, "a"),
                new Token(TokenType.Newline, 3, 1, 4),
                new Token(TokenType.Name, 9, 2, 6, "b"),
                new Token(TokenType.EndMarker, 10, 2, 7)
            };

            AssertTokens(input, expected);
        }

        [Test]
        public void InvalidIndentTest()
        {
            string input = "  a\n b";
            Token[] expected =
            {
                new Token(TokenType.Indent, 0, 1, 1),
                new Token(TokenType.Name, 2, 1, 3, "a"),
                new Token(TokenType.Newline, 3, 1, 4)
            };

            var scanner = AssertTokens(input, expected);

            Assert.Throws<IndentationError>(() => scanner.GetNextToken());
        }

        [Test]
        public void FloatTest()
        {
            // Ordinary float.
            string input = "21.5";
            Token[] expected =
            {
                new Token(TokenType.FloatLiteral, 0, 1, 1, "21.5"),
                new Token(TokenType.EndMarker, 4, 1, 5)
            };

            AssertTokens(input, expected);

            // Float starting with decimal point.
            input = ".1234";
            expected = new Token[]
            {
                new Token(TokenType.FloatLiteral, 0, 1, 1, ".1234"),
                new Token(TokenType.EndMarker, 5, 1, 6)
            };

            AssertTokens(input, expected);

            // Name with float starting with decimal point.
            input = "a.1234";
            expected = new Token[]
            {
                new Token(TokenType.Name, 0, 1, 1, "a"),
                new Token(TokenType.FloatLiteral, 1, 1, 2, ".1234"),
                new Token(TokenType.EndMarker, 6, 1, 7)
            };

            AssertTokens(input, expected);
        }

        [Test]
        public void CommentTest()
        {
            // Comment on single line.
            string input = "# Hello world";
            Token[] expected =
            {
                new Token(TokenType.Comment, 0, 1, 1, "# Hello world"),
                new Token(TokenType.EndMarker, 13, 1, 14)
            };

            AssertTokens(input, expected);

            // Comment on single line ending with newline.
            input = "# Hello world\n";
            expected = new Token[]
            {
                new Token(TokenType.Comment, 0, 1, 1, "# Hello world"),
                new Token(TokenType.EndMarker, 14, 2, 1)
            };

            AssertTokens(input, expected);

            // Comment on single line with indent.
            input = "  # Hello world";
            expected = new Token[]
            {
                new Token(TokenType.Comment, 2, 1, 3, "# Hello world"),
                new Token(TokenType.EndMarker, 15, 1, 16)
            };

            AssertTokens(input, expected);

            // Comment on same line as other token and ending with newline.
            input = "a  # Hello world\n";
            expected = new Token[]
            {
                new Token(TokenType.Name, 0, 1, 1, "a"),
                new Token(TokenType.Comment, 3, 1, 4, "# Hello world"),
                new Token(TokenType.Newline, 16, 1, 17),
                new Token(TokenType.EndMarker, 17, 2, 1)
            };

            AssertTokens(input, expected);

            // Comment on next line after a line with a name token on it.
            input = "a\n# Hello world\n";
            expected = new Token[]
            {
                new Token(TokenType.Name, 0, 1, 1, "a"),
                new Token(TokenType.Newline, 1, 1, 2),
                new Token(TokenType.Comment, 2, 2, 1, "# Hello world"),
                new Token(TokenType.EndMarker, 16, 3, 1)
            };

            AssertTokens(input, expected);
        }

        [Test]
        public void NameTest()
        {
            string input = "this_is_a_name_22 1abc";
            Token[] expected =
            {
                new Token(TokenType.Name, 0, 1, 1, "this_is_a_name_22"),
                new Token(TokenType.IntegerLiteral, 18, 1, 19, "1"),
                new Token(TokenType.Name, 19, 1, 20, "abc"),
                new Token(TokenType.EndMarker, 22, 1, 23)
            };

            AssertTokens(input, expected);

        }

        [Test]
        public void OtherSymbolsTest()
        {
            string input = "()[]{}:;,.";
            Token[] expected =
            {
                new Token(TokenType.LeftParenthesis, 0, 1, 1),
                new Token(TokenType.RightParenthesis, 1, 1, 2),
                new Token(TokenType.LeftSquareBracket, 2, 1, 3),
                new Token(TokenType.RightSquareBracket, 3, 1, 4),
                new Token(TokenType.LeftBrace, 4, 1, 5),
                new Token(TokenType.RightBrace, 5, 1, 6),
                new Token(TokenType.Colon, 6, 1, 7),
                new Token(TokenType.Semicolon, 7, 1, 8),
                new Token(TokenType.Comma, 8, 1, 9),
                new Token(TokenType.Dot, 9, 1, 10),
                new Token(TokenType.EndMarker, 10, 1, 11)
            };

            AssertTokens(input, expected);
        }

        [Test]
        public void SingleLineStringTest()
        {
            // Double quoted string.
            string input = "\"Hello world\"";
            Token[] expected =
            {
                new Token(TokenType.StringLiteral, 0, 1, 1, "Hello world"),
                new Token(TokenType.EndMarker, 13, 1, 14)
            };

            AssertTokens(input, expected);

            // Single quoted string
            input = "'Hello world'";

            AssertTokens(input, expected);

            // Mixing of single and double quotes.
            input = "\"I'm certain this will work.\"";
            expected = new Token[]
            {
                new Token(TokenType.StringLiteral, 0, 1, 1, "I'm certain this will work."),
                new Token(TokenType.EndMarker, 29, 1, 30)
            };

            AssertTokens(input, expected);

            // Escaped double quotes.
            input = "'\\\"Hello world\\\"'";
            expected = new Token[]
            {
                new Token(TokenType.StringLiteral, 0, 1, 1, "\\\"Hello world\\\""),
                new Token(TokenType.EndMarker, 17, 1, 18)
            };

            AssertTokens(input, expected);

            // Escaped single quotes.
            input = "'I\\'m certain this will work.'";
            expected = new Token[]
            {
                new Token(TokenType.StringLiteral, 0, 1, 1, "I'm certain this will work."),
                new Token(TokenType.EndMarker, 30, 1, 31)
            };

            AssertTokens(input, expected);

            // String on multiple lines with escapes.
            input = "'This\\\n string\\\n is\\\n on\\\n multiple\\\n lines.'";
            expected = new Token[]
            {
                new Token(TokenType.StringLiteral, 0, 1, 1, "This string is on multiple lines."),
                new Token(TokenType.EndMarker, 45, 6, 9)
            };

            AssertTokens(input, expected);

            // String on multiple lines without escapes.
            input = "'This\n string\n is\n on\n multiple\n lines.'";
            expected = new Token[]
            {
                new Token(TokenType.Error, 0, 1, 1, "\"")
            };

            AssertTokens(input, expected);

            // Unterminated string.
            input = "'This string was left unterminated.";

            AssertTokens(input, expected);
        }

        [Test]
        public void MultilineStringTest()
        {
            // Single line using double quotes.
            string input = "\"\"\"Hello world\"\"\"";
            Token[] expected =
            {
                new Token(TokenType.StringLiteral, 0, 1, 1, "Hello world"),
                new Token(TokenType.EndMarker, 17, 1, 18)
            };

            AssertTokens(input, expected);

            // Single line using single quotes.
            input = "'''Hello world'''";

            AssertTokens(input, expected);

            // Multline using double quotes.
            input = "\"\"\"Hello\n world\"\"\"";
            expected = new Token[]
            {
                new Token(TokenType.StringLiteral, 0, 1, 1, "Hello\n world"),
                new Token(TokenType.EndMarker, 18, 2, 10)
            };

            AssertTokens(input, expected);

            // Multiline using single quotes.
            input = "'''Hello\n world'''";

            AssertTokens(input, expected);

            // String continued on next line.
            input = "'''Hello\\\n world'''";
            expected = new Token[]
            {
                new Token(TokenType.StringLiteral, 0, 1, 1, "Hello world"),
                new Token(TokenType.EndMarker, 19, 2, 10)
            };

            AssertTokens(input, expected);

            // Unterminated.
            input = "'''Hello\n world";
            expected = new Token[]
            {
                new Token(TokenType.Error, 0, 1, 1, "\"\"\"")
            };

            AssertTokens(input, expected);
        }
    }
}
