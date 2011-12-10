using System;
using System.Text;
using System.Text.RegularExpressions;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.Pathing;
using Styx.Logic.POI;
using System.Linq;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Logic.BehaviorTree;
using System.Globalization;
using ObjectManager = Styx.WoWInternals.ObjectManager;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

namespace HighVoltz
{
    /// <summary>
    /// Utility functions
    /// </summary>
    public static class Util
    {
        static Util()
        {
            IsBankFrameOpen = false;
        }
        /// <summary>
        ///  Random Number Genorator
        /// </summary>
        public static System.Random Rng = new Random(Environment.TickCount);
        /// <summary>
        /// Creates a random upper/lowercase string
        /// </summary>
        /// <returns>Random String</returns>
        public static string RandomString
        {
            get
            {
                int size = Rng.Next(6, 15);
                StringBuilder sb = new StringBuilder(size);
                for (int i = 0; i < size; i++)
                {
                    // random upper/lowercase character using ascii code
                    sb.Append((char)(Rng.Next(2) == 1 ? Rng.Next(65, 91) + 32 : Rng.Next(65, 91)));
                }
                return sb.ToString();
            }
        }

        static WoWPoint _lastPoint = WoWPoint.Zero;
        static DateTime _lastMove = DateTime.Now;
        public static void MoveTo(WoWPoint point)
        {
            if (BotPoi.Current.Type != PoiType.None)
                BotPoi.Clear();
            if (!ObjectManager.Me.Mounted && Mount.ShouldMount(point) && Mount.CanMount())
                Mount.MountUp();
            TreeRoot.StatusText = string.Format("PB: Moving to {0}", point);
            _lastPoint = point;
            _lastMove = DateTime.Now;
            Navigator.MoveTo(point);
        }

        public static WoWPoint GetMoveToDestination()
        {
            if (DateTime.Now.Subtract(_lastMove).TotalSeconds < 4 && _lastPoint != WoWPoint.Zero)
                return _lastPoint;
            else
                return ObjectManager.Me.Location;
        }

        /// <summary>
        /// Converts a string of 3 numbers to a WoWPoint.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        static public WoWPoint StringToWoWPoint(string location)
        {
            WoWPoint loc = WoWPoint.Zero;
            Regex pattern = new Regex(@"-?\d+\.?(\d+)?", RegexOptions.CultureInvariant);
            MatchCollection matches = pattern.Matches(location);
            if (matches != null)
            {
                loc.X = matches[0].ToString().ToSingle();
                loc.Y = matches[1].ToString().ToSingle();
                loc.Z = matches[2].ToString().ToSingle();
            }
            return loc;
        }
        /// <summary>
        ///  Returns number items with a matching id that player has in personal bank
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public static uint GetBankItemCount(uint itemID)
        {
            try
            {
                return (uint)ObjectManager.GetObjectsOfType<WoWItem>().
                    Sum(i => i != null && i.IsValid && i.Entry == itemID ? i.StackCount : 0) - GetCarriedItemCount(itemID);
            }
            catch { return 0; }
        }
        /// <summary>
        /// Returns number items with a matching id that player is carrying
        /// </summary>
        /// <param name="id">Item ID</param>
        /// <returns>Number of items in player Inventory</returns>
        public static uint GetCarriedItemCount(uint id)
        {
            return (uint)ObjectManager.Me.CarriedItems.Sum(i => i != null && i.IsValid && i.Entry == id ? i.StackCount : 0);
        }
        // this factors in the material list
        public static int CalculateRecipeRepeat(Recipe recipe)
        {
            int ret = int.MaxValue;
            foreach (Ingredient ingred in recipe.Ingredients)
            {
                int ingredCnt = (int)ingred.InBagItemCount -
                                (Professionbuddy.Instance.MaterialList.ContainsKey(ingred.ID)
                                     ? Professionbuddy.Instance.MaterialList[ingred.ID]
                                     : 0);
                int repeat = (int)System.Math.Floor((double)(ingredCnt / ingred.Required));
                if (ret > repeat)
                {
                    ret = repeat;
                }
            }
            return ret;
        }
        public static bool IsBankFrameOpen { get; private set; }

        static internal void OnBankFrameOpened(object obj, LuaEventArgs args)
        {
            IsBankFrameOpen = true;
        }

        static internal void OnBankFrameClosed(object obj, LuaEventArgs args)
        {
            IsBankFrameOpen = false;
        }


        static uint _ping = Lua.GetReturnVal<uint>("return GetNetStats()", 3);
        static Stopwatch _pingSW = new Stopwatch();
        /// <summary>
        /// Returns WoW's ping, refreshed every 30 seconds.
        /// </summary>
        static public uint WoWPing
        {
            get
            {
                if (!_pingSW.IsRunning)
                    _pingSW.Start();
                if (_pingSW.ElapsedMilliseconds > 30000)
                {
                    _ping = Lua.GetReturnVal<uint>("return GetNetStats()", 3);
                    _pingSW.Reset();
                    _pingSW.Start();
                }
                return _ping;
            }
        }
        const int _cacheSize = 0x500;
        /// <summary>
        /// Looks for a pattern in WoW's memory and returns the offset of pattern if found otherwise an InvalidDataException is thrown
        /// </summary>
        /// <param name="pattern">the pattern to look for, in space delimited hex string format e.g. "DE AD BE EF" </param>
        /// <param name="mask">the mask specifies what bytes in pattern to ignore, The '?' character means ignore the byte, anthing else is not ignored</param>
        /// <returns>The offset the first match of the pattern was found at.</returns>
        static public uint FindPattern(string pattern, string mask)
        {
            byte[] patternArray = HexStringToByteArray(pattern);
            bool[] maskArray = MaskStringToBoolArray(mask);
            ProcessModule wowModule = ObjectManager.WoWProcess.MainModule;
            uint start = (uint)wowModule.BaseAddress.ToInt32();
            int size = wowModule.ModuleMemorySize;
            var patternLength = mask.Length;

            for (uint cacheOffset = 0; cacheOffset < size; cacheOffset += (uint)(_cacheSize - patternLength))
            {
                byte[] cache = ObjectManager.Wow.ReadBytes((uint)start + cacheOffset, _cacheSize > size - cacheOffset ? size - (int)cacheOffset : _cacheSize);
                for (uint cacheIndex = 0; cacheIndex < (cache.Length - patternLength); cacheIndex++)
                {
                    if (DataCompare(cache, cacheIndex, patternArray, maskArray))
                        return cacheOffset + cacheIndex;
                }
            }
            throw new InvalidDataException("Pattern not found");
        }

        static byte[] HexStringToByteArray(string hexString)
        {
            return hexString.Split(' ')
                .Aggregate(new List<byte>(), (a, b) => { a.Add(byte.Parse(b, NumberStyles.HexNumber)); return a; })
                .ToArray();
        }

        static bool[] MaskStringToBoolArray(string mask)
        {
            return mask.Aggregate(new List<bool>(), (a, b) => { a.Add(b == '?' ? false : true); return a; }).ToArray();
        }

        static bool DataCompare(byte[] data, uint dataOffset, byte[] pattern, bool[] mask)
        {
            return !mask.Where((t, i) => t && pattern[i] != data[dataOffset + i]).Any();
        }
    }
    static class Exts
    {
        public static uint ToUint(this string str)
        {
            uint val;
            uint.TryParse(str, out val);
            return val;
        }

        static Encoding _encodeUTF8 = Encoding.UTF8;

        /// <summary>
        /// Converts a string to a float using En-US based culture
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static float ToSingle(this string str)
        {
            float val;
            float.TryParse(str, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign
            , CultureInfo.InvariantCulture, out val);
            return val;
        }

        /// <summary>
        /// Converts a string to a formated UTF-8 string using \ddd format where ddd is a 3 digit number. Useful when importing names into lua that are UTF-16 or higher.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToFormatedUTF8(this string text)
        {
            StringBuilder buffer = new StringBuilder(_encodeUTF8.GetByteCount(text));
            byte[] utf8Encoded = _encodeUTF8.GetBytes(text);
            foreach (byte b in utf8Encoded)
            {
                buffer.Append(string.Format("\\{0:D3}", b));
            }
            return buffer.ToString();
        }
        /// <summary>
        /// This is a fix for WoWPoint.ToString using current cultures decimal separator.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToInvariantString(this WoWPoint text)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0},{1},{2}", text.X, text.Y, text.Z);
        }
    }
}
