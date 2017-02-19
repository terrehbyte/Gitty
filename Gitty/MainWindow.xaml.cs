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

using LibGit2Sharp;

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

            GitActionTabs.SelectionChanged += HandleTabSelected;
            CommitButton.Click += CommitButton_Click;
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            using (var repo = new Repository(@"C:\Users\terryn\Documents\visual studio 2015\Projects\Gitty"))
            {
                Signature author = new Signature(repo.Config.Get<string>("user.name").Value, repo.Config.Get<string>("user.email").Value, DateTimeOffset.Now);
                repo.Commit(CommitSummaryInput.Text + "\n\n" + CommitDescInput.Text, author, author);

                CommitSummaryInput.Clear();
                CommitDescInput.Clear();
            }
        }

        private ChangeEvent.ChangeType ConvertEnumType(FileStatus libType)
        {
            return ((libType & (FileStatus.ModifiedInIndex   | FileStatus.ModifiedInWorkdir)) != 0)     ? ChangeEvent.ChangeType.MODIFIED :
                   ((libType & (FileStatus.DeletedFromIndex  | FileStatus.DeletedFromWorkdir)) != 0)    ? ChangeEvent.ChangeType.DELETED :
                   ((libType & (FileStatus.NewInIndex        | FileStatus.NewInWorkdir)) != 0)          ? ChangeEvent.ChangeType.ADDED :
                   ((libType & (FileStatus.RenamedInIndex    | FileStatus.RenamedInWorkdir)) != 0)      ? ChangeEvent.ChangeType.RENAMED :
                   ((libType & (FileStatus.TypeChangeInIndex | FileStatus.TypeChangeInWorkdir)) != 0)   ? ChangeEvent.ChangeType.TYPED :
                   ((libType & FileStatus.Unaltered) != 0)                                              ? ChangeEvent.ChangeType.UNALTERED :
                                                                                                          ChangeEvent.ChangeType.UNKNOWN;
        }
        private void HandleTabSelected(object sender, SelectionChangedEventArgs e)
        {
            if (RepoTab.IsSelected)
            {

            }
            else if (LogTab.IsSelected)
            {
                using (var repo = new Repository(@"C:\Users\terryn\Documents\visual studio 2015\Projects\Gitty"))
                {
                    List<CommitRecord> commits = new List<CommitRecord>();

                    int logLimit = 50;
                    foreach (var com in repo.Commits)
                    {
                        if (--logLimit < 0) { break; }

                        commits.Add(new CommitRecord() { Date = com.Author.When.Date.ToShortDateString(), Author = com.Author.Name, Summary = com.MessageShort });
                    }

                    CommitHistory.ItemsSource = commits;
                }
            }
            else if (CommitTab.IsSelected)
            {
                using (var repo = new Repository(@"C:\Users\terryn\Documents\visual studio 2015\Projects\Gitty"))
                {
                    var status = repo.RetrieveStatus();

                    // retrieve unstaged changes
                    var unstagedFlags = FileStatus.ModifiedInWorkdir | FileStatus.DeletedFromWorkdir | FileStatus.NewInWorkdir | FileStatus.RenamedInWorkdir | FileStatus.TypeChangeInWorkdir;
                    var unstaged = from chg in status
                                   where (chg.State & unstagedFlags) != 0
                                   select chg;

                    List<ChangeEvent> unstagedChanges = new List<ChangeEvent>();
                    foreach (var chg in unstaged)
                    {
                        unstagedChanges.Add(new ChangeEvent() { Change = ConvertEnumType(chg.State), FilePath = chg.FilePath, Staged = false });
                    }


                    // retrieve staged changes
                    var stagedFlags = FileStatus.ModifiedInIndex | FileStatus.DeletedFromIndex | FileStatus.NewInIndex | FileStatus.RenamedInIndex | FileStatus.TypeChangeInIndex;
                    var staged = from chg in status
                                 where (chg.State & stagedFlags) != 0
                                 select chg;

                    List<ChangeEvent> stagedChanges = new List<ChangeEvent>();
                    foreach (var chg in staged)
                    {
                        stagedChanges.Add(new ChangeEvent() { Change = ConvertEnumType(chg.State), FilePath = chg.FilePath, Staged = true });
                    }

                    // assign to ui elements
                    UnstagedChanges.ItemsSource = unstagedChanges;
                    StagedChanges.ItemsSource = stagedChanges;
                }
            }

            e.Handled = true;
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
        public enum ChangeType
        {
            MODIFIED,
            DELETED,
            ADDED,
            RENAMED,
            TYPED,
            UNALTERED,
            UNKNOWN
        }

        public bool Staged { get; set; }
        public ChangeType Change { get; set; }
        public string FilePath { get; set; }
    }
}
