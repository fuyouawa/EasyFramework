using System;

namespace EasyFramework.Generic
{
    public enum InvalidIdentifierTypes
    {
        Empty,
        IllegalBegin,
        IllegalContent,
        CSharpKeywordConflict
    }

    public class InvalidIdentifierException : Exception
    {
        public InvalidIdentifierTypes Type;
        public string Identifier;

        public InvalidIdentifierException(InvalidIdentifierTypes type, string identifier)
        {
            Type = type;
            Identifier = identifier;
        }

        public override string Message
        {
            get
            {
                switch (Type)
                {
                    case InvalidIdentifierTypes.Empty:
                        return "Empty Identifier";
                    case InvalidIdentifierTypes.IllegalBegin:
                        return $"Identifiers must begin with a letter or an underscore, now is : {Identifier}";
                    case InvalidIdentifierTypes.IllegalContent:
                        return $"The rest of the identifier can only be letters, numbers, or underscores, now is : {Identifier}";
                    case InvalidIdentifierTypes.CSharpKeywordConflict:
                        return $"The identifier must not conflict with C# keyword, now is : {Identifier}";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    public static class StringExtension
    {
        public static bool IsNullOrEmpty(this string val)
        {
            return string.IsNullOrEmpty(val);
        }

        public static bool IsNullOrWhiteSpace(this string val)
        {
            return string.IsNullOrWhiteSpace(val);
        }

        public static bool IsNotNullOrEmpty(this string val)
        {
            return !string.IsNullOrEmpty(val);
        }
        
        public static bool IsNotNullOrWhiteSpace(this string val)
        {
            return !string.IsNullOrWhiteSpace(val);
        }

        public static string DefaultIfNullOrEmpty(this string val, string defaultValue)
        {
            return IsNullOrEmpty(val) ? defaultValue : val;
        }

        public static bool Contains(this string source, string toCheck, StringComparison comparisonType)
        {
            return source.IndexOf(toCheck, comparisonType) >= 0;
        }

        public static bool IsValidIdentifier(this string identifier, bool throwIfInvalid = false)
        {
            if (identifier.IsNullOrWhiteSpace())
            {
                if (throwIfInvalid)
                    throw new InvalidIdentifierException(InvalidIdentifierTypes.Empty, identifier);
                return false;
            }

            // 标识符必须以字母或下划线开头
            if (!char.IsLetter(identifier[0]) && identifier[0] != '_')
            {
                if (throwIfInvalid)
                    throw new InvalidIdentifierException(InvalidIdentifierTypes.IllegalBegin, identifier);
                return false;
            }

            // 标识符的其余部分只能是字母、数字或下划线
            for (int i = 1; i < identifier.Length; i++)
            {
                if (!Char.IsLetterOrDigit(identifier[i]) && identifier[i] != '_')
                {
                    if (throwIfInvalid)
                        throw new InvalidIdentifierException(InvalidIdentifierTypes.IllegalContent, identifier);
                    return false;
                }
            }

            // 检查是否是 C# 关键字
            string[] keywords = new string[]
            {
                "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", 
                "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else",
                "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", 
                "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", 
                "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", 
                "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", 
                "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", 
                "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", 
                "virtual", "void", "volatile", "while"
            };
            if (Array.Exists(keywords, keyword => keyword.Equals(identifier, StringComparison.OrdinalIgnoreCase)))
            {
                if (throwIfInvalid)
                    throw new InvalidIdentifierException(InvalidIdentifierTypes.CSharpKeywordConflict, identifier);
                return false;
            }

            return true;
        }
    }
}
