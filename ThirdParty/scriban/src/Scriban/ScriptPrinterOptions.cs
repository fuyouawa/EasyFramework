// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using EasyToolKit.ThirdParty.Scriban.Parsing;
using EasyToolKit.ThirdParty.Scriban.Syntax;

namespace EasyToolKit.ThirdParty.Scriban
{
    /// <summary>
    /// Defines the options used for rendering back an AST/<see cref="ScriptNode"/> to a text.
    /// </summary>
#if SCRIBAN_PUBLIC
    public
#else
    internal
#endif
    struct ScriptPrinterOptions
    {
        /// <summary>
        /// The mode used to render back an AST
        /// </summary>
        public ScriptMode Mode;
    }
}