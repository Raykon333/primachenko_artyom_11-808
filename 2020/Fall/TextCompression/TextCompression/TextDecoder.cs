using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TextCompression
{
    class TextDecoder
    {
        public static void Decode(string inputPath, string outputPath)
        {
            using (BinaryReader br = new BinaryReader(File.Open(inputPath, FileMode.Open)))
            {
                var dictTypeByte = br.ReadByte();
                switch (dictTypeByte)
                {
                    case 0:
                        DecodeAutoDict(br, outputPath);
                        break;

                    case 1:
                        DecodeUTF8(br, outputPath);
                        break;

                    default:
                        throw new Exception("Encoded text is in wrong format");
                }
            }
        }

        private static void DecodeAutoDict(BinaryReader br, string outputPath)
        {
            int dictLength = BitConverter.ToInt32(br.ReadBytes(4));
            List<string> words = new List<string>();
            for (int i = 0; i < dictLength; i++)
            {
                words.Add(BitConverter.ToChar(br.ReadBytes(2)).ToString());
            }
            int uTypeByte = br.ReadByte();

            using (StreamWriter sw = new StreamWriter(outputPath))
            {
                int entry;
                switch (uTypeByte)
                {
                    case 0:
                        entry = br.ReadByte();
                        break;

                    case 1:
                        entry = BitConverter.ToInt16(br.ReadBytes(2));
                        break;

                    case 2:
                        entry = BitConverter.ToInt32(br.ReadBytes(4));
                        break;
                    default:
                        throw new Exception("Encoded text is in wrong format");
                }
                string entryWord = words[entry];
                string newWordPart = entryWord;
                sw.Write(entryWord);
                int i = 0;
                while (br.BaseStream.Position != br.BaseStream.Length)
                {
                    switch (uTypeByte)
                    {
                        case 0:
                            entry = br.ReadByte();
                            break;

                        case 1:
                            entry = BitConverter.ToInt16(br.ReadBytes(2));
                            break;

                        case 2:
                            entry = BitConverter.ToInt32(br.ReadBytes(4));
                            break;
                        default:
                            throw new Exception("Encoded text is in wrong format");
                    }
                    if (entry == dictLength + i)
                        words.Add(newWordPart + newWordPart.First());
                    else
                        words.Add(newWordPart + words[entry].First());
                    entryWord = words[entry];
                    newWordPart = entryWord;
                    sw.Write(entryWord);
                    i++;
                }
            }
        }

        private static void DecodeUTF8(BinaryReader br, string outputPath)
        {
            int dictLength = 256;
            List<string> words = new List<string>();

            int uTypeByte = br.ReadByte();

            using (StreamWriter sw = new StreamWriter(outputPath))
            {
                int entry;
                switch (uTypeByte)
                {
                    case 0:
                        entry = br.ReadByte();
                        break;

                    case 1:
                        entry = BitConverter.ToInt16(br.ReadBytes(2));
                        break;

                    case 2:
                        entry = BitConverter.ToInt32(br.ReadBytes(4));
                        break;
                    default:
                        throw new Exception("Encoded text is in wrong format");
                }
                string entryWord;
                if (entry < dictLength)
                    entryWord = Encoding.UTF8.GetString(BitConverter.GetBytes(entry));
                else
                    entryWord = words[entry - dictLength];
                string newWordPart = entryWord;
                sw.Write(entryWord);
                int i = 0;
                while (br.BaseStream.Position != br.BaseStream.Length)
                {
                    switch (uTypeByte)
                    {
                        case 0:
                            entry = br.ReadByte();
                            break;

                        case 1:
                            entry = BitConverter.ToInt16(br.ReadBytes(2));
                            break;

                        case 2:
                            entry = BitConverter.ToInt32(br.ReadBytes(4));
                            break;
                        default:
                            throw new Exception("Encoded text is in wrong format");
                    }
                    if (entry == dictLength + i)
                        words.Add(newWordPart + newWordPart.First());
                    if (entry < dictLength)
                        entryWord = Encoding.UTF8.GetString(BitConverter.GetBytes(entry));
                    else
                        entryWord = words[entry - dictLength];
                    if (entry != dictLength + i)
                        words.Add(newWordPart + entryWord.First());
                    newWordPart = entryWord;
                    sw.Write(entryWord);
                    i++;
                }
            }
        }
    }
}
