using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Saboteur.Model
{
    class CardSetting
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // NSEW: 道がある方角。N:North, S:South, E:East, W:West
        // THROUGHABLE: 通り抜け可能
        // BLOCKED: 塞がっている
        // PATH: 通路カード
        private const int PATH_NSEW_THROUGHABLE_NUM = 5;
        private const int PATH_NSEW_BLOCKED_NUM = 1;
        private const int PATH_NSW_THROUGHABLE_NUM = 5;
        private const int PATH_NSW_BLOCKED_NUM = 1;
        private const int PATH_NEW_THROUGHABLE_NUM = 5;
        private const int PATH_NEW_BLOCKED_NUM = 1;
        private const int PATH_NS_THROUGHABLE_NUM = 4;
        private const int PATH_NS_BLOCKED_NUM = 1;
        private const int PATH_EW_THROUGHABLE_NUM = 3;
        private const int PATH_EW_BLOCKED_NUM = 1;
        private const int PATH_SW_THROUGHABLE_NUM = 5;
        private const int PATH_SW_BLOCKED_NUM = 1;
        private const int PATH_NW_THROUGHABLE_NUM = 4;
        private const int PATH_NW_BLOCKED_NUM = 1;
        private const int PATH_S_BLOCKED_NUM = 1;
        private const int PATH_W_BLOCKED_NUM = 1;

        // スタートカード
        private const int PATH_NSEW_START_NUM = 1;
        // ゴールカード
        private const int PATH_NSEW_TREASURE_NUM = 1;
        private const int PATH_NW_STONE_NUM = 1;
        private const int PATH_SW_STONE_NUM = 1;

        // アクションカード
        private const int BROKEN_PICK_NUM = 3;
        private const int FIX_PICK_NUM = 2;
        private const int BROKEN_LANTERN_NUM = 3;
        private const int FIX_LANTERN_NUM = 2;
        private const int BROKEN_MINECART_NUM = 3;
        private const int FIX_MINECART_NUM = 2;
        private const int FIX_PICK_LANTERN_NUM = 1;
        private const int FIX_PICK_MINECART_NUM = 1;
        private const int FIX_LANTERN_MINECART_NUM = 1;
        private const int ROCKFALL_NUM = 3;
        private const int TREASUREMAP_NUM = 6;

        // 金カード
        private const int THREE_GOLD_NUGGETS_NUM = 4;
        private const int TWO_GOLD_NUGGETS_NUM = 8;
        private const int ONE_GOLD_NUGGET_NUM = 16;

        // ソート用インデックス
        private int sortIndex;

        public CardSetting()
        {
            this.sortIndex = 0;
        }

        /// <summary>
        /// デッキ用のカードを作成する
        /// </summary>
        /// <returns>デッキ用のカード一覧</returns>
        public ICollection<Card> CreateAllDeckCards()
        {
            ICollection<PathCard> pathCards = this.CreatePathCards();
            ICollection<ActionCard> actionCards = this.CreateActionCards();
            List<Card> allCards = new List<Card>();
            allCards.AddRange(pathCards);
            allCards.AddRange(actionCards);
            return allCards;
        }

        /// <summary>
        /// 全ての金カードを作成する
        /// </summary>
        /// <returns>作成した金カードの一覧</returns>
        public ICollection<GoldNuggetCard> CreateAllGoldNuggetCards()
        {
            List<GoldNuggetCard> allGoldNuggetCards = new List<GoldNuggetCard>();
            allGoldNuggetCards.AddRange(this.CreateGoldNuggetCards(3, THREE_GOLD_NUGGETS_NUM));
            allGoldNuggetCards.AddRange(this.CreateGoldNuggetCards(2, TWO_GOLD_NUGGETS_NUM));
            allGoldNuggetCards.AddRange(this.CreateGoldNuggetCards(1, ONE_GOLD_NUGGET_NUM));
            return allGoldNuggetCards;
        }

        /// <summary>
        /// 全ての通路カードを作成する
        /// </summary>
        /// <returns>作成した通路カードの一覧</returns>
        private ICollection<PathCard> CreatePathCards()
        {
            List<PathCard> pathCards = new List<PathCard>();
            // 基本の通路カード
            pathCards.AddRange(this.CreatePathCards(new bool[] { true, true, true, true }, true, StartGoal.None, PATH_NSEW_THROUGHABLE_NUM));
            pathCards.AddRange(this.CreatePathCards(new bool[] { true, true, true, true }, false, StartGoal.None, PATH_NSEW_BLOCKED_NUM));
            pathCards.AddRange(this.CreatePathCards(new bool[] { true, true, false, true }, true, StartGoal.None, PATH_NSW_THROUGHABLE_NUM));
            pathCards.AddRange(this.CreatePathCards(new bool[] { true, true, false, true }, false, StartGoal.None, PATH_NSW_BLOCKED_NUM));
            pathCards.AddRange(this.CreatePathCards(new bool[] { true, false, true, true }, true, StartGoal.None, PATH_NEW_THROUGHABLE_NUM));
            pathCards.AddRange(this.CreatePathCards(new bool[] { true, false, true, true }, false, StartGoal.None, PATH_NEW_BLOCKED_NUM));
            pathCards.AddRange(this.CreatePathCards(new bool[] { true, true, false, false }, true, StartGoal.None, PATH_NS_THROUGHABLE_NUM));
            pathCards.AddRange(this.CreatePathCards(new bool[] { true, true, false, false }, false, StartGoal.None, PATH_NS_BLOCKED_NUM));
            pathCards.AddRange(this.CreatePathCards(new bool[] { false, false, true, true }, true, StartGoal.None, PATH_EW_THROUGHABLE_NUM));
            pathCards.AddRange(this.CreatePathCards(new bool[] { false, false, true, true }, false, StartGoal.None, PATH_EW_BLOCKED_NUM));
            pathCards.AddRange(this.CreatePathCards(new bool[] { false, true, false, true }, true, StartGoal.None, PATH_SW_THROUGHABLE_NUM));
            pathCards.AddRange(this.CreatePathCards(new bool[] { false, true, false, true }, false, StartGoal.None, PATH_SW_BLOCKED_NUM));
            pathCards.AddRange(this.CreatePathCards(new bool[] { true, false, false, true }, true, StartGoal.None, PATH_NW_THROUGHABLE_NUM));
            pathCards.AddRange(this.CreatePathCards(new bool[] { true, false, false, true }, false, StartGoal.None, PATH_NW_BLOCKED_NUM));
            pathCards.AddRange(this.CreatePathCards(new bool[] { false, true, false, false }, false, StartGoal.None, PATH_S_BLOCKED_NUM));
            pathCards.AddRange(this.CreatePathCards(new bool[] { false, false, false, true }, false, StartGoal.None, PATH_W_BLOCKED_NUM));
            // スタートカードとゴールカード
            pathCards.AddRange(this.CreatePathCards(new bool[] { true, true, true, true }, true, StartGoal.Start, PATH_NSEW_START_NUM));
            pathCards.AddRange(this.CreatePathCards(new bool[] { true, true, true, true }, true, StartGoal.Treasure, PATH_NSEW_TREASURE_NUM));
            pathCards.AddRange(this.CreatePathCards(new bool[] { true, false, false, true }, true, StartGoal.Stone, PATH_NW_STONE_NUM));
            pathCards.AddRange(this.CreatePathCards(new bool[] { false, true, false, true }, true, StartGoal.Stone, PATH_SW_STONE_NUM));
            return pathCards;
        }

        /// <summary>
        /// 通路カードを作成する
        /// </summary>
        /// <param name="north">北側に穴が開いているか</param>
        /// <param name="south">南側に穴が開いているか</param>
        /// <param name="east">東側に穴が開いているか</param>
        /// <param name="west">西側に穴が開いているか</param>
        /// <param name="through">通り抜けられるか</param>
        /// <param name="startGoal">スタート・ゴールのマークは何か</param>
        /// <param name="num">作る枚数</param>
        /// <returns>作成した通路カードの一覧</returns>
        private ICollection<PathCard> CreatePathCards(bool[] hole, bool through, StartGoal startGoal, int num)
        {
            ICollection<PathCard> ret = new HashSet<PathCard>();
            for (int index = 0; index < num; index++)
            {
                ret.Add(new PathCard(this.sortIndex++, hole, through, startGoal));
            }
            return ret;
        }

        /// <summary>
        /// アクションカードを作成する
        /// </summary>
        /// <returns>アクションカード</returns>
        private ICollection<ActionCard> CreateActionCards()
        {
            List<ActionCard> actionCards = new List<ActionCard>();
            // 道具破壊・修理カード
            actionCards.AddRange(this.CreateBrokenToolActionCards(Tool.Pick, BROKEN_PICK_NUM));
            actionCards.AddRange(this.CreateBrokenToolActionCards(Tool.Lantern, BROKEN_LANTERN_NUM));
            actionCards.AddRange(this.CreateBrokenToolActionCards(Tool.MineCart, BROKEN_MINECART_NUM));
            actionCards.AddRange(this.CreateFixToolActionCards(new Tool[] { Tool.Pick }, FIX_PICK_NUM));
            actionCards.AddRange(this.CreateFixToolActionCards(new Tool[] { Tool.Lantern }, FIX_LANTERN_NUM));
            actionCards.AddRange(this.CreateFixToolActionCards(new Tool[] { Tool.MineCart }, FIX_MINECART_NUM));
            actionCards.AddRange(this.CreateFixToolActionCards(new Tool[] { Tool.Pick, Tool.Lantern }, FIX_PICK_LANTERN_NUM));
            actionCards.AddRange(this.CreateFixToolActionCards(new Tool[] { Tool.Pick, Tool.MineCart }, FIX_PICK_MINECART_NUM));
            actionCards.AddRange(this.CreateFixToolActionCards(new Tool[] { Tool.Lantern, Tool.MineCart }, FIX_LANTERN_MINECART_NUM));
            // 落石カード
            actionCards.AddRange(this.CreateActionCards(typeof(RockfallActionCard), ROCKFALL_NUM));
            // 宝の地図カード
            actionCards.AddRange(this.CreateActionCards(typeof(TreasureMapActionCard), TREASUREMAP_NUM));

            return actionCards;
        }

        /// <summary>
        /// 道具破壊カードを作成する
        /// </summary>
        /// <param name="tool">道具の種類</param>
        /// <param name="num">作る枚数</param>
        /// <returns>作成した道具破壊カードの一覧</returns>
        private ICollection<BrokenToolActionCard> CreateBrokenToolActionCards(Tool tool, int num)
        {
            ICollection<BrokenToolActionCard> ret = new HashSet<BrokenToolActionCard>();
            for (int index = 0; index < num; index++)
            {
                ret.Add(new BrokenToolActionCard(this.sortIndex++, tool));
            }
            return ret;
        }

        /// <summary>
        /// 道具修理カードを作成する
        /// </summary>
        /// <param name="tools">道具の種類</param>
        /// <param name="num">作る枚数</param>
        /// <returns>作成した道具修理カードの一覧</returns>
        private ICollection<FixToolActionCard> CreateFixToolActionCards(Tool[] tools, int num)
        {
            ICollection<FixToolActionCard> ret = new HashSet<FixToolActionCard>();
            for (int index = 0; index < num; index++)
            {
                ret.Add(new FixToolActionCard(this.sortIndex++, tools));
            }
            return ret;
        }

        /// <summary>
        /// アクションカードを作成する。
        /// どのアクションカードかは型から類推する。
        /// メンバがないもの限定。
        /// </summary>
        /// <param name="type">型</param>
        /// <param name="num">作る枚数</param>
        /// <returns>作成したアクションカードの一覧</returns>
        private ICollection<ActionCard> CreateActionCards(Type type, int num)
        {
            if (!type.IsSubclassOf(typeof(ActionCard)))
            {
                StringBuilder messageSB = new StringBuilder();
                messageSB.Append("Type '").Append(type).Append("' is not subclass of ActionCard class.");
                string message = messageSB.ToString();
                ArgumentException e = new ArgumentException(message, nameof(type));
                logger.Error(message, e);
                throw e;
            }
            ICollection<ActionCard> ret = new HashSet<ActionCard>();
            for (int index = 0; index < num; index++)
            {
                ret.Add((ActionCard) Activator.CreateInstance(type, new object[] { this.sortIndex++ }));
            }
            return ret;
        }
        
        /// <summary>
        /// 金カードを作成する。
        /// </summary>
        /// <param name="numGoldNuggets">金の数</param>
        /// <param name="num">作る枚数</param>
        /// <returns>作成した金カードの一覧</returns>
        private IEnumerable<GoldNuggetCard> CreateGoldNuggetCards(int numGoldNuggets, int num)
        {
            ICollection<GoldNuggetCard> ret = new HashSet<GoldNuggetCard>();
            for (int index = 0; index < num; index++)
            {
                ret.Add(new GoldNuggetCard(this.sortIndex++, numGoldNuggets));
            }
            return ret;
        }
    }
}
