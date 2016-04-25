using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mironiasty.Random.Rownloglerator
{
    public class TaskExecutor<TaskParam>
    {
        private readonly int numberOfConcurrentTasks;

        public TaskExecutor()
            : this(Environment.ProcessorCount) { }

        public TaskExecutor(int numberOfConcurrentTasks)
        {
            this.numberOfConcurrentTasks = numberOfConcurrentTasks;
        }

        public void Execute(IEnumerable<TaskParam> taskParams, Action<TaskParam> task)
        {
            Func<TaskParam, int, Task> taskRunner = (param, number) => Task.Run(() => task(param));
            Execute(taskParams, taskRunner);
        }
        public void Execute(IEnumerable<TaskParam> taskParams, Action<TaskParam, int> task)
        {
            Func<TaskParam, int, Task> taskRunner = (param, number) => Task.Run(() => task(param, number));
            Execute(taskParams, taskRunner);
        }
        private void Execute(IEnumerable<TaskParam> taskParams, Func<TaskParam, int, Task> taskRunner)
        {
            int taskCounter = 0;
            var nextTaskIndex = 0;
            var listOfRunningTasks = new Task[numberOfConcurrentTasks];
            foreach (var taskParam in taskParams)
            {
                listOfRunningTasks[nextTaskIndex] = taskRunner(taskParam, taskCounter);
                if (taskCounter >= numberOfConcurrentTasks - 1)
                {
                    nextTaskIndex = Task.WaitAny(listOfRunningTasks);
                }
                else
                {
                    nextTaskIndex++;
                }
                taskCounter++;
            }
            Task.WaitAll(listOfRunningTasks);
        }

    }
}
