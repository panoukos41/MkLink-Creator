using MkLink_Creator.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace MkLink_Creator.Services
{
    public class MklinkFacade
    {
        public readonly IList<MklinkCommand> Commands;

        public MklinkFacade()
        {
            Commands = new List<MklinkCommand>
            {
                new MklinkCommand { Name = "Directory", Command = "/d", Description = "Creates a directory symbolic link. By default, mklink creates a file symbolic link." },
                new MklinkCommand { Name = "Hard link", Command = "/h", Description = "A hard link is a file system feature that cannot cross a file system boundary and can only target files." },
                new MklinkCommand { Name = "Junction", Command = "/j", Description = "Creates a Directory Junction." }
            };
        }

        public void ExecuteCommand(string command, string linkPath, string destPath, string linkName)
        {
            string Link = InsertQuotes(linkPath + "\\" + linkName);
            string Dest = InsertQuotes(destPath);
            string Command = $"/C mklink {command} {Link} {Dest}";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Normal,
                FileName = "cmd.exe",
                Arguments = Command
            };

            Process process = new Process { StartInfo = startInfo };
            process.Start();
            process.WaitForExit();
            if (process.ExitCode != 0)
                MessageBox.Show("Something went wrong please try again, sometimes the application needs admistrator rights to create Links.", "Error");
            else
            {
                string message = $"Link created successfully at: {Link}";
                MessageBox.Show(message, "Success!");
            }
        }

        private string InsertQuotes(string word)
        {
            word = word.Insert(0, "\"");
            word = word.Insert(word.Count(), "\"");
            return word;
        }
    }
}
