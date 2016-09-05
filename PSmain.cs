using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;
using System.Net;
using System.Xml;
using System.Windows;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Xml.Linq;
using System.Collections.Specialized;

namespace PS
{
    static class PSmain
    {
        static NotifyIcon nIco;
        static string savePath, currPath;
        static Random rnd;
        static Icon b, w;

        static void Main(string[] args)
        {
            w = new Icon(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("PS.cam-w.ico"));
            b = new Icon(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("PS.cam-b.ico"));
            rnd = new Random();
            savePath = Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures) + "\\Screenshots\\Screenshot ";
            nIco = new NotifyIcon();
            nIco.Icon = w;
            nIco.Visible = true;
            initContextMenu();
            nIco.Click += PrintScreenToFile;
            //nIco.ShowBalloonTip(100, "PrintScreen launched!", "Right-click the icon in the system tray to configure it.", ToolTipIcon.Info);
            Application.Run();
        }

        public static byte[] ImageToBArr(Image iin)
        {
            using (var ms = new MemoryStream())
            {
                iin.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }

        public static Bitmap PrintScreen()
        {
            Rectangle screenCombined;
            Graphics gfx = Graphics.FromHwnd(System.IntPtr.Zero);
            screenCombined = new Rectangle(0,0,0,0);
            foreach (Screen screen in Screen.AllScreens)
            {
                screenCombined = Rectangle.Union(screenCombined, screen.Bounds);
            }

            Bitmap printscreen = new Bitmap(screenCombined.Width, screenCombined.Height);
            Graphics graphics = Graphics.FromImage(printscreen as Image);
            graphics.CopyFromScreen(0, 0, 0, 0, printscreen.Size);

            gfx.Dispose();
            graphics.Dispose();
            return printscreen;
        }

        private static void PrintScreenToFile(object a, EventArgs bee)
        {
            MouseEventArgs myArgs = (MouseEventArgs)bee;
            if (myArgs.Button == System.Windows.Forms.MouseButtons.Right)
            {
                return;
            }

            nIco.Icon = b;

            try
            {
                currPath = savePath + System.DateTime.Now.ToString("MM-dd-yy HH.mm.ss ") + rnd.Next(10, 99) + ".png";

                Bitmap I = PrintScreen();
                I.Save(currPath, ImageFormat.Png);
                I.Dispose();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error saving file: " + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            nIco.Icon = w;
        }

        private static void PrintScreenAndPost(object a, object bee)
        {
            nIco.Icon = b;
            Bitmap bit = PrintScreen();
            try
            {
                using (var w = new WebClient())
                {
                    string clientID = "7ee07fb80bd19ec";
                    w.Headers.Add("Authorization", "Client-ID " + clientID);
                    var values = new NameValueCollection
                    {
                        { "image", Convert.ToBase64String(ImageToBArr(bit)) }
                    };

                    byte[] response = w.UploadValues("https://api.imgur.com/3/upload.xml", values);

                    XDocument xml = XDocument.Load(new MemoryStream(response));
                    XElement node = xml.Element("data").Element("link");
                    PS.Post p = new PS.Post(node.Value, bit);
                    p.ShowDialog();
                }
            } catch (Exception e)
            {
                MessageBox.Show("Error uploading file: " + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            nIco.Icon = w;
            bit.Dispose();
        }
        private static void help(object a, object bee)
        {
            MessageBox.Show("PrintScreen is a simple application that captures your screen to a file in your Pictures folder.\nTo use it, simply click/tap on the icon in the system tray.\nTo quit it, just right-click and then hit \"Quit\"", "About PrintScreen", MessageBoxButtons.OK, MessageBoxIcon.None);
        }
        private static void quit(object a, object bee)
        {
            nIco.Visible = false;
            Application.Exit();
        }
        private static void initContextMenu()
        {
            MenuItem PSNameItem = new MenuItem("PrintScreen v1.0");
            MenuItem HelpItem = new MenuItem("Help");
            MenuItem PSItem = new MenuItem("Screenshot");
            MenuItem PSIItem = new MenuItem("Screenshot and post");
            MenuItem QuitItem = new MenuItem("Quit");

            PSNameItem.Enabled = false;

            QuitItem.Click += quit;
            PSItem.Click += PrintScreenToFile;
            PSIItem.Click += PrintScreenAndPost;
            HelpItem.Click += help;

            ContextMenu contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add(PSNameItem);
            contextMenu.MenuItems.Add("-");
            contextMenu.MenuItems.Add(HelpItem);
            contextMenu.MenuItems.Add(PSItem);
            contextMenu.MenuItems.Add(PSIItem);
            contextMenu.MenuItems.Add("-");
            contextMenu.MenuItems.Add(QuitItem);

            nIco.ContextMenu = contextMenu;
        }
    }
}
