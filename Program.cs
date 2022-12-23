using System.Diagnostics;
using System.Xml.Linq;

class DataReader
{
    public static ConsoleColor DefaultColor = ConsoleColor.Yellow;
    public static bool ShouldContinue = true;

    private static string ExecFolderPath = "C:\\";
    private static List<string> folderLst = new List<string>();
    private static string? CurrentFile = null;

    private static int indexChoice = 0;
    private static List<EntryFileFolder> entryFileFolders = new List<EntryFileFolder>();

    public static void Main(string[] args)
    {
        string? tempExecFolderPath = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);

        if (tempExecFolderPath != null)
            ExecFolderPath = tempExecFolderPath;

        Console.ForegroundColor = DataReader.DefaultColor;
        Console.WriteLine("Welcome in DataReader V 1.0 by fedjoyd5\n\nPress ENTER to continue ...");
        Console.ReadKey();

        GenerateListEntryFolderFile(false);

        while (ShouldContinue)
        {
            Console.Clear();

            Console.WriteLine("DataREader V 1.0 by fedjoyd5\n");

            if (CurrentFile != null)
            {
                string tempTextFilePath = ExecFolderPath + "\\";

                for (int i = 0; i < folderLst.Count; i++)
                {
                    tempTextFilePath = tempTextFilePath + folderLst[i] + "\\";
                }

                tempTextFilePath = tempTextFilePath + CurrentFile + ".txt";

                if (!File.Exists(tempTextFilePath))
                {
                    folderLst.Clear();
                    CurrentFile = null;
                    continue;
                }

                string fileData = File.ReadAllText(tempTextFilePath);
                int indexCurrentChar = 0;
                bool InstantPass = false;
                DateTime NextUpdate = DateTime.Now;

                while(indexCurrentChar < fileData.Length)
                {
                    if (Console.KeyAvailable)
                    {
                        Console.ReadKey();
                        InstantPass = true;
                    }

                    if (InstantPass || DateTime.Now > NextUpdate)
                    {
                        if (!InstantPass)
                            NextUpdate = DateTime.Now.AddMilliseconds(10);

                        if (fileData[indexCurrentChar] == ']')
                            Console.ForegroundColor = DataReader.DefaultColor;

                        Console.Write(fileData[indexCurrentChar]);

                        if (fileData[indexCurrentChar] == '[' && (indexCurrentChar + 1) < fileData.Length)
                        {
                            char TempCurrentCharUpper = char.ToUpper(fileData[indexCurrentChar + 1]);

                            switch(TempCurrentCharUpper)
                            {
                                case 'G':
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    indexCurrentChar++;
                                    break;
                                case 'R':
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    indexCurrentChar++;
                                    break;
                                case 'B':
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    indexCurrentChar++;
                                    break;
                                case 'C':
                                    Console.ForegroundColor = ConsoleColor.Cyan;
                                    indexCurrentChar++;
                                    break;
                                case 'M':
                                    Console.ForegroundColor = ConsoleColor.Magenta;
                                    indexCurrentChar++;
                                    break;
                                case 'W':
                                    Console.ForegroundColor = ConsoleColor.White;
                                    indexCurrentChar++;
                                    break;

                                case 'V':
                                case 'T':
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    break;
                                case 'F':
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    break;

                                case 'N':
                                    indexCurrentChar++;
                                    break;
                            }
                        }

                        indexCurrentChar++;
                    }
                }

                Console.WriteLine("\n\nRETURN to exit, ESCAPE to Quit");
            }
            else
            {
                for (int i = 0; i < entryFileFolders.Count; i++)
                {
                    if (i == indexChoice) SelectedTEXT();

                    if (entryFileFolders[i].IsFolder)
                        Console.WriteLine(entryFileFolders[i].Name);
                    else
                        Console.WriteLine(entryFileFolders[i].Name.ToUpper());

                    ResetSelectedTEXT();
                }

                Console.WriteLine("\nUP/DOWN to navigate folder and key, ENTER to enter folder/key, RETURN to exit folder/key, ESCAPE to Quit");
            }

            ConsoleKeyInfo key = Console.ReadKey();

            // traitement

            if (key.Key == ConsoleKey.Escape) ShouldContinue = false;

            if (key.Key == ConsoleKey.UpArrow && CurrentFile == null && indexChoice > 0) indexChoice--;
            if (key.Key == ConsoleKey.DownArrow && CurrentFile == null && indexChoice < (entryFileFolders.Count - 1)) indexChoice++;

            if (key.Key == ConsoleKey.Enter && entryFileFolders.Count > 0)
            {
                if (entryFileFolders[indexChoice].IsFolder)
                {
                    folderLst.Add(entryFileFolders[indexChoice].Name);
                    GenerateListEntryFolderFile(folderLst.Count > 0);
                }
                else
                {
                    CurrentFile = entryFileFolders[indexChoice].Name;
                }
            }

            if (key.Key == ConsoleKey.Backspace && (folderLst.Count > 0 || CurrentFile != null))
            {
                if (CurrentFile != null)
                {
                    CurrentFile = null;
                }
                else
                {
                    folderLst.RemoveAt(folderLst.Count - 1);
                }

                GenerateListEntryFolderFile(folderLst.Count > 0);
            }
        }
    }

    public static void SelectedTEXT()
    {
        Console.BackgroundColor = DataReader.DefaultColor;
        Console.ForegroundColor = ConsoleColor.Black;
    }

    public static void ResetSelectedTEXT()
    {
        Console.ResetColor();

        Console.ForegroundColor = DataReader.DefaultColor;
    }

    public static void GenerateListEntryFolderFile(bool IncludeFile = true)
    {
        indexChoice = 0;

        entryFileFolders.Clear();

        string tempExecFolderPath = ExecFolderPath + "\\";

        for (int i = 0; i < folderLst.Count; i++)
        {
            tempExecFolderPath = tempExecFolderPath + folderLst[i] + "\\";
        }

        if (!Directory.Exists(tempExecFolderPath))
        {
            folderLst.Clear();
            tempExecFolderPath = ExecFolderPath + "\\";
            IncludeFile = false;
        }

        string[] tempArray = Directory.GetDirectories(tempExecFolderPath);

        foreach (string folder in tempArray)
        {
            entryFileFolders.Add(new EntryFileFolder(true, folder.Substring(folder.LastIndexOf('\\') + 1)));
        }

        if (IncludeFile)
        {
            tempArray = Directory.GetFiles(tempExecFolderPath);

            foreach (string file in tempArray)
            {
                if (!file.EndsWith(".txt"))
                    continue;

                string fileName = file.Substring(file.LastIndexOf('\\') + 1);

                int ind = fileName.LastIndexOf('.');
                if (ind != -1)
                    fileName = fileName.Substring(0, ind);

                entryFileFolders.Add(new EntryFileFolder(false, fileName));
            }
        }
    }
}

struct EntryFileFolder
{
    public EntryFileFolder(bool p_isFolder, string p_name)
    {
        IsFolder = p_isFolder;
        Name = p_name;
    }

    public bool IsFolder;
    public string Name;
}