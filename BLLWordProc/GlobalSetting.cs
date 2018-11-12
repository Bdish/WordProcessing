using System;
using System.Collections.Generic;
using System.Text;

namespace BLLWordProc
{
    /// <summary>
    /// Глобальные настройки для класса ManagerDictionary
    /// </summary>
    public static class GlobalSetting
    {

        /// <summary>
        /// Минимальная длина слова
        /// </summary>
        public static int MinLengthWord { get; set; } = 3;

        /// <summary>
        /// Максимальная длина слова
        /// </summary>
        public static int MaxLengthWord { get; set; } = 15;

        /// <summary>
        /// Минимальная частота слова, чтобы попасть в словарь
        /// </summary>
        public static int MinFrequencyWord { get; set; } = 3;

        /// <summary>
        /// Максимальное количество слов, включаемых в список подходящих.
        /// </summary>
        public static int MaxNumberOfWordsReturned { get; set; } = 5;

        
        /// <summary>
        /// Разделители слов в тексте.
        /// </summary>
        public static char[] SeparationCharacters { get; set; }= new char[] { '\n', '\r', ' ', '-', '.', '?', '!', ')', '(', ',', ':','\'','\"' };
    }
}
