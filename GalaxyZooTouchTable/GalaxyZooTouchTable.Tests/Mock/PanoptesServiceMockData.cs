﻿using GalaxyZooTouchTable.Models;
﻿using GalaxyZooTouchTable.Lib;
using Newtonsoft.Json.Linq;
using PanoptesNetClient.Models;
using System.Collections.Generic;

namespace GalaxyZooTouchTable.Tests.Mock
{
    public static class PanoptesServiceMockData
    {
        public static List<TaskAnswer> TaskAnswerList()
        {
            List<TaskAnswer> TaskAnswers = new List<TaskAnswer>();
            TaskAnswer FirstTask = new TaskAnswer("First Task", "T1");
            TaskAnswer SecondTask = new TaskAnswer("Second Task", "T2");
            TaskAnswers.Add(FirstTask);
            TaskAnswers.Add(SecondTask);
            return TaskAnswers;
        }

        public static TableSubject TableSubject()
        {
            return new TableSubject("1", "www.fakewebsite.com", 22.22, 33.33);
        }

        public static Workflow Workflow(string id = null)
        {
            List<TaskAnswer> TaskAnswers = new List<TaskAnswer>()
            {
                new TaskAnswer("Hello", "T1")
            };
            Dictionary<string, WorkflowTask> WorkflowTasks = new Dictionary<string, WorkflowTask>()
            {
                {"T0", new WorkflowTask("Choose an Answer", TaskAnswers ) }
            };
            return new Workflow()
            {
                Id = id,
                Tasks = WorkflowTasks,
                FirstTask = "T0",
                Version = "123.123"
            };
        }

        public static AnswerButton AnswerButton()
        {
            TaskAnswer Answer = new TaskAnswer("Smooth Galaxy", "T1");
            return new AnswerButton(Answer, 1);
        }

        public static List<TableSubject> TableSubjects()
        {
            List<TableSubject> subjectList = new List<TableSubject>();
            subjectList.Add(TableSubject());
            return subjectList;
        }

        public static CompletedClassification CompletedClassification()
        {
            return new CompletedClassification(AnswerButton(), GlobalData.GetInstance().PinkUser, "1");
        }
    }
}
