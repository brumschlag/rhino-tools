namespace pipelines
{
    using System;

    [global::System.Serializable]
    public class TaskFailedException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public TaskFailedException() { }
        public TaskFailedException(string message) : base(message) { }
        public TaskFailedException(string message, Exception inner) : base(message, inner) { }
        protected TaskFailedException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}