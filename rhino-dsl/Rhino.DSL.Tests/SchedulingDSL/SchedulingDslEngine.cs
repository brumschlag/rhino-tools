namespace Rhino.DSL.Tests.SchedulingDSL
{
    using System;
    using Boo.Lang.Compiler;

    public class SchedulingDslEngine : DslEngine
    {
        protected override void CustomizeCompiler(BooCompiler compiler, CompilerPipeline pipeline, Uri[] urls)
        {
            pipeline.Insert(1, new AnonymousBaseClassCompilerStep(typeof (BaseScheduler), "Prepare",
                                                                  "Rhino.DSL.Tests.SchedulingDSL"));
        }
    }
}