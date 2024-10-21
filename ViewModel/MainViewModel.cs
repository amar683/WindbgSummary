using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using ClosedXML.Excel;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;

namespace WindbgSummary
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _dumpFilePath;
        private string _analysisResult;
        private bool _isAnalyzing;

        public string DumpFilePath
        {
            get => _dumpFilePath;
            set
            {
                _dumpFilePath = value;
                OnPropertyChanged(nameof(DumpFilePath));
                (AnalyzeCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public string AnalysisResult
        {
            get => _analysisResult;
            set
            {
                _analysisResult = value;
                OnPropertyChanged(nameof(AnalysisResult));
            }
        }

        public bool IsAnalyzing
        {
            get => _isAnalyzing;
            set
            {
                _isAnalyzing = value;
                OnPropertyChanged(nameof(IsAnalyzing));
                (AnalyzeCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public ICommand BrowseCommand { get; }
        public ICommand AnalyzeCommand { get; }

        public MainViewModel()
        {
            BrowseCommand = new RelayCommand(Browse);
            AnalyzeCommand = new RelayCommand(Analyze, CanAnalyze);

            // Initialize properties
            DumpFilePath = string.Empty;
            AnalysisResult = string.Empty;
            IsAnalyzing = false;
        }

        private void Browse()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Dump Files (*.dmp)|*.dmp"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                DumpFilePath = openFileDialog.FileName;
            }
        }

        public bool CanAnalyze()
        {
            return !string.IsNullOrWhiteSpace(DumpFilePath) && !IsAnalyzing;
        }

        private async void Analyze()
        {
            if (!CanAnalyze())
                return;

            IsAnalyzing = true;
            AnalysisResult = "Analysis in progress...";

            try
            {
                string windbgResult = await Task.Run(() => RunWindbgCommands(DumpFilePath));
                string analyzedData = await AnalyzeWithChatGPT(windbgResult);
                await Task.Run(() => ExportToExcel(analyzedData));
                AnalysisResult = "Analysis complete. Report generated.";
            }
            catch (HttpRequestException ex)
            {
                AnalysisResult = $"Error communicating with ChatGPT API: {ex.Message}";
            }
            catch (Exception ex)
            {
                AnalysisResult = $"An error occurred: {ex.Message}";
            }
            finally
            {
                IsAnalyzing = false;
            }
        }

        private string RunWindbgCommands(string dumpFile)
        {
            // Define the set of WinDbg commands
            string[] commands = { "!clrstack", "!heapstat" };
            StringBuilder output = new StringBuilder();

            foreach (var command in commands)
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "windbg.exe";
                    process.StartInfo.Arguments = $"-z \"{dumpFile}\" -c \"{command};q\"";
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;  
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;

                    process.Start();

                    string stdout = process.StandardOutput.ReadToEnd();
                    string stderr = process.StandardError.ReadToEnd();
                    output.AppendLine($"Command: {command}");
                    output.AppendLine(stdout);
                    output.AppendLine(stderr);  
                    output.AppendLine(new string('-', 80)); 

                    process.WaitForExit();
                }
            }

            return output.ToString();
        }


        private async Task<string> AnalyzeWithChatGPT(string windbgResult)
        {
            string apiUrl = "https://api.openai.com/v1/chat/completions";
            string apiKey = ""; // Replace with your actual API key

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new { role = "system", content = "You are an expert in analyzing Windows crash dumps. Provide a concise summary of the key issues found in the dump." },
                        new { role = "user", content = windbgResult }
                    }
                };

                string jsonContent = JsonConvert.SerializeObject(requestBody);
                HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Error: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();
                JObject result = JObject.Parse(jsonResponse);
                return result["choices"][0]["message"]["content"].ToString();
            }
        }

        private void ExportToExcel(string analyzedData)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Analysis Report");

                var lines = analyzedData.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                worksheet.Cell(1, 1).Value = "Windbg Analysis Result";  
                for (int i = 0; i < lines.Length; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = lines[i];
                }

                worksheet.Column(1).AdjustToContents();

                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string filePath = Path.Combine(documentsPath, "Windbg_Analysis_Report.xlsx");

                workbook.SaveAs(filePath);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}