using System.Runtime.InteropServices;

namespace devkit2.Common
{
    public class IconUtil
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);

        public static Icon MakeOverlay(Icon baseIcon, Image overlay, float overlayPercent = 0.6f)
        {
            int width = baseIcon.Width;
            int height = baseIcon.Height;

            Bitmap bmp = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);

                // vẽ icon gốc
                g.DrawIcon(baseIcon, 0, 0);

                // tính size theo %
                int overlayWidth = (int)(width * overlayPercent);
                int overlayHeight = (int)(height * overlayPercent);

                // vẽ giữa
                int x = (width - overlayWidth) / 2 + 1;
                int y = (height - overlayHeight) / 2 + 1;
                //int x = 0;
                //int y = height - overlayHeight;

                g.DrawImage(overlay, new Rectangle(x, y, overlayWidth, overlayHeight));
            }

            // convert bitmap → icon (fix leak)
            IntPtr hIcon = bmp.GetHicon();
            using (Icon temp = Icon.FromHandle(hIcon))
            {
                Icon finalIcon = (Icon)temp.Clone();
                DestroyIcon(hIcon);
                return finalIcon;
            }
        }
    }
}
