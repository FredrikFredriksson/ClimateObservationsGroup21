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

namespace ClimateObservationsG21
{

    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        ClimateObservations co = new ClimateObservations();

        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                co.RemoveObserver(8);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

            

            //var category = co.GetCategory(5);

            //MessageBox.Show($"{category}");

            //var observers = co.GetObservers();


            //try
            //{
            //Observer observer = new Observer
            //{
            //    FirstName = "Nicklas",
            //    LastName = "Mellqvist"
            //    };

            //    observer = co.AddObserver(observer);
            //}
            //catch (Exception ex)
            //{

            //    MessageBox.Show(ex.Message);
            //}


            //lstBox.ItemsSource = observers;


            
            


        }

        /*
               peak = new Peak
               {
                   PeakName = "Aconcagua",
                   RangeId = 1
               };//inget peakid här för att vi behöver låta db bestämma detta ju

               peak = db.AddPeak(peak);//FUNKADE! Lillsjöhögen är med!
               */


    }

}