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
             

            if (_repoDictionary.Get().Count()>0)//Проверка на пустоту словаря 
            {
                throw new Exception("Error: Dictionary words in Data Base is not Empty.");
            }

            //проверка текста на пустоту            
            //из текста удалить все знаки препинания
            //получить массив слов из текста
            string[] arrayWords=CheckTextAndSplitOnWords(text);

            //очищаем временный словарь
            _dictionery.Clear();

            //составляем словарь с частотами
            CreatDictionaryWordsAndFrequencies(arrayWords);
            
            //добавление в бд
            foreach (KeyValuePair<string, int> word in _dictionery)
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
           
            //проверка текста на пустоту            
            //из текста удалить все знаки препинания
            //получить массив слов из текста
            string[] arrayWords = CheckTextAndSplitOnWords(text);

            //очищаем временный (промежуточный) словарь
            _dictionery.Clear();

            //составляем словарь с частотами
            CreatDictionaryWordsAndFrequencies(arrayWords);


            //Слова в базе данных
            List<DictionaryWord> wordInDB;

            foreach (KeyValuePair<string, int> word in _dictionery)
            {
                //проверка на несколько значений в бд и выдать исключение
                wordInDB = _repoDictionary.Get(x => x.Word == word.Key).ToList();

                if (wordInDB.Count > 1)
                {
                    throw new Exception("Error: A few words in the database. UpdateDictionary");
                }

                
                if (LimitForAddToDictionary(word.Key, word.Value))
                {

                    if (wordInDB.Count == 1)
                    {
                        //Обновляем в базе существующее слово
                        wordInDB[0].Frequency += word.Value;
                        _repoDictionary.Update(wordInDB[0]);
                    }
                    else
                    {
                        //Создаем в базе новое слово
                        DictionaryWord newWord = new DictionaryWord { /*Id = _repoDictionary.Get().Count() + 1,*/ Word = word.Key, Frequency = word.Value };
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
            foreach(var item in _repoDictionary.Get())
                                    _repoDictionary.Remove(item);
        }

        /// <summary>
        /// Поиск подходящих слов в словаре по префиксу.
        /// </summary>
        /// <param name="prefix">Слово или часть слова для поиска в словаре.</param>
        /// <returns>Список подходящих под префикс слов.</returns>
        public override List<DictionaryWord> FindWords(string prefix)
        {
            
            List<DictionaryWord> resultListWords = _repoDictionary.Get(x => x.Word.Contains(prefix)).OrderByDescending(x => x.Frequency).Take(GlobalSetting.MaxNumberOfWordsReturned).ToList();
            var grouped = resultListWords.GroupBy(x => x.Frequency).Select(group => new { Key = group.Key, Items = group.OrderBy(t => t.Word) });
            return grouped.Select(x => x.Items).SelectMany(x => x.ToList()).ToList();
        }

        /// <summary>
        /// Проверка текста на пустоту и разбиения текста на слова.
        /// </summary>
        /// <param name="text">Исходный текст.</param>
        /// <returns>Список слов в тексте.</returns>
        private string[] CheckTextAndSplitOnWords(string text)
        {
           
            if (text == null || text == "")//Проверка на пустоту текста
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
        private void CreatDictionaryWordsAndFrequencies(string[] arrayWords)
        {
            foreach (var word in arrayWords)
            {
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
            
            if (frequency >= GlobalSetting.MinFrequencyWord //количество повторений слова в тексте, минимальный порог для попадания в словарь
                && word.Length >= GlobalSetting.MinLengthWord //количество символов в слове, минимальный порог для попадания в словарь
                && word.Length <= GlobalSetting.MaxLengthWord//количество символов в слове, максимальный порог для попадания в словарь
                )
            {
                return true;
            }
                return false;
        }
    }
}
