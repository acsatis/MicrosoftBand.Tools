using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Band;
using MicrosoftBand.Portables;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MicrosoftBand.ThemeSelector
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void MainPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var bandManager = await BandManager.CreateConnectionAsync())
                {
                    tbFirmwareVersion.Text = await bandManager.BandClient.GetFirmwareVersionAsync();
                    tbHardwareVersion.Text = await bandManager.BandClient.GetHardwareVersionAsync();
                }
            }
            catch (Exception)
            {
                MessageDialog dialog = new MessageDialog("Nem elérhető a Band");
                await dialog.ShowAsync();
            }
        }

        private async void BtnSubmitColor_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var bandManager = await BandManager.CreateConnectionAsync())
                {
                    BandTheme theme = await bandManager.BandClient.PersonalizationManager.GetThemeAsync();

                    theme.Base = GetSliderColor().ToBandColor();

                    await bandManager.BandClient.PersonalizationManager.SetThemeAsync(theme);
                }
            }
            catch (Exception)
            {
                MessageDialog dialog = new MessageDialog("Nem elérhető a Band");
                await dialog.ShowAsync();
            }
        }

        private void Slider_OnValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            rectBackground.Fill = new SolidColorBrush(GetSliderColor());
        }

        private Color GetSliderColor()
        {
            byte r = Convert.ToByte(sliderRed.Value);
            byte g = Convert.ToByte(sliderGreen.Value);
            byte b = Convert.ToByte(sliderBlue.Value);

            return Color.FromArgb(255, r, g, b);
        }
    }
}
