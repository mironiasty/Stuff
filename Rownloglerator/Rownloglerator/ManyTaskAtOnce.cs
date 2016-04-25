using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mironiasty.Random.Rownloglerator
{
    public class ManyTaskAtOnce
    {
        public void Start(int totalTasks)
        {
            var tasks = GenerateTasks(totalTasks);
            var num = 0;
            foreach (var task in tasks)
            {
                var waiter = Task.Run(() => DifficultTask(task, num));
                waiter.Wait();
                num++;
            }
        }

        public void Start(int totalTasks, int concurrent)
        {
            var taskExecutor = new TaskExecutor<TimeSpan>(concurrent);
            var tasks = GenerateTasks(10);

            taskExecutor.Execute(tasks, DifficultTask);
        }

        private static List<TimeSpan> GenerateTasks(int totalTask)
        {
            var rand = new System.Random();
            var tasks = new List<TimeSpan>();
            for (var i = 0; i < totalTask; i++)
            {
                tasks.Add(TimeSpan.FromMilliseconds(rand.Next(1000, 3000)));
            }

            return tasks;
        }

        public static void DifficultTask(TimeSpan timeToTake, int num)
        {
            Console.WriteLine($"Start {num}");
            var future = DateTime.Now + timeToTake;
            while (DateTime.Now < future)
            {
            }
            Console.WriteLine($"Stop {num}");
        }
    }
}
