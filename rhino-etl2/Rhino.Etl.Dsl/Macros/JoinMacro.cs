namespace Rhino.Etl.Dsl.Macros
{
    using System.Collections.Generic;
    using Boo.Lang.Compiler.Ast;
    using Rhino.Etl.Core.Operations;

    /// <summary>
    /// Create a class based on <see cref="JoinOperation"/> and tranform the code
    /// into a join condition
    /// </summary>
    public class JoinMacro : AbstractClassGeneratorMacro<JoinOperation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JoinMacro"/> class.
        /// </summary>
        public JoinMacro()
            : base("Initialize")
        {
        }

        /// <summary>
        /// Expands the specified macro
        /// </summary>
        /// <param name="macro">The macro.</param>
        /// <returns></returns>
        public override Statement Expand(MacroStatement macro)
        {
            Statement expand = base.Expand(macro);
            return expand;
        }
    }
}