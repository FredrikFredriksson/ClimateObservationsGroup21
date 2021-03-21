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

    // vid vald observation behöver mesasurement.Value, category.Name och unit.Abbreviation presenteras?

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        ClimateObservations co = new ClimateObservations();
        List<Observer> listOfObservers = new List<Observer>();

        List<Observation> listOfObservations = new List<Observation>();

        public MainWindow()
        {
            InitializeComponent();

            listOfObservers = co.GetListOfObservers();            
            lstboxObservers.ItemsSource = listOfObservers;
            
        }

        private void btnAddObserver_Click(object sender, RoutedEventArgs e)
        {
            string firstName = txtBoxFirstName.Text;
            string lastName = txtBoxLastName.Text;

            co.AddObserver(firstName, lastName);

            lstboxObservers.ItemsSource = null;
            lstboxObservers.ItemsSource = co.GetListOfObservers();
        }

        private void btnRemoveObserver_Click(object sender, RoutedEventArgs e)
        {
            Observer observer = lstboxObservers.SelectedItem as Observer;

            co.RemoveObserver(observer);

            lstboxObservers.ItemsSource = null;
            lstboxObservers.ItemsSource = co.GetListOfObservers();
                    
        }

        private void lstboxObservers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Observer observer = lstboxObservers.SelectedItem as Observer;


            lstboxObservations.ItemsSource = co.GetListObservations(observer);
            
        }

        private void lstboxObservations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Observation observation = lstboxObservations.SelectedItem as Observation;
           

            lstBoxListOfMeasurements.ItemsSource = null;
            lstBoxListOfMeasurements.ItemsSource = co.GetListOfMeasurements(observation);
        }

        private void lstBoxListOfMeasurements_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Measurement measurement = lstBoxListOfMeasurements.SelectedItem as Measurement;

            lstBoxCategory.ItemsSource = null;
            lstBoxCategory.ItemsSource = co.GetListOfCategories(measurement);
            
        }

        private void lstBoxCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            Category category = lstBoxCategory.SelectedItem as Category;

            lstBoxUnit.ItemsSource = co.GetListOfUnits(category);

        }














        //Measurement measurement = new Measurement();

        //Observation observation = new Observation();

        //observation = co.GetObservation(3);


        //measurement = co.GetMeasurement(observation);




        //var listOfObservations = co.GetObservations();

        //lstBox.ItemsSource = listOfObservations;


        // varg är categori 5, vinterpäls är kategori 12 som är underkategori till varg (5).



        //
        //Ange värde: "2" Measurement.Value = 2, Ange kategori: Varg - category.Id 5, Ange päls: Vinterpäls - category-Id = 12 category.Basecategory = 5







        //co.SetMeasurement(2, 2, 10);


        //try
        //{
        //    co.RemoveObserver(8);
        //}
        //catch (Exception ex)
        //{

        //    MessageBox.Show(ex.Message);
        //}



        ////var category = co.GetCategory(5);

        ////MessageBox.Show($"{category}");

        ////var observers = co.GetObservers();


        ////try
        ////{
        ////Observer observer = new Observer
        ////{
        ////    FirstName = "Nicklas",
        ////    LastName = "Mellqvist"
        ////    };

        ////    observer = co.AddObserver(observer);
        ////}
        ////catch (Exception ex)
        ////{

        ////    MessageBox.Show(ex.Message);
        ////}


        ////lstBox.ItemsSource = observers;

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