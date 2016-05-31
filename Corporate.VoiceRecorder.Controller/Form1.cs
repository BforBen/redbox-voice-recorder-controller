using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Windows.Automation;
using System.Runtime.InteropServices;

namespace GuildfordBoroughCouncil.Corporate.VoiceRecorder.Controller
{
    public partial class Form1 : Form
    {
        private Redbox Recorder;

        AutomationFocusChangedEventHandler focusHandler;
        
        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        private delegate void stringDelegate(string s);
        private delegate void boolDelegate(bool State);

        private static string GetText(IntPtr hWnd)
        {
            // Allocate correct string length first
            int length = GetWindowTextLength(hWnd);
            StringBuilder sb = new StringBuilder(length + 1);
            GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        public Form1()
        {
            InitializeComponent();
            TrayIcon.Icon = Properties.Resources.Power;
            this.WindowState = FormWindowState.Minimized;

            Recorder = new Redbox();
            Recorder.ChannelsChange += Recorder_ChannelsChange;
        }

        private void Recorder_ActiveCallChange(object sender, ActiveCallChangedEventArgs data)
        {
            if (data.NewValue)
            {
                Automation.AddAutomationFocusChangedEventHandler(focusHandler);
            }
            else
            {
                Automation.RemoveAutomationFocusChangedEventHandler(focusHandler);
            }
            SetRecording();
        }

        private void Recorder_IsRecordingChange(object sender, IsRecordingChangedEventArgs data)
        {
            SetRecording();
        }

        private void Recorder_ChannelsChange(object sender, ChannelsChangedEventArgs data)
        {
            if (ExtensionList.InvokeRequired)
            {
                // If so, call Invoke, passing it a lambda expression which calls
                // UpdateText with the same label and text, but on the UI thread instead.
                ExtensionList.Invoke((Action)(() => UpdateChannels(data.NewValue)));
                return;
            }
        }
        
        private void OnFocusChanged(object src, AutomationFocusChangedEventArgs e)
        {
            try
            {
                // Get current control that has focus
                AutomationElement focusedElement = src as AutomationElement;

                // Get it's name (may be useful)
                //string controlName = focusedElement.Current.Name;

                // Get the handle to this control
                IntPtr hwnd = (IntPtr)focusedElement.Current.NativeWindowHandle;

                this.Invoke((MethodInvoker)delegate()
                {
                    this.Text = hwnd.ToString();
                });

                // If focus has changed the first thing we should do for quickness is to see if the current active window is the one we want
                // Check if the windows title of the window this control belongs to has a title of "Print"
                CheckWindow(GetText(GetActiveWindow()));


                // While we have a handle that isn't the desktop (very top) then keep stepping up to see if we find the Print Dialog
                while (hwnd != IntPtr.Zero)
                {
                    // Check if the windows title of the window this control belongs to has a title of "Print"
                    CheckWindow(GetText(hwnd));

                    // Get parent control
                    hwnd = GetParent(hwnd);
                }
            }
            catch (InvalidOperationException) { }
        }

        private void UpdateChannels(List<KeyValuePair<uint, string>> Channels)
        {
            ExtensionList.DataSource = new BindingSource(Channels.OrderBy(k => k.Value), null);
            ExtensionList.DisplayMember = "Value";
            ExtensionList.ValueMember = "Key";
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
        }

        private void CheckWindow(string WindowTitle)
        {
            string Title = WindowTitle.ToLower();

            bool ActivePaymentsWindow = (Title.Contains("civica") || Title.Contains("autorityicon") || Title.Contains("webpay") || Title.Contains("payments") || Title.Contains("www2.guildford.gov.uk") || Title == "www2.guildford.gov.uk");

            if (ActivePaymentsWindow && Recorder.IsRecording)
            {
                Recorder.SuppressRecording();
            }
            else if (!ActivePaymentsWindow && !String.IsNullOrWhiteSpace(Title) && !Recorder.IsRecording)
            {
                Recorder.ResumeRecording();
            }
        }
        
        private void SetRecording()
        {
            if (Recorder.ActiveCall)
            {
                if (Recorder.IsRecording)
                {
                    TrayIcon.Icon = Properties.Resources.VolumeUp;
                    TrayIcon.Text = "Recording";
                    TrayIcon.ShowBalloonTip(4000, "Voice Recorder", "Current call is being recording", ToolTipIcon.Info);
                }
                else
                {
                    TrayIcon.Icon = Properties.Resources.Mute;
                    TrayIcon.Text = "Recording suppressed";
                    TrayIcon.ShowBalloonTip(4000, "Voice Recorder", "Current call is not being recorded.", ToolTipIcon.Info);
                }
            }
            else
            {
                TrayIcon.Icon = Properties.Resources.Power;
                TrayIcon.Text = "No active call";
                TrayIcon.ShowBalloonTip(1, "Voice Recorder", "No active call.", ToolTipIcon.Info);
            }
        }

        #region Menu Items

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ChangeDeviceMenuItem_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
        }

        #endregion

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Catch ALT + F4 etc and prevent it otherwise shut down the connection to Redbox
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
            }
            else
            {
                Recorder.CloseConnection();
            }
        }

        private void SetExtension_Click(object sender, EventArgs e)
        {
            focusHandler = new AutomationFocusChangedEventHandler(OnFocusChanged);
            Recorder.Device = ((KeyValuePair<uint, string>)ExtensionList.SelectedItem).Key;

            Recorder.IsRecordingChange += Recorder_IsRecordingChange;
            Recorder.ActiveCallChange += Recorder_ActiveCallChange;
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
        }

    }
}
