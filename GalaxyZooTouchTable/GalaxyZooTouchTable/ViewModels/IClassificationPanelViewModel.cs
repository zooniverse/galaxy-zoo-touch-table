using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using PanoptesNetClient.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace GalaxyZooTouchTable.ViewModels
{
    public interface IClassificationPanelViewModel : INotifyPropertyChanged
    {
        void AddSubscribers();
        void ChooseAnswer(AnswerButton button);
        void GetSubjectQueue();
        void GetSubjectReductions();
        Task GetWorkflow();
        void HandleNotificationsOnSubmit();
        void Load();
        void OnChangeView(ClassifierViewEnum view);
        void OnCloseClassifier(object sender);
        void OnContinueClassification(object sender);
        void OnGetSubjectById(string subjectID);
        void OnOpenClassifier(object sender);
        void OnSelectAnswer(object sender);
        void OnSendRequestToUser(TableUser UserToNotify);
        List<AnswerButton> ParseTaskAnswers(List<TaskAnswer> answers);
        void PrepareForNewClassification();
        void ResetAnswerCount();
        void ResetTimer();
        void ResetTimer(object sender, PropertyChangedEventArgs e);
        void ShowStillThereModal(object sender, System.EventArgs e);
        void StartNewClassification(Subject subject);
        void StartTimer();
        void ToggleCloseConfirmation(object sender);
    }
}
