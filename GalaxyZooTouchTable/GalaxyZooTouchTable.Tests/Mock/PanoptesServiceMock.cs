using GalaxyZooTouchTable.Services;
using Newtonsoft.Json.Linq;
using PanoptesNetClient.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace GalaxyZooTouchTable.Tests.Mock
{
    public class PanoptesServiceMock : IPanoptesService
    {
        private Workflow MockWorkflow { get; set; }
        private Subject MockSubject { get; set; }

        public PanoptesServiceMock()
        {
            ConstructMockSubject();
            ConstructMockWorkflow();
        }

        private void ConstructMockSubject()
        {
            List<dynamic> mockLocation = new List<dynamic>()
            {
                new JArray()
            };
            MockSubject = new Subject()
            {
                Id = "1",
                Locations = mockLocation
            };

        }

        private void ConstructMockWorkflow()
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
            MockWorkflow = new Workflow()
            {
                Id = "1",
                Tasks = WorkflowTasks,
                FirstTask = "T0",
                Version = "123.123"
            };
        }

        public Task CreateClassificationAsync(Classification classification)
        {
            throw new NotImplementedException();
        }

        public Task<Subject> GetSubjectAsync(string id)
        {
            return Task.FromResult<Subject>(new Subject());
        }

        public Task<List<Subject>> GetSubjectsAsync(string route, NameValueCollection query)
        {
            List<Subject> subjectList = new List<Subject>();
            subjectList.Add(MockSubject);
            return Task.FromResult<List<Subject>>(subjectList);
        }

        public Task<Workflow> GetWorkflowAsync(string id)
        {
            return Task.FromResult<Workflow>(MockWorkflow);
        }
    }
}
