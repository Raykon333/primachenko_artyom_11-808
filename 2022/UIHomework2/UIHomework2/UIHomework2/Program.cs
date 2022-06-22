using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UIHomework2
{
    class FileManager
    {
        string CurrentPath;

        public FileManager()
        {
            ChangeDirectory(false, Path.GetPathRoot(Environment.SystemDirectory));
        }

        public void ShowCurrentDirectoryInfo(bool showName, bool showPath, bool showSubfolders, bool showFiles)
        {
            if (showName)
                Console.WriteLine("Name: " + CurrentPath.Split("\\").Last());
            if (showPath)
                Console.WriteLine("Path: " + CurrentPath);
            if (showSubfolders)
            {
                Console.WriteLine("Subfolders:");
                var subfolders = Directory.GetDirectories(CurrentPath);
                for (int i = 0; i < subfolders.Length; i++)
                {
                    Console.WriteLine("\t" + subfolders[i].Split("\\").Last());
                }
            }
            if (showFiles)
            {
                Console.WriteLine("Files:");
                var files = Directory.GetFiles(CurrentPath);
                for (int i = 0; i < files.Length; i++)
                {
                    Console.WriteLine("\t" + files[i].Split("\\").Last());
                }
            }
        }

        public void ChangeDirectory(bool isPathRelative, string path)
        {
            string newPath;
            if (isPathRelative)
            {
                newPath = Path.Combine(CurrentPath, path);
            }
            else
            {
                newPath = path;
            }
            if (!Directory.Exists(newPath))
                Console.WriteLine("Directory doesn't exist");
            else
            {
                CurrentPath = Path.GetFullPath(newPath);
                Console.Title = CurrentPath;
            }
        }
        
        public void CopyFiles(string[] fileNames, string copyTo)
        {
            Directory.SetCurrentDirectory(CurrentPath);
            List<string> unsuccesful = new List<string>();
            for (int i = 0; i < fileNames.Length; i++)
            {
                try
                {
                    File.Copy(fileNames[i], copyTo + "\\" + fileNames[i]);
                }
                catch(Exception e)
                {
                    unsuccesful.Add(fileNames[i]);
                }
            }
            Console.WriteLine(fileNames.Length - unsuccesful.Count() + " files copied successfully");
            if (unsuccesful.Count() > 0)
            {
                Console.WriteLine("The following files couldn't be copied:");
                for (int i = 0; i < unsuccesful.Count(); i++)
                {
                    Console.WriteLine("\t" + unsuccesful[i]);
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            int choice = 0;
            Console.WriteLine();
            Console.WriteLine("Choose option:");
            Console.WriteLine("1 - Command language mode");
            Console.WriteLine("2 - Menu mode (current)");
            while (choice < 1 || choice > 2)
                int.TryParse(Console.ReadKey().KeyChar.ToString(), out choice);
            Console.WriteLine();

            if (choice == 1)
            {
                Console.WriteLine("Enter commands:");
                var cl = new CommandLanguage();
                cl.Run();
            }
            if (choice == 2)
            {
                var menu = new Menu();
                menu.Run();
            }
        }
    }

    class CommandLanguage
    {
        bool isRunning;
        FileManager fileManager;

        public void Run()
        {
            isRunning = true;
            fileManager = new FileManager();
            while (isRunning)
            {
                Console.WriteLine();
                var command = Console.ReadLine();
                Interpret(command);
            }
        }

        void Interpret(string command)
        {
            var split = command.Split(' ');
            var main = split[0];
            var args = split.Skip(1).ToArray();

            switch(main)
            {
                case "info":
                    Info(args);
                    break;
                case "help":
                    Help(args);
                    break;
                case "dir":
                    Dir(args);
                    break;
                case "copy":
                    Copy(args);
                    break;
                case "exit":
                    isRunning = false;
                    break;
                default:
                    Console.WriteLine("Uknown command.");
                    break;
            }
        }

        void Info(string[] args)
        {
            bool invalidArgs = false;
            bool showName = false;
            bool showFiles = false;
            bool showSubfolders = false;
            bool showPath = false;
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-n":
                    case "--name":
                        showName = true;
                        break;
                    case "-s":
                    case "--subfolders":
                        showSubfolders = true;
                        break;
                    case "-f":
                    case "--files":
                        showFiles = true;
                        break;
                    case "-p":
                    case "--path":
                        showPath = true;
                        break;
                    default:
                        invalidArgs = true;
                        break;
                }
            }
            if (invalidArgs)
            {
                Console.WriteLine("Invalid arguments.");
                return;
            }

            if (!showName && !showFiles && !showSubfolders && !showPath)
            {
                showName = true;
                showFiles = true;
                showSubfolders = true;
                showPath = true;
            }
            fileManager.ShowCurrentDirectoryInfo(showName, showPath, showSubfolders, showFiles);
        }

        //ДОПИСАТЬ
        //ДОПИСАТЬ
        //ДОПИСАТЬ
        void Help(string[] args)
        {
            if (args.Length == 0)
                Console.WriteLine(" help\n info\n dir\n copy\n exit");
            else if (args.Length == 1)
            {
                switch (args[0])
                {
                    case "help":
                        Console.WriteLine("--------------");
                        break;
                    case "info":
                        Console.WriteLine("--------------");
                        break;
                    case "dir":
                        Console.WriteLine("---------------");
                        break;
                    case "copy":
                        Console.WriteLine("---------------");
                        break;
                    default:
                        Console.WriteLine($"Command {args[0]} doesn't exist");
                        break;
                }
            }
            else
                Console.WriteLine("Too many arguments.");
        }

        void Dir(string[] args)
        {
            bool invalidArguments = false;
            if (args.Length > 2)
                invalidArguments = true;
            else
            {
                bool rArgument = args.Contains("-r") || args.Contains("--relative");
                bool aArgument = args.Contains("-a") || args.Contains("--absolute");
                if (rArgument && aArgument)
                    invalidArguments = true;
                if (!invalidArguments)
                {
                    bool isRelative = !aArgument;
                    string path = args.Where(a => a != "-r" && a != "--relative" && a != "-a" && a != "--absolute").FirstOrDefault();
                    if (path == default)
                        invalidArguments = true;
                    else
                        fileManager.ChangeDirectory(isRelative, path);
                }
            }
            if (invalidArguments)
                Console.WriteLine("Invalid arguments.");
        }

        void Copy(string[] args)
        {
            bool invalidArgs = false;
            int k = 0;
            string path = null;
            List<string> files = new List<string>();
            invalidArgs = args.Length < 4;
            while (k < args.Length && !invalidArgs)
            {
                if (k < args.Length && args[k] == "-p" || args[k] == "--path")
                {
                    path = args[k + 1];
                    k += 2;
                }
                else if (k < args.Length && args[k] == "-f" || args[k] == "--files")
                {
                    k++;
                    while (k < args.Length && args[k][0] != '-')
                    {
                        files.Add(args[k]);
                        k++;
                    }
                }
                else
                {
                    invalidArgs = true;
                    break;
                }
            }
            if (invalidArgs)
                Console.WriteLine("Invalid arguments.");
            else
                fileManager.CopyFiles(files.ToArray(), path);
        }
    }

    class Menu
    {
        FileManager fileManager;

        public Menu()
        {
            fileManager = new FileManager();
        }

        public void Run()
        {
            int nextMenu = 0;
            while (nextMenu != 4)
            {
                switch (nextMenu)
                {
                    case 0:
                        nextMenu = MainMenu();
                        break;
                    case 1:
                        nextMenu = InfoMenu();
                        break;
                    case 2:
                        nextMenu = DirMenu();
                        break;
                    case 3:
                        nextMenu = CopyMenu();
                        break;
                    case 4:
                        break;
                }
            }
        }

        //0
        int MainMenu()
        {
            int choice = 0;
            Console.WriteLine();
            Console.WriteLine("Choose option:");
            Console.WriteLine("1 - Get current directory info");
            Console.WriteLine("2 - Go to new directory");
            Console.WriteLine("3 - Copy files from current directory to another");
            Console.WriteLine("4 - End program");
            while (choice < 1 || choice > 4)
                int.TryParse(Console.ReadKey().KeyChar.ToString(), out choice);
            Console.WriteLine();

            return choice;
        }

        //1
        int InfoMenu()
        {
            int choice = 0;
            Console.WriteLine();
            Console.WriteLine("Choose option:");
            Console.WriteLine("1 - Show directory name");
            Console.WriteLine("2 - Show path to directory");
            Console.WriteLine("3 - List subfolders");
            Console.WriteLine("4 - List files");
            Console.WriteLine("5 - Go back");
            while (choice < 1 || choice > 5)
                int.TryParse(Console.ReadKey().KeyChar.ToString(), out choice);
            Console.WriteLine();

            switch (choice)
            {
                case 1:
                    fileManager.ShowCurrentDirectoryInfo(true, false, false, false);
                    break;
                case 2:
                    fileManager.ShowCurrentDirectoryInfo(false, true, false, false);
                    break;
                case 3:
                    fileManager.ShowCurrentDirectoryInfo(false, false, true, false);
                    break;
                case 4:
                    fileManager.ShowCurrentDirectoryInfo(false, false, false, true);
                    break;
                case 5:
                    return 0;
            }
            return 1;
        }

        //2
        int DirMenu()
        {
            Console.WriteLine();
            Console.WriteLine("Enter path to new directory:");
            string path = Console.ReadLine();
            fileManager.ChangeDirectory(false, path);
            return 0;
        }

        //3
        int CopyMenu()
        {
            Console.WriteLine();
            Console.WriteLine("Enter files names divided by space:");
            string[] files = Console.ReadLine().Split(' ');
            Console.WriteLine("Enter path to new directory:");
            string path = Console.ReadLine();
            fileManager.CopyFiles(files, path);
            return 0;
        }
    }
}
