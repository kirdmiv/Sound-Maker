using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthesizer
    {
        public class Element
        {
            int length;
            int start;
            double frequency;
            double compressor;
            public Element(double frequency, double compressor, double start, double length, int sampleRate)
            {
                this.frequency = Math.PI * 2 * frequency / sampleRate;
                this.start = (int)(start * sampleRate);
                this.length = (int)(length * sampleRate);
                this.compressor = compressor / sampleRate;
            }
            public void Get(ref short[] data, int sampleRate)
            {
                double result;
                int position;
                for (int index = start; index < start + length * 2; index++)
                {
                    position = index - start;
                    result = 0.5 * Sine(position, frequency);
                    result += 0.4 * Sine(position, frequency / 4);
                    result += 0.2 * Sine(position, frequency / 2);
                    result *= Length(compressor, frequency, position, length, sampleRate) * short.MaxValue * 0.25;
                    result += data[index];
                    if (result > short.MaxValue) result = short.MaxValue;
                    if (result < -short.MaxValue) result = -short.MaxValue;
                    data[index] = (short)(result);
                }
            }
            private static double Length(double compressor, double frequency, double position, double length, int sampleRate)
            {
                return Math.Exp((compressor * frequency * sampleRate * (position / sampleRate)) / (length / sampleRate));
            }
            private static double Sine(int index, double frequency)
            {
                return Math.Sin(frequency * index);
            }
        }
        public class Track
        {

            private int sampleRate;
            private List<Element> elements = new List<Element>();
            private short[] data;
            private int length;
            private static Dictionary<string, int> notes = new Dictionary<string, int> {{"C", -9}, {"C#", -8}, {"D", -7},
        {"D#",-6}, {"E",-5},  {"F", -4}, {"F#", -3}, {"G", -2}, {"G#", -1}, {"A", 0}, {"A#",1}, {"B",2}, {"L",3} , {"S",4}};
            private double dtime = 0.25;
            private double position = 0;


        public short[] getData()
        {
            return data;
        }

        private static double GetNote(int key, int octave)
        {
            return 27.5 * Math.Pow(2, (key + octave * 12.0) / 12.0);
        }
        public Track(int sampleRate)
        {
            this.sampleRate = sampleRate;
        }
        public void Add(double frequency, double compressor, double start, double length)
        {
            if (this.length < (start + length * 2 + 1) * sampleRate) this.length = (int)(start + length * 2 + 1) * sampleRate;
            elements.Add(new Element(frequency, compressor, start, length, sampleRate));
        }
        public void Synthesize()
        {
            data = new short[length];
            foreach (var element in elements)
            {
                element.Get(ref data, sampleRate);
            }
        }
        public void SaveWave(Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream);
            short frameSize = (short)(16 / 8);
            writer.Write(0x46464952);
            writer.Write(36 + data.Length * frameSize);
            writer.Write(0x45564157);
            writer.Write(0x20746D66);
            writer.Write(16);
            writer.Write((short)1);
            writer.Write((short)1);
            writer.Write(sampleRate);
            writer.Write(sampleRate * frameSize);
            writer.Write(frameSize);
            writer.Write((short)16);
            writer.Write(0x61746164);
            writer.Write(data.Length * frameSize);
            for (int index = 0; index < data.Length; index++)
            {
                foreach (byte element in BitConverter.GetBytes(data[index]))
                {
                    stream.WriteByte(element);
                }
            }
        }

        public void Music(string melody, double temp = 60.0)
            {
                string[] words = melody.Split(' ');
                foreach (string word in words)
                {
                    int note = notes[word.Substring(0, word.Length - 1)];
                    int octave = Convert.ToInt32(word.Substring(word.Length - 1, 1));
                        switch (note)
                        {
                            case 3:
                                dtime = Math.Pow(0.5, octave + 1) * 8 * (60.0 / temp);
                                break;
                            case 4:
                                length += (int)(Math.Pow(0.5, octave + 1) * 8 * (60.0 / temp));
                                position += Math.Pow(0.5, octave + 1) * 8 * (60.0 / temp);
                                break;
                            default:
                                Add(GetNote(note, octave), -0.51, position, dtime);
                                position += dtime;
                                break;
                    }
                }
     
                }

        
    }
            
        }