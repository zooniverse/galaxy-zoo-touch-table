﻿using GalaxyZooTouchTable.Models;
using Newtonsoft.Json.Linq;
using PanoptesNetClient.Models;
using System.Collections.Generic;

namespace GalaxyZooTouchTable.Tests.Mock
{
    public static class PanoptesServiceMockData
    {
        public static Workflow Workflow(string id)
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
    }
}