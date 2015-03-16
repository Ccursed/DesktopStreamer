using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopStreamer
{
    #region Delegates
    
    public delegate void InstanceFinishedHandler(LivestreamerWrapper instance);
    public delegate void InstanceStartedHandler(LivestreamerWrapper instance);
    public delegate void InstanceStateChangeHandler(LivestreamerWrapper instance, LivestreamerWrapper.Status oldState, LivestreamerWrapper.Status newState);
    #endregion

    public class LivestreamerWrapper
    {
        #region Events
        
        public event InstanceFinishedHandler instanceFinished;
        public event InstanceStartedHandler instanceStarted;
        public event InstanceStateChangeHandler instanceChangedState;

        #endregion

        #region Static Variables

        private static int instanceCount = 0;
        private static string livestreamerExecutable;
        private static string livestreamerDirectory;
        #endregion

        #region Member Variables

        public int id { get; private set; }
        public string name { get; private set; }
        public Logger log { get; private set; }
        public Process lsInstance;
        public ProcessStartInfo lsStartInfo;

        private Status state;
        public Status State
        {
            get { return state; } 
            private set
            {
                Status old = state;
                state = value;
                if(instanceChangedState != null) instanceChangedState(this, old, state);
            }
        }

        Quality quality { get; set; }
        string playerPath { get; set; }
        string streamUrl { get; set; }
        public Dictionary<Argument, string> dctArguments { get; private set; }
        #endregion

        #region Enums

        public enum Quality
        {
            Low, Medium, High, Best
        };

        public enum Argument
        {

        };

        public enum Status
        {
            Idle, Starting, Working, Error, Finished
        };
        #endregion

        #region Static Creation Stuff

        public static void Init(string livestreamerDirectory)
        {
            LivestreamerWrapper.livestreamerDirectory = livestreamerDirectory;
            LivestreamerWrapper.livestreamerExecutable = livestreamerDirectory + @"\livestreamer.exe";
        }

        public static LivestreamerWrapper CreateInstance(string name)
        {
            if (string.IsNullOrEmpty(livestreamerExecutable)) throw new Exception("LiveStreamWrapper.CreateInstance. Tried to create a lsInstance without initializing the wrapper!");
            return new LivestreamerWrapper(instanceCount++, string.IsNullOrEmpty(name) ? instanceCount.ToString() : name);
        }

        public static LivestreamerWrapper CreateInstance()
        {
            return CreateInstance(null);
        }

        public static string CreateStartParameter(string streamUrl, Quality quality, string playerpath, Dictionary<Argument, string> dctArguments)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("\"" + streamUrl + "\" ");
            switch (quality)
            {
                case Quality.Low: builder.Append("low "); break;
                case Quality.Medium: builder.Append("medium "); break;
                case Quality.High: builder.Append("high "); break;
                case Quality.Best: builder.Append("best "); break;
                default: builder.Append("best "); break;
            }
            builder.Append("-p \"" + playerpath + "\" ");
            return builder.ToString();
        }
        #endregion

        #region Instance Stuff

        private LivestreamerWrapper(int id, string name)
        {
            this.id = id;
            this.name = name;
            this.lsInstance = new Process();
            this.lsStartInfo = new ProcessStartInfo();
            this.dctArguments = new Dictionary<Argument, string>();
            this.log = new Logger();
            this.state = Status.Idle;
            CreateLsProcessInfo();
        }

        private void CreateLsProcessInfo()
        {
            lsStartInfo.CreateNoWindow = true;
            lsStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            lsStartInfo.WorkingDirectory = livestreamerDirectory;
            lsStartInfo.FileName = "livestreamer.exe";
            lsStartInfo.RedirectStandardOutput = true;
            lsStartInfo.RedirectStandardError = true;
            lsStartInfo.UseShellExecute = false;

            lsInstance.EnableRaisingEvents = true;
            lsInstance.StartInfo = lsStartInfo;
            lsInstance.OutputDataReceived += CatchProcessOutput;
            lsInstance.Exited += CatchProcessExit;
        }

        public void Start()
        {
            lsInstance.Start();
            state = Status.Starting;
            lsInstance.BeginOutputReadLine();
            if(instanceStarted != null) instanceStarted(this);
        }

        public string SetArguments(string args)
        {
            lsStartInfo.Arguments = args;
            return lsStartInfo.Arguments;
        }

        public void Close()
        {
            lsInstance.Close();
        }

        public bool HasFinished()
        {
            return state == Status.Finished;
        }

        public bool HasStarted()
        {
            return state == Status.Working;
        }

        #region EventHandling

        private void CatchProcessOutput(object sender, DataReceivedEventArgs args)
        {
            if (args.Data == null) return;
            log.Add(args.Data);
            if (args.Data.StartsWith(@"error:")) State = Status.Error;
            else if (args.Data.StartsWith(@"[cli][info] Found matching plugin")) State = Status.Starting;
            else if (args.Data.StartsWith(@"[cli][info] Starting player")) State = Status.Working;
            else if (args.Data.StartsWith(@"[cli][info] Player closed")) State = Status.Finished;
        }

        private void CatchProcessExit(object sender, EventArgs args)
        {
            state = Status.Finished;
            if(instanceFinished != null) instanceFinished(this);
        }
        #endregion

        #endregion
    }
}
