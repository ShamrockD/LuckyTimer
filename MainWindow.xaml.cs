using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Windows.Threading;
using Microsoft.VisualBasic;
using System.Media;

namespace LuckyTimer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private members

        private int _minutes = 0;
        private int _seconds = 0;
        private int _hours = 0;

        private bool _isSecondTimerInProgress = false;

        private DispatcherTimer _mainTimerDispatcher = new DispatcherTimer();
        private DispatcherTimer _secondTimerDispatcher = new DispatcherTimer();

        // should we store time values before launching
        private bool _shouldWeStoreTimeBeforeLaunch = true;
        private int _beforeLaunchHours = 0;
        private int _beforeLaunchMinutes = 0;
        private int _beforeLaunchSeconds = 0;
        private int _secondTimerBeforeLaunchSeconds = 0;
        private int _secondTimerBeforeLaunchMinutes = 0;

        #endregion
        
        public MainWindow()
        {
            InitializeComponent();
            
            _mainTimerDispatcher.Tick += new EventHandler(MainTimer_Tick);
            _mainTimerDispatcher.Interval = new TimeSpan(0, 0, 1);

            _secondTimerDispatcher.Tick += new EventHandler(SecondTimer_Tick);
            _secondTimerDispatcher.Interval = new TimeSpan(0, 0, 1);
        }
        private void MainTimer_Tick(object? sender, EventArgs eventArgs)
        {
            #region If seconds == 0

            if (_seconds == 0)
            {
                if (_minutes == 0)
                {
                    if (_hours == 0)
                    {
                        _mainTimerDispatcher.IsEnabled = false;

                        if (checkBox_sound.IsChecked == true)
                        {
                            PlaySound();
                        }

                        if (checkBox_TurnOffPc.IsChecked == true && checkBox_secondTimer.IsChecked == false)
                        {
                            ShutDownPc();
                        }

                        if (checkBox_secondTimer.IsChecked == true)
                        {
                            _isSecondTimerInProgress = true;
                            _secondTimerDispatcher.IsEnabled = true;
                        }
                        else
                        {
                            btn_StartPauseButton.Content = "Start";
                            btn_Stop.Visibility = Visibility.Hidden;

                            EnableButtons();
                            RestoreTimeValues();
                        }
                    }
                    else
                    {
                        _hours -= 1;
                        tb_Hours.Text = _hours.ToString("00");
                        _minutes = 59;
                        tb_Minutes.Text = _minutes.ToString("00");
                        _seconds = 60;
                    }
                }
                else
                {
                    _minutes -= 1;
                    tb_Minutes.Text = _minutes.ToString("00");
                    _seconds = 60;
                }
            }

            #endregion

            if (_mainTimerDispatcher.IsEnabled == true)
            {
                _seconds -= 1;
                tb_Seconds.Text = _seconds.ToString("00");
            }
        }

        private void ShutDownPc()
        {
            System.Diagnostics.Process.Start("shutdown", "/s /t 0");
        }

        private void EnableButtons()
        {
            groupBox_MainTimerPresets.IsEnabled = true;
            groupBox_SecondTimerPreset.IsEnabled = true;

            tb_Hours.IsReadOnly = false;
            tb_Minutes.IsReadOnly = false;
            tb_Seconds.IsReadOnly = false;
            tb_SecTimerMinutes.IsReadOnly = false;
            tb_SecTimerSeconds.IsReadOnly = false;
        }
        
        private void DisableButtons()
        {
            groupBox_MainTimerPresets.IsEnabled = false;
            groupBox_SecondTimerPreset.IsEnabled = false;

            tb_Hours.IsReadOnly = true;
            tb_Minutes.IsReadOnly = true;
            tb_Seconds.IsReadOnly = true;
            tb_SecTimerMinutes.IsReadOnly = true;
            tb_SecTimerSeconds.IsReadOnly = true;
        }
        
        private void btn_StartPauseButton_OnClick(object sender, RoutedEventArgs e)
        {
            DisableButtons();

            _hours = Convert.ToInt32(tb_Hours.Text);
            _minutes = Convert.ToInt32(tb_Minutes.Text);
            _seconds = Convert.ToInt32(tb_Seconds.Text);

            if (_shouldWeStoreTimeBeforeLaunch == true)
            {   //MainTimer
                _beforeLaunchHours = _hours;
                _beforeLaunchMinutes = _minutes;
                _beforeLaunchSeconds = _seconds;
                //SecondTimer
                _secondTimerBeforeLaunchMinutes = Convert.ToInt32(tb_SecTimerMinutes.Text);
                _secondTimerBeforeLaunchSeconds = Convert.ToInt32(tb_SecTimerSeconds.Text);

                _shouldWeStoreTimeBeforeLaunch = false;
            }

            if (_mainTimerDispatcher.IsEnabled == false)
            {
                btn_StartPauseButton.Content = "Pause";

                if (_secondTimerDispatcher.IsEnabled == false && _isSecondTimerInProgress == false)
                {
                    _mainTimerDispatcher.IsEnabled = true;
                }
                else if (_secondTimerDispatcher.IsEnabled == false && _isSecondTimerInProgress == true)
                {
                    _secondTimerDispatcher.IsEnabled = true;
                }
                else
                {
                    _secondTimerDispatcher.IsEnabled = false;
                    btn_StartPauseButton.Content = "Continue";
                }

                btn_Stop.Visibility = Visibility.Visible;
            }
            else
            {
                _mainTimerDispatcher.IsEnabled = false;
                btn_StartPauseButton.Content = "Continue";
            }
            
        }
        
        private void btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            _shouldWeStoreTimeBeforeLaunch = true;

            _hours = 0;
            _minutes = 0;
            _seconds = 0;

            _mainTimerDispatcher.IsEnabled = false;
            _secondTimerDispatcher.IsEnabled = false;

            _isSecondTimerInProgress = false;
            btn_StartPauseButton.Content = "Start";

            btn_Stop.Visibility = Visibility.Hidden;
            
            RestoreTimeValues();
            EnableButtons();
        }
        
        private void SecondTimer_Tick(object? sender, EventArgs e)
        {
            _minutes = Convert.ToInt32(tb_SecTimerMinutes.Text);
            _seconds = Convert.ToInt32(tb_SecTimerSeconds.Text);

            if (_seconds == 0)
            {
                if (_minutes == 0)
                {
                    _secondTimerDispatcher.IsEnabled = false;
                    _isSecondTimerInProgress = false;

                    if (checkBox_sound.IsChecked == true)
                    {
                        PlaySound();
                    }

                    if (checkBox_TurnOffPc.IsChecked == true)
                    {
                        ShutDownPc();
                    }

                    btn_StartPauseButton.Content = "Start";
                    btn_Stop.Visibility = Visibility.Hidden;
                    
                    EnableButtons();
                    RestoreTimeValues();
                }
                else
                {
                    _minutes -= 1;
                    tb_SecTimerMinutes.Text = _minutes.ToString("00");
                    _seconds = 60;
                }
            }

            if (_secondTimerDispatcher.IsEnabled == true)
            {
                _seconds -= 1;
                tb_SecTimerSeconds.Text = _seconds.ToString("00");
            }
        }
        
        private void btn_Preset_Click(object sender, RoutedEventArgs e)
        {
            string tag = (sender as Button).Tag.ToString();

            if (tag.Contains("."))
            {
                string[] time = tag.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                tb_Minutes.Text = string.Format("{0:D2}", int.Parse(time[0]));
                tb_Seconds.Text = string.Format("{0:D2}", int.Parse(time[1]));

                tb_SecTimerMinutes.Text = "00";
                tb_SecTimerSeconds.Text = "00";
            }
            else if(tag.Contains("+"))
            {
                string[] time = tag.Split("+".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                
                tb_Minutes.Text = string.Format("{0:D2}", int.Parse(time[0]));
                tb_Seconds.Text = "00";

                tb_SecTimerMinutes.Text = string.Format("{0:D2}", int.Parse(time[1]));
                tb_SecTimerSeconds.Text = "00";
            }
            else
            {
                tb_Minutes.Text = "00";
                tb_Seconds.Text = string.Format("{0:D2}", int.Parse(tag));

                tb_SecTimerMinutes.Text = "00";
                tb_SecTimerSeconds.Text = "00";
            }
        }
        
        private void RestoreTimeValues()
        {
            tb_Hours.Text = _beforeLaunchHours.ToString("00");
            tb_Minutes.Text = _beforeLaunchMinutes.ToString("00");
            tb_Seconds.Text = _beforeLaunchSeconds.ToString("00");
            tb_SecTimerMinutes.Text = _secondTimerBeforeLaunchMinutes.ToString("00");
            tb_SecTimerSeconds.Text = _secondTimerBeforeLaunchSeconds.ToString("00");
        }
        private void TextBox_NumberValidation(object sender, TextCompositionEventArgs e)
        {
            // Regex that match disalowed text
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
            base.OnPreviewKeyDown(e);
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as TextBox).Text == "")
            {
                (sender as TextBox).Text = "00";
            }
            else
            {
                (sender as TextBox).Text = Convert.ToInt32((sender as TextBox).Text).ToString("00");

                if (Convert.ToInt32((sender as TextBox).Text) > 59)
                {
                    (sender as TextBox).Text = "59";
                }
            }
        }

        private void PlaySound()
        {
            if (soundRadioBtn1.IsChecked == true)
            {
                for(int i = 0; i <3; i++)
                {
                    for (int j = 3; j > 0; j--)
                    {
                        System.Threading.Thread playSoundThread = new System.Threading.Thread(
                            new System.Threading.ThreadStart(
                                delegate()
                                {
                                    Console.Beep(4500, 50);
                                }
                            ));
                        playSoundThread.Start();
                    }
                }
            }

            if (soundRadioBtn2.IsChecked == true)
            {
                for(int i = 0; i <3; i++)
                {
                    for (int j = 3; j > 0; j--)
                    {
                        System.Threading.Thread playSoundThread = new System.Threading.Thread(
                            new System.Threading.ThreadStart(
                                delegate()
                                {
                                    Console.Beep(2500, 50);
                                }
                            ));
                        playSoundThread.Start();
                        
                    }
                }
            }
            
            if (soundRadioBtn3.IsChecked == true)
            {
                for(int i = 0; i <3; i++)
                {
                    for (int j = 3; j > 0; j--)
                    {
                        System.Threading.Thread playSoundThread = new System.Threading.Thread(
                            new System.Threading.ThreadStart(
                                delegate()
                                {
                                    Console.Beep(6500, 50);
                                }
                            ));
                        playSoundThread.Start();
                        
                    }
                }
            }
        }
    }
}