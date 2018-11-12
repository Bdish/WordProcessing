using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BLLWordProc;
using DALWordProc.EFDbContext;
using DALWordProc.Entities;
using DALWordProc.Repository.Implementations;
using DALWordProc.Repository.Interfaces;
using Microsoft.Extensions.CommandLineUtils;

namespace ConsoleWordProc
{
    //на рефакторинг
    class Program
    {
        /// <summary>
        /// Определение кодировки UTF8 по сигнатуре кодировки в фале.
        /// </summary>
        /// <param name="path">Полный путь и имя файла.</param>
        /// <returns>Подтверждена ли кодировка.</returns>
        public static bool IsUTF8(string path)
        {
            DetectEncodingType detect = new DetectEncodingType();
            detect.AddDetectEncodingType(EncodingType.UTF8,
                (data) => { if (data.Length >= 3 && data[0] == 0xEF && data[1] == 0xBB && data[2] == 0xBF) return true; return false; });

            detect.SetBOM(path);

            EncodingType type = EncodingType.NotDefined;     
            
            type = detect.Detect();    
            
            if (type == EncodingType.UTF8)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Настройка команд для идентификации ввода в консоли cmd.exe.
        /// </summary>
        /// <param name="args"> аргументы сонсоли cmd.exe</param>
        /// <returns>Настроенный экземпляр реализации команд консоли cmd.exe</returns>
        public static CommandLineApplication InitCommandLine(string[] args)
        {
            

             CommandLineApplication commandLineApplication = new CommandLineApplication(throwOnUnexpectedArg: false);

             var createDictionary = commandLineApplication.Option(
             "--create | -c",
             "The command to create a dictionary, you must specify the name of the file with the text. A dictionary will be created from the text.",
             CommandOptionType.SingleValue);

             var updateDictionary = commandLineApplication.Option(
                 "--update | -u",
                 "The command to update the dictionary, you must specify the name of the file with the text." +
                 " New words will be added from the text, and existing words in the dictionary will be updated.",
                 CommandOptionType.SingleValue);

             var deleteDictionary = commandLineApplication.Option(
                 "--delete | -d",
                 "The command to delete the dictionary.",
                 CommandOptionType.NoValue);

             commandLineApplication.HelpOption("-? | -h | --help");


             commandLineApplication.OnExecute(() =>
             {
                 int numberInsertCommand = 0;
                 if (createDictionary.HasValue())
                 {
                     numberInsertCommand++;
                 }

                 if (updateDictionary.HasValue())
                 {
                     numberInsertCommand++;
                 }

                 if (deleteDictionary.HasValue())
                 {
                     numberInsertCommand++;
                 }

                 if (numberInsertCommand > 1)
                 {
                     Console.WriteLine("You can specify only one command-line parameter at a time (commands)");
                     Environment.Exit(0);
                     return 0;
                 }

                 DBDictionaryWord dbContext;
                 IGenericRepository<DictionaryWord> repo;
                 ManagerDictionary manager;
                 try
                 {
                     dbContext = new DBDictionaryWord();
                     repo = new GenericRepository<DictionaryWord>(dbContext);
                     manager = new ManagerDictionary(repo);
                 }
                 catch(Exception ex)
                 {
                     Console.WriteLine(ex.Message);
                     Environment.Exit(0);
                     return 0;
                 }

                 if (createDictionary.HasValue())
                 {
                    
                    if (IsUTF8(createDictionary.Value()))
                    {
                         
                        string text = "";
                        text = File.ReadAllText(createDictionary.Value(), Encoding.Default);
                        manager.CreateDictionary(text);
                         
                    }
                    else
                    {
                        Console.WriteLine("Error: The file format is not UTF8.");
                    }
                    
                     
                 }


                 if (updateDictionary.HasValue())
                 {
                     
                    if (IsUTF8(updateDictionary.Value()))
                    {
                        string text = "";
                        text = File.ReadAllText(updateDictionary.Value(), Encoding.Default);
                        manager.UpdateDictionary(text);
                    }
                    else
                    {
                        Console.WriteLine("Error: The file format is not UTF8.");
                    }
                     
                 }


                 if (deleteDictionary.HasValue())
                 {
                         
                        manager.DeleteDictionary();
                         
                     }
                 Environment.Exit(0);
                 return 0;
             });
            return commandLineApplication;
             

             
        }

        static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                try
                {
                    var commandLineApplication = InitCommandLine(args);
                    commandLineApplication.Execute(args);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Environment.Exit(0);
                    return;
                }
            }

            //без параметров командной строки, при этом оно должно автоматически переходить в режим ввода, стандартный поток ввода – с клавиатуры
            StringBuilder prefix = new StringBuilder();
            ConsoleKeyInfo info ;
            while (true)
            {

                info = Console.ReadKey(true);
                Console.Write(info.KeyChar);
                
                if (info.Key == ConsoleKey.Enter && prefix.Length==0 || info.Key == ConsoleKey.Escape)//выход при нажатии Esc или ввода пустой строки
                {
                Environment.Exit(0);
                break;
                }
                else
                if (info.Key == ConsoleKey.Enter && prefix.Length > 0)//ввели не пустую строку
                {
                    List<DictionaryWord> arrayWords ;
                    try
                    {
                        DBDictionaryWord dbContext = new DBDictionaryWord();
                        IGenericRepository<DictionaryWord> repo = new GenericRepository<DictionaryWord>(dbContext);
                        BaseManagerDictionary manager = new ManagerDictionary(repo);
                        arrayWords = manager.FindWords(prefix.ToString());
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Environment.Exit(0);
                        return;
                    }

                    Console.WriteLine();

                    foreach (var word in arrayWords)
                    {
                        Console.WriteLine("- "+word.Word);
                    }
                    prefix.Clear();
                }
                else
                {
                    prefix.Append(info.KeyChar);//собираем символы ввода не esc и не enter
                }
               
            }
        }

        

    }
}
