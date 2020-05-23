using Saboteur.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using log4net;
using System.Reflection;

namespace Saboteur.Model
{
    class FieldMap
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 初期配置の盤面横の数。
        /// 周囲の何も置かない一枠も含む。
        /// </summary>
        private const int INIT_WIDTH = 11;

        /// <summary>
        /// 初期配置の盤面縦の数。
        /// 周囲の何も置かない一枠も含む。
        /// </summary>
        private const int INIT_HEIGHT = 7;

        /// <summary>
        /// 盤面に置かれているカード
        /// </summary>
        public FieldBlock[,] Field { get; private set; }

        /// <summary>
        /// 金塊に到達しているか。
        /// set不可。
        /// </summary>
        public bool IsReachedTreasure {
            get
            {
                foreach (FieldBlock block in this.Field)
                {
                    // 到達済みで、金塊があるか
                    if (block.IsReached && block.PathCard.StartGoal == StartGoal.Treasure)
                    {
                        return true;
                    }
                }
                return false;
            } 
        }

        /// <summary>
        /// 盤面をカード配置前の状態にリセットする。
        /// </summary>
        public void Reset()
        {
            // 周りの何も置かれていない部分まで確保する
            this.Field = new FieldBlock[INIT_WIDTH, INIT_HEIGHT];
            for (int x = 0; x < FieldMap.INIT_WIDTH; x++)
            {
                for (int y = 0; y < FieldMap.INIT_HEIGHT; y++)
                {
                    this.Field[x, y] = new FieldBlock();
                }
            }
        }

        /// <summary>
        /// 盤面をラウンド開始時の状態に初期化する
        /// </summary>
        /// <param name="pathCards">使用可能なカード一覧</param>
        /// <returns>使用したカードの一覧</returns>
        public ICollection<PathCard> Initialize(ICollection<PathCard> pathCards)
        {
            this.Reset();

            // 使用するカードを抜き出す
            IList<PathCard> startCards = new List<PathCard>();
            IList<PathCard> goalCards = new List<PathCard>();
            foreach(PathCard pathCard in pathCards)
            {
                switch (pathCard.StartGoal)
                {
                    case StartGoal.Start:
                        startCards.Add(pathCard);
                        break;
                    case StartGoal.Stone:
                    case StartGoal.Treasure:
                        goalCards.Add(pathCard);
                        break;
                }
            }
            List<PathCard> usedCard = new List<PathCard>();
            usedCard.AddRange(startCards);
            usedCard.AddRange(goalCards);

            // サポートする条件に一致するかチェック
            if (startCards.Count != 1)
            {
                throw new NotSupportedException("Start card must be only one.");
            }
            if (goalCards.Count != 3)
            {
                throw new NotSupportedException("Goal cards must be just three.");
            }

            // 盤面にセット
            this.Field[1, 3].PutPathCard(startCards[0]);
            CardUtility.Shuffle(ref goalCards);
            this.Field[9, 1].PutPathCard(goalCards[0]);
            this.Field[9, 3].PutPathCard(goalCards[1]);
            this.Field[9, 5].PutPathCard(goalCards[2]);

            return usedCard;
        }

        public bool PutPathCard(PathCard pathCard, int x, int y)
        {
            if (pathCard == null) throw new ArgumentNullException(nameof(pathCard));

            // 置けるかチェック
            bool canPut = CanPutPathCard(pathCard, x, y);
            if (!canPut) return false;

            // 通路カードを置く
            bool isPut = this.Field[x, y].PutPathCard(pathCard);

            // 周囲に裏になっているカードがあれば、表にして向きを整える。
            // ただし、ゴールカードは接続が一致しない可能性がある。
            this.OpenReversedCardsAround(x, y);

            // フィールド端に置かれた場合は、周囲を拡大する

            return isPut;
        }

        /// <summary>
        /// (x, y)の周囲に、到達している裏のカードがある場合、オープンする。
        /// </summary>
        /// <param name="x">横座標</param>
        /// <param name="y">縦座標</param>
        private void OpenReversedCardsAround(int x, int y)
        {
            // チェックを実施するための条件：
            // ・その方角に向けて穴が空いているか
            // ・その方角のカードが裏になっているか

            // 北方向チェック
            if (this.Field[x, y].PathCard.Hole[(int)Direction.North] && !this.Field[x, y - 1].IsOpen && !this.Field[x, y - 1].IsReached)
            {
                this.OpenReversedCard(x, y - 1, Direction.South);
            }
            // 南方向チェック
            if (this.Field[x, y].PathCard.Hole[(int)Direction.South] && !this.Field[x, y + 1].IsOpen && !this.Field[x, y + 1].IsReached)
            {
                this.OpenReversedCard(x, y + 1, Direction.North);
            }
            // 東方向チェック
            if (this.Field[x, y].PathCard.Hole[(int)Direction.East] && !this.Field[x + 1, y].IsOpen && !this.Field[x + 1, y].IsReached)
            {
                this.OpenReversedCard(x + 1, y, Direction.West);
            }
            // 西方向チェック
            if (this.Field[x, y].PathCard.Hole[(int)Direction.West] && !this.Field[x - 1, y].IsOpen && !this.Field[x - 1, y].IsReached)
            {
                this.OpenReversedCard(x - 1, y, Direction.East);
            }
        }

        /// <summary>
        /// (x, y)のカードをオープンする。
        /// ただし、(x, y)が盤面端ではないものとする。
        /// また、(x, y)の周囲のカードとつながらない場合があるが、このときは<paramref name="priorityDirection"/>の方角の整合性を優先してオープンする。
        /// </summary>
        /// <param name="x">横座標</param>
        /// <param name="y">縦座標</param>
        /// <param name="priorityDirection">優先する方角。(x, y)の座標にとっての方角。</param>
        private void OpenReversedCard(int x, int y, Direction priorityDirection)
        {
            // 既に表になっていれば何もしない
            if (this.Field[x, y].IsOpen) return;

            // 周囲との接続チェック
            if (this.Field[x, y].PathCard.Hole[(int)Direction.North] == this.Field[x, y - 1].PathCard.Hole[(int)Direction.South]
                && this.Field[x, y].PathCard.Hole[(int)Direction.South] == this.Field[x, y + 1].PathCard.Hole[(int)Direction.North]
                && this.Field[x, y].PathCard.Hole[(int)Direction.East] == this.Field[x + 1, y].PathCard.Hole[(int)Direction.West]
                && this.Field[x, y].PathCard.Hole[(int)Direction.West] == this.Field[x - 1, y].PathCard.Hole[(int)Direction.East])
            {
                this.Field[x, y].IsOpen = true;
                return;
            }

            // (x, y)の通路カードを上下逆転する
            this.Field[x, y].PathCard.UpsideDown();

            // 周囲との接続チェック
            if (this.Field[x, y].PathCard.Hole[(int)Direction.North] == this.Field[x, y - 1].PathCard.Hole[(int)Direction.South]
                && this.Field[x, y].PathCard.Hole[(int)Direction.South] == this.Field[x, y + 1].PathCard.Hole[(int)Direction.North]
                && this.Field[x, y].PathCard.Hole[(int)Direction.East] == this.Field[x + 1, y].PathCard.Hole[(int)Direction.West]
                && this.Field[x, y].PathCard.Hole[(int)Direction.West] == this.Field[x - 1, y].PathCard.Hole[(int)Direction.East])
            {
                this.Field[x, y].IsOpen = true;
                return;
            }

            // 上下逆転しても整合しなければ、優先する方角が正しくなるように配置する
            this.Field[x, y].PathCard.UpsideDown();
            Direction oppositeDirection = DirectionUtility.GetOppositeDirection(priorityDirection);
            if (this.Field[x, y].PathCard.Hole[(int)priorityDirection] == this.Field[x, y].PathCard.Hole[(int)oppositeDirection])
            {
                this.Field[x, y].IsOpen = true;
                return;
            }
            // 上下逆転した場合の、優先する方角チェック
            this.Field[x, y].PathCard.UpsideDown();
            if (this.Field[x, y].PathCard.Hole[(int)priorityDirection] == this.Field[x, y].PathCard.Hole[(int)oppositeDirection])
            {
                this.Field[x, y].IsOpen = true;
                return;
            }

            // それでも一致しないことがあるか。基本ないはず。
            throw new NotSupportedException("Can't open reversed card.");
        }

        /// <summary>
        /// 到達情報を最新化する。
        /// 裏のカードは到達判定しない。
        /// </summary>
        /// <return>到達していた数</return>
        public int RefleshReached()
        {
            int xLength = this.Field.GetLength(0);
            int yLength = this.Field.GetLength(1);

            int prevReachedNum = this.ClearReached();
            int reachedNum = prevReachedNum;
            while (true) {
                for (int x = 0; x < xLength; x++)
                {
                    for (int y = 0; y < yLength; y++)
                    {
                        if (this.Field[x, y].PathCard == null || this.Field[x, y].IsReached || !this.Field[x, y].IsOpen)
                        {
                            // カードがない場合、もしくは到達済み、もしくは裏になっている場合はチェック不要
                            continue;
                        }
                        // 各方向の到達チェック
                        // (if文条件式にまとめて組み込めば、最初に見つかった後の方角の判定を省略できるが、読みづらいので不採用)
                        bool reachFromNorth = (this.Field[x, y].PathCard.Hole[(int)Direction.North] && this.Field[x, y - 1].PathCard.Hole[(int)Direction.South] &&
                            this.Field[x, y - 1].IsReached && this.Field[x, y - 1].PathCard.CanThrough);
                        bool reachFromSouth = (this.Field[x, y].PathCard.Hole[(int)Direction.South] && this.Field[x, y + 1].PathCard.Hole[(int)Direction.North] &&
                            this.Field[x, y + 1].IsReached && this.Field[x, y + 1].PathCard.CanThrough);
                        bool reachFromEast = (this.Field[x, y].PathCard.Hole[(int)Direction.East] && this.Field[x + 1, y].PathCard.Hole[(int)Direction.West] &&
                            this.Field[x + 1, y].IsReached && this.Field[x + 1, y].PathCard.CanThrough);
                        bool reachFromWest = (this.Field[x, y].PathCard.Hole[(int)Direction.West] && this.Field[x - 1, y].PathCard.Hole[(int)Direction.East] &&
                            this.Field[x - 1, y].IsReached && this.Field[x - 1, y].PathCard.CanThrough);
                        if (reachFromNorth || reachFromSouth || reachFromEast || reachFromWest)
                        {
                            this.Field[x, y].IsReached = true;
                            reachedNum++;
                        }
                    }
                }
                // 接続チェックに更新がなければ、終了
                if (prevReachedNum == reachedNum) break;

                prevReachedNum = reachedNum;
            }
            logger.DebugFormat("RefleshReached: reachedNum={0}", reachedNum);
            return reachedNum;
        }

        /// <summary>
        /// スタートカード以外の到達情報をクリアする
        /// </summary>
        /// <returns>到達カードの数</returns>
        private int ClearReached()
        {
            int reachedNum = 0;
            foreach(FieldBlock block in this.Field)
            {
                // Startカードであればtrue, それ以外はfalseが入る
                block.IsReached = (block.PathCard.StartGoal == StartGoal.Start);
                if (block.IsReached) reachedNum++;
            }
            logger.DebugFormat("ClearReached: reachedNum={0}", reachedNum);
            return reachedNum;
        }

        /// <summary>
        /// 通路カードを、(x, y)の位置に配置できるか
        /// 条件：
        /// ①以下の条件を満たせば置けない
        /// ・北or南or東or西のいずれかの方向で、穴状況が一致しないこと
        ///   ただし、裏のカードはチェックしない。
        /// ②以下の条件を満たしていなければ、置けない
        /// ・北or南or東or西のいずれかの方向で、到達していること
        /// </summary>
        /// <param name="pathCard">置きたい通路カード</param>
        /// <param name="x">横座標</param>
        /// <param name="y">縦座標</param>
        /// <returns></returns>
        public bool CanPutPathCard(PathCard pathCard, int x, int y)
        {
            if (pathCard == null) throw new ArgumentNullException(nameof(pathCard), "pathCard is null");

            // 既に置かれていれば、置けない
            if (this.Field[x, y].PathCard != null)
            {
                logger.DebugFormat("Field[{0}, {1}].PathCard has already put", x, y);
                return false;
            }

            // 周囲と一致しなければ、置けない。
            // ただしカードが裏であれば一致チェックは省く。
            int xLength = this.Field.GetLength(0);
            int yLength = this.Field.GetLength(1);
            logger.DebugFormat("({0}, {1}) check", x, y);
            // 北方向チェック
            if (0 < y
                && this.Field[x, y - 1].IsOpen
                && this.Field[x, y - 1].PathCard.Hole[(int)Direction.South] != pathCard.Hole[(int)Direction.North])
            {
                logger.DebugFormat("North is unmatched: ({0},{1}).IsOpen={2}, ({0},{1}).PathCard.Hole[{3}]={4}, pathCard.Hole[{5}]={6}",
                    x, y - 1, this.Field[x, y - 1].IsOpen,
                    Direction.South, this.Field[x, y - 1].PathCard.Hole[(int)Direction.South],
                    Direction.North, pathCard.Hole[(int)Direction.North]);
                return false;
            }
            // 南方向チェック
            if (y < yLength - 1
                && this.Field[x, y + 1].IsOpen
                && this.Field[x, y + 1].PathCard.Hole[(int)Direction.North] != pathCard.Hole[(int)Direction.South])
            {
                logger.DebugFormat("South is unmatched: ({0},{1}).IsOpen={2}, ({0},{1}).PathCard.Hole[{3}]={4}, pathCard.Hole[{5}]={6}",
                    x, y + 1, this.Field[x, y + 1].IsOpen,
                    Direction.North, this.Field[x, y + 1].PathCard.Hole[(int)Direction.North],
                    Direction.South, pathCard.Hole[(int)Direction.South]);
                return false;
            }
            // 東方向チェック
            if (0 < x
                && this.Field[x + 1, y].IsOpen
                && this.Field[x + 1, y].PathCard.Hole[(int)Direction.West] != pathCard.Hole[(int)Direction.East])
            {
                logger.DebugFormat("East is unmatched: ({0},{1}).IsOpen={2}, ({0},{1}).PathCard.Hole[{3}]={4}, pathCard.Hole[{5}]={6}",
                    x + 1, y, this.Field[x + 1, y].IsOpen,
                    Direction.West, this.Field[x + 1, y].PathCard.Hole[(int)Direction.West],
                    Direction.East, pathCard.Hole[(int)Direction.East]);
                return false;
            }
            // 西方向チェック
            if (x < xLength - 1
                && this.Field[x - 1, y].IsOpen
                && this.Field[x - 1, y].PathCard.Hole[(int)Direction.East] != pathCard.Hole[(int)Direction.West])
            {
                logger.DebugFormat("West is unmatched: ({0},{1}).IsOpen={2}, ({0},{1}).PathCard.Hole[{3}]={4}, pathCard.Hole[{5}]={6}",
                    x - 1, y, this.Field[x - 1, y].IsOpen,
                    Direction.East, this.Field[x, y - 1].PathCard.Hole[(int)Direction.East],
                    Direction.West, pathCard.Hole[(int)Direction.West]);
                return false;
            }

            // 置けない条件のいずれにも当てはまらないとき、置ける
            bool ret = (this.Field[x, y - 1].IsReached || this.Field[x, y + 1].IsReached
                || this.Field[x + 1, y].IsReached || this.Field[x - 1, y].IsReached);
            logger.DebugFormat("({1},{3}).IsReached={6}, ({1}, {5}).IsReached={7}, ({2},{4}).IsReached={8}, ({0},{4}).IsReached={9}: ret={10}",
                x - 1, x, x + 1, y - 1, y, y + 1,
                this.Field[x, y - 1].IsReached, this.Field[x, y + 1].IsReached,
                this.Field[x + 1, y].IsReached, this.Field[x - 1, y].IsReached,
                ret);
            return ret;
        }
    }
}
