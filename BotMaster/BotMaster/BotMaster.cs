using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

class BotMaster
{
    /// <summary>
    /// Danh sách các luồng kết nối tới máy Bot
    /// </summary>
    static List<StreamWriter> ListStremWriter = new List<StreamWriter>();

    /// <summary>
    /// Hàm có chức năng lắng nghe các kết nối
    /// </summary>
    public static void Main()
    {
        //Địa chỉ máy Boss
        IPAddress address = IPAddress.Parse("192.168.111.137");

        //Lắng nghe kết nối
        TcpListener listener = new TcpListener(address, 9999);
        Console.WriteLine("Waiting for connection...");
        listener.Start();

        //Nhận các kết nối từ máy Bot
        while (true)
        {
            Socket soc = listener.AcceptSocket();

            Thread t = new Thread((obj) =>
            {
                DoWork((Socket)obj);
            });
            t.Start(soc);
        }
    }

    /// <summary>
    /// Hàm có chức năng gửi các câu lệnh cho máy bot
    /// </summary>
    /// <param name="soc">luồng của máy bot</param>
    static void DoWork(Socket soc)
    {
        Console.WriteLine("Connection received from: {0}",
                          soc.RemoteEndPoint);
        //Tạo luồng tới máy Bot
        var stream = new NetworkStream(soc);
        var writer = new StreamWriter(stream);
        ListStremWriter.Add(writer);

        try
        {
            while (true)
            {
                //Lấy nội dung màn hình console
                string mess = Console.ReadLine();

                //Gửi nội dung tới từng máy Bot
                foreach (StreamWriter streamWriter in ListStremWriter)
                {
                    streamWriter.AutoFlush = true;
                    streamWriter.WriteLine(mess);
                }
                
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
        }

        Console.WriteLine("Client disconnected: {0}",
                          soc.RemoteEndPoint);

        //Đóng luồng của Bot đóng kết nối
        ListStremWriter.Remove(writer);
        writer.Close();
        stream.Close();
        soc.Close();
    }

}