﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace FakeProcessor
{
    class Program
    {
        static string connectionString = "HostName=HotfIotHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=5ymD940oarMkF2s+UxNNNjM/enzW1s4pv41WbBySkzI=";
        static string iotHubD2cEndpoint = "messages/events";
        static EventHubClient eventHubClient;
        static void Main(string[] args)
        {
            Console.WriteLine("Receive messages\n");
            eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, iotHubD2cEndpoint);

            var d2cPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;

            foreach (string partition in d2cPartitions)
            {
                ReceiveMessagesFromDeviceAsync(partition);
            }
            Console.ReadLine();
        }

        private async static Task ReceiveMessagesFromDeviceAsync(string partition)
        {
            var eventHubReceiver = eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, DateTime.Now);
            while (true)
            {
                EventData eventData = await eventHubReceiver.ReceiveAsync();
                if (eventData == null) continue;

                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                Console.WriteLine(string.Format("Message received. Partition: {0} Data: '{1}'", partition, data));
            }
        }
    }
}
