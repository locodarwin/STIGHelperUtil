using Microsoft.Win32;
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
using System.Xml;

namespace STIGhelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<string, string> _testResults = new Dictionary<string, string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenResultsFile_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            OpenFileDialog dlg = new OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".htm";
            dlg.Filter = "All files (*.*)|*All-Settings*.*";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                GetResultDictionary resultDict = new GetResultDictionary();
                int numPassed = resultDict.LoadDictionary(filename);
                _testResults = resultDict.results;
                int numTests = _testResults.Count();
                int percentPassed = (numPassed * 100) / numTests;

                ResultsFileTests.Text = "found " +  numTests + " tests with " + numPassed + " passing = " + percentPassed + "%";
            }

        }

        private void SeedCheckpointFile_Click(object sender, RoutedEventArgs e)
        {
			if (_testResults.Count <= 0)
			{
				ResultsFileTests.Text = "Must first open a results file";
			}
			else
			{
				// Create OpenFileDialog 
				OpenFileDialog dlg = new OpenFileDialog();

				// Set filter for file extension and default file extension 
				dlg.DefaultExt = ".ckl";
				dlg.Filter = "Ckl files (*.*)|*.ckl";

				// Display OpenFileDialog by calling ShowDialog method 
				Nullable<bool> result = dlg.ShowDialog();

				// Get the selected file name and display in a TextBox 
				if (result == true)
				{
					try
					{
						ClobberCheckpointFile cpf = new ClobberCheckpointFile();
						cpf.fileName = dlg.FileName;
						cpf.clobber(_testResults, System.Convert.ToBoolean(OverRideSettings.IsChecked));
						ResultsFileTests.Text = "operation finished";
					}
					catch
					{
						ResultsFileTests.Text = "Failed: perhaps chk, results mismatch";
					}
				}
			}
        }
    }
}
