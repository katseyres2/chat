using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chat
{
    internal static class Program
    {
        static Form1 form = null;

        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            while(true)
            {
                if (form == null)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    form = new Form1();
                    Console.WriteLine("1...");
                    Application.Run(form);
                    Console.WriteLine("2...");
                }

                //if (Form1.clientMessage.CompareTo(Form1.newClientMessage) != 0)
                //{
                //    Form1.clientMessage = Form1.newClientMessage;
                //    form.displayClientMessage(Form1.clientMessage);
                //}

                //Console.WriteLine("Watching...");
                //Thread.Sleep(1000);
            }

            //Client.Connect("127.0.0.1");
        }
    }
}

