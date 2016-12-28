using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Microsoft.Band;

namespace MicrosoftBand.Portables
{
    /// <summary>
    /// Manage a connection to selected Band device
    /// The lifecycle of the Manager handle the Band Connection lifecycle (Dispose)
    /// </summary>
    public class BandManager : IDisposable
    {
        /// <summary>
        /// paired band infos (paired to operation system not to app)
        /// </summary>
        public IBandInfo[] PairedBands { get; private set; }

        /// <summary>
        /// number of band, that can we use
        /// </summary>
        public int NumberOfPairedBands => PairedBands?.Length ?? 0;

        /// <summary>
        /// Actual selected, and connected Band client
        /// </summary>
        public IBandClient BandClient { get; private set; }

        /// <summary>
        /// Connection is initialized and connected to 
        /// </summary>
        public bool Initialized { get; private set; }

        private BandManager()
        {
            
        }

        /// <summary>
        /// Create a BandManager object to get paired Microsoft Band devices
        /// </summary>
        public static async Task<BandManager> CreateAsync()
        {
            //Get the paired Bands
            var manager = new BandManager
            {
                PairedBands = await BandClientManager.Instance.GetBandsAsync(),
                Initialized = true
            };

            return manager;
        }

        /// <summary>
        /// Create a connection (Client) to selected Band device
        /// </summary>
        /// <param name="numberOfConnectedBand">which ndex of Band device select to connection</param>
        /// <returns></returns>
        public async Task ConnectToAsync(int numberOfConnectedBand)
        {
            if (PairedBands == null || PairedBands.Length == 0)
                throw new ActionNotSupportedException("Can't find Microsoft Band Devide");

            try
            {
                BandClient = await BandClientManager.Instance.ConnectAsync(PairedBands[numberOfConnectedBand]);
                //Connected Successfully
                await BandClient.NotificationManager.VibrateAsync(Microsoft.Band.Notifications.VibrationType.RampUp);
            }
            catch (BandException ex)
            {
                // handle a Band connection exception
                Debug.WriteLine("Can't connect to Band");
            }
        }

        /// <summary>
        /// Create a connection with the selected index of Band device
        /// </summary>
        /// <param name="numberOfConnectedBand"></param>
        /// <returns></returns>
        public static async Task<BandManager> CreateConnectionAsync(int numberOfConnectedBand = 0)
        {
            var manager = await BandManager.CreateAsync();

            await manager.ConnectToAsync(numberOfConnectedBand);

            return manager;
        }

        public void Dispose()
        {
            try
            {
                BandClient.Dispose();
            }
            catch (Exception)
            {
                Debug.WriteLine("Can't dispose to Band");
            }
            finally
            {
                Initialized = false;
            }
        }
    }
}
