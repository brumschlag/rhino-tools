using System;
using System.Runtime.InteropServices;
using MbUnit.Framework;
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks.Tests.Callbacks
{
	[TestFixture]
	public class CallbackTests
	{
		private MockRepository mocks;
		private IDemo demo;
		private bool callbackCalled;

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
		}
		
		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository();
			demo = (IDemo) mocks.CreateMock(typeof (IDemo));
			callbackCalled = false;
		}

		[Test]
		public void CallbackIsCalled()
		{
			demo.VoidStringArg("Ayende");
			LastCall.On(demo).Callback(new DelegateDefinations.StringDelegate(StringMethod));
			mocks.Replay(demo);
			demo.VoidStringArg("");
			mocks.Verify(demo);
			Assert.IsTrue(callbackCalled);
		}

		[Test]
		public void GetSameArgumentsAsMethod()
		{
			demo.VoidThreeArgs(0, "", 0f);
			LastCall.On(demo).Callback(new DelegateDefinations.ThreeArgsDelegate(ThreeArgsAreSame));
			mocks.Replay(demo);
			demo.VoidThreeArgs(1, "Ayende", 3.14f);
			mocks.Verify(demo);
			Assert.IsTrue(callbackCalled);
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException), "Callback arguments didn't match the method arguments")]
		public void DifferentArgumentsFromMethodThrows()
		{
			demo.VoidThreeArgs(0, "", 0f);
			LastCall.On(demo).Callback(new DelegateDefinations.OtherThreeArgsDelegate(OtherThreeArgs));
		}

		[Test]
		public void IgnoreArgsWhenUsingCallbacks()
		{
			demo.VoidThreeArgs(0, "", 0f);
			LastCall.On(demo).Callback(new DelegateDefinations.ThreeArgsDelegate(ThreeArgsAreSame));
			mocks.Replay(demo);
			demo.VoidThreeArgs(1, "Ayende", 3.14f);
			mocks.Verify(demo);
		}

		[Test]
		public void SetReturnValueOnMethodWithCallback()
		{
			demo.ReturnIntNoArgs();
			LastCall.On(demo).Callback(new DelegateDefinations.NoArgsDelegate(NoArgsMethod)).Return(5);
			mocks.Replay(demo);
			Assert.AreEqual(5, demo.ReturnIntNoArgs());
			mocks.Verify(demo);
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException), "Callback arguments didn't match the method arguments")]
		public void CallbackWithDifferentSignatureFails()
		{
			demo.VoidThreeArgs(0, "", 0f);
			LastCall.On(demo).Callback(new DelegateDefinations.StringDelegate(StringMethod));
		}

		[Test]
		[ExpectedException(typeof (ExpectationViolationException), "IDemo.VoidThreeArgs(callback method: CallbackTests.ThreeArgsAreSame); Expected #1, Actual #0.")]
		public void GetMessageFromCallbackWhenNotReplaying()
		{
			demo.VoidThreeArgs(0, "", 0f);
			LastCall.On(demo).Callback(new DelegateDefinations.ThreeArgsDelegate(ThreeArgsAreSame));
			mocks.Replay(demo);
			mocks.Verify(demo);
		}

		[Test]
		[ExpectedException(typeof (ExpectationViolationException), "IDemo.VoidThreeArgs(1, \"Ayende\", 3.14); Expected #1, Actual #2.")]
		public void GetMessageFromCallbackWhenCalledTooMuch()
		{
			demo.VoidThreeArgs(0, "", 0f);
			LastCall.On(demo).Callback(new DelegateDefinations.ThreeArgsDelegate(ThreeArgsAreSame));
			mocks.Replay(demo);
			demo.VoidThreeArgs(1, "Ayende", 3.14f);
			demo.VoidThreeArgs(1, "Ayende", 3.14f);
			mocks.Verify(demo);
		}


		[Test]
		[ExpectedException(typeof (InvalidOperationException), "Previous method 'IDemo.ReturnIntNoArgs(callback method: CallbackTests.NoArgsMethod);' require a return value or an exception to throw.")]
		public void CallbackWhenMethodHasReturnValue()
		{
			demo.ReturnIntNoArgs();
			LastCall.On(demo).Callback(new DelegateDefinations.NoArgsDelegate(NoArgsMethod));
			mocks.Replay(demo);
		}


		[Test]
		[ExpectedException(typeof (InvalidOperationException), "This method has already been set to CallbackExpectation.")]
		public void CallbackAndConstraintsOnSameMethod()
		{
			demo.StringArgString("");
			LastCall.On(demo).Callback(new DelegateDefinations.StringDelegate(StringMethod))
				.Constraints(Is.Anything());
		}

		[Test]
		[ExpectedException(typeof (ExternalException), "I'm not guilty, is was /him/")]
		public void ExceptionInCallback()
		{
			demo.ReturnIntNoArgs();
			LastCall.On(demo).Callback(new DelegateDefinations.NoArgsDelegate(NoArgsThrowing)).Return(5);
			mocks.Replay(demo);
			Assert.AreEqual(5, demo.ReturnIntNoArgs());
		}

		[Test]
		[ExpectedException(typeof (ExpectationViolationException), "IDemo.VoidThreeArgs(1, \"Ayende\", 3.14); Expected #0, Actual #1.")]
		public void CallbackCanFailExpectationByReturningFalse()
		{
			demo.VoidNoArgs();
			LastCall.On(demo).Callback(new DelegateDefinations.NoArgsDelegate(NoArgsMethodFalse));
			mocks.Replay(demo);
			demo.VoidThreeArgs(1, "Ayende", 3.14f);
		}

		#region Implementation Details

		private bool StringMethod(string s)
		{
			callbackCalled = true;
			return true;
		}

		private bool OtherThreeArgs(int i, string s, string s2)
		{
			return true;
		}

		private bool ThreeArgsAreSame(int i, string s, float f)
		{
			Assert.AreEqual(1, i);
			Assert.AreEqual("Ayende", s);
			Assert.AreEqual(3.14f, f);
			callbackCalled = true;
			return true;
		}

		private bool NoArgsMethod()
		{
			return true;
		}

		private bool NoArgsMethodFalse()
		{
			return false;
		}

		private bool NoArgsThrowing()
		{
			throw new ExternalException("I'm not guilty, is was /him/");
		}

		#endregion
	}

	public class DelegateDefinations
	{
		public delegate void VoidThreeArgsDelegate(int i, string s, float f);

		public delegate bool StringDelegate(string s);

		public delegate bool ThreeArgsDelegate(int i, string s, float f);
		public delegate bool OtherThreeArgsDelegate(int i, string s, string s2);

		public delegate bool NoArgsDelegate();
		public delegate bool IntArgDelegate(int i);

	}
}
