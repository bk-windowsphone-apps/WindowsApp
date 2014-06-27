using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using BkWinApp.Resources;
using Windows.Phone.Speech.Recognition;
using System.Net.NetworkInformation;

namespace BkWinApp
{
    public class TodoItem
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "complete")]
        public bool Complete { get; set; }
    }
    public class Registrations
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "handle")]
        public string Handle { get; set; }
    }
    public partial class MainPage : PhoneApplicationPage
    {
        // MobileServiceCollectionView implements ICollectionView (useful for databinding to lists) and 
        // is integrated with Mobile Service to make it easy to bind data to the ListView
        private MobileServiceCollection<TodoItem, TodoItem> items;
        private MobileServiceUser user; // user to authenticate
        //Declaration of SpeechRecognizer object
        private SpeechRecognizerUI recoWithUI;
        private bool isNewPageInstance;

        private async System.Threading.Tasks.Task Authenticate()
        {
            //Verify network connectivity is available before attempting authentication
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("Network connection unavailable. Please close app, verify your network connection, and try again.");
                return;
            }
            while (user == null)
            {
                string message;
                try
                {
                    user = await App.MobileService.LoginAsync(MobileServiceAuthenticationProvider.MicrosoftAccount);
                    message = string.Format("You are now logged in - {0}", user.UserId);
                }
                catch (InvalidOperationException)
                {
                    message = "Please log in using MS account. Login Required";
                }
                MessageBox.Show(message);
            }
        }

        // Create and configure the SpeechRecognizerUI object.
        private void ConfigureRecognizer()
        {
            recoWithUI = new SpeechRecognizerUI();
            recoWithUI.Settings.ListenText = "Speak your voice reminder.";
            recoWithUI.Settings.ReadoutEnabled = true;
            recoWithUI.Settings.ShowConfirmation = true;
        }

        private IMobileServiceTable<TodoItem> todoTable = App.MobileService.GetTable<TodoItem>();

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            this.Loaded += MainPage_Loaded;
            isNewPageInstance = true;
        }

        private async void InsertTodoItem(TodoItem todoItem)
        {
            // This code inserts a new TodoItem into the database. When the operation completes
            // and Mobile Services has assigned an Id, the item is added to the CollectionView
            await todoTable.InsertAsync(todoItem);
            items.Add(todoItem);
        }

        private async void RefreshTodoItems()
        {
            // This code refreshes the entries in the list view be querying the TodoItems table.
            // The query excludes completed TodoItems
            try
            {
                items = await todoTable
                    .Where(todoItem => todoItem.Complete == false)
                    .ToCollectionAsync();
            }
            catch (MobileServiceInvalidOperationException e)
            {
                MessageBox.Show(e.Message, "Error loading items", MessageBoxButton.OK);
            }

            ListItems.ItemsSource = items;
        }

        private async void UpdateCheckedTodoItem(TodoItem item)
        {
            // This code takes a freshly completed TodoItem and updates the database. When the MobileService 
            // responds, the item is removed from the list 
            await todoTable.UpdateAsync(item);
            items.Remove(item);
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshTodoItems();
        }
        // bar button
        private void refBtn_Click(object sender, EventArgs e)
        {
            RefreshTodoItems();
        }

       /* private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var todoItem = new TodoItem { Text = TodoInput.Text };
            InsertTodoItem(todoItem);
        }*/

        // Initiate the capture of a voice note and store it to the 
        // Azure database if the user is satisfied
        private async void speechBtn_Click(object sender, EventArgs e)
        {
            // Begin recognition using the default grammar and store the result.
            SpeechRecognitionUIResult recoResult = await recoWithUI.RecognizeWithUIAsync();

            // Check that a result was obtained
            if (recoResult.RecognitionResult != null)
            {
                // Determine if the user wants to save the note.
                var result = MessageBox.Show(string.Format("Heard you say \"{0}\" Save?", recoResult.RecognitionResult.Text), "Confirmation", MessageBoxButton.OKCancel);

                // Save the result to the Azure Mobile Service DB if the user is satisfied.
                if (result == MessageBoxResult.OK)
                {
                    var todoItem = new TodoItem { Text = recoResult.RecognitionResult.Text };
                    InsertTodoItem(todoItem);
                }
            }
        }

        private void CheckBoxComplete_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TodoItem item = cb.DataContext as TodoItem;
            item.Complete = true;
            UpdateCheckedTodoItem(item);
        }

       // protected override void OnNavigatedTo(NavigationEventArgs e)
       // {
       //   RefreshTodoItems();
       //}   // commented-out to disable and replaced with authentication MainPage_Loaded

        async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await Authenticate();
            RefreshTodoItems();
        }

        // Configure application bar 
        private void BuildApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton speechButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/microphone.png", UriKind.Relative));
            speechButton.Text = "Speak";
            ApplicationBar.Buttons.Add(speechButton);
            speechButton.Click += new EventHandler(speechBtn_Click);
            
            ApplicationBarIconButton refreshButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/refresh.png", UriKind.Relative));
            refreshButton.Text = "Refresh";
            ApplicationBar.Buttons.Add(refreshButton);
            refreshButton.Click += new EventHandler(refBtn_Click);

        }

        // Configure app bar and voice recognition, as well as initiate authentication and list refresh
        // the first time page is navigated to.
        protected async override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (isNewPageInstance)
            {
                isNewPageInstance = false;
                await Authenticate();
                if (user != null)
                {
                    BuildApplicationBar();
                    ConfigureRecognizer();
                    RefreshTodoItems();
                }
            }
        }

        // Ensure logout from Azure Mobile Services when user leaves application
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (user != null)
            {
                App.MobileService.Logout();
            }
        }

    }
}