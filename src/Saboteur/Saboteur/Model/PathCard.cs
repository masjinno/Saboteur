using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Media.Imaging;

namespace Saboteur.Model
{
    /// <summary>
    /// パスカード
    /// </summary>
    class PathCard : Card
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>掘られているか。Direction型を使うこと。</summary>
        public bool[] Hole { get; private set; }

        /// <summary>通り抜けられるか</summary>
        public bool CanThrough { get; private set; }

        /// <summary>スタートカードか、ゴールカード(金塊、石ころ)</summary>
        public StartGoal StartGoal { get; private set; }

        public PathCard(int index, bool[] hole, bool through, StartGoal startGoal) : base(index)
        {
            this.Hole = hole;
            this.CanThrough = through;
            this.StartGoal = startGoal;

            this.SetImage();
        }

        /// <summary>
        /// 上下を逆にする。
        /// カードの掘られている位置を反転する。
        /// </summary>
        public void UpsideDown()
        {
            bool[] beforeHole = this.Hole;
            this.Hole[(int)Direction.North] = beforeHole[(int)Direction.South];
            this.Hole[(int)Direction.South] = beforeHole[(int)Direction.North];
            this.Hole[(int)Direction.East] = beforeHole[(int)Direction.West];
            this.Hole[(int)Direction.West] = beforeHole[(int)Direction.East];

            // TODO: 画像の上下反転

        }

        protected override string CreateImageFilePath()
        {
            StringBuilder fileNameSB = new StringBuilder();
            // 穴の方角
            if (this.Hole[(int)Direction.North]) fileNameSB.Append("N");
            if (this.Hole[(int)Direction.South]) fileNameSB.Append("S");
            if (this.Hole[(int)Direction.East]) fileNameSB.Append("E");
            if (this.Hole[(int)Direction.West]) fileNameSB.Append("W");
            // 通り抜けられるか
            if (this.CanThrough) fileNameSB.Append("Throughable");
            else fileNameSB.Append("Blocked");
            // スタート・ゴールについて
            switch (StartGoal)
            {
                case StartGoal.Start:
                    fileNameSB.Append("Start");
                    break;
                case StartGoal.Treasure:
                    fileNameSB.Append("Treasure");
                    break;
                case StartGoal.Stone:
                    fileNameSB.Append("Stone");
                    break;
            }
            // 締め
            fileNameSB.Append("PathCard.png");
            string filePath = Path.GetFullPath(Path.Combine(Card.IMAGE_FILE_DIR, fileNameSB.ToString()));
            if (!File.Exists(filePath))
            {
                logger.Error("Image file doesn't exist. [" + filePath + "]");
            }

            return filePath;
        }
    }
}
