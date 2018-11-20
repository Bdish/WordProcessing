using DALWordProc.Entities;
using DALWordProc.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLLWordProc
{

    /// <summary>
    ///  Класс для  реализации менеджера за текстовым процессором. 
    ///  Создание, добавление, удаление словаря с частотами слов  
    ///  и поиск слов по префиксу.
    ///  Словарь представляет из себя список слов и частота их появления в тексте,
    ///  по которому и создавался словарь.
    /// </summary>
    public class ManagerDictionary : BaseManagerDictionary
    {
        /// <summary>
        /// Репозиторий для взаимодействия с базой данных.
        /// </summary>
        private IGenericRepository<DictionaryWord> _repoDictionary;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repoDictionary"> Репозиторий базы данных</param>
        public ManagerDictionary(IGenericRepository<DictionaryWord> repoDictionary)
        {
            _repoDictionary = repoDictionary;           
        }

        /// <summary>
        /// Создание нового словаря. Если существует старый словарь, то будет ошибка.
        /// </summary>
        /// <param name="text">Текст со словами.</param>
        public override void CreateDictionary(string text)
        {
            
             // Словарь слов и их частот.
            Dictionary<string, int> dictionery=new Dictionary<string, int>();

            if (_repoDictionary.Get().Any())//Проверка на пустоту словаря 
            {
                throw new Exception("Error: Dictionary words in Data Base is not Empty.");
            }

            //проверка текста на пустоту            
            //из текста удалить все знаки препинания
            //получить массив слов из текста
            string[] arrayWords=CheckTextAndSplitOnWords(text);

            //очищаем временный словарь
            dictionery.Clear();

            //составляем словарь с частотами
            CreatDictionaryWordsAndFrequencies(arrayWords, dictionery);
            
            //добавление в бд
            foreach (KeyValuePair<string, int> word in dictionery)
            {
                if (LimitForAddToDictionary(word.Key, word.Value))
                {
                    DictionaryWord newWord = new DictionaryWord {/*Id = _repoDictionary.Get().Count()+1,*/ Word = word.Key,Frequency = word.Value };
                    _repoDictionary.Create(newWord);
                }
            }           
        }

        /// <summary>
        /// Модификация существующего словаря.
        /// </summary>
        /// <param name="text">Текст со словами.</param>
        public override void UpdateDictionary(string text)
        {

            // Словарь слов и их частот.
            Dictionary<string, int> dictionery = new Dictionary<string, int>();

            //проверка текста на пустоту            
            //из текста удалить все знаки препинания
            //получить массив слов из текста
            string[] arrayWords = CheckTextAndSplitOnWords(text);

            //очищаем временный (промежуточный) словарь
            dictionery.Clear();

            //составляем словарь с частотами
            CreatDictionaryWordsAndFrequencies(arrayWords,dictionery);

            //Слово в базе данных
            IEnumerable<DictionaryWord> wordInDB;

            foreach (KeyValuePair<string, int> word in dictionery)
            {
                //проверка на несколько значений в бд и выдать исключение
                wordInDB = _repoDictionary.Get(x => x.Word == word.Key);
               
                if (LimitForAddToDictionary(word.Key, word.Value))
                {
                    if (wordInDB.Any())                       
                    {
                        //Обновляем в базе существующее слово

                        DictionaryWord specificWord = wordInDB.FirstOrDefault();
                        specificWord.Frequency += word.Value;
                        _repoDictionary.Update(specificWord);
                    }
                    else
                    {
                        //Создаем в базе новое слово
                        DictionaryWord newWord = new DictionaryWord { Word = word.Key, Frequency = word.Value };
                        _repoDictionary.Create(newWord);
                    }
                }
            }
        }

        /// <summary>
        /// Удаление словаря.
        /// </summary>
        public override void DeleteDictionary()
        {
            //очищаем таблицу + счетчики
            // _repoDictionary.ExecuteSQLExpression("TRUNCATE TABLE[DictionaryWords]");
            _repoDictionary.RemoveAll();
        }

        /// <summary>
        /// Поиск подходящих слов в словаре по префиксу.
        /// </summary>
        /// <param name="prefix">Слово или часть слова для поиска в словаре.</param>
        /// <returns>Список подходящих под префикс слов.</returns>
        public override IEnumerable<DictionaryWord> FindWords(string prefix)
        {

            IEnumerable<DictionaryWord> resultListWords = _repoDictionary.Get(x => x.Word.StartsWith(prefix)).OrderByDescending(x => x.Frequency).Take(GlobalSetting.MaxNumberOfWordsReturned);
            var grouped = resultListWords.GroupBy(x => x.Frequency).Select(group => new { Key = group.Key, Items = group.OrderBy(t => t.Word) });
            return grouped.Select(x => x.Items).SelectMany(/*x => x.ToList()*/x=>x);
        }

        /// <summary>
        /// Проверка текста на пустоту и разбиения текста на слова.
        /// </summary>
        /// <param name="text">Исходный текст.</param>
        /// <returns>Список слов в тексте.</returns>
        private string[] CheckTextAndSplitOnWords(string text)
        {
           
            if (String.IsNullOrEmpty(text))//Проверка на пустоту текста
            {
                throw new Exception("Error: Text is empty or null in create or update dictionary words.");
            }
            
            string[] arrayWords = text.ToLower().Split(GlobalSetting.SeparationCharacters, StringSplitOptions.RemoveEmptyEntries);//разделяем входную строку с узлами интернет на массив адресов

            return arrayWords;

        }

        /// <summary>
        /// Создание временного словаря со словами и их частотой появления в тексте.
        /// </summary>
        /// <param name="arrayWords">Список слов в тексте.</param>
        private void CreatDictionaryWordsAndFrequencies(string[] arrayWords,Dictionary<string, int> dictionery)
        {
            foreach (var word in arrayWords)
            {

               int frequency;
                //
                //
                //if (_dictionery.ContainsKey(word)) 91 стр Джон Скит - C# для профессионалов. Тонкости программирования - 2014
                if (dictionery.TryGetValue(word, out frequency))
                {
                    dictionery[word]++;//повышаем частоту
                }
                else
                {
                    dictionery.Add(word, 1);//новое слово с единичной частотой
                }

                /*    

                try
                {
                    if (_dictionery[word] >= 1) //уже не новое слово
                    {
                        _dictionery[word]++;//повышаем частоту
                    }
                }
                catch
                {
                    _dictionery.Add(word, 1);//новое слово с единичной частотой
                }
                */

            }
        }

        /// <summary>
        /// Проверка ограничений по длине слов и количества повтарений слова для попадания в словарь.
        /// </summary>
        /// <param name="word">слово</param>
        /// <param name="frequency">Частота повторения слова в исходном тексте</param>
        /// <returns></returns>
        private bool LimitForAddToDictionary(string word,int frequency)
        {

            return frequency >= GlobalSetting.MinFrequencyWord //количество повторений слова в тексте, минимальный порог для попадания в словарь
                && word.Length >= GlobalSetting.MinLengthWord //количество символов в слове, минимальный порог для попадания в словарь
                && word.Length <= GlobalSetting.MaxLengthWord//количество символов в слове, максимальный порог для попадания в словарь
                ;
            
        }
    }
}
