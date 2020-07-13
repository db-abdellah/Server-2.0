using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace MainServer
{
    class Server
    {
        private TcpListener listener;
        public Server() { }
        public void Start()
        {
            try
            {
                int port = (int)Int64.Parse(ConfigurationManager.AppSettings["port"].ToString());
                //Listening  on port 6060
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                Console.WriteLine("Server Started...");
                //listening Loop
                while (true)
                {

                    TcpClient client = listener.AcceptTcpClient();
                    //Starting new Thread of the connected client
                    Thread thread = new Thread(ProccessClient);
                    thread.Start(client);

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (listener != null)
                    listener.Stop();
            }


        }

        //thread Loop
        private void ProccessClient(object cl)
        {

            TcpClient client = (TcpClient)cl;
            if (authorizeClient(client))
            {
                try
                {
                    StreamReader reader = new StreamReader(client.GetStream());
                   
                   
                       
                        //Recevoir d'id d'image et id du projet
                        String projectId = reader.ReadLine();
                        String imageId= reader.ReadLine();
                    Console.WriteLine(imageId);

                    

                    NetworkStream netStream = client.GetStream();
                    MemoryStream ms = new MemoryStream();
                    netStream.CopyTo(ms);
                    Image receivedImg = Image.FromStream(ms);
                        
                        
                     
                        //Enregistrer l'image 
                        saveImage(receivedImg, imageId, projectId);
                    

                    
                    client.Close();

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    if (client != null)
                        client.Close();
                }
            }
            else
                return;
        }

        //Autorisation du client
        private Boolean authorizeClient(TcpClient client)
        {
            int i = 0;
            StreamReader reader = new StreamReader(client.GetStream());
            StreamWriter writer = new StreamWriter(client.GetStream());
            Boolean authorized = false;
            String username = String.Empty;
            String password = String.Empty;
            while (!authorized)
            {
                if (i == 3)
                {
                    client.Close();
                    return false;
                }
                //recevoir le username
                username = reader.ReadLine();
                //recevoir le MDP
                password = reader.ReadLine();
                if (username.Equals("admin") && password.Equals("ensa"))
                {
                    authorized = true;
                    writer.WriteLine("true");
                    writer.Flush();
                    return true;
                }

                else
                {
                    writer.WriteLine("Incorrect username or password");
                    writer.Flush();
                    i++;


                }

            }
            return false;
        }

        //Enregistrer l'image
        private void saveImage(Image image, String imageId, String projectId)
        {
            String path = projectId + "\\" + imageId + ".jpg";
            String dir = ConfigurationManager.AppSettings["MainFolder"].ToString();
            image.Save(dir + path);
        }


    }//end of class


}//end of namespace
