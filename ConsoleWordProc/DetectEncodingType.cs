using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace ConsoleWordProc
{
    /// <summary>
    /// Кодировки текста 
    /// </summary>
    public enum EncodingType
    {
        NotDefined = 0,
        Unicode = 1,
        UTF8 = 2,
        UTF16 = 3
    }

    /// <summary>
    /// Класс для определения кодировки текста в файле по сигнатуре кодировки (BOM => Byte Order Mark) в начале файла.
    /// Чтобы кодировка определилась правильно, необходимо создать предикать определения кодировки
    /// по сигнатуре и добавить саму кодировку в перечесляемый тип EncodingType.
    /// Способ использования:
    /// 
    /// DetectEncodingType detect = new DetectEncodingType();
    /// 
    /// Добавить предикат для определения типа 
    /// detect.AddDetectEncodingType(EncodingType.UTF8 /*Определяемый тип*/,
    /// (data) => { if (data.Length >= 3 && data[0] == 0xEF && data[1] == 0xBB && data[2] == 0xBF) return true; return false; }/*Предикат для определения*/);
    /// 
    /// Указываем путь к файлу
    /// detect.SetData(path);
    /// 
    /// EncodingType type = EncodingType.NotDefined;
    /// 
    /// Определяем тип
    /// type = detect.Detect();
    /// </summary>
    public class DetectEncodingType
    {
        /// <summary>
        /// Сигнатура кодировки
        /// </summary>
        private byte[] _data;

        /// <summary>
        /// Предикаты для определения кодировки
        /// </summary>
        private Dictionary<EncodingType, Predicate<byte[]>> _detectEncodingType;

        public DetectEncodingType()
        {
            _detectEncodingType = new Dictionary<EncodingType, Predicate<byte[]>>();
        }

        /// <summary>
        /// Добавление предикатов для идентификации кодировки текста по сигнатуре BOM.
        /// </summary>
        /// <param name="type">Идентифицируемая кодировка</param>
        /// <param name="detectType">Предикат для идентификации кодировки</param>
        public void AddDetectEncodingType(EncodingType type, Predicate<byte[]> detectType)
        {
            _detectEncodingType.Add(type, detectType);
        }

        /// <summary>
        /// Задание сигнатуры кодировки BOM из файла.
        /// </summary>
        /// <param name="data">данные о сигнатуре</param>
        public void SetBOM(byte[] data)
        {
            _data = data;
        }

        /// <summary>
        /// Получение сигнатуры кодировки BOM из файла.
        /// </summary>
        /// <param name="pathToFile">Полный путь к файлу и имя файла</param>
        public void SetBOM(string pathToFile, int countBytesForRead=5)
        {
            if (File.Exists(pathToFile))
            {

                _data = new byte[countBytesForRead];//вынести в константы

                FileStream file = new FileStream(pathToFile, FileMode.Open);

                file.Read(_data, 0, countBytesForRead);

                file.Close();
            }
            else
            {
                throw new Exception("File "+ pathToFile+ " not exists");
            }
        }

        /// <summary>
        /// Запуск определения кодировки. Вернуться должна только одна кодировка.
        /// </summary>
        /// <returns> Кодировка текста в файле.</returns>
        public EncodingType Detect()
        {

            if (_detectEncodingType.Count == 0)
            {
                throw new Exception("Not added  detection of text encoding. Add Predicate and EncodingType in AddDetectEncodingType.");
            }

            if (_data == null || _data.Length == 0)
            {
                throw new Exception("The Byte Order Mark in Null Or Empty");
            }

            //Список кодировок, идентификация которых вернет true.
            List<KeyValuePair<EncodingType, bool>> resultDetect = new List<KeyValuePair<EncodingType, bool>>(_detectEncodingType.Count);

            //флаг опренделения кодировки.
            bool flagResultDetect;

            foreach (var item in _detectEncodingType)
            {
                flagResultDetect = item.Value(_data);

                if (flagResultDetect)//кодировка определена
                {
                    resultDetect.Add(KeyValuePair.Create(item.Key, flagResultDetect));
                }
            }

            if (resultDetect.Count() > 1)//если несколько кодировок true то ошибка
            {
                throw new Exception("Error: Detect multiple encodings of the text. Must be one.");
            }

            if (resultDetect.Count() == 0)//если ни одной кодировки
            {
                return EncodingType.NotDefined;
            }

            return resultDetect[0].Key;//одна кодировка
        }




    }

    
}
