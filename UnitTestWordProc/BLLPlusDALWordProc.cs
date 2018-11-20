using BLLWordProc;
using DALWordProc.EFDbContext;
using DALWordProc.Entities;
using DALWordProc.Repository.Implementations;
using DALWordProc.Repository.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using Unity.Lifetime;

namespace UnitTestWordProc
{
    [TestFixture]
    public class BLLPlusDALWordProc
    {
        [TestCase("жил жил жил дом дом ом ом ом сом сом сом сом йцукенгшщзфывапh йцукенгшщзфывапh йцукенгшщзфывапh йцукенгшщзфываg йцукенгшщзфываg йцукенгшщзфываg", "жил 3 сом 4 йцукенгшщзфываg 3 ")]
        public void CreateDictionary(string text, string trueResult)
        {
            GlobalSetting.MaxLengthWord = 15;
            GlobalSetting.MaxNumberOfWordsReturned = 5;
            GlobalSetting.MinFrequencyWord = 3;
            GlobalSetting.MinLengthWord = 3;
            GlobalSetting.SeparationCharacters = new char[] { ' ' };

            DBDictionaryWord dbContext = new DBDictionaryWord();
            IGenericRepository<DictionaryWord> repo = new GenericRepository<DictionaryWord>(dbContext);
            BaseManagerDictionary manager = new ManagerDictionary(repo);

            if (repo.Get().Any() )
            {
                manager.DeleteDictionary();
            }

            manager.CreateDictionary(text);

            StringBuilder result = new StringBuilder();

            foreach (var word in repo.Get())
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
        public void UpdateDictionary(string text1, string text2, string trueResult)
        {
          /*  IUnityContainer container = new UnityContainer();
            
            container.RegisterType<DBDictionaryWord>(new ContainerControlledLifetimeManager());
            container.RegisterType<IGenericRepository<DictionaryWord>, GenericRepository<DictionaryWord>>();
            container.RegisterType<BaseManagerDictionary, ManagerDictionary>(new ContainerControlledLifetimeManager());
            var repo = container.Resolve<GenericRepository<DictionaryWord>>();
            var manager = container.Resolve<ManagerDictionary>();*/
            DBDictionaryWord dbContext = new DBDictionaryWord();
            IGenericRepository<DictionaryWord> repo = new GenericRepository<DictionaryWord>(dbContext);
            BaseManagerDictionary manager = new ManagerDictionary(repo);

            if (repo.Get().Any())
            {
                manager.DeleteDictionary();
            }
            manager.CreateDictionary(text1);

            manager.UpdateDictionary(text2);

            StringBuilder result = new StringBuilder();

            foreach (var word in repo.Get())
            {
                result.Append(word.Word).Append(" ").Append(word.Frequency).Append(" ");
            }

            NUnit.Framework.Assert.AreEqual(result.ToString(), trueResult);
        }

        [TestCase("жил жил жил ", "")] 
        public void DeleteDictionary(string text, string trueResult)
        {
            DBDictionaryWord dbContext = new DBDictionaryWord();
            IGenericRepository<DictionaryWord> repo = new GenericRepository<DictionaryWord>(dbContext);
            BaseManagerDictionary manager = new ManagerDictionary(repo);

            if (repo.Get().Any())
            {
                manager.DeleteDictionary();
            }

            manager.CreateDictionary(text);

            manager.DeleteDictionary();

            StringBuilder result = new StringBuilder();

            foreach (var word in repo.Get())
            {
                result.Append(word.Word).Append(" ").Append(word.Frequency).Append(" ");
            }

            NUnit.Framework.Assert.AreEqual(result.ToString(), trueResult);
        }

        [TestCase("жил жил жил дом дом дом сом сом сом ", "ж", "жил ")]
        [TestCase("жил жил жил жало жало жало ", "ж", "жало жил ")]
        [TestCase("блок блок блок блок блок блок" +
            " блондинка блондинка блондинка блондинка блондинка" +
            " блоха блоха блоха блоха блоха блоха" +
            " бл бл бл бл бл" +
            " блокпост блокпост блокпост блокпост" +
            " блокада блокада блокада " +
            " бло", "бло", "блок блоха блондинка блокпост блокада ")]
        public void FindWords(string text, string prefix, string trueListWords)
        {
            DBDictionaryWord dbContext = new DBDictionaryWord();
            IGenericRepository<DictionaryWord> repo = new GenericRepository<DictionaryWord>(dbContext);
            BaseManagerDictionary manager = new ManagerDictionary(repo);

            if (repo.Get().Any())
            {
                manager.DeleteDictionary();
            }

            manager.CreateDictionary(text);

            List<DictionaryWord> words = manager.FindWords(prefix).ToList();


            StringBuilder result = new StringBuilder();

            foreach (var word in words)
            {
                result.Append(word.Word).Append(" ");
            }

            NUnit.Framework.Assert.AreEqual(result.ToString(), trueListWords);
        }
    }
}
