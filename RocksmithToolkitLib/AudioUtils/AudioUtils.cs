using System;
using System.IO;
using System.Text;

using RocksmithToolkitLib.AudioUtils.Lufs;
using RocksmithToolkitLib.AudioUtils.Wav;

namespace RocksmithToolkitLib.AudioUtils
{
    public static class AudioUtilities
    {
        private const decimal defaultLoadness = -7;

        public static decimal GetLoudness(string fileName, Action<double,double> updateProgress = null)
        {
            if (Path.GetExtension(fileName) != ".wav")
                return defaultLoadness;

            try {
                WavReader wavReader;

                using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    wavReader = new WavReader(fileStream, Encoding.Default);
                }

                WavReader.FmtChunk fmt = (WavReader.FmtChunk)wavReader.Riff.Chunks["fmt "];
                double[][] buffer = wavReader.GetSampleData();
            
                R128LufsMeter r128LufsMeter = new R128LufsMeter();
                r128LufsMeter.Prepare(fmt.SampleRate, buffer.Length);
                r128LufsMeter.StartIntegrated();
                r128LufsMeter.ProcessBuffer(buffer,
                    (double current, double total) => { updateProgress?.Invoke(current, total); });
                r128LufsMeter.StopIntegrated();

                // Report input loudness
                return -16 - Convert.ToDecimal(r128LufsMeter.IntegratedLoudness);
            }
            catch (Exception ex) {
                return defaultLoadness;
            }
        }
    }
}
