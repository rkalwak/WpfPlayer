using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
namespace Wpf_Player
{
    class Network
    {
        static string hostName = "127.0.0.1";
        static int port = 65000;
        private string requestedFile;
        private string folderPath;
        private string requestedFilePath;
        public Network() 
        {
            folderPath = String.Empty;
            requestedFile = String.Empty;
            requestedFilePath=String.Empty;
        }

        public string FolderPath
        {
            set { folderPath = value; }
            get { return folderPath; }
        }
        public string RequestedFile
        {
            set { requestedFile = value; }
            get { return requestedFile; }
        }
        public string RequestedFilePath
        {
            set{requestedFilePath=value;}
            get{return requestedFilePath;}
        }
        
        public void SendMessage()
        {
            TcpClient client = new TcpClient();

            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 30000);
            try
            {
                client.Connect(serverEndPoint);

            }
            catch
            {


            }

            NetworkStream clientStream = client.GetStream();

            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] buffer = encoder.GetBytes(requestedFile);

            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.Flush();

        }
        public void FetchFileFromServer()
        {

            TcpClient client = new TcpClient(hostName, port);
            if (client.Connected)
            {

                NetworkStream netStream = client.GetStream();

                try
                {

                    BufferedStream s_in = new BufferedStream(netStream);
                    byte[] buffer = new byte[8192];
                    int bytesRead;
                    
                    string filePath = folderPath + requestedFile + ".mp3";
                    Stream s_out = File.OpenWrite(filePath);
                    while ((bytesRead = s_in.Read(buffer, 0, 8192)) > 0)
                    {
                        s_out.Write(buffer, 0, bytesRead);
                    }
                    s_out.Flush();
                    s_in.Close();
                    s_out.Close();
                    if (File.Exists(filePath))
                    {
                        
                        Song song = new Song(filePath);               
                        string artist = song.Artist;
                        string title = song.Title;                      
                        requestedFilePath=folderPath+artist+"-"+title+".mp3";
                        File.Move(filePath,requestedFilePath);
                    }
                }
                catch (Exception ex)
                {

                }
                
            }


        }
    }
}
