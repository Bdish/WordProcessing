using ConsoleWordProc;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UnitTestWordProc
{
    [TestFixture]
    public class ConsoleWordProc
    {


        /// <summary>
        /// Тестирование индентификации типа текста в файле.
        /// В данном тестировании должны создаваться файлы c/без BOM с/без текстом (ом)
        /// </summary>
        /// <param name="text">Текст для сохранния в файл (любой)</param>
        /// <param name="format">Формат текста</param>
        /// <param name="BOM">Создание сегмента BOM в файле</param>
        /// <param name="resultType">Что после декодирования должны получить</param>
        [TestCase("", "UTF8", true, EncodingType.UTF8)]
        [TestCase("", "Unicode", true, EncodingType.NotDefined)]
        [TestCase("asdfsdf", "UTF8",true, EncodingType.UTF8)]
        [TestCase("asdfsdf", "UTF32", true, EncodingType.NotDefined)]
        [TestCase("asdfsdf", "Unicode", true, EncodingType.NotDefined)]
        [TestCase("asdfsdf", "ASCII", true, EncodingType.NotDefined)]
        [TestCase("asdfsdf","UTF8",false, EncodingType.NotDefined)]
        [TestCase("asdfsdf", "UTF32", false, EncodingType.NotDefined)]
        [TestCase("asdfsdf", "Unicode", false, EncodingType.NotDefined)]
        [TestCase("asdfsdf", "ASCII", false, EncodingType.NotDefined)]
        [TestCase("", "UTF8", false, EncodingType.NotDefined)]
        [TestCase("", "UTF32", false, EncodingType.NotDefined)]
        [TestCase("", "Unicode", false, EncodingType.NotDefined)]
        [TestCase("", "ASCII", false, EncodingType.NotDefined)]
        public void TestDetectEncodingTextFromFile(string text,string format, bool BOM, EncodingType resultType)
        {
            Encoding typeFormat;//определяем формат файла
            switch (format)
            {
                case "UTF8":
                    {
                        typeFormat = new UTF8Encoding(BOM);
                        break;
                    }
                case "UTF32":
                    {
                        typeFormat =new System.Text.UTF32Encoding(true, BOM);
                        break;
                    }
                case "Unicode":
                    {
                        typeFormat = new System.Text.UnicodeEncoding(true, BOM);
                        break;
                    }
                case "ASCII":
                    {
                        typeFormat =new System.Text.UnicodeEncoding(true, BOM);
                        break;
                    }
                default:
                    {
                        typeFormat =new System.Text.UnicodeEncoding(true, BOM);
                        break;
                    }
            }

            //Создание файла
            //Сохранение в определенном формате
            //чтение файла
            if (File.Exists("Foo.txt"))
            {
                File.Delete("Foo.txt");
            }

            using (var file = new StreamWriter("Foo.txt", false, typeFormat))
            {
                file.WriteLine(text);
            }

            //проверка идентификации кодировки текста в файле
            DetectEncodingType detect = new DetectEncodingType();

            //индетификация только по Utf8
            detect.AddDetectEncodingType(EncodingType.UTF8,
                (data) => { if (data.Length >= 3 && data[0] == 0xEF && data[1] == 0xBB && data[2] == 0xBF) return true; return false; });

            detect.SetBOM("Foo.txt");

            EncodingType type = EncodingType.NotDefined;

            type = detect.Detect();

            Assert.AreEqual(type, resultType);
        }

       
        
    }
}
