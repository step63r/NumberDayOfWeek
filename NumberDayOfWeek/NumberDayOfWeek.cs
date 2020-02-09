using System;
using System.Globalization;
using System.Runtime.Serialization;
using Microsoft.VisualBasic;

namespace NumberDayOfWeek
{
    [Serializable]
    public class NumberDayOfWeek : IComparable, IFormattable, ISerializable, IComparable<NumberDayOfWeek>, IEquatable<NumberDayOfWeek>
    {
        /************************************************************
         *                                                          *
         * 日本の日時表記法に基づく月ごとの暦週は以下で決定される。 *
         * 　 1日～ 7日：第1                                        *
         * 　 8日～14日：第2                                        *
         * 　15日～21日：第3                                        *
         * 　22日～28日：第4                                        *
         * 　29日～    ：第5                                        * 
         * 　                                                       *
         ************************************************************/

        /// <summary>
        /// 暦週（月ごと）
        /// </summary>
        private int WeekNumber;

        /// <summary>
        /// 曜日
        /// </summary>
        private DayOfWeek DoW;

        /// <summary>
        /// メンバ変数を引数に持つコンストラクタ
        /// </summary>
        /// <param name="param1">暦週（月ごと）</param>
        /// <param name="param2">曜日</param>
        public NumberDayOfWeek(int param1, DayOfWeek param2)
        {
            WeekNumber = param1;
            DoW = param2;
        }

        /// <summary>
        /// DateTime型を引数に持つコンストラクタ
        /// </summary>
        /// <param name="param1"></param>
        public NumberDayOfWeek(DateTime param1)
        {
            WeekNumber = (int)Math.Ceiling(param1.Day / 7.0);
            DoW = param1.DayOfWeek;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(NumberDayOfWeek obj)
        {
            int ret = 1;

            // nullより大きい
            if (obj != null)
            {
                // 暦週を比較
                // 暦週が等しい場合は曜日を比較
                if (WeekNumber != obj.WeekNumber)
                {
                    ret = WeekNumber - obj.WeekNumber;
                }
                else
                {
                    // 日曜が0、土曜が6
                    ret = (int)DoW - (int)obj.DoW;
                }
            }

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            int ret = 1;

            // nullより大きい
            if (obj != null)
            {
                var other = obj as NumberDayOfWeek;

                if (other != null)
                {
                    // 暦週を比較
                    // 暦週が等しい場合は曜日を比較
                    if (WeekNumber != other.WeekNumber)
                    {
                        ret = WeekNumber - other.WeekNumber;
                    }
                    else
                    {
                        // 日曜が0、土曜が6
                        ret = (int)DoW - (int)other.DoW;
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString("G", CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public string ToString(string format)
        {
            return ToString(format, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public string ToString(string format, IFormatProvider provider)
        {
            if (string.IsNullOrEmpty(format))
            {
                format = "G";
            }

            if (provider == null)
            {
                provider = CultureInfo.CurrentCulture;
            }

            switch (format)
            {
                case "G":
                    // 1st Sunday
                    return string.Format("{0} {1}", AddOrdinal(WeekNumber), DoW.ToString());
                case "g":
                    // 1st Sun
                    return string.Format("{0} {1}", AddOrdinal(WeekNumber), DoW.ToString().Substring(0, 3));
                case "J":
                    // 第１日曜日
                    return string.Format("第{0}{1}", Strings.StrConv(WeekNumber.ToString(), VbStrConv.Wide), ConvertToJapanDoW());
                case "j":
                    // 第1日曜日
                    return string.Format("第{0}{1}", WeekNumber.ToString(), ConvertToJapanDoW());
                default:
                    throw new FormatException(string.Format("The {0} format string is not supported.", format));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("property1", WeekNumber, typeof(int));
            info.AddValue("property2", DoW, typeof(DayOfWeek));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Equals(NumberDayOfWeek obj)
        {
            return (WeekNumber == obj.WeekNumber && DoW == obj.DoW);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = obj as NumberDayOfWeek;
            return (WeekNumber == other.WeekNumber && DoW == other.DoW);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return WeekNumber + (int)DoW;
        }

        /// <summary>
        /// .net - Is there an easy way to create ordinals in C#? - Stack Overflow
        /// http://stackoverflow.com/questions/20156/is-there-an-easy-way-to-create-ordinals-in-c
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private static string AddOrdinal(int num)
        {
            if (num <= 0) return num.ToString();

            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return num + "th";
            }

            switch (num % 10)
            {
                case 1:
                    return num + "st";
                case 2:
                    return num + "nd";
                case 3:
                    return num + "rd";
                default:
                    return num + "th";
            }
        }

        /// <summary>
        /// DayOfWeek構造体から日本語の曜日名を取得する
        /// </summary>
        /// <param name="obj">DayOfWeek構造体</param>
        /// <returns>曜日の完全名</returns>
        private string ConvertToJapanDoW()
        {
            string ret = "";

            switch ((int)DoW)
            {
                case 0:
                    ret = "日曜日";
                    break;
                case 1:
                    ret = "月曜日";
                    break;
                case 2:
                    ret = "火曜日";
                    break;
                case 3:
                    ret = "水曜日";
                    break;
                case 4:
                    ret = "木曜日";
                    break;
                case 5:
                    ret = "金曜日";
                    break;
                case 6:
                    ret = "土曜日";
                    break;
            }

            return ret;
        }

    }
}
