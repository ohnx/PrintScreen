using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace PS
{
    static class PSmain
    {
        static NotifyIcon nIco;
        static string savePath, currPath;
        static System.Random rnd;
        static Rectangle screenCombined;
        static Graphics gfx = Graphics.FromHwnd(System.IntPtr.Zero);
        static Icon b, w;

        static void Main(string[] args)
        {
            w = new Icon(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("PS.cam-w.ico"));
            b = new Icon(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("PS.cam-b.ico"));
            rnd = new System.Random();
            savePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures) + "\\Screenshots\\Screenshot ";
            nIco = new NotifyIcon();
            nIco.Icon = w;
            nIco.Visible = true;
            initContextMenu();
            nIco.Click += PrintScreen;
            //nIco.ShowBalloonTip(100, "PrintScreen launched!", "Right-click the icon in the system tray to configure it.", ToolTipIcon.Info);
            Application.Run();
        }
        private static void PrintScreen(object a, object bee)
        {
            nIco.Icon = b;
            try
            {
                screenCombined = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
                foreach (Screen screen in Screen.AllScreens)
                {
                    screenCombined = Rectangle.Union(screenCombined, screen.Bounds);
                }

                Bitmap printscreen = new Bitmap(screenCombined.Width, screenCombined.Height);
                Graphics graphics = Graphics.FromImage(printscreen as Image);
                graphics.CopyFromScreen(0, 0, 0, 0, printscreen.Size);

                currPath = savePath + System.DateTime.Now.ToString("MM-dd-yy HH.mm.ss ") + rnd.Next(10, 99) + ".png";

                printscreen.Save(currPath, ImageFormat.Png);
            }
            catch (System.Exception e)
            {
                MessageBox.Show("Error saving file: " + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            nIco.Icon = w;
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
            MenuItem QuitItem = new MenuItem("Quit");

            PSNameItem.Enabled = false;

            QuitItem.Click += quit;
            PSItem.Click += PrintScreen;
            HelpItem.Click += help;

            ContextMenu contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add(PSNameItem);
            contextMenu.MenuItems.Add("-");
            contextMenu.MenuItems.Add(HelpItem);
            contextMenu.MenuItems.Add(PSItem);
            contextMenu.MenuItems.Add("-");
            contextMenu.MenuItems.Add(QuitItem);

            nIco.ContextMenu = contextMenu;
        }
    }
}
