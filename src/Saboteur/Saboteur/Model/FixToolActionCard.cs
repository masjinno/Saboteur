using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Reflection;

namespace Saboteur.Model
{
    /// <summary>
    /// 道具修理カード
    /// </summary>
    class FixToolActionCard : ActionCard
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Tool[] Target { get; private set; }

        public FixToolActionCard(int index, Tool[] target) : base(index)
        {
            this.Target = target;

            this.SetImage();
        }

        protected override string CreateImageFilePath()
        {
            StringBuilder fileNameSB = new StringBuilder();
            fileNameSB.Append("Fix");
            foreach (Tool t in Enum.GetValues(typeof(Tool)))
            {
                if (this.Target.Contains(t))
                {
                    fileNameSB.Append(t);
                }
            }
            fileNameSB.Append("ActionCard.png");
            string filePath = Path.GetFullPath(Path.Combine(Card.IMAGE_FILE_DIR, fileNameSB.ToString()));
            if (!File.Exists(filePath))
            {
                logger.Error("Image file doesn't exist. [" + filePath + "]");
            }

            return filePath;
        }
    }
}
