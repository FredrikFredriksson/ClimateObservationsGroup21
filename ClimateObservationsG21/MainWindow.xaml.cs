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

        ClimateObservations co;
        NewObservationViewModel vm;
        Observer observer;

        public MainWindow()
        {
            InitializeComponent();

            vm = new NewObservationViewModel();
            co = new ClimateObservations();
            observer = new Observer();
            comboboxObserver.ItemsSource = co.GetListOfObservers(); 
            comboboxBasecategory.ItemsSource = co.GetListOfBaseCategories();
        }

        #region private methods

        private void ClearFields()
        {
            
        }

        private void UpdateView(NewObservationViewModel vm, Observation observation)
        {
            lstboxObservations.ItemsSource = co.GetListObservations(vm.Observer);
            comboboxObserver.SelectedItem = vm.Observer;
            lblObservationInfo.Content = $"Observationen gjordes {vm.Date?.ToString("yyyy-MM-dd")} av {vm.Observer.FirstName} {vm.Observer.LastName}";
            lstboxObservations.ItemsSource = null;
            lstboxObservations.ItemsSource = co.GetListObservations(vm.Observer);
            lstboxObservations.SelectedItem = observation;
            listboxMeasurements.ItemsSource = null;
            listboxMeasurements.ItemsSource = vm.Measurements;
            listBoxAddedMeasurements.ItemsSource = null;
            lblRegisterNewObservation.Content = null;
            lblRegisterNewObservation.Content = $"Registrera en ny observation för {vm.Observer}";
        }
        
        #endregion

        #region button click
        private void btnAddObserver_Click(object sender, RoutedEventArgs e)
        {
            string firstName = txtBoxFirstName.Text;
            string lastName = txtBoxLastName.Text;

            co.AddObserver(firstName, lastName);

            comboboxObserver.ItemsSource = null;
            comboboxObserver.ItemsSource = co.GetListOfObservers();
            txtBoxFirstName.Text = null;
            txtBoxLastName.Text = null;
        }

        private void btnRemoveObserver_Click(object sender, RoutedEventArgs e)
        {
            try //lagt till, återstår att testa om det fungerar med try catch
            {
                observer = comboboxObserver.SelectedItem as Observer;

                co.RemoveObserver(observer);

                comboboxObserver.ItemsSource = null;
                comboboxObserver.ItemsSource = co.GetListOfObservers();                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
            
                    
        }

        private void btnSaveToObservation_Click(object sender, RoutedEventArgs e)
        {
            vm.Observer = comboboxObserver.SelectedItem as Observer;
            vm.GeolocationId = 1;
            vm.Date = Calendar.SelectedDate;

            UpdateView(vm, co.AddViewModelToDatabase(vm));

            listBoxAddedMeasurements.ItemsSource = null;
        }

        private void buttonChangeValue_Click(object sender, RoutedEventArgs e)
        {
            Measurement measurement = listboxMeasurements.SelectedItem as Measurement;
            Observation observation = lstboxObservations.SelectedItem as Observation;
            
            measurement.Value = int.Parse(txtChangeValue.Text);

            int i = co.UpdateMeasurementWithTransaction(measurement);

            MessageBox.Show($"{i} antal rader påverkade");

            UpdateView(vm, observation);
        }


        // FIXA DENNA
        private void buttonAddMeasurement_Click(object sender, RoutedEventArgs e)
        {
            var category = comboboxSubcategory1.SelectedItem as Category;
            var subcategory = comboboxSubCategory2.SelectedItem as Category;
            int value = int.Parse(txtValue.Text);
            category.Unit = co.GetUnit(category);
            category.SubCategory = subcategory;

            Measurement measurement = new Measurement
            {
                Value = value,
                Category = category,
                CategoryId = category.Id
            };
            vm.Measurements.Add(measurement);

            listBoxAddedMeasurements.ItemsSource = null;
            listBoxAddedMeasurements.ItemsSource = vm.Measurements;
        }

        #endregion

        #region selection changed

        private void lstboxObservations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Observation observation = lstboxObservations.SelectedItem as Observation;

            if (observation != null)
            {
                vm = co.GetViewModel(observation);
            }
            UpdateView(vm, observation);
        }

        private void comboboxObserver_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            vm.Observer = comboboxObserver.SelectedItem as Observer;

            lstboxObservations.ItemsSource = null;
            listboxMeasurements.ItemsSource = null;

            if (vm.Observer != null)
            {
                lstboxObservations.ItemsSource = co.GetListObservations(vm.Observer);
            }
        }

        private void comboboxBasecategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Category category = comboboxBasecategory.SelectedItem as Category;
            comboboxSubcategory1.ItemsSource = null;
            comboboxSubCategory2.ItemsSource = null;

            

            comboboxSubcategory1.ItemsSource = co.GetListOfSubCategories(category);
            lblUnit.Content = null;
        }

        private void comboboxSubcategory1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Category category = comboboxSubcategory1.SelectedItem as Category;
            comboboxSubCategory2.ItemsSource = null;

            if (category != null)
            {
                comboboxSubCategory2.ItemsSource = co.GetListOfSubCategories(category);
                lblUnit.Content = co.GetUnit(category);
            }

        }

        #endregion

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            vm.Measurements.Clear();
        }
    }

}