using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Gitty
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            List<RepositoryListing> listing = new List<RepositoryListing>();
            listing.Add(new RepositoryListing() { Name = "Vextar", FilePath = @"C:\repos\vexxy\" });
            listing.Add(new RepositoryListing() { Name = "Guity", FilePath = @"C:\repos\dev\_archive\guity\" });

            LocalRepoList.ItemsSource = listing;

            List<CommitRecord> history = new List<CommitRecord>();
            history.Add(new CommitRecord() { Date = "5/5/95", Author = @"Terry Nguyen <terreh@terrehbyte.com>", Summary = "Correct typo in README.md" });
            history.Add(new CommitRecord() { Date = "5/6/95", Author = @"Terry Nguyen <terreh@terrehbyte.com>", Summary = "Add initial project files" });

            CommitHistory.ItemsSource = history;

            List<ChangeEvent> stagedChanges = new List<ChangeEvent>();
            stagedChanges.Add(new ChangeEvent() { ChangeType = "Add", FilePath = @"README.md" });
            stagedChanges.Add(new ChangeEvent() { ChangeType = "Remove", FilePath = @"password.txt" });

            StagedChanges.ItemsSource = stagedChanges;

            List<ChangeEvent> pendingChanges = new List<ChangeEvent>();
            pendingChanges.Add(new ChangeEvent() { ChangeType = "Modify", FilePath = @"README.md" });
            pendingChanges.Add(new ChangeEvent() { ChangeType = "Add", FilePath = @"main.cs" });

            UnstagedChanges.ItemsSource = pendingChanges;
        }
    }

    public class RepositoryListing
    {
        public string Name { get; set; }
        public string FilePath { get; set; }    // should be a URI
    }

    public class CommitRecord
    {
        public string Date { get; set; }
        public string Author { get; set; }
        public string Summary { get; set; }
    }

    public class ChangeEvent
    {
        public string ChangeType { get; set; }
        public string FilePath { get; set; }
    }
}
