using System;
using System.Runtime.CompilerServices;

namespace SharpNeedle.Utilities
{
    public class BinaryHelper
    {
        public static unsafe uint MakeSignature(string sig)
        {
            Span<byte> result = stackalloc byte[4];
            Unsafe.AsRef<uint>(Unsafe.AsPointer(ref result[0])) = 0;

            for (int i = 0; i < Math.Min(sig.Length, 4); i++)
                result[i] = (byte)sig[i];

            return Unsafe.As<byte, uint>(ref result[0]);
        }
    }
}
