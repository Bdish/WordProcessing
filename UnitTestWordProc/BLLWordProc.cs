using BLLWordProc;
using DALWordProc.EFDbContext;
using DALWordProc.Entities;
using DALWordProc.Repository.Implementations;
using DALWordProc.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTestWordProc
{
    [TestFixture]
    public class BLLWordProc
    {
       
        /// <summary>
        /// Длина слов не должна превышать 15 символов.
        /// Минимальное число символов, воспринимаемых приложением как слово – 3.
        /// Минимальное число повторений слова в тексте для включения этого слова в БД – 3.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="dictionary"></param>
        
        [TestCase("жил жил жил дом дом ом ом ом сом сом сом сом йцукенгшщзфывапh йцукенгшщзфывапh йцукенгшщзфывапh йцукенгшщзфываg йцукенгшщзфываg йцукенгшщзфываg", "жил 3 сом 4 йцукенгшщзфываg 3 ")]
        public void CreateDictionary(string text,string trueResult)
        {

            /* var mockDbSet = new Mock<DbSet<DictionaryWord>>();

             var dbSetContent = new List<DictionaryWord>();

             mockDbSet.As<IQueryable<DictionaryWord>>().Setup(m => m.Provider).Returns(dbSetContent.AsQueryable().Provider);
             mockDbSet.As<IQueryable<DictionaryWord>>().Setup(m => m.Expression).Returns(dbSetContent.AsQueryable().Expression);
             mockDbSet.As<IQueryable<DictionaryWord>>().Setup(m => m.ElementType).Returns(dbSetContent.AsQueryable().ElementType);
             mockDbSet.As<IQueryable<DictionaryWord>>().Setup(m => m.GetEnumerator()).Returns(() => dbSetContent.GetEnumerator());
             mockDbSet.Setup(m => m.Add(It.IsAny<DictionaryWord>())).Callback<DictionaryWord>((s) => dbSetContent.Add(s));
             mockDbSet.Setup(m => m.Remove(It.IsAny<DictionaryWord>())).Callback<DictionaryWord>((s) => dbSetContent.Remove(s));

             var mockDbContext = new Mock<DBDictionaryWord>();

             mockDbContext.Setup(c => c.Set<DictionaryWord>()).Returns(mockDbSet.Object);

             GenericRepository<DictionaryWord> repoDictionaryWord = new GenericRepository<DictionaryWord>(mockDbContext.Object);*/
            IGenericRepository<DictionaryWord> repoDictionaryWord = new FakeRepository();
            ManagerDictionary manager = new ManagerDictionary(repoDictionaryWord);

            manager.CreateDictionary(text);

            StringBuilder result = new StringBuilder();

            foreach (var word in repoDictionaryWord.Get())
            {
                result.Append(word.Word).Append(" ").Append(word.Frequency).Append(" ");
            }
            
            NUnit.Framework.Assert.AreEqual(result.ToString(), trueResult);



        }

        [TestCase("жил жил жил ", "сом сом сом сом", "жил 3 сом 4 ")] // 1 & 1
        [TestCase("жил жил жил ", "жил жил жил", "жил 6 ")] // 1 + 1
        [TestCase("жил жил  ", "жил жил жил", "жил 3 ")] // 0 + 1
        [TestCase("жил жил жил", "жил жил  ", "жил 3 ")] // 1 + 0
        [TestCase("жил жил  ", "жил жил ", "")] //0 + 0
        public void UpdateDictionary(string text1,string text2, string trueResult)
        {
            IGenericRepository<DictionaryWord> repo = new FakeRepository();
            ManagerDictionary manager = new ManagerDictionary(repo);
            

            manager.CreateDictionary(text1);

            manager.UpdateDictionary(text2);


            StringBuilder result = new StringBuilder();

            foreach (var word in repo.Get())
            {
                result.Append(word.Word).Append(" ").Append(word.Frequency).Append(" ");
            }

            NUnit.Framework.Assert.AreEqual(result.ToString(), trueResult);

        }

        [TestCase("жил жил жил дом дом дом сом сом сом ", "ж", "жил ")]
        [TestCase("жил жил жил жало жало жало ", "ж", "жало жил ")]
        public void FindWords(string text,string prefix,string trueListWords)
        {
            IGenericRepository<DictionaryWord> repo = new FakeRepository();
            ManagerDictionary manager = new ManagerDictionary(repo);


            manager.CreateDictionary(text);

            List<DictionaryWord> words=manager.FindWords(prefix).ToList();


            StringBuilder result = new StringBuilder();

            foreach (var word in words)
            {
                result.Append(word.Word).Append(" ");
            }

            NUnit.Framework.Assert.AreEqual(result.ToString(), trueListWords);
        }

        //проверка на исключения UpdateDictionary и TestCreateDictionary
    }
}
