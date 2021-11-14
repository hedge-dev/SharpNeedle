using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SharpNeedle.Utilities
{
    public class BinaryHelper
    {
        public static unsafe TSize MakeSignature<TSize>(string sig, byte placeholder = 0) where TSize : unmanaged
        {
            Span<byte> result = stackalloc byte[Unsafe.SizeOf<TSize>()];
            result.Fill(placeholder);

            for (int i = 0; i < Math.Min(sig.Length, Unsafe.SizeOf<TSize>()); i++)
                result[i] = (byte)sig[i];

            return Unsafe.As<byte, TSize>(ref result[0]);
        }
    }
}
