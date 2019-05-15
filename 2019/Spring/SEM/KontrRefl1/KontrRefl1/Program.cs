using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace KontrRefl1
{
    class StudentInfo
    {
        public readonly string Name;
        public readonly int Completed;

        public StudentInfo(string name, int completed)
        {
            Name = name;
            Completed = completed;
        }
    }

    class TaskInfo
    {
        public readonly string TaskName;
        public readonly string ClassName;
        public readonly string MethodName;
        public readonly string ReturnValue;
        public readonly string[] ParamsNames;
        public readonly string[] ParamsValues;

        public TaskInfo(string taskName, string className, string methodName,
            string returnValue, string[] paramsNames, string[] paramsValues)
        {
            TaskName = taskName;
            ClassName = className;
            MethodName = methodName;
            ReturnValue = returnValue;
            ParamsNames = paramsNames;
            ParamsValues = paramsValues;
        }
    }

    class Program
    {

        static List<StudentInfo> Result = new List<StudentInfo>();
        static List<TaskInfo> Tasks = new List<TaskInfo>();

        static void ProcessStudentFolder(DirectoryInfo studentFolder)
        {
            string studentName = studentFolder.Name;
            int completed = 0;

            foreach(var task in Tasks)
            {
                string studentSolution = File.ReadAllText(
                    Path.Combine(studentFolder.FullName, task.TaskName));
                if (studentSolution == task.ReturnValue)
                    completed++;
            }

            Result.Add(new StudentInfo(studentName, completed));
        }

        static void ProcessTask(FileInfo taskFile)
        {
            string task = File.ReadAllText(taskFile.FullName);
            string[] taskInfo = task.Split(';');
            string[] paramsNames = new string[(taskInfo.Length - 3) / 2];
            string[] paramsValues = new string[(taskInfo.Length - 3) / 2];
            for (int i = 3; i < taskInfo.Length; i++)
            {
                if (i % 2 == 1)
                    paramsNames[i / 2 - 1] = taskInfo[i];
                else
                    paramsValues[i / 2 - 2] = taskInfo[i];
            }
            Tasks.Add(new TaskInfo(Path.GetFileName(taskFile.Name), taskInfo[0],
                taskInfo[1], taskInfo[2], paramsNames, paramsValues));
        }

        static void Main(string[] args)
        {
            DirectoryInfo root = new DirectoryInfo(@"C:\root");
            DateTime time = new DateTime(2019, 1, 1, 0, 0, 0);
            Parallel.ForEach(root.GetFiles("*.txt"), ProcessTask);
            Parallel.ForEach(root.GetDirectories()
                .Where(folder => folder.GetFiles()
                .Any(file => file.LastWriteTime > time)),
                ProcessStudentFolder);
            Parallel.ForEach(Result,
                (student) => Console.WriteLine(student.Name + "\t" + student.Completed));
        }
    }
}
