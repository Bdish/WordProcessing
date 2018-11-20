using DALWordProc.Entities;
using DALWordProc.Repository.Implementations;
using DALWordProc.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTestWordProc
{
    public class FakeRepository : IGenericRepository<DictionaryWord>
    {
        private List<DictionaryWord> _dictionary;

        public FakeRepository()
        {
            _dictionary = new List<DictionaryWord>();
        }

        public void Create(DictionaryWord item)
        {
            if (_dictionary.Count > 0)
            { 
                int maxId = _dictionary.Select(x => x.Id).Max();
                item.Id = maxId + 1;
            }
            else
            {
                item.Id = 0;
            }

            _dictionary.Add(item);
        }

        public void ExecuteSQLExpression(string SQLExpression)
        {
            //switch case

            if (SQLExpression.Replace(" ", "").ToUpper() == "TRUNCATE TABLE [DictionaryWords]".Replace(" ", "").ToUpper())
            {
                _dictionary.Clear();
            }
            
        }

        public DictionaryWord FindById(int id)
        {
            return _dictionary.Find(x => x.Id == id);
        }

        public IEnumerable<DictionaryWord> Get()
        {
            return _dictionary;
        }

        public IEnumerable<DictionaryWord> Get(Func<DictionaryWord, bool> predicate)
        {
            return _dictionary.Where(predicate);
        }

        public void Remove(DictionaryWord item)
        {
            _dictionary.Remove(item);
        }

        public void RemoveAll()
        {
            _dictionary.Clear();
        }

        public void Update(DictionaryWord item)
        {
            int index = _dictionary.FindIndex(x=>x.Id==item.Id);
            _dictionary[index].Word = item.Word;
            _dictionary[index].Frequency = item.Frequency;
        }


    }
}
