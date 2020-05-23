using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Saboteur.Model
{
    /// <summary>
    /// 金カード
    /// </summary>
    class GoldNuggetCard : Card
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// カードにある金の数
        /// </summary>
        public int Num { get; private set; }

        public GoldNuggetCard(int index, int num) : base(index)
        {
            this.Num = num;

            this.SetImage();
        }

        protected override string CreateImageFilePath()
        {
            StringBuilder fileNameSB = new StringBuilder();
            switch (this.Num)
            {
                case 1: fileNameSB.Append("One"); break;
                case 2: fileNameSB.Append("Two"); break;
                case 3: fileNameSB.Append("Three"); break;
            }
            fileNameSB.Append("GoldNuggetCard.png");
            string filePath = Path.GetFullPath(Path.Combine(Card.IMAGE_FILE_DIR, fileNameSB.ToString()));
            if (!File.Exists(filePath))
            {
                logger.Error("Image file doesn't exist. [" + filePath + "]");
            }

            return filePath;
        }
    }
}
