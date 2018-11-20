using DALWordProc.Entities;
using System.Collections.Generic;

namespace BLLWordProc
{
    /// <summary>
    /// Базовый класс для  реализации менеджера за текстовым процессором.
    /// </summary>
    public abstract class BaseManagerDictionary
    {
        

        
        /// <summary>
        /// Создание словаря. Словарь должен быть пустым.
        /// </summary>
        /// <param name="text">Текст со словами.</param>
        /// <returns></returns>
        public abstract void CreateDictionary(string text);

        /// <summary>
        /// Добавление в существующий словарь новых слов.
        /// </summary>
        /// <param name="text">Текст со словами.</param>
        /// <returns></returns>
        public abstract void UpdateDictionary(string text);

        /// <summary>
        /// Удаление словаря.
        /// </summary>
        /// <returns></returns>
        public abstract void DeleteDictionary();

        
        /// <summary>
        /// Поиск слов по префиксу.
        /// </summary>
        /// <param name="prefix"> Префикс для поиска слов.</param>
        /// <returns> Список подходящих под префикс слов.</returns>
        public abstract IEnumerable<DictionaryWord> FindWords(string prefix);
    }
}
