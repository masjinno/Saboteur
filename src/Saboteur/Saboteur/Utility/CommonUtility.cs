using System;
using System.Collections.Generic;
using System.Text;

namespace Saboteur.Utility
{
    class CommonUtility
    {
        /// <summary>
        /// リストを文字列型にする
        /// </summary>
        /// <typeparam name="T">文字列にするリストの型</typeparam>
        /// <param name="list">文字列にするリスト</param>
        /// <returns>文字列になったリスト</returns>
        public static string listToString<T>(IList<T> list)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            for (int index = 0; index < list.Count; index++)
            {
                sb.Append(list[index].ToString());
                if (index + 1 < list.Count)
                {
                    sb.Append(",");
                }
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
}
