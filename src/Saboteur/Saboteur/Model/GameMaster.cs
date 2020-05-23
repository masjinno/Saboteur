using Saboteur.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using log4net;
using System.Reflection;

namespace Saboteur.Model
{
    class GameMaster
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ICollection<Card> allDeckCards;
        private readonly ICollection<GoldNuggetCard> allGoldNugetCards;

        /// <summary>
        /// デッキ。
        /// ゲーム開始時はここにすべてのPathカードとActionカードを集める。
        /// ※スタートカード、ゴールカードも含む。
        /// プレイヤーにカード配布後は、山札として扱う。
        /// </summary>
        private IList<Card> deck;

        /// <summary>
        /// 金カード。
        /// プレイヤーに配られていないカードのリスト。
        /// </summary>
        private IList<GoldNuggetCard> goldNuggetCards;

        /// <summary>
        /// プレイヤー情報
        /// </summary>
        private IList<PlayerData> players;

        /// <summary>
        /// 金鉱の盤面
        /// </summary>
        private FieldMap field;

        /// <summary>金鉱掘りカード</summary>
        private RoleCard goldMinerRoleCard;
        /// <summary>お邪魔者カード</summary>
        private RoleCard saboteurRoleCard;

        /// <summary>
        /// ゲームに必要な準備をする。
        /// ・カードインスタンスを作成する
        /// ・カードを置く場所を用意する
        /// ・プレイヤーを呼べるようにする
        /// </summary>
        public GameMaster()
        {
            CardSetting setting = new CardSetting();
            this.allDeckCards = setting.CreateAllDeckCards();
            this.allGoldNugetCards = setting.CreateAllGoldNuggetCards();
            this.players = new List<PlayerData>();
            this.field = new FieldMap();
            this.goldMinerRoleCard = new RoleCard(-1, Role.GoldMiner);
            this.saboteurRoleCard = new RoleCard(-2, Role.Saboteur);
        }

        /// <summary>
        /// プレイヤーを用意する。
        /// 名前を付けること。
        /// </summary>
        /// <param name="playerNames"></param>
        public void InitializePlayers(List<string> playerNames)
        {
            playerNames.ForEach(playerName => this.players.Add(new PlayerData(playerName)));
        }

        /// <summary>
        /// ゲームを初期化する
        /// </summary>
        public void InitializeGame()
        {
            this.InitializeGoldNuggetCards();
        }

        /// <summary>
        /// ラウンドを初期化する
        /// </summary>
        public void InitializeRound()
        {
            this.InitializeDeck();
        }

        /// <summary>
        /// 金カードの一覧を用意する。
        /// ゲーム開始時に行う。
        /// </summary>
        private void InitializeGoldNuggetCards()
        {
            this.goldNuggetCards = new List<GoldNuggetCard>(this.allGoldNugetCards);
            CardUtility.Shuffle(ref this.goldNuggetCards);
        }

        /// <summary>
        /// デッキを初期化する。
        /// ラウンドの最初に行う。
        /// </summary>
        private void InitializeDeck()
        {
            this.deck = new List<Card>(this.allDeckCards);
            CardUtility.Shuffle(ref this.deck);
        }
    }
}
