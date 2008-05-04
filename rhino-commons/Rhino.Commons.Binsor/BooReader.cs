﻿#region license

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
using System.Collections.Generic;
using System.IO;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Pipelines;
using Boo.Lang.Compiler.Steps;
using Boo.Lang.Extensions;
using Boo.Lang.Parser;
using Castle.MicroKernel;
using Castle.MicroKernel.SubSystems.Resource;
using Castle.Core.Resource;
using Castle.Windsor;

namespace Rhino.Commons.Binsor
{
	using System.Reflection;
	using DSL;

	public static class BooReader
	{
		private static readonly BooToken tokenThatIsNeededToKeepReferenceToTheBooParserAssembly = new BooToken();

		internal static ICollection<INeedSecondPassRegistration> NeedSecondPassRegistrations
		{
			get
			{
				ICollection<INeedSecondPassRegistration> data = (ICollection<INeedSecondPassRegistration>)
				                                                Local.Data[tokenThatIsNeededToKeepReferenceToTheBooParserAssembly];
				if (data == null)
				{
					Local.Data[tokenThatIsNeededToKeepReferenceToTheBooParserAssembly] =
						data = new List<INeedSecondPassRegistration>();
				}
				return data;
			}
			set { Local.Data[tokenThatIsNeededToKeepReferenceToTheBooParserAssembly] = value; }
		}

		public static Component GetComponentByName(string name)
		{
			foreach (INeedSecondPassRegistration secondPassRegistration in NeedSecondPassRegistrations)
			{
				Component component = secondPassRegistration as Component;
				if (component == null)
					continue;
				if (component.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
					return component;
			}
			throw new InvalidOperationException("Could not find component named: " + name);
		}

		public static void Read(IWindsorContainer container, string fileName)
		{
			Read(container, fileName, "");
		}

		public static void Read(IWindsorContainer container, string fileName, string environment)
		{
			Read(container, fileName, environment, GenerationOptions.Memory);
		}

		public static void Read(
			IWindsorContainer container, string fileName, string environment,
			GenerationOptions generationOptions)
		{
			try
			{
				Execute(container, GetConfigurationInstanceFromFile(
				                   	fileName, environment, container, generationOptions));
			}
			finally
			{
				NeedSecondPassRegistrations = null;
			}
		}

		public static void Execute(IWindsorContainer container, AbstractConfigurationRunner abstractConfiguration)
		{
			try
			{
				using(AbstractConfigurationRunner.UseLocalContainer(container))
				{
					abstractConfiguration.Run();
					foreach(INeedSecondPassRegistration needSecondPassRegistration in NeedSecondPassRegistrations)
					{
						needSecondPassRegistration.RegisterSecondPass();
					}
				}
			}
			finally
			{
				NeedSecondPassRegistrations = null;
			}
		}

		public static void Read(IWindsorContainer container, Stream stream, string name)
		{
			Read(container, stream, name, "");
		}

		public static void Read(
			IWindsorContainer container, Stream stream,
			string name, string environment)
		{
			Read(container, stream, GenerationOptions.Memory, name, environment);
		}

		public static void Read(
			IWindsorContainer container, Stream stream,
			GenerationOptions generationOptions, string name,
			string environment)
		{
			try
			{
				using (AbstractConfigurationRunner.UseLocalContainer(container))
				{
					AbstractConfigurationRunner conf = GetConfigurationInstanceFromStream(
						name, environment, container, stream, generationOptions);
					conf.Run();
					foreach (INeedSecondPassRegistration needSecondPassRegistration in NeedSecondPassRegistrations)
					{
						needSecondPassRegistration.RegisterSecondPass();
					}
				}
			}
			finally
			{
				NeedSecondPassRegistrations = null;
			}
		}

		public static AbstractConfigurationRunner GetConfigurationInstanceFromFile(
			string fileName, string environment, IWindsorContainer container,
			GenerationOptions generationOptions)
		{
			string baseDirectory = Path.GetDirectoryName(fileName);
			UrlResolverDelegate urlResolver = CreateWindorUrlResolver(container);
			using (TextReader reader = urlResolver(fileName, null))
			{
				return GetConfigurationInstance(
					Path.GetFileNameWithoutExtension(fileName), environment,
					new ReaderInput(Path.GetFileNameWithoutExtension(fileName), reader),
					generationOptions,
					new AutoReferenceFilesCompilerStep(baseDirectory, urlResolver));
			}
		}

		public static AbstractConfigurationRunner GetConfigurationInstanceFromStream(
			string name, string environment, IWindsorContainer container, Stream stream,
			GenerationOptions generationOptions)
		{
			UrlResolverDelegate urlResolver = CreateWindorUrlResolver(container);
			using (StreamReader reader = new StreamReader(stream))
			{
				return GetConfigurationInstance(
					name, environment, new ReaderInput(name, reader),
					generationOptions, new AutoReferenceFilesCompilerStep(urlResolver));
			}
		}

		private static AbstractConfigurationRunner GetConfigurationInstance(
			string name, string environment, ICompilerInput input,
			GenerationOptions generationOptions,
			ICompilerStep autoReferenceStep)
		{
			BooCompiler compiler = new BooCompiler();
			compiler.Parameters.Ducky = true;
			if (generationOptions == GenerationOptions.Memory)
				compiler.Parameters.Pipeline = new CompileToMemory();
			else
				compiler.Parameters.Pipeline = new CompileToFile();

			compiler.Parameters.Pipeline.Insert(1, autoReferenceStep);
			compiler.Parameters.Pipeline.Insert(2, new BinsorCompilerStep(environment));
			compiler.Parameters.Pipeline.Replace(
				typeof (ProcessMethodBodiesWithDuckTyping),
				new TransformUnknownReferences());
			compiler.Parameters.Pipeline.InsertAfter(typeof (TransformUnknownReferences),
			                                         new RegisterComponentAndFacilitiesAfterCreation());

			compiler.Parameters.OutputType = CompilerOutputType.Library;
			compiler.Parameters.Input.Add(input);
			compiler.Parameters.References.Add(typeof (BooReader).Assembly);
			compiler.Parameters.References.Add(typeof (MacroMacro).Assembly);

			TryAddAssembliesReferences(compiler.Parameters, "Rhino.Commons.NHibernate", "Rhino.Commons.ActiveRecord");

			CompilerContext run = compiler.Run();
			if (run.Errors.Count != 0)
			{
				throw new CompilerError(string.Format("Could not compile configuration! {0}", run.Errors.ToString(true)));
			}
			Type type = run.GeneratedAssembly.GetType(name);
			return Activator.CreateInstance(type) as AbstractConfigurationRunner;
		}

		/// <summary>
		/// Tries the add the speicifed assemblies to the compiler's references
		/// In a late bound way
		/// </summary>
		/// <param name="cp">The compiler parameters.</param>
		/// <param name="assembliesToAdd">The assemblies to add.</param>
		private static void TryAddAssembliesReferences(CompilerParameters cp, params string[] assembliesToAdd)
		{
			foreach (string assembly in assembliesToAdd)
			{
				try
				{
					cp.References.Add(Assembly.Load(assembly));
				}
				catch
				{
					// we don't worry if we can't load the assemblies
				}
			}
		}

		private static UrlResolverDelegate CreateWindorUrlResolver(IWindsorContainer container)
		{
			IResourceSubSystem subSystem = (IResourceSubSystem)
			                               container.Kernel.GetSubSystem(SubSystemConstants.ResourceKey);

			return delegate(string url, string basePath)
			{
				IResource resource;

				if (url.IndexOf(':') < 0)
				{
					url = "file://" + url;
				}

				if (!string.IsNullOrEmpty(basePath))
				{
					resource = subSystem.CreateResource(url, basePath);
				}
				else
				{
					resource = subSystem.CreateResource(url);
				}
				using (resource)
				using (TextReader reader = resource.GetStreamReader())
					return new StringReader(reader.ReadToEnd());
			};
		}

		public enum GenerationOptions
		{
			Memory,
			File
		}
	}
}