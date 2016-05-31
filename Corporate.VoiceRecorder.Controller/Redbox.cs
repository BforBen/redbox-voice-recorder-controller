using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RedBox.RAI;

namespace GuildfordBoroughCouncil.Corporate.VoiceRecorder.Controller
{
    public class IsRecordingChangedEventArgs : EventArgs
    {
        public bool OldValue { get; internal set; }
        public bool NewValue { get; internal set; }

        public IsRecordingChangedEventArgs(bool oldValue, bool newValue)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }
    }

    public class ActiveCallChangedEventArgs : EventArgs
    {
        public bool OldValue { get; internal set; }
        public bool NewValue { get; internal set; }

        public ActiveCallChangedEventArgs(bool oldValue, bool newValue)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }
    }

    public class ChannelsChangedEventArgs : EventArgs
    {
        public List<KeyValuePair<uint, string>> OldValue { get; internal set; }
        public List<KeyValuePair<uint, string>> NewValue { get; internal set; }

        public ChannelsChangedEventArgs(List<KeyValuePair<uint, string>> oldValue, List<KeyValuePair<uint, string>> newValue)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }
    }

    public class Redbox
    {
        private RaiConnection _Recorder;
        private bool _IsConnected = false;
        private uint _Device = 0;
        private uint? _RecordingCallId;

        private bool _ActiveCall;
        private List<KeyValuePair<uint, string>> _Channels;
        
        public bool ActiveCall
        {
            get
            {
                return this._ActiveCall;
            }
            private set
            {
                if (this._ActiveCall != value)
                {
                    bool old = this._ActiveCall;
                    this._ActiveCall = value;
                    OnActiveCallChange(this, new ActiveCallChangedEventArgs(old, value));
                }
            }
        }

        public List<KeyValuePair<uint, string>> Channels
        {
            get
            {
                return this._Channels;
            }
            private set
            {
                if (this._Channels != value)
                {
                    List<KeyValuePair<uint, string>> old = this._Channels;
                    this._Channels = value;
                    OnChannelsChange(this, new ChannelsChangedEventArgs(old, value));
                }
            }
        }

        private bool _IsRecording;

        public bool IsRecording
        {
            get
            {
                return this._IsRecording;
            }
            private set
            {
                if (this._IsRecording != value)
                {
                    bool old = this._IsRecording;
                    this._IsRecording = value;
                    OnIsRecordingChange(this, new IsRecordingChangedEventArgs(old, value));
                }
            }
        }

        public uint Device
        {
            get
            {
                return _Device;
            }
            set
            {
                _Device = value;
            }
        }

        // Delegate
        public delegate void IsRecordingChangeHandler(object sender, IsRecordingChangedEventArgs data);
        // The event
        public event IsRecordingChangeHandler IsRecordingChange;
        // The method which fires the Event
        protected void OnIsRecordingChange(object sender, IsRecordingChangedEventArgs data)
        {
            // Check if there are any Subscribers
            if (IsRecordingChange != null)
            {
                // Call the Event
                IsRecordingChange(this, data);
            }
        }

        // Delegate
        public delegate void ActiveCallChangeHandler(object sender, ActiveCallChangedEventArgs data);
        // The event
        public event ActiveCallChangeHandler ActiveCallChange;
        // The method which fires the Event
        protected void OnActiveCallChange(object sender, ActiveCallChangedEventArgs data)
        {
            // Check if there are any Subscribers
            if (ActiveCallChange != null)
            {
                // Call the Event
                ActiveCallChange(this, data);
            }
        }

        // Delegate
        public delegate void ChannelsChangeHandler(object sender, ChannelsChangedEventArgs data);
        // The event
        public event ChannelsChangeHandler ChannelsChange;
        // The method which fires the Event
        protected void OnChannelsChange(object sender, ChannelsChangedEventArgs data)
        {
            // Check if there are any Subscribers
            if (ChannelsChange != null)
            {
                // Call the Event
                ChannelsChange(this, data);
            }
        }

        public Redbox()
        {
            _Recorder = new RaiConnection(Properties.Settings.Default.RecorderIpAddress, Properties.Settings.Default.RecorderUsername, Properties.Settings.Default.RecorderPassword, RaiLoginType.Extended, false, 0);

            _Recorder.Error += Error;
            _Recorder.LoginFailure += LoginFailure;
            _Recorder.LoginSuccess += LoginSuccess;
            _Recorder.ConnectionFailure += ConnectionFailure;

            _Recorder.GetChannelNamesResponse += GetChannelNamesResponse;

            _Recorder.SuppressionStartResponse += SuppressionStartResponse;
            _Recorder.SuppressionStopResponse += SuppressionStopResponse;
            _Recorder.SubscribedEvents.CallStartRecording += SubscribedEvents_CallStartRecording;
            _Recorder.SubscribedEvents.CallStopRecording += SubscribedEvents_CallStopRecording;

            _Recorder.GetCallDetailsResponse += GetCallDetailsResponse;

            _Recorder.EnableConnection();
        }

        private void GetChannelNamesResponse(object sender, RaiGetChannelNamesResponseArgs e)
        {
            var NewChannels = new List<KeyValuePair<uint, string>>();

            foreach (var c in e.Items)
            {
                NewChannels.Add(new KeyValuePair<uint, string>(c.DeviceId, c.ChannelName));    
            }

            Channels = NewChannels;
        }

        private void GetCallDetailsResponse(object sender, RaiGetCallDetailsResponseArgs e)
        {
            var Device = e.DatabaseRecord.GetIntegerDatabaseField(RaiDatabaseField.DeviceId);

            // Only care if the device of the call matches the one we're interested in
            if (Device.Value == _Device)
            {
                _RecordingCallId = e.CallId;
                IsRecording = true;
                ActiveCall = true;
            }
        }

        private void SubscribedEvents_CallStopRecording(object sender, RaiCallStopRecordingEventArgs e)
        {
            if (e.CallId == _RecordingCallId)
            {
                IsRecording = false;
                ActiveCall = false;
            }
        }

        private void SubscribedEvents_CallStartRecording(object sender, RaiCallStartRecordingEventArgs e)
        {
            _Recorder.GetCallDetails(e.CallId);
        }

        private void SuppressionStartResponse(object sender, RaiSuppressionResponseEventArgs e)
        {
            if (e.Result == RaiSuppressionResult.Started)
            {
                IsRecording = false;
            }
        }

        private void SuppressionStopResponse(object sender, RaiSuppressionResponseEventArgs e)
        {
            if (e.Result != RaiSuppressionResult.Started)
            {
                IsRecording = true;
            }
        }

        private void Error(object sender, RaiErrorEventArgs e) { }

        private void ConnectionFailure(object sender, RaiConnectionFailureEventArgs e)
        {
            _IsConnected = false;
        }

        private void LoginFailure(object sender, RaiLoginFailureEventArgs e)
        {
            _IsConnected = false;
        }

        private void LoginSuccess(object sender, RaiLoginSuccessEventArgs e)
        {
            _IsConnected = true;
            _Recorder.GetChannelNames();
        }

        public bool IsConnected
        {
            get
            {
                return _IsConnected;
            }
        }

        public void SuppressRecording()
        {
            _Recorder.SuppressDeviceStart(_Device, false);
        }

        public void ResumeRecording()
        {
            _Recorder.SuppressDeviceStop(_Device);
        }

        public void CloseConnection()
        {
            _Recorder.DisableConnection();
            _Recorder.Dispose();
        }
    }
}
