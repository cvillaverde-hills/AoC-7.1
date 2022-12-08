using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp2
{

    struct FileStruct
    {
        public string filename;
        public int size;
    }

    class DirectoryNode
    {
        public string name;
        public int totalSize;
        public DirectoryNode parent;
        public List<DirectoryNode> children;
        public List<FileStruct> files;

        public DirectoryNode(string name, DirectoryNode? current)
        {
            this.name = name;
            totalSize = 0;
            parent = current;
            children = new List<DirectoryNode>();
            files = new List<FileStruct>();
        }

        public void AddFile(FileStruct file)
        {
            files.Add(file);
            totalSize += file.size;
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            string[] input = File.ReadAllLines("input.txt");

            DirectoryNode tree = new DirectoryNode("/", null);

            int line = 1;

            ParseInput(tree, input, ref line);

            Console.WriteLine(BFS(tree));
        }

        private static int BFS(DirectoryNode tree)
        {
            int acc = 0;
            Queue<DirectoryNode> queue = new Queue<DirectoryNode>();

            queue.Enqueue(tree);

            while (queue.Count!= 0)
            {
                DirectoryNode currentDirectory = queue.Dequeue();
                if (currentDirectory.totalSize < 100000)
                {
                    Console.WriteLine($"{currentDirectory.name} {currentDirectory.totalSize}");
                    acc += currentDirectory.totalSize;
                }

                foreach (var dir in currentDirectory.children)
                {
                    queue.Enqueue(dir);
                }
            }
            return acc;
        }

        private static void ParseInput(DirectoryNode dir, string[] input, ref int line)
        {
            while (line < input.Length)
            {
                string currentline = input[line];
                if (currentline == "$ ls")
                {
                    line++;
                }
                else if (currentline.Substring(0, 4) == "dir ")
                {
                    string directoryName = currentline.Substring(4);
                    dir.children.Add(new DirectoryNode(directoryName, dir));
                    line++;
                }
                else if (currentline.Substring(0, 4) == "$ cd")
                {
                    line++;
                    if (currentline == "$ cd ..")
                    {               
                        dir.parent.totalSize += dir.totalSize;
                        ParseInput(dir.parent, input, ref line);
                        return;
                    }
                    else
                    { // cd to a directory    
                        string directoryName = currentline.Substring(5);
                        ParseInput(dir.children.Find(x => x.name == directoryName), input, ref line); // find it in the list of directories
                        return;
                    }
                }
                else
                { // file
                    FileStruct file = new FileStruct();
                    file.filename = currentline.Split(" ")[1];
                    file.size = int.Parse(currentline.Split(" ")[0]);
                    dir.AddFile(file);
                    line++;
                }
            }
            // navigating back to the root
            while (dir.name != "/")
            {
                dir.parent.totalSize += dir.totalSize;
                dir = dir.parent;
            }
        }
    }
}
