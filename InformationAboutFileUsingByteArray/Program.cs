using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*http://www.ue.eti.pg.gda.pl/fpgalab/zadania.spartan3/zad_vga_struktura_pliku_bmp_en.html на этом сайте вы можете ознакомиться 
 со структурой BMP файла, а именно расположением информации в байтах*/
namespace InformationAboutFileUsingByteArray
{
    class Program
    {
        /*Создание структуры для содержания метаданных изображения*/
        struct MetadataOfImageBMP
        {
            public delegate void GetInfo();

            private string Signature;
            private uint Size; // or int
            private uint reversed;
            private byte sizeInfoHeader;
            private uint width;
            private uint height;
            private string planes;
            private uint bitCount;
            private string compression;
            private uint sizeImage;// long or stringParse| size without compression or with compression
            private uint XpixelsPerM;
            private uint YpixelsPerM;
            private uint ColorsUsed;
            private uint ColorsImportant;

            /*Инкапсулирования наших полей для безопасного обращения к полям*/
            public string SignatureImage { get => Signature; set => Signature = value; }
            public uint SizeImage { get => Size; set => Size = value; }
            public uint Reserved { get => reversed; set => reversed = value = 0; }
            public byte SizeInfoHeader { get => sizeInfoHeader; set => sizeInfoHeader = value; }
            public uint Width { get => width; set => width = value; }
            public uint Height { get => height; set => height = value; }
            public string Planes { get => planes; set => planes = value; } // ploskostei
            public uint BitCount { get => bitCount; set => bitCount = value; }
            public string Compression { get => compression; set => compression = value; }
            public uint SizeImageWithXORCompression { get => sizeImage; set => sizeImage = value; }
            public uint XpixelsPerMBmp { get => XpixelsPerM; set => XpixelsPerM = value; }
            public uint YpixelsPerMBmp { get => YpixelsPerM; set => YpixelsPerM = value; }
            public uint ColorsUsedBmp { get => ColorsUsed; set => ColorsUsed = value; }
            public uint ColorsImportantBmp { get => ColorsImportant; set => ColorsImportant = value; }

            /*Создания приватного метода, который будет служить нашим 'триггером' для делегата,
             делегат - это обертка для метода, которому известно тот, как нужно вызывать метод */
            private void GetInfoAboutFile()
            {
                Console.WriteLine("Signature Image: " + SignatureImage);
                Console.WriteLine("Size Image: " + SizeImage + " Kbytes");
                Console.WriteLine("Reversed: " + Reserved);
                Console.WriteLine("Size info header: " + SizeInfoHeader);
                Console.WriteLine("Width Image: " + Width);
                Console.WriteLine("Height Image: " + Height);
                Console.WriteLine("Planes: " + Planes + " number of color planes being used");
                Console.WriteLine("Bit Count: " + BitCount + " bits per pixel");
                Console.WriteLine("Compression: " + Compression);
                Console.WriteLine("SizeImageWithXORCompression: " + SizeImageWithXORCompression + " image size with compression/" +
                    "without compression");
                Console.WriteLine("X pixels Per M: " + XpixelsPerMBmp);
                Console.WriteLine("Y pixel per M: " + YpixelsPerMBmp);
                Console.WriteLine("Color used: " + ColorsUsedBmp);
                Console.WriteLine("Colors Important: " + ColorsImportantBmp);
            }
            /*Передача метода делегату GetInfo и вызов метода Invoke на экземпляре делегата*/
            public void GetInfoViaDelegate()
            {
                GetInfo info = new GetInfo(GetInfoAboutFile);
                info.Invoke();
            }
        }

        static void Main(string[] args)
        {
            /*Создания экземпляра структуры , переменных для хранения информации и константного поля bytes в 16-ой системе 
             для прохода по массиву картинки (по первым 54 байтам)*/
            MetadataOfImageBMP photoMeta = new MetadataOfImageBMP();
            const int bytes = 0x00036;
            uint size = 0;
            uint height = 0;
            uint width = 0;
            string numPlanes = null;
            byte bitCount = 0;
            byte compression = 0;
            uint compressionImageSize = 0;
            uint pixelsPerX = 0;
            uint pixelsPerY = 0;
            uint colorImportant = 0;
            uint usedColor = 0;
            uint count = 0;
            string hexFromByte = null;
            /*Вводите полный, корректный путь для изображения, можете добавить блок finally и поместить начальную реализацию,
             либо сделать константное поле с путем к файлу в самом проекте*/
            Console.WriteLine("Enter full path to image(*.bmp only)");
            string path = Console.ReadLine();
            try
            {
                /*Создаем массив с указанием первых 54 байтов изображения*/
                byte[] test = new byte[0x00036];
                /*Делаем простую проверку существования пути к файлу и создаем экземпляр объекта BinaryReader, благодаря ему мы можем
                 считывать информацию проходясь по каждому байту, который нам нужен, в нашем случае первые 54-ти байта*/
                if (path != null || File.Exists(path) == true)
                {
                    BinaryReader br = new BinaryReader(File.OpenRead(path));
                    for (int i = 0x00000; i < bytes; i++)
                    {
                        br.BaseStream.Position = i;
                        test[i] = br.ReadByte();
                        if (test[0] == 66 && test[1] == 77)
                            photoMeta.SignatureImage = "BM";
                        /*Здесь мы рассчитываем размер изображения по формуле 
                         D*256^3 + C*256^2 + B*256 + A, благодаря этой формуле мы рассчитываем большую часть информации в наших 54 байтах
                         */
                        size = (uint)Math.Pow(256, 3) * test[5] + (uint)Math.Pow(256, 2) * test[4] + (uint)Math.Pow(256,1) * test[3] + test[2];
                        photoMeta.SizeImage = size;

                        width = test[21] * (uint)Math.Pow(256, 3) + test[20] * (uint)Math.Pow(256, 2) + test[19] * (uint)Math.Pow(256, 1)
                            + test[18];
                        photoMeta.Width = width;

                        height = test[25] * (uint)Math.Pow(256, 3) + test[24] * (uint)Math.Pow(256, 2) + test[23] * (uint)Math.Pow(256, 1)
                            + test[22];
                        photoMeta.Height = height;

                        numPlanes = test[26].ToString("X2") + " " + test[27].ToString("X2");
                        photoMeta.Planes = numPlanes;

                        bitCount = (byte)(test[29] * 256 + test[28]);
                        photoMeta.BitCount = bitCount;

                        compression = (byte)(test[33] * Math.Pow(256, 3) + test[32] * Math.Pow(256, 2) + test[31] * 256 + test[30]);
                        if (compression == 0)
                        {
                            photoMeta.Compression = $"{compression} = BI_RGB no compression";
                        }
                        else if (compression == 1)
                        {
                            photoMeta.Compression = $"{compression} = BI_RLE8 8bit RLE encoding ";
                        }
                        else if (compression == 2)
                        {
                            photoMeta.Compression = $"{compression} = BI_RlE4 4bit RLE encoding";
                        }
                        else if (compression == 3)
                        {
                            photoMeta.Compression = $"{compression} = BI_BITFIELDS";
                        }
                        else if (compression == 4)
                        {
                            photoMeta.Compression = $"{compression} = BI_JPEG";
                        }
                        else if (compression == 5)
                        {
                            photoMeta.Compression = $"{compression} = BI_PNG";
                        }
                        else if (compression == 6)
                        {
                            photoMeta.Compression = $"{compression} = BI-ALPHABITFIELDS";
                        }
                        else if(compression == 11)
                        {
                            photoMeta.Compression = $"{compression} = BI-CMYK";
                        }
                        else if (compression == 12)
                        {
                            photoMeta.Compression = $"{compression} = BI-CMYKRLE8";
                        }
                        else if (compression == 13)
                        {
                            photoMeta.Compression = $"{compression} = BI-CMYKRLE4";
                        }

                        photoMeta.SizeInfoHeader = 40;

                        compressionImageSize = test[37] * (uint)Math.Pow(256, 3) + test[36] * (uint)Math.Pow(256, 2) + test[35] * (uint)Math.Pow(256, 1)
                            + test[34];

                        photoMeta.SizeImageWithXORCompression = compressionImageSize;

                        pixelsPerX = test[41] * (uint)Math.Pow(256, 3) + test[40] * (uint)Math.Pow(256, 2) + test[39] * (uint)Math.Pow(256, 1)
                            + test[38];

                        photoMeta.XpixelsPerMBmp = pixelsPerX;

                        pixelsPerY = test[45] * (uint)Math.Pow(256, 3) + test[44] * (uint)Math.Pow(256, 2) + test[43] * (uint)Math.Pow(256, 1)
                            + test[42];

                        photoMeta.YpixelsPerMBmp = pixelsPerY;
                        /*Ниже я делаю проверку за выход диапазона uint, возможно такое что количество цветов превышать вместительность
                         переменной*/
                        usedColor = test[49] * (uint)Math.Pow(256, 3) + test[48] * (uint)Math.Pow(256, 2) + test[47] * (uint)Math.Pow(256, 1)
                                             + test[46];
                        if (Int32.MaxValue < usedColor)
                        {
                            throw new OverflowException("Int overflow. Change type of variable ColorUsedBmp in Structure");
                        }
                        else
                        {
                            photoMeta.ColorsUsedBmp = usedColor;
                        }

                        colorImportant = test[53] * (uint)Math.Pow(256, 3) + test[52] * (uint)Math.Pow(256, 2) + test[51] * (uint)Math.Pow(256, 1)
                            + test[50];

                        photoMeta.ColorsImportantBmp = colorImportant;
                    }
                    br.Close();
                }
                else
                {
                    throw new FileNotFoundException("File not found", path);
                }

                foreach (var item in test)
                {
                    Console.Write(item + " ");
                }
                Console.WriteLine();
                Console.WriteLine("************Byte array representation to hex string****************");

                foreach (var item in ByteArrayToString(test).ToUpper())
                {
                    count++;
                    if (count % 2 == 0)
                    {
                        Console.Write(item + " ");
                        hexFromByte += item + " ";
                    }
                    else
                    {
                        Console.Write(item);
                        hexFromByte += item;
                    }
                }
                Console.WriteLine("********************************************************************");
                photoMeta.GetInfoViaDelegate();
                Console.WriteLine();
            }

            catch (FileNotFoundException fileEx)
            {
                Console.WriteLine("{0}-Invalid path or file name", fileEx.Message);
            }
            catch (PathTooLongException longEx)
            {
                Console.WriteLine("{0}-Path of file very long", longEx.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}-UUUUUps, something gone wrong", ex.Message);
            }

        }
        /*Метод для перевода байтов в 16-ую систему*/
        public static string ByteArrayToString(byte[] ba)
        {
                StringBuilder hex = new StringBuilder(ba.Length * 2);
                foreach (byte b in ba)
                    hex.AppendFormat("{0:x2}", b);
                return hex.ToString();
        }        
    }
}

