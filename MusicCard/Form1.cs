using AxWMPLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SlimDX.DirectSound;
using SlimDX.Multimedia;
using System.IO;
using System.Windows.Forms.VisualStyles;
using System.Runtime.CompilerServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Threading;
using System.Runtime.Versioning;
using System.Runtime.InteropServices;

namespace MusicCard
{
    public partial class Form1 : Form
    {
        public static string PATH = "C:\\Users\\Marcin\\Desktop\\music.wav";
        public static string GENERATE_PATH = "C:\\Users\\Marcin\\Desktop\\new.wav";
        private static double currentPlayTime; 
        private static bool isPaused = false;
        private static SecondarySoundBuffer soundBuffer = null;

        private OpenFileDialog dialog1 = new OpenFileDialog();
        private Mp3Player _mp3Player;
        private Mp3Player _mp3Player1;

        private System.Windows.Forms.TextBox textBox;

        public Form1()
        {
            InitializeComponent();
            
        }

        [DllImport("winmm.DLL", EntryPoint = "PlaySound", SetLastError = true, CharSet = CharSet.Unicode, ThrowOnUnmappableChar = true)]
        private static extern bool PlaySound(string szSound, IntPtr hMod, PlaySoundFlags flags);

        public enum PlaySoundFlags : int
        {   
            SND_SYNC = 0x0000,
            SND_ASYNC = 0x0001,
            SND_NODEFAULT = 0x0002,
            SND_LOOP = 0x0008,
            SND_NOSTOP = 0x0010,
            SND_NOWAIT = 0x00002000,
            SND_FILENAME = 0x00020000,
            SND_RESOURCE = 0x00040004
        }


        private void axWindowsMediaPlayer1_Enter(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.URL = dialog1.FileName;

            if (isPaused)
            {
                isPaused = false;
                axWindowsMediaPlayer1.Ctlcontrols.currentPosition = currentPlayTime;
                axWindowsMediaPlayer1.Ctlcontrols.play();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.stop();
            currentPlayTime = 0;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!isPaused)
            {
                axWindowsMediaPlayer1.Ctlcontrols.pause();
                currentPlayTime = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
                isPaused = true;
            }
        }


        private void button4_Click(object sender, EventArgs e)
        {
            if (isPaused)
            {
                soundBuffer.CurrentPlayPosition = (int) currentPlayTime;
                soundBuffer.Play(0, PlayFlags.None);
                isPaused = false;
                return;
            }

            DirectSound directSound = new DirectSound(DirectSoundGuid.DefaultPlaybackDevice);
            directSound.SetCooperativeLevel(this.Handle, CooperativeLevel.Priority);

            using (WaveStream waveStream = new WaveStream(dialog1.FileName))
            {
                byte[] buffer = new byte[waveStream.Length];

                SoundBufferDescription description = new SoundBufferDescription();
                description.Format = waveStream.Format;
                description.Flags = BufferFlags.GlobalFocus | BufferFlags.ControlEffects;
                description.SizeInBytes = (int)waveStream.Length;

                waveStream.Read(buffer, 0, (int)waveStream.Length);

                soundBuffer = new SecondarySoundBuffer(directSound, description);
                soundBuffer.Write(buffer, 0, LockFlags.None);
                soundBuffer.Play(0, PlayFlags.None);

            }


        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (soundBuffer == null)
                return;

            if (checkBox1.Checked)
            {
                Guid flanger = SoundEffectGuid.StandardFlanger;
                Guid chorus = SoundEffectGuid.StandardChorus;
                Guid[] effects = new Guid[] {flanger, chorus};

                currentPlayTime = soundBuffer.CurrentPlayPosition;
                soundBuffer.Stop();
                soundBuffer.SetEffects(effects);
                soundBuffer.CurrentPlayPosition = (int)currentPlayTime;
                soundBuffer.Play(0, PlayFlags.None);
            }
            else
            {
                Guid[] effects = new Guid[] {};

                currentPlayTime = soundBuffer.CurrentPlayPosition;
                soundBuffer.Stop();
                soundBuffer.SetEffects(effects);
                soundBuffer.CurrentPlayPosition = (int)currentPlayTime;
                soundBuffer.Play(0, PlayFlags.None);
            }

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (soundBuffer != null)
            {
                soundBuffer.Stop();
                currentPlayTime = 0;
                isPaused = true;
            }
                
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (soundBuffer != null)
            {
                currentPlayTime = soundBuffer.CurrentPlayPosition;
                soundBuffer.Stop();
                isPaused = true;
            }
                
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            if(_mp3Player != null)  
                _mp3Player.Dispose();   
            if(_mp3Player1 != null)
                _mp3Player1.Dispose();  

            dialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            dialog1.Filter = "Wav Files (*.wav)|*.wav";
            dialog1.ShowDialog();

            _mp3Player = new Mp3Player(dialog1.FileName);
            //_mp3Player.Repeat = false;
            _mp3Player.Repeat = true;
            textBox1.Text = dialog1.SafeFileName;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            PlaySound(dialog1.FileName, new System.IntPtr(), PlaySoundFlags.SND_ASYNC);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            PlaySound(null, new System.IntPtr(), PlaySoundFlags.SND_SYNC);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            //_mp3Player.Repeat = false;
            if (_mp3Player != null)
                _mp3Player.Play();
            // _mp3Player.Repeat = true;

        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (_mp3Player != null)
                _mp3Player.Stop();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (_mp3Player != null)
                _mp3Player.Dispose();
            _mp3Player1 = new Mp3Player();
            _mp3Player1.Record();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            //if (_mp3Player == null)
            _mp3Player1.StopRecord();
            _mp3Player = new Mp3Player(dialog1.FileName);
            _mp3Player.Repeat = true;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button14_Click(object sender, EventArgs e)
        {
            _mp3Player.Stop();
            _mp3Player.Dispose();
            _mp3Player = new Mp3Player(dialog1.FileName);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void button15_Click(object sender, EventArgs e)
        {
            int duration = 3; // in seconds

            WaveFormat format = new WaveFormat();
            format.BitsPerSample = 16;
            format.BlockAlignment = 4;
            format.FormatTag = WaveFormatTag.Pcm;
            format.Channels = 2;
            format.BitsPerSample = 8;
            format.SamplesPerSecond = 44100;
            format.AverageBytesPerSecond = format.SamplesPerSecond * format.BlockAlignment;

            int sineWaveFreq = 1000;    // in Hz
            int bufferLength = 3 * format.SamplesPerSecond;

            byte[] buffer = new byte[bufferLength];

            for (int i = 0; i < bufferLength; i++)
            {
                buffer[i] = (byte)Math.Sin(2 * Math.PI * sineWaveFreq * i);
            }


            DirectSound directSound = new DirectSound(DirectSoundGuid.DefaultPlaybackDevice);
            directSound.SetCooperativeLevel(this.Handle, CooperativeLevel.Priority);
               
            

            using (WaveStream waveStream = new WaveStream(GENERATE_PATH))
            {
                /*
                byte[] buffer = new byte[waveStream.Length];

                SoundBufferDescription description = new SoundBufferDescription();
                description.Format = waveStream.Format;
                description.Flags = BufferFlags.GlobalFocus | BufferFlags.ControlEffects;
                description.SizeInBytes = (int)waveStream.Length;

                waveStream.Read(buffer, 0, (int)waveStream.Length);

                soundBuffer = new SecondarySoundBuffer(directSound, description);
                soundBuffer.Write(buffer, 0, LockFlags.None);
                soundBuffer.Play(0, PlayFlags.None);
                */
            }
        }

    }
}