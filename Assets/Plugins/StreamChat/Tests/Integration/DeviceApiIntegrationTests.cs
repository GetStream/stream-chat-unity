#if STREAM_TESTS_ENABLED
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.Requests;
using UnityEngine.TestTools;

namespace StreamChat.Tests.Integration
{
    /// <summary>
    /// Integration tests for Users
    /// </summary>
    public class DeviceApiIntegrationTests : BaseIntegrationTests
    {
        [UnityTest]
        public IEnumerator Test_complete_device_cycle_add_list_remove()
        {
            yield return Client.WaitForClientToConnect();
            yield return Test_complete_device_cycle_add_list_remove_Async().RunTaskAsEnumerator(Client);
        }

        private async Task Test_complete_device_cycle_add_list_remove_Async()
        {
            const string NewDeviceId = "aaaa-bbb-ccc-111-2222-333";
            
            //Add device, expect no errors
            await Client.DeviceApi.AddDeviceAsync(new CreateDeviceRequest
            {
                Id = NewDeviceId,
                PushProvider = PushProviderType.Firebase,
            });

            //List devices, expect newly added device returned
            var listDevices = await Client.DeviceApi.ListDevicesAsync(Client.UserId);
            Assert.NotNull(listDevices.Devices);
            var addedDevice = listDevices.Devices.FirstOrDefault(d => d.Id == NewDeviceId);
            Assert.NotNull(addedDevice);
            Assert.AreEqual(PushProviderType.Firebase, addedDevice.PushProvider);
            
            //Remove devices, expect no errors
            await Client.DeviceApi.RemoveDeviceAsync(NewDeviceId, Client.UserId);
            
            //Expect device list empty
            listDevices = await Client.DeviceApi.ListDevicesAsync(Client.UserId);
            Assert.That(listDevices.Devices, Is.Null.Or.Empty);
        }
    }
}
#endif