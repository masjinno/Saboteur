using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Media.Imaging;

namespace Saboteur.Model
{
    /// <summary>
    /// カード抽象クラス
    /// </summary>
    abstract class Card
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>画像ファイルを置くディレクトリパス</summary>
        protected const string IMAGE_FILE_DIR = @"image\";

        /// <summary>カード順。ソートに利用する。</summary>
        public int Index { get; private set; }

        /// <summary>画像データ</summary>
        public BitmapImage Image { get; private set; }

        public Card(int index)
        {
            this.Index = index;
        }

        /// <summary>
        /// 画像を設定する。
        /// 具象クラスのコンストラクタで、処理の最後に呼び出すこと。
        /// </summary>
        protected void SetImage()
        {
            try
            {
                string imageFilePath = this.CreateImageFilePath();
                logger.DebugFormat("imageFilePath={0}", imageFilePath);
                Uri uri = new Uri(imageFilePath, UriKind.Absolute);
                // logger.DebugFormat("uri={0}", uri.AbsoluteUri);
                this.Image = new BitmapImage(uri);
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Caught an exception: ").Append(ex.GetType()).Append(", message=").Append(ex.Message);
                logger.Error(sb.ToString(), ex);
                throw ex;
            }
        }

        /// <summary>
        /// 画像ファイルのフルパスを生成する。
        /// 具象クラスによってファイルが異なるため、Cardクラスでは実装しない。
        /// </summary>
        /// <returns>画像ファイルパス</returns>
        protected abstract string CreateImageFilePath();
    }
}
