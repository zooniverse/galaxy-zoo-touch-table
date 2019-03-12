﻿using GalaxyZooTouchTable.Models;
using Newtonsoft.Json.Linq;
using PanoptesNetClient.Models;
using System.Collections.Generic;

namespace GalaxyZooTouchTable.Tests.Mock
{
    public static class PanoptesServiceMockData
    {
        public static TableSubject TableSubject = new TableSubject("1", "www.fakewebsite.com", 22.22, 33.33);

        public static List<TaskAnswer> TaskAnswerList()
        {
            List<TaskAnswer> TaskAnswers = new List<TaskAnswer>();
            TaskAnswer FirstTask = new TaskAnswer()
            {
                Label = "First Task",
                Next = "T1"
            };
            TaskAnswer SecondTask = new TaskAnswer()
            {
                Label = "Second Task",
                Next = "T2"
            };
            TaskAnswers.Add(FirstTask);
            TaskAnswers.Add(SecondTask);
            return TaskAnswers;
        }

        public static Workflow Workflow(string id = null)
        {
            List<TaskAnswer> TaskAnswers = new List<TaskAnswer>()
            {
                new TaskAnswer()
                {
                    Label="Hello", Next="T1"
                }
            };
            Dictionary<string, WorkflowTask> WorkflowTasks = new Dictionary<string, WorkflowTask>()
            {
                {"T0", new WorkflowTask(){ Question = "Choose an Answer", Answers = TaskAnswers } }
            };
            return new Workflow()
            {
                Id = id,
                Tasks = WorkflowTasks,
                FirstTask = "T0",
                Version = "123.123"
            };
        }

        public static Subject Subject()
        {
            List<dynamic> mockLocation = new List<dynamic>()
                {
                    new JArray()
                };
            return new Subject()
            {
                Id = "1",
                Locations = mockLocation
            };

        }

        public static AnswerButton AnswerButton()
        {
            TaskAnswer Answer = new TaskAnswer();
            Answer.Label = "Smooth Galaxy";
            Answer.Next = "T1";
            return new AnswerButton(Answer, 1);
        }

        public static List<Subject> Subjects()
        {
            List<Subject> subjectList = new List<Subject>();
            subjectList.Add(Subject());
            return subjectList;
        }

        public static List<TableSubject> TableSubjects()
        {
            List<TableSubject> subjectList = new List<TableSubject>();
            subjectList.Add(TableSubject);
            return subjectList;
        }
    }
}
