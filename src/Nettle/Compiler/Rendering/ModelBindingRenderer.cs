﻿namespace Nettle.Compiler.Rendering
{
    using Nettle.Compiler.Parsing.Blocks;
    using Nettle.Functions;

    /// <summary>
    /// Represents a model binding renderer
    /// </summary>
    internal class ModelBindingRenderer : NettleRendererBase, IBlockRenderer
    {
        /// <summary>
        /// Constructs the renderer with required dependencies
        /// </summary>
        /// <param name="functionRepository">The function repository</param>
        public ModelBindingRenderer
            (
                IFunctionRepository functionRepository
            )

            : base(functionRepository)
        { }

        /// <summary>
        /// Determines if the renderer can render the code block specified
        /// </summary>
        /// <param name="block">The code block</param>
        /// <returns>True, if it can be rendered; otherwise false</returns>
        public bool CanRender
            (
                CodeBlock block
            )
        {
            Validate.IsNotNull(block);

            var blockType = block.GetType();

            return
            (
                blockType == typeof(ModelBinding)
            );
        }

        /// <summary>
        /// Renders the code block specified into a string
        /// </summary>
        /// <param name="context">The template context</param>
        /// <param name="block">The code block to render</param>
        /// <param name="flags">The template flags</param>
        /// <returns>The rendered block</returns>
        public string Render
            (
                ref TemplateContext context,
                CodeBlock block,
                params TemplateFlag[] flags
            )
        {
            Validate.IsNotNull(block);

            var binding = (ModelBinding)block;

            var value = ResolveBindingValue
            (
                ref context,
                binding.BindingPath
            );

            return ToString(value, flags);
        }
    }
}
