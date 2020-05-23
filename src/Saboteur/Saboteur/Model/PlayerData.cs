using System;
using System.Collections.Generic;
using System.Text;

namespace Saboteur.Model
{
    /// <summary>
    /// プレイヤーデータ
    /// </summary>
    class PlayerData
    {
        /// <summary>プレイヤー名</summary>
        public string Name { get; set; }

        /// <summary>役割</summary>
        public Role? Role { get; set; }

        /// <summary>手札</summary>
        public IList<Card> Hand { get; private set; }

        /// <summary>得点</summary>
        public ICollection<GoldNuggetCard> Gain { get; private set; }

        /// <summary>壊された道具</summary>
        public ICollection<BrokenToolActionCard> BrokenTools { get; private set; }

        public PlayerData(string name)
        {
            this.Name = name;
            this.Role = null;
            this.Hand = new List<Card>();
            this.Gain = new List<GoldNuggetCard>();
            this.BrokenTools = new List<BrokenToolActionCard>();
        }

        /// <summary>
        /// 手札を配る
        /// </summary>
        /// <param name="cards">配るカード</param>
        public void DealHand(IList<Card> cards)
        {
            this.Hand.Clear();
            foreach (Card card in cards)
            {
                this.Hand.Add(card);
            }
        }

        /// <summary>
        /// 手札にカードを追加する
        /// </summary>
        /// <param name="card">追加するカード</param>
        public void AddHand(Card card)
        {
            this.Hand.Add(card);
        }

        /// <summary>
        /// 手札からカードを除外する
        /// </summary>
        /// <param name="card">除外するカード</param>
        /// <returns>
        /// 手札からカードを除外したか。
        /// 手札に該当カードがなければ除外できない。
        /// true⇒除外した
        /// false⇒除外できなかった
        /// </returns>
        public bool RemoveHand(Card card)
        {
            return this.Hand.Remove(card);
        }

        /// <summary>
        /// 得点をリセットする
        /// </summary>
        public void ResetGain()
        {
            this.Gain.Clear();
        }

        /// <summary>
        /// 得点を追加する
        /// </summary>
        /// <param name="goldNuggetCard">追加する金カード</param>
        public void AddGain(GoldNuggetCard goldNuggetCard)
        {
            this.Gain.Add(goldNuggetCard);
        }

        /// <summary>
        /// 得点数を取得する
        /// </summary>
        /// <returns>得点数</returns>
        public int GetTotalGain()
        {
            int sum = 0;
            foreach(GoldNuggetCard card in this.Gain)
            {
                sum += card.Num;
            }
            return sum;
        }

        /// <summary>
        /// 破壊されたカードをリセットする
        /// </summary>
        public void ResetBrokenTools()
        {
            this.BrokenTools.Clear();
        }

        /// <summary>
        /// 道具を壊す
        /// </summary>
        /// <param name="targetTool">
        /// 壊す道具。
        /// 画像情報も欲しいので、Card型で指定。
        /// </param>
        /// <returns>
        /// 壊したかどうか。
        /// 既に壊れている場合は壊せない。
        /// true⇒壊した
        /// false⇒壊せなかった
        /// </returns>
        public bool BreakTool(BrokenToolActionCard targetTool)
        {
            foreach (BrokenToolActionCard card in this.BrokenTools)
            {
                if (card.Target == targetTool.Target)
                {
                    // 既に壊されていたら破壊失敗
                    return false;
                }
            }
            this.BrokenTools.Add(targetTool);
            return true;
        }

        /// <summary>
        /// 壊された道具を修理する。
        /// </summary>
        /// <param name="targetTool">修理する道具</param>
        /// <returns>
        /// 修理したか。
        /// 壊されていない場合は修理できない。
        /// true⇒修理した
        /// false⇒修理しなかった
        /// </returns>
        public bool FixTool(Tool targetTool)
        {
            foreach (BrokenToolActionCard card in this.BrokenTools)
            {
                if (card.Target == targetTool)
                {
                    // 壊されていたら修理する(除外する)
                    return this.BrokenTools.Remove(card);
                }
            }
            return true;
        }
    }
}
