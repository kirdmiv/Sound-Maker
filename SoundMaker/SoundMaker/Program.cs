using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Synthesizer;
using System.Media;
using System.IO;

namespace SoundMaker
{
    class Program
    {

        private static string Readfile(string filename)
        {   
            string melody, line;
            int last_index;
            melody = "";
            StreamReader file = new StreamReader(filename);
            while ((line = file.ReadLine()) != null) {
                last_index = melody.Length - 1;
                if (last_index > 0) {
                    if (melody[last_index] == ' ')
                    {
                        melody += line;
                    }
                    else
                    {
                        melody += " " + line;
                    }
                }
                else
                {
                    melody += line;
                }
            }
            file.Close();
            return melody;
        }

        static void Main(string[] args)
        {
            ConsoleKey key;
            short[] data;
            string music, name;
            bool b = true;
            bool bb = true;
            int sampleRate = 8000;
            Track test = new Track(sampleRate);
            Stream stream = new MemoryStream();
            SoundPlayer player;
            while (bb == true) {
                b = true;
                test = new Track(sampleRate);
                music = Console.ReadLine();
                test.Music(music, 70);
                test.Synthesize();
                stream = new MemoryStream();
                test.SaveWave(stream);
                stream.Position = 0;
                player = new SoundPlayer(stream);
                player.PlayLooping();
                Console.WriteLine("Next step:\n Press s to save\n Press n to make new sonds\n Press Esc to quit...");

                while (b == true)
                {
                    key = Console.ReadKey(true).Key;
                    switch (key)
                    {
                        case ConsoleKey.S:
                            Console.WriteLine("Enter file name...");
                            name = Console.ReadLine();
                            Stream file = File.Create(name + ".wav"); // Создаем новый файл и стыкуем его с потоком.
                            data = test.getData();
                            test.SaveWave(file); // Записываем наши данные в поток.
                            file.Close();
                            Console.WriteLine("Next step:\n Press s to save\n Press n to make new sonds\n Press Esc to quit...");
                            break;

                        case ConsoleKey.R:
                            try
                            {
                                test = new Track(sampleRate);
                                string filus = Console.ReadLine();
                                music = Readfile(filus);
                                test.Music(music, 70);
                                test.Synthesize();
                                stream = new MemoryStream();
                                test.SaveWave(stream);
                                stream.Position = 0;
                                player = new SoundPlayer(stream);
                                player.PlayLooping();
                                Console.WriteLine("Next step:\n Press s to save\n Press n to make new sonds\n Press Esc to quit...");

                            }
                            catch (System.UnauthorizedAccessException)
                            {
                                Console.WriteLine("Accesess denied!!!");
                            }
                            catch (System.IO.FileNotFoundException)
                            {
                                Console.WriteLine("File not found!!!");
                            }
                            break;


                        case ConsoleKey.N:
                            b = false;
                            break;
                        case ConsoleKey.Escape:
                            player.Dispose();
                            bb = false;
                            b = false;
                            break;
                    }

                }
            }
        }
    }
}
