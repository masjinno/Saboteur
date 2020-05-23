using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Saboteur.Model
{
    class FieldBlock
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>置かれているカード</summary>
        public PathCard PathCard { get; private set; }

        /// <summary>スタートから到達できているか</summary>
        public bool IsReached { get; set; }

        /// <summary>
        /// 表になっているか
        /// </summary>
        public bool IsOpen { get; set; }

        public FieldBlock()
        {
            this.PathCard = null;
        }

        /// <summary>
        /// 通路カードを置く。
        /// 通路カードが既に置かれていれば、置けない。
        /// </summary>
        /// <param name="pathCard">配置する通路カード</param>
        /// <returns>
        /// 配置できたかどうか。
        /// true ⇒ 配置できた
        /// false ⇒ 配置できなかった (既にカードが置かれている)
        /// </returns>
        public bool PutPathCard(PathCard pathCard)
        {
            // 既に置かれていればreturn false
            if (this.PathCard != null)
            {
                logger.Warn("PathCard has already put.");
                return false;
            }

            this.PathCard = pathCard;
            // スタートカードもしくは通常の通路カードであれば表で置く
            this.IsOpen = (this.PathCard.StartGoal == StartGoal.Start || this.PathCard.StartGoal == StartGoal.None);
            return true;
        }

        /// <summary>
        /// 通路カードを取り除く。
        /// 通路カードが置かれていなければ、取り除けない。
        /// </summary>
        /// <returns>
        /// 取り除けたかどうか。
        /// true ⇒ 取り除けた
        /// false ⇒ 取り除けなかった
        /// </returns>
        public bool RemovePathCard()
        {
            if (this.PathCard == null)
            {
                logger.Warn("There is no PathCard.");
                return false;
            }
            this.PathCard = null;
            return true;
        }
    }
}
