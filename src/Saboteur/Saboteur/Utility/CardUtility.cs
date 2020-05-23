using log4net;
using Saboteur.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Saboteur.Utility
{
    class CardUtility
    {
        private static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// シャッフルする。
        /// カード以外もシャッフルできる。
        /// </summary>
        /// <typeparam name="T">シャッフルするオブジェクトの型</typeparam>
        /// <param name="cards">カードのリスト</param>
        public static void Shuffle<T>(ref IList<T> cards)
        {
            Random random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
            int index = 0;
            while (index < cards.Count)
            {
                int randomNum = random.Next(index, cards.Count);
                T nextCard = cards[randomNum];
                cards.RemoveAt(randomNum);
                cards.Insert(index, nextCard);
                index++;
            }
        }
    }
}
