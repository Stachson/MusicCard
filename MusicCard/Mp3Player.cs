using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicCard
{
    class Mp3Player : IDisposable
    {
        public bool Repeat { get; set; }
        private OpenFileDialog dialog2;

        [DllImport("winmm.dll")]//, EntryPoint = "mciSendStringA", ExactSpelling = true, CharSet = CharSet.Ansi,SetLastError = true  )]
        private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);

        public Mp3Player(string fileName)
        {
            const string FORMAT = @"open ""{0}"" type mpegvideo alias MediaFile";
            string command = String.Format(FORMAT, fileName); 
            mciSendString(command, null, 0, IntPtr.Zero);
        }

        public Mp3Player()
        {
            mciSendString("Open new Type waveaudio alias MediaFile", null, 0, IntPtr.Zero);
        }

        public void Play()
        {
            string command = "play MediaFile";
            if (Repeat)
                command += " REPEAT";
            mciSendString(command, null, 0, IntPtr.Zero);
            // Repeat = false;
        }

        public void Stop()
        {
            string command = "stop MediaFile";
            mciSendString(command, null, 0, IntPtr.Zero);
        }

        public void Record()
        {
            // mciSendString("Open new Type waveaudio alias MediaFile", null, 0, IntPtr.Zero);
            string command = "record MediaFile";
            mciSendString(command, null, 0, IntPtr.Zero);
        }

        public void StopRecord()
        {
            string command = "save MediaFile c:\\TestRecord\\result.wav";
            mciSendString(command, null, 0, IntPtr.Zero);
            command = "close MediaFile ";
            mciSendString(command, null, 0, IntPtr.Zero);
        }



        public void Dispose()
        {
            string command = "close MediaFile ";
            mciSendString(command, null, 0, IntPtr.Zero);
        }
    }
}

