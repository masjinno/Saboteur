using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Saboteur.Model
{
    class RoleCard : Card
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Role role;

        public RoleCard(int index, Role role) : base(index)
        {
            this.role = role;

            this.SetImage();
        }

        protected override string CreateImageFilePath()
        {
            StringBuilder fileNameSB = new StringBuilder();
            fileNameSB.Append(this.role).Append("RoleCard.png");
            string filePath = Path.GetFullPath(Path.Combine(Card.IMAGE_FILE_DIR, fileNameSB.ToString()));
            if (!File.Exists(filePath))
            {
                logger.Error("Image file doesn't exist. [" + filePath + "]");
            }

            return filePath;
        }
    }
}
