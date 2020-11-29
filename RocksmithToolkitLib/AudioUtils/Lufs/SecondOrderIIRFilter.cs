using System;

/*
 *    This file is a part of the LUFS metering utils
 *    Copyright (C) 2020  Xuan525
 *
 *    This program is free software: you can redistribute it and/or modify
 *    it under the terms of the GNU General Public License as published by
 *    the Free Software Foundation, either version 3 of the License, or
 *    (at your option) any later version.
 *
 *    This program is distributed in the hope that it will be useful,
 *    but WITHOUT ANY WARRANTY; without even the implied warranty of
 *    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *    GNU General Public License for more details.
 *
 *    You should have received a copy of the GNU General Public License
 *    along with this program.  If not, see <https://www.gnu.org/licenses/>.
 *    
 *    Email : shanboxuan@me.com
 *    Github : https://github.com/xuan525
*/

namespace RocksmithToolkitLib.AudioUtils.Lufs
{
    /// <summary>
    /// Porting from : https://github.com/klangfreund/LUFSMeter/blob/master/filters/SecondOrderIIRFilter.cpp
    /// </summary>
    public class SecondOrderIIRFilter
    {
        private readonly double B0At48k, B1At48k, B2At48k, A1At48k, A2At48k;
        private double B0, B1, B2, A1, A2;
        private readonly double Q, VH, VB, VL, ArctanK;
        private int NumChannels;
        private double[] Z1;
        private double[] Z2;

        /// <summary>
        /// Constructor of SecondOrderIIRFilter
        /// </summary>
        /// <param name="b0At48k"></param>
        /// <param name="b1At48k"></param>
        /// <param name="b2At48k"></param>
        /// <param name="a1At48k"></param>
        /// <param name="a2At48k"></param>
        public SecondOrderIIRFilter(double b0At48k, double b1At48k, double b2At48k, double a1At48k, double a2At48k)
        {
            B0At48k = b0At48k;
            B1At48k = b1At48k;
            B2At48k = b2At48k;
            A1At48k = a1At48k;
            A2At48k = a2At48k;
            NumChannels = 0;

            double KoverQ = (2.0 - 2.0 * A2At48k) / (A2At48k - A1At48k + 1.0);
            double K = Math.Sqrt((A1At48k + A2At48k + 1.0) / (A2At48k - A1At48k + 1.0));
            Q = K / KoverQ;
            ArctanK = Math.Atan(K);
            VB = (B0At48k - B2At48k) / (1.0 - A2At48k);
            VH = (B0At48k - B1At48k + B2At48k) / (A2At48k - A1At48k + 1.0);
            VL = (B0At48k + B1At48k + B2At48k) / (A1At48k + A2At48k + 1.0);
        }

        /// <summary>
        /// Prepare for processing
        /// </summary>
        /// <param name="sampleRate">Sample rate of sample data</param>
        /// <param name="numChannels">Number of channels of sample data</param>
        public void Prepare(double sampleRate, int numChannels)
        {
            NumChannels = numChannels;
            Z1 = new double[numChannels];
            Z2 = new double[numChannels];

            double sampleRate48k = 48000.0;
            if (sampleRate == sampleRate48k)
            {
                B0 = B0At48k;
                B1 = B1At48k;
                B2 = B2At48k;
                A1 = A1At48k;
                A2 = A2At48k;
            }
            else
            {
                double K = Math.Tan(ArctanK * sampleRate48k / sampleRate);
                double commonFactor = 1.0 / (1.0 + K / Q + K * K);
                B0 = (VH + VB * K / Q + VL * K * K) * commonFactor;
                B1 = 2.0 * (VL * K * K - VH) * commonFactor;
                B2 = (VH - VB * K / Q + VL * K * K) * commonFactor;
                A1 = 2.0 * (K * K - 1.0) * commonFactor;
                A2 = (1.0 - K / Q + K * K) * commonFactor;
            }
        }

        /// <summary>
        /// Process a buffer of samples
        /// </summary>
        /// <param name="buffer">The buffer need to be processed</param>
        public void ProcessBuffer(double[][] buffer)
        {
            int numOfChannels = Math.Min(NumChannels, buffer.Length);

            for (int channel = 0; channel < numOfChannels; channel++)
            {
                double[] samples = buffer[channel];

                for (int i = 0; i < buffer[channel].Length; i++)
                {
                    double inVal = samples[i];

                    double factorForB0 = inVal - A1 * Z1[channel] - A2 * Z2[channel];
                    double outVal = B0 * factorForB0 + B1 * Z1[channel] + B2 * Z2[channel];

                    Z2[channel] = Z1[channel];
                    Z1[channel] = factorForB0;

                    samples[i] = outVal;
                }
            }
        }
    }
}
