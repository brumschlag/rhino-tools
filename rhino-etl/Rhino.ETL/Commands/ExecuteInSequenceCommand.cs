using System;
using System.Collections.Generic;
using System.Text;
using Rhino.ETL.Engine;

namespace Rhino.ETL.Commands
{
	public class ExecuteInSequenceCommand : AbstractCommand, ICommandContainer
	{
		private bool started = false;
		List<ICommand> commands = new List<ICommand>();

		public ExecuteInSequenceCommand(Target target) : base(target)
		{
		}

		public IList<ICommand> Commands
		{
			get { return commands; }
		}

		public void ForceEndOfCompletionWithoutFurtherWait()
		{
			//nothing to do here, we never actually wait
		}

		public void Add(ICommand command)
		{
			commands.Add(command);
		}

		public void WaitForCompletion(TimeSpan timeOut)
		{
			if (!started)
				throw new InvalidOperationException("Called WaitForCompletion before calling Execute");
		}

		protected override void DoExecute()
		{
			started = true;
			foreach (ICommand command in commands)
			{
				command.Execute();
			}
			RaiseCompleted();
		}
	}
}
