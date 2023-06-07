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
using System.Windows.Shapes;

namespace FirstProject_CarRaceGameWPF
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
           InitializeComponent();
           
        }

        private void Start(object sender, RoutedEventArgs e)
        {
            if (("").Equals(playerName.Text)) {

                MessageBox.Show("Unesite ime");
            }
            else {
                MainWindow mw = new MainWindow();
                this.Close();
                mw.ShowDialog();

  
            }
        }
    }
}
