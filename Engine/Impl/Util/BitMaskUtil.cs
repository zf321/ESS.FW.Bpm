namespace ESS.FW.Bpm.Engine.Impl.Util
{
    /// <summary>
    ///     Util class for manipulating bit-flag in ints.
    ///     Currently, only 8-bits are supporten, but can be extended to use all
    ///     31 bits in the integer (1st of 32 bits is used for sign).
    ///     
    /// </summary>
    public class BitMaskUtil
    {
        // First 8 masks as constant to prevent having to math.pow() every time a bit needs flippin'.
        private const int FlagBit1 = 1; // 000...00000001
        private const int FlagBit2 = 2; // 000...00000010
        private const int FlagBit3 = 4; // 000...00000100
        private const int FlagBit4 = 8; // 000...00001000
        private const int FlagBit5 = 16; // 000...00010000
        private const int FlagBit6 = 32; // 000...00100000
        private const int FlagBit7 = 64; // 000...01000000
        private const int FlagBit8 = 128; // 000...10000000

        private static readonly EngineUtilLogger Log = ProcessEngineLogger.UtilLogger;

        private static readonly int[] Masks =
        {
            FlagBit1, FlagBit2, FlagBit3, FlagBit4, FlagBit5, FlagBit6,
            FlagBit7, FlagBit8
        };

        /// <summary>
        ///     Set bit to '1' in the given int.
        /// </summary>
        /// <param name="current"> integer value </param>
        /// <param name="bitNumber"> number of the bit to set to '1' (right first bit starting at 1). </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static int setBitOn(final int value, final int bitNumber)
        public static int SetBitOn(int value, int bitNumber)
        {
            EnsureBitRange(bitNumber);
            // To turn on, OR with the correct mask
            return value | Masks[bitNumber - 1];
        }

        /// <summary>
        ///     Set bit to '0' in the given int.
        /// </summary>
        /// <param name="current"> integer value </param>
        /// <param name="bitNumber"> number of the bit to set to '0' (right first bit starting at 1). </param>
        public static int SetBitOff(int value, int bitNumber)
        {
            EnsureBitRange(bitNumber);
            // To turn on, OR with the correct mask
            return value & ~Masks[bitNumber - 1];
        }

        /// <summary>
        ///     Check if the bit is set to '1'
        /// </summary>
        /// <param name="value"> integer to check bit </param>
        /// <param name="number"> of bit to check (right first bit starting at 1) </param>
        public static bool IsBitOn(int value, int bitNumber)
        {
            EnsureBitRange(bitNumber);
            return (value & Masks[bitNumber - 1]) == Masks[bitNumber - 1];
        }

        /// <summary>
        ///     Set bit to '0' or '1' in the given int.
        /// </summary>
        /// <param name="current"> integer value </param>
        /// <param name="bitNumber"> number of the bit to set to '0' or '1' (right first bit starting at 1). </param>
        /// <param name="bitValue"> if true, bit set to '1'. If false, '0'. </param>
        public static int SetBit(int value, int bitNumber, bool bitValue)
        {
            if (bitValue)
                return SetBitOn(value, bitNumber);
            return SetBitOff(value, bitNumber);
        }

        public static int GetMaskForBit(int bitNumber)
        {
            return Masks[bitNumber - 1];
        }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private static void ensureBitRange(final int bitNumber)
        private static void EnsureBitRange(int bitNumber)
        {
            if ((bitNumber <= 0) && (bitNumber > 8))
                throw Log.InvalidBitNumber(bitNumber);
        }
    }
}