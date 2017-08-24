﻿namespace Nettle.Compiler.Parsing
{
    using Nettle.Compiler.Parsing.Blocks;
    using System;

    /// <summary>
    /// Represents a function code block parser
    /// </summary>
    /// <typeparam name="T">The code block type</typeparam>
    internal abstract class NestedBlockParser<T> : NettleParser, IBlockParser<T>
        where T : NestableCodeBlock
    {
        /// <summary>
        /// Constructs the parser with a blockifier
        /// </summary>
        /// <param name="blockifier">The blockifier</param>
        protected NestedBlockParser
            (
                IBlockifier blockifier
            )
        {
            Validate.IsNotNull(blockifier);

            this.Blockifier = blockifier;
        }

        /// <summary>
        /// Gets the blockifier
        /// </summary>
        protected IBlockifier Blockifier { get; private set; }

        /// <summary>
        /// When implemented, parses the signature into a code block object
        /// </summary>
        /// <param name="templateContent">The template content</param>
        /// <param name="positionOffSet">The position offset index</param>
        /// <param name="signature">The block signature</param>
        /// <returns>The parsed code block</returns>
        public abstract T Parse
        (
            ref string templateContent,
            ref int positionOffSet,
            string signature
        );

        /// <summary>
        /// Extracts the body of a nested code block
        /// </summary>
        /// <param name="templateContent">The template content</param>
        /// <param name="positionOffSet">The position offset index</param>
        /// <param name="signature">The variable signature</param>
        /// <param name="openTagName">The open tag name</param>
        /// <param name="closeTagName">The end tag name</param>
        /// <returns>The extracted block</returns>
        protected NestableCodeBlock ExtractNestedBody
            (
                ref string templateContent,
                ref int positionOffSet,
                string signature,
                string openTagName,
                string closeTagName
            )
        {
            var startIndex = signature.Length;
            var templateLength = templateContent.Length;
            var body = String.Empty;
            var openTagCount = 0;
            var closeTagCount = 0;
            var endFound = false;

            for (int currentIndex = startIndex; currentIndex < templateLength; currentIndex++)
            {
                body += templateContent[currentIndex];

                if (body.Length > 1)
                {
                    if (body.EndsWith(@"{{" + openTagName))
                    {
                        openTagCount++;
                    }
                    else if (body.EndsWith(@"{{" + closeTagName + @"}}"))
                    {
                        closeTagCount++;
                    }
                }

                if (openTagCount > 0 && openTagCount == closeTagCount)
                {
                    //The final closing tag was found
                    endFound = true;
                    break;
                }
            }

            if (false == endFound)
            {
                throw new NettleParseException
                (
                    "No '{{{0}}}' tag was found.".With
                    (
                        closeTagName
                    ),
                    templateLength
                );
            }

            signature += body + "{{{0}}}".With(closeTagName);

            var blocks = this.Blockifier.Blockify(body);
            var startPosition = positionOffSet;
            var endPosition = signature.Length - 1;

            TrimTemplate
            (
                ref templateContent,
                ref positionOffSet,
                signature
            );

            return new NestableCodeBlock()
            {
                Signature = signature,
                StartPosition = startPosition,
                EndPosition = endPosition,
                Body = body,
                Blocks = blocks
            };
        }
    }
}