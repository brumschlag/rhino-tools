using System;

namespace Rhino.DHT
{
    public class Value
    {
        public DateTime? ExpiresAt { get; set; }
        public string Key { get; set; }
        public int Version { get; set; }
        public int[] ParentVersions { get; set; }
        public byte[] Data { get; set; }
        public byte[] Sha256Hash { get; set; }
    }
}