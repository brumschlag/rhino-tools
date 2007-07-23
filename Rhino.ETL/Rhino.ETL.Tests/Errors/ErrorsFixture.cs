using System;
using System.Collections.Generic;
using System.Text;
using Boo.Lang.Compiler;
using MbUnit.Framework;
using System.IO;

namespace Rhino.ETL.Tests.Errors
{
	[TestFixture]
	public class ErrorsFixture
	{
		public void Evaluate(string file, string exepctedMessage)
		{
			try
			{
				EtlContextBuilder.FromFile(Path.Combine("Errors", file));
				Assert.Fail("Expected exception to occur with message: "+exepctedMessage);
			}
			catch (Exception e)
			{
				Assert.Contains(e.Message, exepctedMessage);
			}	
		}

		[Test]
		public void CommandAsRoot()
		{
			Evaluate("command_as_root.retl",
				"A command statement can appear only under a source or destination elements");
		}

		[Test]
		public void CommandContainsNonStringExpression()
		{
			Evaluate("command_contains_non_string_expression.retl",
				"A command must contain a single string expression");
		}

		[Test]
		public void CommandMustContainsSingleExpression()
		{
			Evaluate("command_with_more_than_single_statement.retl",
				"A command must contains exactly one string expression");
		}

		[Test]
		public void ScriptThatThrows()
		{
			Evaluate("script_throw_exception.retl",
				@"script_throw_exception has thrown an exception when evaluated: Object reference not set to an instance of an object");
		}

		[Test]
		public void SourceWithNoName()
		{
			Evaluate("source_no_name.retl",
                @"SourceMacro must have a name");
		
		}

        [Test]
        public void TransformWithNoName()
        {
            Evaluate("transform_no_name.retl",
                @"TransformMacro must have a name");

        }
	}
}
