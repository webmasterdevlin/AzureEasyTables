using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace ToDoEasyTables
{
    public class Todo : BaseViewModel
    {
        public string Id { get; set; }

        private string _title;

        public string Title
        {
            get => _title;
            set
            {
                _title = value; 
                OnPropertyChanged();
            }
        }

        private bool _isDone;

        public bool IsDone
        {
            get => _isDone;
            set
            {
                _isDone = value; 
                OnPropertyChanged();
            }
        }

        private DateTime? _deadline;

        public DateTime? Deadline
        {
            get => _deadline;
            set
            {
                _deadline = value; 
                OnPropertyChanged();
            }
        }
    }
}