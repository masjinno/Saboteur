using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Saboteur.Model
{
    /// <summary>
    /// 宝の地図
    /// </summary>
    class TreasureMapActionCard : ActionCard
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public TreasureMapActionCard(int index) : base(index)
        {
            this.SetImage();
        }

        protected override string CreateImageFilePath()
        {
            string fileName = "TreasureMapActionCard.png";
            string filePath = Path.GetFullPath(Path.Combine(Card.IMAGE_FILE_DIR, fileName.ToString()));
            if (!File.Exists(filePath))
            {
                logger.Error("Image file doesn't exist. [" + filePath + "]");
            }

            return filePath;
        }
    }
}
