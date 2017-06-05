using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todos.ViewModels
{
    class TodoItemViewModel
    {
        private ObservableCollection<Models.TodoItem> allItems = new ObservableCollection<Models.TodoItem>();
        public ObservableCollection<Models.TodoItem> AllItems { get { return this.allItems; } }

        private Models.TodoItem selectedItem = default(Models.TodoItem);
        public Models.TodoItem SelectedItem { get { return selectedItem; } set { this.selectedItem = value; }  }

        public TodoItemViewModel()
        {
            // 加入两个用来测试的item

            allItems.Add(new Models.TodoItem("123", "123", DateTime.Now));
            allItems.Add(new Models.TodoItem("456", "456", DateTime.Now));
        }

        public void AddTodoItem(string title, string description, DateTimeOffset date)
        {
            this.allItems.Add(new Models.TodoItem(title, description, date));
        }

        public void RemoveTodoItem(string id)
        {
            // DIY
            this.allItems.Remove(this.selectedItem);
            // set selectedItem to null after remove
            this.selectedItem = null;
        }

        public void UpdateTodoItem(string id, string title, string description, DateTimeOffset date)
        {
            // DIY
            this.selectedItem.title = title;
            this.selectedItem.description = description;
            this.selectedItem.date = date;
            // set selectedItem to null after update
            this.selectedItem = null;
        }

    }
}
