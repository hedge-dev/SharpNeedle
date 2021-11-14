using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amicitia.IO.Binary;
using Amicitia.IO.Binary.Extensions;
using Amicitia.IO.Streams;
using SharpNeedle.IO;

namespace SharpNeedle.HedgehogEngine.Mirage
{
    public abstract class SampleChunkResource : ResourceBase
    {
        protected uint FileSize { get; set; }
        public uint DataVersion { get; set; }
        protected uint DataSize { get; set; }

        protected abstract void Read(BinaryObjectReader reader);
        protected abstract void Write(BinaryObjectWriter reader);

        // ReSharper disable AccessToDisposedClosure
        public override void Read(IFile file)
        {
            BaseFile = file;
            Name = Path.GetFileNameWithoutExtension(file.Name);

            using var reader = new BinaryObjectReader(file.Open(), StreamOwnership.Retain, Endianness.Big);
            FileSize = reader.Read<uint>();
            DataVersion = reader.Read<uint>();
            DataSize = reader.Read<uint>();

            reader.ReadOffset(() =>
            {
                reader.PushOffsetOrigin();
                
                Read(reader);

                reader.PopOffsetOrigin();
            });

            reader.ReadOffsetValue();
            try
            {
                reader.ReadOffset(() => reader.ReadString(StringBinaryFormat.NullTerminated));
            }catch{}
        }

        // ReSharper disable AccessToDisposedClosure
        public override void Write(IFile file)
        {
            using var writer = new BinaryObjectWriter(file.Open(FileAccess.Write), StreamOwnership.Retain, Endianness.Big);

            var beginToken = writer.At(0, SeekOrigin.Current);
            
            writer.Write(0); // File Size
            writer.Write(DataVersion);

            var dataToken = writer.At(0, SeekOrigin.Current);
            writer.Write(0); // Data Size

            {
                writer.WriteOffset(() =>
                {
                    var start = writer.Position;
                    Write(writer);
                    var size = writer.Position - start;
                    using var token = writer.At(0, SeekOrigin.Current);

                    dataToken.Dispose();
                    writer.Write((uint)size);
                });

                writer.WriteOffset(() =>
                {
                    var offsets = writer.OffsetHandler.OffsetPositions.ToArray().AsMemory(1);
                    writer.Write(offsets.Length);
                    
                    foreach (var o in offsets.Span)
                        writer.Write((uint)o);
                }, 4);
                
                writer.WriteStringOffset(StringBinaryFormat.NullTerminated, file.Name, alignment: 8);
                writer.Flush();
            }
            using var end = writer.At(0, SeekOrigin.Current);
            
            beginToken.Dispose();
            writer.Write((uint)writer.Length);

            dataToken.Dispose();
        }
    }
}
