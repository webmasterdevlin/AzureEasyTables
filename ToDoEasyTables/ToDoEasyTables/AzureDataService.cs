using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System;
using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ToDoEasyTables
{
    public class AzureDataService
    {
        private readonly MobileServiceClient _mobileServiceClient = new MobileServiceClient("http://easytodo.azurewebsites.net/");
        private IMobileServiceSyncTable<Todo> _todoTable;
        public async Task Initialize()
        {
            if (_mobileServiceClient?.SyncContext?.IsInitialized ?? false) return;
            const string path = "easytodoDb.db"; // rename this each to you create a new table
            //setup our local sqlite store and intialize our table
            MobileServiceSQLiteStore store = new MobileServiceSQLiteStore(path);
            store.DefineTable<Todo>();

            if (_mobileServiceClient != null)
            {
                if (_mobileServiceClient.SyncContext != null)
                    await _mobileServiceClient.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());

                //Get our sync table that will call out to azure
                _todoTable = _mobileServiceClient.GetSyncTable<Todo>();
            }
        }
        public async Task SyncTodo()
        {
            try
            {
                await _todoTable.PullAsync("allTodos", _todoTable.CreateQuery());
                await _mobileServiceClient.SyncContext.PushAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        public async Task<IEnumerable> GetTodos()
        {
            await Initialize();
            await SyncTodo();
            return await _todoTable.ToEnumerableAsync();
        }
        public async Task AddTodo(Todo todo)
        {
            await Initialize();
            await _todoTable.InsertAsync(todo);
            await SyncTodo();
        }
        public async Task DeleteTodo(Todo todo)
        {
            await _todoTable.DeleteAsync(todo);
            await SyncTodo();
        }
        public async Task UpdateTodo(Todo todo)
        {
            await _todoTable.UpdateAsync(todo);
            await SyncTodo();
        }
    }
}