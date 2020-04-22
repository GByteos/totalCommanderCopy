using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace totalCommanderCopy
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WindowWidth = 150;
            

            if (args.Length == 2)
            {
                Console.WriteLine("Dou you want to copy the following files?\n");

                Console.WriteLine(args[0]);

                List<string> files = File.ReadAllLines(args[0], Encoding.GetEncoding("Windows-1252")).ToList();
                List<string> filesExisting = new List<string>();

                foreach (var file in files)
                {
                    if (File.Exists(file))
                    {
                        Console.WriteLine(file);
                        filesExisting.Add(file);
                    }
                    else if (Directory.Exists(file))
                    {
                        Console.WriteLine(file);
                        filesExisting.Add(file);
                    }
                }

                bool valid = false;

                int returnSumm = 0;

                Console.WriteLine($"\nTo: {args[1]}\n");

                while (!valid)
                {                    
                    Console.Write("Y/N:");

                    char keyPressed = Console.ReadKey().KeyChar;

                    if (keyPressed == 'Y' | keyPressed == 'y' | keyPressed == 'J' | keyPressed == 'j')
                    {
                        string arguments = $" /np /njh /njs /R:5 /W:5 /MT:20"; //$" /e /eta /nfl /ndl /R:5 /W:5 /MT:20";

                        ProcessStartInfo roboCopyStartInfo = new ProcessStartInfo();
                        roboCopyStartInfo.FileName = @"C:\Windows\System32\Robocopy.exe";
                        roboCopyStartInfo.RedirectStandardOutput = true;
                        roboCopyStartInfo.UseShellExecute = false;

                        string sourcePath;
                        string destinationPath = args[1];

                        bool isFileSelected = false;                        

                        List<string> directories = new List<string>();

                        foreach (var file in files)
                        {
                            if (file[file.Length - 1] == '\\')
                            {
                                directories.Add(file);                                
                            }
                            else
                            {
                                if (!isFileSelected)
                                {
                                    sourcePath = Path.GetDirectoryName(file);
                                    roboCopyStartInfo.Arguments = $"\"{ sourcePath }\" \"{ destinationPath }";
                                }

                                roboCopyStartInfo.Arguments += $" \"{ Path.GetFileName(file) }\"";
                                                                
                                isFileSelected = true;
                            }                            
                        }

                        if (isFileSelected)
                        {
                            roboCopyStartInfo.Arguments += arguments;

                            Console.WriteLine($"\n{ roboCopyStartInfo.Arguments }\n");

                            using (Process robocopy = Process.Start(roboCopyStartInfo))
                            {
                                using (StreamReader reader = robocopy.StandardOutput)
                                {
                                    while (!reader.EndOfStream)
                                    {
                                        string line = reader.ReadLine();
                                        Console.WriteLine(line);
                                    }
                                }

                                if (robocopy.ExitCode > 1)
                                {
                                    returnSumm = robocopy.ExitCode;
                                }
                            }
                        }

                        if (directories.Count > 0)
                        {
                            destinationPath = destinationPath.Remove(destinationPath.Length - 1, 1);

                            foreach (var directory in directories)
                            {
                                sourcePath = Path.GetDirectoryName(directory);
                                string dir = directory.Remove(directory.Length - 1, 1);
                                dir = Path.GetFileName(dir);

                                roboCopyStartInfo.Arguments = $"\"{ sourcePath }\" \"{ destinationPath }\\{ dir }\" /e{ arguments }";

                                Console.WriteLine($"\n{ roboCopyStartInfo.Arguments }");

                                using (Process robocopy = Process.Start(roboCopyStartInfo))
                                {
                                    using (StreamReader reader = robocopy.StandardOutput)
                                    {
                                        while (!reader.EndOfStream)
                                        {
                                            string line = reader.ReadLine();
                                            Console.WriteLine(line);
                                        }
                                    }

                                    if (robocopy.ExitCode > 1)
                                    {
                                        returnSumm = robocopy.ExitCode;
                                    }                                    
                                }
                            }
                        }                        

                        valid = true;
                    }

                    else if ((keyPressed == 'N') | (keyPressed == 'n'))
                    {
                        valid = true;
                    }
                }
                if (returnSumm > 1)
                {
                    Console.WriteLine($"Exit code: { returnSumm }");

                    Console.ReadKey();
                }
            }                       
        }
    }
}
