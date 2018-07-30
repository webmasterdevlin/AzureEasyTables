using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ToDoEasyTables
{
    public class MainViewModel : BaseViewModel
    {
        private readonly AzureDataService _azureDataService = new AzureDataService();
        private int _todosIndex;

        private ObservableCollection<Todo> _todoObservable;

        public ObservableCollection<Todo> TodosObservable
        {
            get => _todoObservable;
            set
            {
                _todoObservable = value;
                OnPropertyChanged();
            }
        }

        private string _newTodo;

        public string NewTodo
        {
            get => _newTodo;
            set
            {
                _newTodo = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _todoDeadline = DateTime.Today;

        public DateTime? TodoDeadline
        {
            get => _todoDeadline;
            set
            {
                _todoDeadline = value;
                OnPropertyChanged();
            }
        }

        private string _todoId;

        public string TodoId
        {
            get => _todoId;
            set
            {
                _todoId = value;
                OnPropertyChanged();
            }
        }

        private bool _isOnActivityIndicator = true;

        public bool IsOnActivityIndicator
        {
            get => _isOnActivityIndicator;
            set
            {
                _isOnActivityIndicator = value;
                OnPropertyChanged();
            }
        }

        private bool _isRefreshing;

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            TodosObservable = new ObservableCollection<Todo>();
            Initialized();
            CheckActivityIndicator();
        }

        private async void Initialized()
        {
            IEnumerable todos = await _azureDataService.GetTodos();
            foreach (Todo todo in todos)
            {
                TodosObservable.Add(todo);
            }

            CheckActivityIndicator();
        }

        private async void ReInitialized()
        {
            IEnumerable todos = await _azureDataService.GetTodos();
            foreach (Todo todo in todos)
            {
                TodosObservable.Add(todo);
            }
        }

        private ICommand _refreshCommand;
        public ICommand RefreshCommand => _refreshCommand = _refreshCommand ?? new Command(() =>
        {
            IsOnActivityIndicator = !IsOnActivityIndicator;
            TodosObservable.Clear();
            ReInitialized();
            IsOnActivityIndicator = !IsOnActivityIndicator;
            IsRefreshing = false;
        });

        private ICommand _addTodoCommand;
        public ICommand AddTodoCommand => _addTodoCommand = _addTodoCommand ?? new Command(async () => await CreateEditTodo());

        private ICommand _editDeleteTodoCommand;
        public ICommand EditDeleteTodoCommand =>
            _editDeleteTodoCommand = _editDeleteTodoCommand ?? new Command<Todo>(async todo => await ActionSheet(todo));

        public void CheckActivityIndicator()
        {
            if (TodosObservable == null || TodosObservable.Count >= 1)
            {
                IsOnActivityIndicator = false;
            }
        }

        private async Task CreateEditTodo()
        {
            if (string.IsNullOrWhiteSpace(NewTodo))
            {
                await Application.Current.MainPage.DisplayAlert("Hi there!", "You need to write a todo task", "OK");
                return;
            }

            if (TodoId != null)
            {
                var editedTodo = new Todo
                {
                    Id = TodoId,
                    Title = NewTodo,
                    Deadline = TodoDeadline
                };

                TodosObservable[_todosIndex].Title = NewTodo;
                NewTodo = "";
                TodoDeadline = DateTime.Today;
                await _azureDataService.UpdateTodo(editedTodo);
                return;
            }

            Todo todo = new Todo
            {
                Title = NewTodo,
                IsDone = false,
                Deadline = TodoDeadline
            };

            TodosObservable.Add(todo);
            NewTodo = "";
            TodoDeadline = DateTime.Today;
            await _azureDataService.AddTodo(todo);
        }

        private async Task ActionSheet(Todo todo)
        {
            var actionSheet = await Application.Current.MainPage.DisplayActionSheet("What do you want to do? ",
                "Cancel", "Delete", "Edit details", "Mark done");
            switch (actionSheet)
            {
                case "Edit details":
                    TodoId = todo.Id;
                    NewTodo = todo.Title;
                    TodoDeadline = todo.Deadline;
                    _todosIndex = TodosObservable.IndexOf(todo);
                    break;

                case "Mark done":
                    todo.IsDone = !todo.IsDone;
                    await _azureDataService.UpdateTodo(todo);
                    break;

                case "Delete":
                    TodosObservable.Remove(todo);
                    await _azureDataService.DeleteTodo(todo);
                    break;
            }
        }
    }
}