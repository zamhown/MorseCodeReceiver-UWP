//Copyright (c) 2017 Howard Zhang

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.UI.Core; // Required to access the core dispatcher object
using Windows.Devices.Sensors; // Required to access the sensor platform and the ALS

namespace MorseCodeReceiver
{
    public sealed partial class MainPage : Page
    {
        private LightSensor _lightsensor; // Our app' s lightsensor object

        private const int MIN_LUX = 20;

        /// <summary>
        /// The state code of Morse code inputting
        /// </summary>
        private int state = 0;
        /// <summary>
        /// The time span of the state
        /// </summary>
        private int span = 0;
        /// <summary>
        /// Have the letter been readed?
        /// </summary>
        private bool readed = false;

        private async void ReadingChangedAsync(object sender, LightSensorReadingChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
              {
                  LightSensorReading reading = e.Reading;
                  var lux = reading.IlluminanceInLux;
                  if (lux < MIN_LUX)
                  {
                      if (state == 0)
                      {
                          state = 1;
                          span = 0;
                      }
                      if (span < 100) span++;
                  }
                  else
                  {
                      if (state > 0)
                      {
                          if (state == 1)
                          {
                              if (span <= 10 && span >= 5)
                              {
                                  txtOutput.Text += "·";
                                  readed = false;
                              }
                              else if (span > 10 && span <= 50)
                              {
                                  txtOutput.Text += "-";
                                  readed = false;
                              }
                          }
                          state = 0;
                          span = 0;
                      }
                      span++;
                      if(span > 10 && !readed)
                      {
                          //Letter recognition
                          //TODO: Binary search tree can be used to improve efficiency.
                          switch (txtOutput.Text)
                          {
                              case "·-":
                                  txtMsg.Text += "A";
                                  break;
                              case "-···":
                                  txtMsg.Text += "B";
                                  break;
                              case "-·-·":
                                  txtMsg.Text += "C";
                                  break;
                              case "-··":
                                  txtMsg.Text += "D";
                                  break;
                              case "·":
                                  txtMsg.Text += "E";
                                  break;
                              case "··-·":
                                  txtMsg.Text += "F";
                                  break;
                              case "--·":
                                  txtMsg.Text += "G";
                                  break;
                              case "····":
                                  txtMsg.Text += "H";
                                  break;
                              case "··":
                                  txtMsg.Text += "I";
                                  break;
                              case "·---":
                                  txtMsg.Text += "J";
                                  break;
                              case "-·-":
                                  txtMsg.Text += "K";
                                  break;
                              case "·-··":
                                  txtMsg.Text += "L";
                                  break;
                              case "--":
                                  txtMsg.Text += "M";
                                  break;
                              case "-·":
                                  txtMsg.Text += "N";
                                  break;
                              case "---":
                                  txtMsg.Text += "O";
                                  break;
                              case "·--·":
                                  txtMsg.Text += "P";
                                  break;
                              case "--·-":
                                  txtMsg.Text += "Q";
                                  break;
                              case "·-·":
                                  txtMsg.Text += "R";
                                  break;
                              case "···":
                                  txtMsg.Text += "S";
                                  break;
                              case "-":
                                  txtMsg.Text += "T";
                                  break;
                              case "··-":
                                  txtMsg.Text += "U";
                                  break;
                              case "···-":
                                  txtMsg.Text += "V";
                                  break;
                              case "·--":
                                  txtMsg.Text += "W";
                                  break;
                              case "-··-":
                                  txtMsg.Text += "X";
                                  break;
                              case "-·--":
                                  txtMsg.Text += "Y";
                                  break;
                              case "--··":
                                  txtMsg.Text += "Z";
                                  break;
                          }
                          txtOutput.Text = "";
                          readed = true;
                      }

                      if (span == 20)
                      {
                          //Next word
                          txtMsg.Text += " ";
                      }
                      else if (span == 100)
                      {
                          //Refresh
                          txtMsg.Text = "";
                          span = 0;
                      }
                  }
                  string des = lux > MIN_LUX ? "High" : "Low";
                  txtData.Text = $"{des} {state} {span}";
              });
        }

        /// <summary>
        /// Refresh
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtMsg_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            txtMsg.Text = "";
            txtOutput.Text = "";
            state = 0;
            span = 0;
        }

        public MainPage()
        {
            InitializeComponent();
            _lightsensor = LightSensor.GetDefault(); // Get the default light sensor object

            // Assign an event handler for the ALS reading-changed event
            if (_lightsensor != null)
            {
                // Establish the report interval for all scenarios
                uint minReportInterval = _lightsensor.MinimumReportInterval;
                uint reportInterval = minReportInterval > 20 ? minReportInterval : 20;
                _lightsensor.ReportInterval = reportInterval;

                // Establish the even thandler
                _lightsensor.ReadingChanged += new TypedEventHandler<LightSensor, LightSensorReadingChangedEventArgs>(ReadingChangedAsync);
            }
        }
    }
}