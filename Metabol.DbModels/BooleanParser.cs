﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ILOG.Concert;
using ILOG.CPLEX;
using Exception = System.Exception;

namespace Metabol.DbModels
{
    [Flags]
    public enum Token
    {
        None = 0,
        Or,
        And,
        Var,
        //Xor,
        ClosedParenthesis,
        OpenParenthesis
    }

    public class BooleanParser
    {
        protected readonly Cplex cplex = new Cplex();
        protected readonly IEnumerator<Tuple<string, Token>> _tokens;
        protected readonly HashSet<Token> _operators = new HashSet<Token>();
        //protected ConcurrentDictionary<string, IIntVar> VarNames = new ConcurrentDictionary<string, IIntVar>();
        public ConcurrentDictionary<string, INumVar> Vars = new ConcurrentDictionary<string, INumVar>();
        public IConstraint RootVar;

        //public List<IConstraint> Constraints = new List<IConstraint>();
        //private static int _count;


        protected BooleanParser(IEnumerable<Tuple<string, Token>> tokens)
        {
            _operators.Add(Token.And);
            _operators.Add(Token.Or);

            _tokens = tokens.GetEnumerator();
            _tokens.MoveNext();
        }

        protected BooleanParser(IEnumerable<Tuple<string, Token>> tokens, Cplex model) : this(tokens)
        {
            cplex = model;
        }

        public static BooleanParser Parse(string exp)
        {
            exp = OpertorPrecedence(exp);
            var tokens = Tokenizer.Tokenize(exp);
            var parser = new BooleanParser(tokens);
            parser.cplex.EndModel();
            parser.RootVar = parser.Parse();
            return parser;
        }

        public static BooleanParser Parse(string exp, ConcurrentDictionary<string, INumVar> vars, Cplex model)
        {
            exp = OpertorPrecedence(exp);
            var tokens = Tokenizer.Tokenize(exp);
            var parser = new BooleanParser(tokens, model) { Vars = vars };
            //parser.cplex.EndModel();
            parser.RootVar = parser.Parse();
            return parser;
        }


        //makes <and> operator precede <or>
        private static string OpertorPrecedence(string exp)
        {
            var str = exp;
            str = str.Replace("(", "(((");
            str = str.Replace(")", ")))");
            str = str.Replace(" or ", ")) or ((");
            str = str.Replace(" and ", ") and (");
            //str = str.Replace(" xor ", ") xor (");

            return $"(({str}))";
        }

        protected IConstraint Parse()
        {
            while (_tokens.Current != null)
            {
                var left = ParseBoolean();

                while (_tokens.Current != null && _operators.Contains(_tokens.Current.Item2))
                {
                    var operand = _tokens.Current;
                    if (!_tokens.MoveNext())
                        throw new Exception("Missing expression after operand");

                    var right = ParseBoolean();

                    switch (operand.Item2)
                    {
                        case Token.And:
                            var and = cplex.And();
                            and.Add(left);
                            and.Add(right);
                            left = and;
                        
                            break;
                        case Token.Or:
                            var or = cplex.Or();
                            or.Add(left);
                            or.Add(right);
                            left = or;
                         
                            break;
                    }
                }

                return left;
            }

            throw new Exception("Empty expression");
        }

        protected IConstraint ParseBoolean()
        {
            switch (_tokens.Current.Item2)
            {
                case Token.Var:

                    var current = _tokens.Current;
                    _tokens.MoveNext();
                    var vv = cplex.Eq(Vars.GetOrAdd(current.Item1, cplex.BoolVar(current.Item1)), 1);
                    return vv;
                case Token.OpenParenthesis:
                    _tokens.MoveNext();

                    var expInPars = Parse();

                    if (_tokens.Current == null)//|| _tokens.Current.Item2 != Token.ClosedParenthesis
                        throw new Exception("Expecting Closing Parenthesis");

                    _tokens.MoveNext();

                    return expInPars;
                case Token.ClosedParenthesis:
                    throw new Exception("Unexpected Closing Parenthesis");
            }

            var val = Parse();
            return val;
        }


    }

    internal class Tokenizer
    {
        private readonly StringReader _reader;
        //private readonly string _text;

        private Tokenizer(string text)
        {
            //_text = text;
            _reader = new StringReader(text);
        }

        internal static IEnumerable<Tuple<string, Token>> Tokenize(string text)
        {
            return new Tokenizer(text).Tokenize();
        }

        public IEnumerable<Tuple<string, Token>> Tokenize()
        {
            var tokens = new List<Tuple<string, Token>>();
            while (_reader.Peek() != -1)
            {
                while (char.IsWhiteSpace((char)_reader.Peek()))
                {
                    _reader.Read();
                }

                if (_reader.Peek() == -1)
                    break;

                var c = (char)_reader.Peek();
                switch (c)
                {
                    case '(':
                        tokens.Add(new Tuple<string, Token>("(", Token.OpenParenthesis));
                        _reader.Read();
                        break;
                    case ')':
                        tokens.Add(new Tuple<string, Token>(")", Token.ClosedParenthesis));
                        _reader.Read();
                        break;
                    default:
                        //if (char.IsLetterOrDigit(c))
                        //{
                        var token = ParseKeyword();
                        tokens.Add(token);
                        //}
                        //else
                        //{
                        //    var remainingText = _reader.ReadToEnd() ?? string.Empty;
                        //    throw new Exception(
                        //        $"Unknown grammar found at position {_text.Length - remainingText.Length} : '{remainingText}'");
                        //}
                        break;
                }
            }
            return tokens;
        }

        private Tuple<string, Token> ParseKeyword()
        {

            var text = new StringBuilder();
            while (char.IsLetterOrDigit((char)_reader.Peek()))
            {
                text.Append((char)_reader.Read());
            }

            var keyword = text.ToString().ToLower();

            switch (keyword)
            {
                case "and":
                    return new Tuple<string, Token>(keyword, Token.And);
                case "or":
                    return new Tuple<string, Token>(keyword, Token.Or);

                //case "xor":
                //    return new Tuple<string, Token>(keyword, Token.Xor);

                default:
                    return new Tuple<string, Token>(keyword, Token.Var);
            }
        }
    }

}