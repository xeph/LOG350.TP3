namespace OdbcSQLite
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TasksListView_Initialized(object sender, System.EventArgs e)
        {
            var table = new System.Data.DataTable("Tasks");
            table.Columns.Add(new System.Data.DataColumn("Name"));
            table.Columns.Add(new System.Data.DataColumn("DeadlineID"));
            table.Columns.Add(new System.Data.DataColumn("PriorityID"));
            table.Columns.Add(new System.Data.DataColumn("Completion"));

            var row1 = table.NewRow();
            row1["Name"] = "Rapport du TP3";
            row1["DeadlineID"] = 1;
            row1["PriorityID"] = 1;
            row1["Completion"] = 0.15;
            table.Rows.Add(row1);

            var row2 = table.NewRow();
            row2["Name"] = "Entrevue pour un stage";
            row2["DeadlineID"] = 2;
            row2["PriorityID"] = 2;
            row2["Completion"] = 0;
            table.Rows.Add(row2);

            TasksListView.ItemsSource = table.DefaultView;
        }
    }
}