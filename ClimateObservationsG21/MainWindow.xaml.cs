﻿using System;
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

        ClimateObservations co;
        NewObservationViewModel vm;

        public MainWindow()
        {
            InitializeComponent();

            vm = new NewObservationViewModel();
            co = new ClimateObservations();
            comboboxObserver.ItemsSource = co.GetListOfObservers();
            comboboxBasecategory.ItemsSource = co.GetListOfBaseCategories();
            comboboxGeolocation.ItemsSource = co.GetListOfGeolocations();
        }

        #region private methods

        private void UpdateTextFields()
        {
            lblRegisterNewObservation.Content = null;
            lblRegisterNewObservation.Content = $"Registrera en ny observation för {vm.Observer}";
            lblObservationInfo.Content = null;
            lblUnit.Content = null;
        }

        private void UpdateView(NewObservationViewModel vm, Observation observation)
        {
            lstboxObservations.ItemsSource = co.GetListObservations(vm.Observer);
            comboboxObserver.SelectedItem = vm.Observer;
            lblObservationInfo.Content = $"Observation {vm.Geolocation.Area.Name}, {vm.Geolocation.Area.Country.Name} gjordes {vm.Date?.ToString("yyyy-MM-dd")} av {vm.Observer.FirstName} {vm.Observer.LastName}";
            lstboxObservations.ItemsSource = null;
            lstboxObservations.ItemsSource = co.GetListObservations(vm.Observer);
            lstboxObservations.SelectedItem = observation;
            listboxMeasurements.ItemsSource = null;
            listboxMeasurements.ItemsSource = vm.Measurements;
            listBoxAddedMeasurements.ItemsSource = null;
            lblRegisterNewObservation.Content = null;
            lblRegisterNewObservation.Content = $"Registrera en ny observation för {vm.Observer}";
            lblUnit.Content = null;
        }

        #endregion

        #region button click
        private void btnAddObserver_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string firstName = txtBoxFirstName.Text;
                string lastName = txtBoxLastName.Text;
                co.AddObserver(firstName, lastName);

                comboboxObserver.ItemsSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try
            {
                comboboxObserver.ItemsSource = co.GetListOfObservers();
                txtBoxFirstName.Text = null;
                txtBoxLastName.Text = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnRemoveObserver_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                co.RemoveObserver(vm.Observer);
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
            vm.Geolocation = comboboxGeolocation.SelectedItem as Geolocation;
            vm.Date = Calendar.SelectedDate;
            UpdateView(vm, co.AddViewModel(vm));
            listBoxAddedMeasurements.ItemsSource = null;
        }

        private void buttonChangeValue_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Measurement measurement = listboxMeasurements.SelectedItem as Measurement;
                Observation observation = lstboxObservations.SelectedItem as Observation;
                measurement.Value = int.Parse(txtChangeValue.Text);
                int i = co.UpdateMeasurementWithTransaction(measurement);
                MessageBox.Show($"Värdet uppdaterat");
                UpdateView(vm, observation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Mycket kod här under. Vet inte hur vi kan lösa det något annat sätt. 
        private void buttonAddMeasurement_Click(object sender, RoutedEventArgs e)
        {
            Category category = new Category();
            Category baseCategory = new Category();
            int value = int.Parse(txtValue.Text);

            if (comboboxSubCategory2.SelectedItem != null)
            {
                category = comboboxSubCategory2.SelectedItem as Category;
                baseCategory = comboboxSubcategory1.SelectedItem as Category;
            }
            else
            {
                category = comboboxSubcategory1.SelectedItem as Category;
                baseCategory = comboboxBasecategory.SelectedItem as Category;
            }
            category.BaseCategory = baseCategory;
            Measurement measurement = new Measurement
            {
                Value = value,
                Category = category
            };
            vm.Measurements.Add(measurement);
            listBoxAddedMeasurements.ItemsSource = null;
            listBoxAddedMeasurements.ItemsSource = vm.Measurements;
            txtValue.Text = null;
        }

        #endregion

        #region selection changed
        private void lstboxObservations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Observation observation = lstboxObservations.SelectedItem as Observation;
                if (observation != null)
                {
                    vm = co.GetViewModel(observation);
                }
                UpdateView(vm, observation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboboxObserver_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            vm.Observer = comboboxObserver.SelectedItem as Observer;
            lstboxObservations.ItemsSource = null;
            listboxMeasurements.ItemsSource = null;
            try
            {
                if (vm.Observer != null)
                {
                    lstboxObservations.ItemsSource = co.GetListObservations(vm.Observer);
                }
                UpdateTextFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboboxBasecategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Category category = comboboxBasecategory.SelectedItem as Category;
                comboboxSubcategory1.ItemsSource = null;
                comboboxSubCategory2.ItemsSource = null;
                comboboxSubcategory1.ItemsSource = co.GetListOfSubCategories(category);
                lblUnit.Content = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboboxSubcategory1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Category category = comboboxSubcategory1.SelectedItem as Category;
                comboboxSubCategory2.ItemsSource = null;
                if (category != null)
                {
                    comboboxSubCategory2.ItemsSource = co.GetListOfSubCategories(category);
                    lblUnit.Content = category.Unit;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            vm.Measurements.Clear();
            listBoxAddedMeasurements.ItemsSource = null;
        }
        #endregion

    }
}