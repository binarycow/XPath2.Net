﻿// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System;
using System.Text;

namespace Wmhelp.XPath2.Value
{
    class StringTokenizer
    {
        private readonly string _text;
        private int _offset;

        public StringTokenizer(string text)
        {
            _text = text;
        }

        public int Token { get; private set; }

        public string Value { get; private set; }

        public int LineCount { get; private set; }

        public int Offset => _offset;

        public static readonly int TokenInt = 1;        

        public override string ToString()
        {
            return _text;
        }

        public int NextToken()
        {
            int anchor = _offset;
            char ch = '\0';
            Value = String.Empty;
            while (_offset < _text.Length)
            {
                ch = _text[_offset];
                if (Char.IsWhiteSpace(ch) && ch != '\n' && ch != '\r')
                    _offset++;
                else
                    break;
            }
            switch (ch)
            {
                case '\0':
                    Token = ch;
                    break;
                
                case '\n':
                    _offset++;
                    if (_offset < _text.Length - 1 && _text[_offset] == '\r')
                        _offset++;
                    LineCount++;
                    Token = ch;
                    break;

                case '\r':
                    _offset++;
                    if (_offset < _text.Length - 1 && _text[_offset] == '\n')
                        _offset++;
                    LineCount++;
                    Token = '\n';
                    break;

                default:
                    if (Char.IsDigit(ch))
                    {
                        StringBuilder sb = new StringBuilder();
                        while (_offset < _text.Length)
                        {
                            ch = _text[_offset];
                            if (!Char.IsDigit(ch))
                                break;
                            sb.Append(ch);
                            _offset++;
                        }
                        Token = TokenInt;
                        Value = sb.ToString();
                    }
                    else
                    {
                        Token = ch;
                        _offset++;
                    }
                    break;

            }
            return Token;
        }

        public void SkipTo(char ch)
        {
            SkipTo(new char[] { ch });
        }

        public void SkipTo(char[] charset)
        {
            SkipTo(new String(charset));
        }

        public void SkipTo(string charset)
        {
            int anchor = _offset;
            while (Token != 0 && charset.IndexOf((char)Token) == -1)
                NextToken();
            Value = _text.Substring(anchor, _offset - anchor);
        }

        public string SkipToEOL()
        {
            if (_offset == _text.Length)
                return String.Empty;
            int anchor = _offset;
            while (_offset < _text.Length)
            {
                char ch = _text[_offset];
                if (ch == '\n')
                {
                    _offset++;
                    if (_offset < _text.Length && _text[_offset] == '\r')
                        _offset++;
                    LineCount++;
                    Token = '\n';
                    return _text.Substring(anchor, _offset - anchor);
                }
                else if (ch == '\r')
                {
                    _offset++;
                    if (_offset < _text.Length && _text[_offset] == '\n')
                        _offset++;
                    LineCount++;
                    Token = '\n';
                    return _text.Substring(anchor, _offset - anchor);
                }
                else
                    _offset++;
            }
            return _text.Substring(anchor);
        }
    }
}
