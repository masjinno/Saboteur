using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Saboteur.Model
{
    /// <summary>
    /// 落石カード
    /// </summary>
    class RockfallActionCard : ActionCard
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public RockfallActionCard(int index) : base(index)
        {
            this.SetImage();
        }

        protected override string CreateImageFilePath()
        {
            string fileName = "RockfallActionCard.png";
            string filePath = Path.GetFullPath(Path.Combine(Card.IMAGE_FILE_DIR, fileName.ToString()));
            if (!File.Exists(filePath))
            {
                logger.Error("Image file doesn't exist. [" + filePath + "]");
            }

            return filePath;
        }
    }
}
