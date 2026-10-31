using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace LeigodMonitor
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MonitorForm());
        }
    }

    class MonitorForm : Form
    {
        const string ProcessName = "leigod";
        Label label;
        Timer timer;
        NotifyIcon tray;
        float fontSize = 20;
        Color fontColor = Color.White;
        bool startup;
        string statusText = "雷神：检测中";
        string config = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LeigodMonitor", "config.txt");

        public MonitorForm()
        {
            LoadConfig();
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            BackColor = Color.Magenta;
            TransparencyKey = Color.Magenta;
            AutoSize = false;
            ClientSize = new Size(180, 40);
            label = new Label { AutoSize = true, ForeColor = fontColor, BackColor = Color.Transparent, UseCompatibleTextRendering = true, Font = new Font("Microsoft YaHei UI", fontSize), Text = statusText, Visible = false };
            Controls.Add(label);
            Shown += delegate { Position(); UpdateStatus(); };
            timer = new Timer { Interval = 10000 };
            timer.Tick += delegate { UpdateStatus(); };
            timer.Start();
            tray = new NotifyIcon { Icon = SystemIcons.Application, Visible = true, Text = "雷神监控" };
            tray.ContextMenuStrip = BuildMenu();
            tray.DoubleClick += delegate { Show(); TopMost = true; Position(); };
            FormClosed += delegate { tray.Visible = false; tray.Dispose(); timer.Dispose(); };
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x20 | 0x80000;
                return cp;
            }
        }

        void Position()
        {
            var r = Screen.PrimaryScreen.WorkingArea;
            Location = new Point(r.Right - Width - 20, r.Top + 20);
        }

        void UpdateStatus()
        {
            bool active = Process.GetProcessesByName(ProcessName).Any();
            statusText = active ? "雷神：已开启" : "雷神：未开启";
            fontColor = active ? Color.Red : Color.White;
            label.ForeColor = fontColor;
            TopMost = true;
            Invalidate();
            Position();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
            using (var brush = new SolidBrush(fontColor)) e.Graphics.DrawString(statusText, label.Font, brush, 2, 2);
            base.OnPaint(e);
        }

        ContextMenuStrip BuildMenu()
        {
            var m = new ContextMenuStrip();
            var sizes = new ToolStripMenuItem("字体大小");
            foreach (int n in new[] { 14, 16, 18, 20, 24, 28, 32, 36 })
            {
                var x = new ToolStripMenuItem(n + " px") { Tag = n, Checked = Math.Abs(fontSize - n) < 0.1f };
                x.Click += delegate(object s, EventArgs e) { SetFontSize((int)((ToolStripMenuItem)s).Tag); };
                sizes.DropDownItems.Add(x);
            }
            m.Items.Add(sizes);
            var boot = new ToolStripMenuItem("开机自动启动") { Checked = startup, CheckOnClick = true };
            boot.Click += delegate(object s, EventArgs e) { startup = ((ToolStripMenuItem)s).Checked; SetStartup(startup); SaveConfig(); };
            m.Items.Add(boot);
            m.Items.Add(new ToolStripSeparator());
            var quit = new ToolStripMenuItem("退出");
            quit.Click += delegate { Close(); };
            m.Items.Add(quit);
            return m;
        }

        void SetFontSize(int n)
        {
            fontSize = n;
            label.Font = new Font(label.Font.FontFamily, fontSize);
            ClientSize = new Size(label.PreferredWidth + 4, label.PreferredHeight + 4);
            Position();
            SaveConfig();
            var old = tray.ContextMenuStrip;
            tray.ContextMenuStrip = BuildMenu();
            old.Dispose();
        }

        void LoadConfig()
        {
            try
            {
                if (!File.Exists(config)) return;
                foreach (var line in File.ReadAllLines(config))
                {
                    if (line.StartsWith("FontSize=")) float.TryParse(line.Substring(9), out fontSize);
                    if (line.StartsWith("Startup=")) bool.TryParse(line.Substring(8), out startup);
                }
                if (fontSize < 8 || fontSize > 100) fontSize = 20;
            }
            catch { fontSize = 20; startup = false; }
        }

        void SaveConfig()
        {
            try { Directory.CreateDirectory(Path.GetDirectoryName(config)); File.WriteAllLines(config, new[] { "FontSize=" + fontSize, "Startup=" + startup }); } catch { }
        }

        void SetStartup(bool enabled)
        {
            try
            {
                using (var k = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    if (enabled) k.SetValue("LeigodMonitor", "\"" + Application.ExecutablePath + "\"");
                    else k.DeleteValue("LeigodMonitor", false);
                }
            }
            catch { }
        }
    }
}
