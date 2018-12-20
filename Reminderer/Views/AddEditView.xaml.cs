using Reminderer.Framework;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Reminderer.Views
{
    /// <summary>
    /// Interaction logic for AddEditView.xaml
    /// </summary>
    ///    

    public partial class AddEditView : UserControl
    {
        private bool _reminderCondition {
            get
            {
                return RemindToggleSwitch.IsToggled == true && AtThisTimeRadioButton.IsChecked == true;
            }
        }

        public AddEditView()
        {
            InitializeComponent();

            RepeatLabel.Visibility = System.Windows.Visibility.Hidden;
            RepeatSwitchGrid.Visibility = System.Windows.Visibility.Hidden;
            RepeatingDaysLabel.Visibility = System.Windows.Visibility.Hidden;
            RepeatingDaysPicker.Visibility = System.Windows.Visibility.Hidden;
            NumDaysComboBoxGrid.Visibility = System.Windows.Visibility.Hidden;
        }

        private void RemindToggleSwitch_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AnimateRepeatOptions();
        }

        private void AtThisTimeRadioButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AnimateRepeatOptions();
        }

        private void AnimateRepeatOptions()
        {
            bool isSchedule = DatePicker.Visibility == System.Windows.Visibility.Visible;

            DoubleAnimation db = new DoubleAnimation();
            DoubleAnimation db2 = new DoubleAnimation();
            DoubleAnimation db3 = new DoubleAnimation();

            if (isSchedule)
            {
                RepeatLabel.Visibility = System.Windows.Visibility.Hidden;
                RepeatSwitchGrid.Visibility = System.Windows.Visibility.Hidden;
                RepeatingDaysLabel.Visibility = System.Windows.Visibility.Hidden;
                RepeatingDaysPicker.Visibility = System.Windows.Visibility.Hidden;

                NumDaysComboBoxGrid.Visibility = System.Windows.Visibility.Visible;

                SetDoubleAnimations(db, db2, db3);

                NumDaysComboBoxGrid.BeginAnimation(HeightProperty, db3);
            }
            else
            {
                if (!_reminderCondition) return;

                RepeatLabel.Visibility = System.Windows.Visibility.Visible;
                RepeatSwitchGrid.Visibility = System.Windows.Visibility.Visible;
                RepeatingDaysLabel.Visibility = System.Windows.Visibility.Visible;
                RepeatingDaysPicker.Visibility = System.Windows.Visibility.Visible;

                NumDaysComboBoxGrid.Visibility = System.Windows.Visibility.Hidden;

                SetDoubleAnimations(db, db2, db3);

                RepeatSwitchGrid.BeginAnimation(Grid.HeightProperty, db);
                RepeatingDaysPicker.BeginAnimation(UserControl.HeightProperty, db);
                RepeatLabel.BeginAnimation(Label.HeightProperty, db2);
                RepeatingDaysLabel.BeginAnimation(Label.HeightProperty, db2);
                RepeatToggleSwitch.BeginAnimation(HeightProperty, db3);
            }
        }

        private void SetDoubleAnimations(DoubleAnimation db, DoubleAnimation db2, DoubleAnimation db3)
        {
            if (RemindToggleSwitch.IsToggled)
            {
                db.From = RemindGrid.ActualHeight;
                db.To = 0;
                db.Duration = TimeSpan.FromSeconds(0.5);

                db2.From = RemindLabel.ActualHeight;
                db2.To = 0;
                db2.Duration = TimeSpan.FromSeconds(0.5);

                db3.From = RemindToggleSwitch.ActualHeight;
                db3.To = 0;
                db3.Duration = TimeSpan.FromSeconds(0.5);
            }
            else
            {
                db.From = 0;
                db.To = RemindGrid.ActualHeight;
                db.Duration = TimeSpan.FromSeconds(0.5);

                db2.From = 0;
                db2.To = RemindLabel.ActualHeight;
                db2.Duration = TimeSpan.FromSeconds(0.5);

                db3.From = 0;
                db3.To = RemindToggleSwitch.ActualHeight;
                db3.Duration = TimeSpan.FromSeconds(0.5);
            }
        }
    }
}
