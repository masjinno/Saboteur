using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Saboteur.Model
{
    /// <summary>
    /// 道具破壊カード
    /// </summary>
    class BrokenToolActionCard : ActionCard
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Tool Target { get; private set; }

        public BrokenToolActionCard(int index, Tool target) : base(index)
        {
            this.Target = target;

            this.SetImage();
        }

        protected override string CreateImageFilePath()
        {
            StringBuilder fileNameSB = new StringBuilder();
            fileNameSB.Append("Broken").Append(this.Target).Append("ActionCard.png");
            string filePath = Path.GetFullPath(Path.Combine(Card.IMAGE_FILE_DIR, fileNameSB.ToString()));
            if (!File.Exists(filePath))
            {
                logger.Error("Image file doesn't exist. [" + filePath + "]");
            }

            return filePath;
        }
    }
}
