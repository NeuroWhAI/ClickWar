using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace ClickWar.Util
{
    public class Utility
    {
        public static Random Random = new Random();

        //##################################################################################

        public static Color GetRandomColor()
        {
            int green = Random.Next(0, 255);
            return Color.FromArgb(Random.Next(200, 255), green, Math.Abs(green - 255));
        }

        //##################################################################################

        public static string[] GetHostIPList()
        {
            List<string> ipList = new List<string>();

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            for (int i = 0; i < host.AddressList.Length; i++)
            {
                if (host.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    ipList.Add(host.AddressList[i].ToString());
            }


            var macInfo = NetworkInterface.GetAllNetworkInterfaces();

            if (macInfo.Length > 0)
                ipList.Add(macInfo[0].GetPhysicalAddress().ToString());


            return ipList.ToArray();
        }

        public static async Task<string[]> GetHostIPListAsync()
        {
            List<string> ipList = new List<string>();

            IPHostEntry host = await Dns.GetHostEntryAsync(Dns.GetHostName());
            for (int i = 0; i < host.AddressList.Length; i++)
            {
                if (host.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    ipList.Add(host.AddressList[i].ToString());
            }


            var macInfo = NetworkInterface.GetAllNetworkInterfaces();

            if (macInfo.Length > 0)
                ipList.Add(macInfo[0].GetPhysicalAddress().ToString());


            return ipList.ToArray();
        }

        //##################################################################################

        public static int EncodeValue(int value, out int key)
        {
            key = Random.Next(128, 1024);
            return ((value ^ key) - 1);
        }

        public static int DecodeValue(int value, int key)
        {
            return ((value + 1) ^ key);
        }
    }
}
