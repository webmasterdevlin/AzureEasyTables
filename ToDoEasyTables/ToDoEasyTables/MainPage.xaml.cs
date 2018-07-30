using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ToDoEasyTables
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();

            BindingContext = new MainViewModel();
		}

	    private void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
	    {
	        ListTodos.SelectedItem = null;
	    }
    }
}
