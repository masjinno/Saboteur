using log4net;
using Saboteur.Model;
using Saboteur.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Saboteur.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IList<string> strList = new List<string>
            {
                "AAA",
                "BBB",
                "CCC",
                "DDD",
                "EEE",
                "FFF"
            };
            //logger.Debug(CardUtility.listToString(strList));
            CardUtility.Shuffle(ref strList);
            logger.Debug(CommonUtility.listToString(strList));

            GameMaster gm = new GameMaster();

            MessageBox.Show("debug completed.");
        }
    }
}
