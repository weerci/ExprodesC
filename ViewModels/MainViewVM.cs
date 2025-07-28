using ExprodesC.Models;
using ExprodesC.Services;
using Func;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

namespace ExprodesC.ViewModels
{
    public class MainViewVM() : BaseVM
    {
        private readonly IDialogService? _dialogService;

        public MainViewVM(IDialogService dialogService, IProject project) : this()
        {
            _dialogService = dialogService;
            Project = project;

            LoadProject = ReactiveCommand.Create(loadProject);
            CloseMessage = ReactiveCommand.Create(() => { MessageIsOpen = false; });
            SaveProject = ReactiveCommand.Create(saveProject, this.WhenAnyValue(vm => vm.Project.IsChanged).Select(n=>n));
            CloseProject = ReactiveCommand.Create(closeProject);

            Log.Errors.Subscribe(x => { Message = x.Last().Item.Current; MessageIsOpen = true; });
            Log.Messages.Subscribe(x => { Message = x.Last().Item.Current; MessageIsOpen = true; });
        }

        #region Properties

        public IProject Project { get; } = null!;

        [Reactive] public bool HasError { get; private set; }
        [Reactive] public ExpMessage? Message { get; private set; }
        [Reactive] public bool MessageIsOpen { get; private set; }

        #endregion

        #region Command

        /// <summary>
        /// Из файла проекта (.mgj) или из файла подготовленных к разбору генотипов (.txt, .csv) загружается проект или набор генотипов
        /// </summary>
        public RxCommandUnit? LoadProject { get; }
        public RxCommandUnit? CloseMessage { get; }

        /// <summary>
        /// Создается новый проект
        /// </summary>
        public ICommand? NewProject { get; }

        /// <summary>
        /// Проект закрывается
        /// </summary>
        public ICommand? CloseProject { get; }

        /// <summary>
        /// Сохранение проекта
        /// </summary>
        public RxCommandUnit? SaveProject { get; }

        #endregion

        #region Imp

        private async void loadProject()
        {
            var file = await _dialogService!.OpenFileAsync();
            if (file is null) return;


            Project.LoadFromFile(FileName.Open(file.Path.LocalPath))
                .Match(OnError: e => { Log.SendError(Lang.Resources.err_load_profile, e); });
        }

        private async void saveProject()
        {
            if (Project.PathToSavedFile != null)
                Project.Save(Project.PathToSavedFile);
            else
            {
                var file = await _dialogService!.SaveFileAsync();
                if (file is null) return;

                Project.Save(FileName.OpenOrCreate(file.Path.AbsolutePath));
            }
        }

        private void closeProject()
        {
            Project.Close();
        }
        #endregion

    }
}
