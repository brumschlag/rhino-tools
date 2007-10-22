#region license

// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion


using System;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;

namespace Rhino.Commons.Binsor.Macros
{
	[CLSCompliant(false)]
	public class FactorySupportMacro : AbstractBinsorMacro
	{
		public override Statement Expand(MacroStatement macro)
		{
			MacroStatement component = macro.ParentNode.ParentNode as MacroStatement;

			if (component == null ||
				(!component.Name.Equals("component", StringComparison.InvariantCultureIgnoreCase)))
			{
				AddCompilerError(macro.LexicalInfo,
								 "A factorySupport statement can appear only under a component");
				return null;
			}

			if (!EnsureNoStatements(macro, "factorySupport"))
			{
				return null;
			}

			FactorySupportVisitor visitor = new FactorySupportVisitor();
			if (visitor.CreateExtension(macro, Errors))
			{
				RegisterExtension(component, visitor.Extension);
			}

			return null;
		}
	}

	internal class FactorySupportVisitor : DepthFirstVisitor
	{
		private bool _found;
		private MethodInvocationExpression _extension;
		private CompilerErrorCollection _compileErrors;

		public bool CreateExtension(MacroStatement macro,
		                            CompilerErrorCollection compileErrors)
		{
			_found = false;
			_compileErrors = compileErrors;

			if (macro.Arguments.Count != 1)
			{
				_compileErrors.Add(CompilerErrorFactory.CustomError(macro.LexicalInfo,
					"A factorySupport statement must be in the form @factory.<CreateMethod>[()]"));
				return false;
			}

			Visit(macro.Arguments[0]);

			return _found;
		}

		public Expression Extension
		{
			get { return _extension; }
		}

		public override void OnMemberReferenceExpression(MemberReferenceExpression node)
		{
			ReferenceExpression factory = node.Target as ReferenceExpression;
			if (factory == null || !factory.Name.StartsWith("@"))
			{
				_compileErrors.Add(CompilerErrorFactory.CustomError(
				                   	node.Target.LexicalInfo,
				                   	"The factory does not seem to reference a component"));
				return;
			}

			_extension = new MethodInvocationExpression(
				AstUtil.CreateReferenceExpression(typeof(FactorySupportExtension).FullName)
				);
			_extension.Arguments.Add(factory);
			_extension.Arguments.Add(new StringLiteralExpression(node.Name));

			_found = true;
		}

		public override void OnMethodInvocationExpression(MethodInvocationExpression node)
		{
			Visit(node.Target);
		}
	}
}