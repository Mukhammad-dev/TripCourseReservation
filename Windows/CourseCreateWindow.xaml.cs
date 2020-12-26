﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using TripCourseReservation.CRUD;
using TripCourseReservation.Entities;
using TripCourseReservation.Shared;
using TripCourseReservation.View_Models;

namespace TripCourseReservation
{
    /// <summary>
    /// Interaction logic for CourseCreateWindow.xaml
    /// </summary>
    public partial class CourseCreateWindow : Window
    {
        #region Properties

        private ICourseCRUD courseCRUD = new XmlCRUD();

        Course course = new Course();
        List<Term> terms = new List<Term>();
        DataValidation dataValidation = new DataValidation();
        StringBuilder errorMessage = new StringBuilder();
        bool isAnyDataInXmlFile = false;

        #endregion

        #region Constructor
        public CourseCreateWindow()
        {
            isAnyDataInXmlFile = dataValidation.CheckIfDataExist(); 

            InitializeComponent();
            InitializeDefaults();
            SaveButtonAbility();
        }
        #endregion

        #region Events
        private void Tr_Inc_Yes_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Tr_Inc_Yes.IsChecked = false;
        }

        private void Tr_Inc_No_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            switch (Tr_Inc_No.IsEnabled)
            {
                case true:
                    Tr_Inc_No.IsChecked = true;
                    break;
                case false:
                    Tr_Inc_No.IsChecked = false;
                    break;
            }
        }

        private void onSaveTerm(object sender, RoutedEventArgs e)
        {
            if (Title.Text == "")
            {
                MessageBox.Show("Term cannot be created without Course title");
            }
            else
            {
                if (ValidateTermFieldData())
                {
                    errorMessage.Clear();
                    MessageBox.Show(errorMessage.ToString());
                }
                else
                {
                    Terms.ItemsSource = null;
                    Terms.ItemsSource = AddTerm();
                    ClearTermsField();
                    SaveButtonAbility();
                }
            }
        }

        private void onSave(object sender, RoutedEventArgs e)
        {
            if (dataValidation.ValidateCourseBasicData(Title.Text, Description.Text, Trainer.Text, Code.Text, ref errorMessage)
                || dataValidation.CheckIfTermExists(terms, ref errorMessage))
            {
                MessageBox.Show(errorMessage.ToString());
            }
            else if ((ValidateTermFieldData() == false))
            {
                MessageBoxResult result = MessageBox.Show("All unsaved Term changes will lose, do you want to continue",
                    "Confirmation", MessageBoxButton.YesNo);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        GenerateCourse();
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
            else
            {
                GenerateCourse();
            }
        }

        private void onCancel(object sender, RoutedEventArgs e)
        {
            IsFieldsAreDirty();
        }
        #endregion

        #region Methods
        private void InitializeDefaults()
        {
            No.IsChecked = true;
        }

        private bool ValidateTermFieldData()
        {
            return dataValidation.ValidateTermFieldData(DateFrom.SelectedDate, DateTo.SelectedDate, Price.Text, Capacity.Text, ref errorMessage);
        }

        private List<Term> AddTerm()
        {
            var term = new Term();
            term.Event = Title.Text + " : " + DateFrom.SelectedDate.Value.ToString("dd/M/yyyy") + " - " + DateTo.SelectedDate.Value.ToString("dd/M/yyyy");
            term.DateFrom = DateFrom.SelectedDate.Value;
            term.DateTo = DateTo.SelectedDate.Value;
            term.Price = Convert.ToDouble(Price.Text);
            term.Capacity = Convert.ToInt32(Capacity.Text);

            if(Yes.IsChecked == true)
            {
                if (Tr_Inc_Yes.IsChecked == true)
                {
                    term.TransportIncluded = true;
                    term.PickUpPlace = PickUpPlace.Text;
                }else if (Tr_Inc_No.IsChecked == true)
                {
                    term.TransportIncluded = false;
                }
                else
                {
                    term.TransportIncluded = null;
                }
                    
                //term.TransportIncluded = Tr_Inc_Yes.IsChecked == true ? true : false;
            }
            terms.Add(term);

            return terms;
        }

        private void ClearTermsField()
        {
            DateFrom.SelectedDate = null;
            DateTo.SelectedDate = null;
            Price.Text = "0";
            Capacity.Text = "0";
        }

        private void GenerateCourse()
        {
            course.Code = Code.Text;
            course.Title = Title.Text;
            course.Description = Description.Text;
            course.CanContainTransport = Yes.IsChecked == true ? true : false;
            course.Trainer = Trainer.Text;
            course.Terms = terms;

            if (isAnyDataInXmlFile)
            {
                if (dataValidation.CheckIfCourseAlreadyExists(course))
                {
                    MessageBox.Show("The Course with current Code already exists, please choose another Code.");
                }
                else
                {
                    courseCRUD.AddCourse(course);
                    this.Close();
                }
            }
            else
            {
                courseCRUD.CreateCourse(course);
                this.Close();
            }
        }

        private bool IsFieldsAreDirty()
        {
            if(Code.Text != "" || Description.Text != "" || Trainer.Text != "" || Title.Text != "" || DateFrom.SelectedDate.HasValue || DateTo.SelectedDate.HasValue
                || Price.Text != "0" || Capacity.Text != "0" ||  Tr_Inc_Yes.IsEnabled == true)
            {
                MessageBoxResult result = MessageBox.Show("You didnt save course changes. All unsaved Course changes will lose, do you want to continue",
                    "Confirmation", MessageBoxButton.YesNo);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        this.Close();
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }

            this.Close();
            return true;
        }

    private void SaveButtonAbility()
        {
            Save.IsEnabled = terms.Count > 0 ? true : false;
        }
        #endregion
    }
}