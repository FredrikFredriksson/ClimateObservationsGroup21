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

        NewObservationViewModel vm = new NewObservationViewModel();
        ClimateObservations co = new ClimateObservations();

        List<Observer> listOfObservers = new List<Observer>();
        List<Country> listOfCountries = new List<Country>();
        List<Observation> listOfObservations = new List<Observation>();

        Category mainCategory = new Category();
        Category subCategory = new Category();
        Category sub2Category = new Category();

        Observer observer = new Observer();
        
        

        public MainWindow()
        {
            InitializeComponent();

            listOfObservers = co.GetListOfObservers();            
            comboboxObserver.ItemsSource = listOfObservers;
            comboboxCountry.ItemsSource = co.GetListOfCountries();
            comboboxCategory.ItemsSource = co.GetListOfBaseCategories();

            

        }

        private void btnAddObserver_Click(object sender, RoutedEventArgs e)
        {
            string firstName = txtBoxFirstName.Text;
            string lastName = txtBoxLastName.Text;

            co.AddObserver(firstName, lastName);

            comboboxObserver.ItemsSource = null;
            comboboxObserver.ItemsSource = co.GetListOfObservers();
        }

        private void btnRemoveObserver_Click(object sender, RoutedEventArgs e)
        {
            try //lagt till, återstår att testa om det fungerar med try catch
            {
                Observer observer = comboboxObserver.SelectedItem as Observer;

                co.RemoveObserver(observer);

                comboboxObserver.ItemsSource = null;
                comboboxObserver.ItemsSource = co.GetListOfObservers();                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
            
                    
        }

        private void lstboxObservers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            observer = comboboxObserver.SelectedItem as Observer;
            lstboxObservations.ItemsSource = co.GetListObservations(observer);


            //Observer observer = lstboxObservers.SelectedItem as Observer;


            //lstboxObservations.ItemsSource = co.GetListObservations(observer);
            
        }

        private void btnAddObservation_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSaveToObservation_Click(object sender, RoutedEventArgs e)
        {

            vm.Observer = comboboxObserver.SelectedItem as Observer;
            vm.GeolocationId = 1;
            vm.Date = DateTime.Now;
            

            co.AddViewModelToDatabase(vm);
            
        }

        private void comboboxCountry_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Country country = comboboxCountry.SelectedItem as Country;

            comboboxArea.ItemsSource = co.GetListOfArea(country);

        }

        private void comboboxCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Category category = comboboxCategory.SelectedItem as Category;

            comboboxSubCategory1.ItemsSource = null;
            comboboxSubCategory1.ItemsSource = co.GetListOfSubCategories(category);
        }

        private void comboboxSubCategory1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Category category = comboboxSubCategory1.SelectedItem as Category;
            comboboxSubCategory2.ItemsSource = co.GetListOfSubCategories(category);
            lblUnit.Content = co.GetUnit(category);
        }

        private void btnShowInfoObservation_Click(object sender, RoutedEventArgs e)
        {
            NewObservationViewModel vm = new NewObservationViewModel();

            

        }

        private void lstboxObservations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Observation observation = lstboxObservations.SelectedItem as Observation;

            vm = co.GetViewModel(observation);

            listboxMeasurements.ItemsSource = vm.Measurements; 
        }

        private void comboboxObserver_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            observer = comboboxObserver.SelectedItem as Observer;
            lstboxObservations.ItemsSource = co.GetListObservations(observer);

        }

        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mainCategory = comboboxCategory.SelectedItem as Category;
            subCategory = comboboxSubCategory1.SelectedItem as Category;
            sub2Category = comboboxSubCategory2.SelectedItem as Category;
            int value = int.Parse(txtValue.Text);

            Measurement measurement = new Measurement
            {
                Value = value,
                CategoryId = subCategory.Id
            };

            vm.Measurements.Add(measurement);

        }

        private void buttonChangeValue_Click(object sender, RoutedEventArgs e)
        {

            

            Measurement measurement = listboxMeasurements.SelectedItem as Measurement;

            measurement.Value = int.Parse(txtChangeValue.Text);

            int i = co.UpdateMeasurementWithTransaction(measurement);

            MessageBox.Show($"{i} antal rader påverkade");
        }



        


        //private void lstboxObservations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    Observation observation = lstboxObservations.SelectedItem as Observation;


        //    lstBoxListOfMeasurements.ItemsSource = null;
        //    lstBoxListOfMeasurements.ItemsSource = co.GetListOfMeasurements(observation);
        //}

        //private void lstboxObservations_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    Observation observation = lstboxObservations.SelectedItem as Observation;

        //    //var listMeasurement = co.GetListOfMeasurements(3);
        //    //var category = co.GetListOfCategories(listMeasurement[0]);


        //    //lblTest.Content = listMeasurement[0];
        //    //lblTest2.Content = category[0];



        //    ///lblTest3.Content = co.GetUnit(category[0].UnitId);
        //}

        //private void btnShowInfoObservation_Click(object sender, RoutedEventArgs e)
        //{

        //    //NewObservationViewModel vm = new NewObservationViewModel();
        //    //använd index där index = selected value

        //    NewObservationViewModel vm = new NewObservationViewModel();


        //    Observation observation = lstboxObservations.SelectedItem as Observation;


        //    vm = co.GetViewModel(observation);

        //    lstBoxListOfMeasurements.ItemsSource = vm.Measurements;


        //    lblTest.Content = vm.Measurements[0].Category.Name; 


        //    ///lstBoxListOfMeasurements.ItemsSource = $"{vm.Measurements[0].Observation.Date} {vm.Measurements[0].Value} {vm.Measurements[0].Category.Unit.Abbreviation} {vm.Measurements[0].Category.Name}";
        //}

        //private void lstBoxListOfMeasurements_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    Measurement measurement = lstBoxListOfMeasurements.SelectedItem as Measurement;

        //    lstBoxCategory.ItemsSource = null;
        //    lstBoxCategory.ItemsSource = co.GetListOfCategories(measurement);

        //}

        //private void lstBoxCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{

        //    Category category = lstBoxCategory.SelectedItem as Category;

        //    lstBoxUnit.ItemsSource = co.GetListOfUnits(category);

        //}





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



    }

}