using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DS4Windows
{
    internal class SoundUtils
    {
        #region Init
        public SoundUtils()
        {
            WaveStreamGenerated += Play;
        }
        #endregion

        #region Variables 
        private static readonly bool UseTestWav = false;
        private static bool IsPlaying = false;
        
        private static readonly MemoryStream WavStream = new();
        private readonly static SoundPlayer TonePlayer = new();
        #endregion

        #region Events/Delegates
        private delegate void StreamData();
        private static event StreamData WaveStreamGenerated;
        #endregion

        #region Sinewave
        internal static void SineGenerator(double frequency, int LengthInSeconds = 1, int SampleRate = 8000)
        {
            if (IsPlaying)
            {
                return;
            }
            if (File.Exists("Test.wav") && UseTestWav == true)
            {
                TonePlayer.SoundLocation = "Test.wav";
            }

            BinaryWriter WavStreamWriter = new(WavStream);

            int _bufferSize = (SampleRate * LengthInSeconds);
            short[] _dataBuffer;
            _dataBuffer = new short[_bufferSize];
            double _amplitude = (short.MaxValue / 2);

            for (int n = 0; n < _dataBuffer.Length; n++)
            {
                _dataBuffer[n] = (short)(_amplitude * Math.Sin((2 * Math.PI * n * frequency) / SampleRate));
            }
            byte[] _byteData = new byte[_dataBuffer.Length * 2];

            WriteWavHeaderStream(WavStream, 2, SampleRate, Buffer.ByteLength(_byteData));
            Buffer.BlockCopy(_dataBuffer, 0, _byteData, 0, _byteData.Length);

            WavStreamWriter.Write(_byteData, 0, _byteData.Length);

            WavStream.Seek(0, SeekOrigin.Begin);

            TonePlayer.Stream = WavStream;

            WaveStreamGenerated?.Invoke();
        }

        internal static void Play()
        {
            TonePlayer.Play();
            IsPlaying = true;
        }

        internal static void Stop()
        {
            TonePlayer.Stop();
            IsPlaying = false;
        }

        #endregion

        #region WavHeader
        /**
        char riff[4];           * "RIFF"                                  *
        long flength;           * file length in bytes                    *
        char wave[4];           * "WAVE"                                  *
        char fmt[4];            * "fmt "                                  *
        long chunk_size;        * size of FMT chunk in bytes (usually 16) *
        short format_tag;       * 1=PCM, 257=Mu-Law, 258=A-Law, 259=ADPCM *
        short num_chans;        * 1=mono, 2=stereo                        *
        long srate;             * Sampling rate in samples per second     *
        long bytes_per_sec;     * bytes per second = srate*bytes_per_samp *
        short bytes_per_samp;   * 2=16-bit mono, 4=16-bit stereo          *
        short bits_per_samp;    * Number of bits per sample               *
        char data[4];           * "data"                                  *
        long dlength;           * data length in bytes (filelength - 44)  *
        **/
        private static void WriteWavHeaderStream(Stream stream, int ChannelCount, int SampleRate, int LengthInBytes, int BitDepth = 16, bool isFloatingPoint = false)
        {
            stream.Seek(0, SeekOrigin.Begin);

            stream.Write(Encoding.ASCII.GetBytes("RIFF"), 0, 4); // RIFF header/Chunk ID.
            stream.Write(BitConverter.GetBytes((LengthInBytes) + 36), 0, 4); // Chunk size.
            stream.Write(Encoding.ASCII.GetBytes("WAVE"), 0, 4); // Format.
            stream.Write(Encoding.ASCII.GetBytes("fmt "), 0, 4); //  Sub-chunk 1/Sub-chunk 1 ID.
            stream.Write(BitConverter.GetBytes(16), 0, 4); // Sub-chunk 1 size.
            stream.Write(BitConverter.GetBytes((short)(isFloatingPoint ? 3 : 1)), 0, 2); // Audio format (floating point (3) or PCM (1)). Any other format indicates compression.
            stream.Write(BitConverter.GetBytes((short)ChannelCount), 0, 2); // Channels.
            stream.Write(BitConverter.GetBytes(SampleRate), 0, 4); // Sample rate.
            stream.Write(BitConverter.GetBytes(SampleRate * ChannelCount * (BitDepth / 8)), 0, 4); // Bytes rate.
            stream.Write(BitConverter.GetBytes((short)ChannelCount * (BitDepth / 8)), 0, 2); // Block align.
            stream.Write(BitConverter.GetBytes((short)BitDepth), 0, 2); // Bits per sample.
            stream.Write(Encoding.ASCII.GetBytes("data"), 0, 4); // Sub-chunk 2/Sub-chunk 2 ID.
            stream.Write(BitConverter.GetBytes(LengthInBytes), 0, 4); // Sub-chunk 2 size.
        }
        /**
        private static void WriteWavHeaderBinary(Stream stream, int ChannelCount, int SampleRate, int LengthInBytes, int BitDepth = 16, bool isFloatingPoint = false)
        {
            BinaryWriter bw = new(stream);

            bw.Write("RIFF".ToCharArray()); // RIFF header/Chunk ID.
            bw.Write(LengthInBytes + 36); // Chunk size.
            bw.Write("WAVE".ToCharArray()); // Format.
            bw.Write("fmt ".ToCharArray()); //  Sub-chunk 1/Sub-chunk 1 ID.
            bw.Write(16); // Sub-chunk 1 size.
            bw.Write((short)(isFloatingPoint ? 3 : 1)); // Audio format (floating point (3) or PCM (1)). Any other format indicates compression.
            bw.Write((short)ChannelCount); // Channels.
            bw.Write(SampleRate); // Sample rate.
            bw.Write((SampleRate * ((BitDepth * ChannelCount) / 8))); // Bytes rate.
            bw.Write((short)((BitDepth * ChannelCount) / 8)); // Block align.
            bw.Write((short)BitDepth); // Bits per sample.
            bw.Write("data".ToCharArray()); // Sub-chunk 2/Sub-chunk 2 ID.
            bw.Write(LengthInBytes); // Sub-chunk 2 size.
        } **/
        #endregion


    }
}
