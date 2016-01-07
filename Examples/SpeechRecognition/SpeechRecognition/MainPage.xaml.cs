using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using HOD.Client;
using Windows.UI;
using Windows.Data.Json;
using Windows.Storage;
using Windows.Storage.Pickers;
using HOD.Response.Parser;

namespace SpeechRecognition
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Windows.UI.Core.CoreDispatcher messageDispatcher = Window.Current.CoreWindow.Dispatcher;
        HODClient hodClient = new HODClient("your-api-key");
        string jobID = "";
        StorageFile file = null;
        HODResponseParser parser = new HODResponseParser();
        DispatcherTimer timer;
        int count = 0;
        public MainPage()
        {
            this.InitializeComponent();
            hodClient.onErrorOccurred += HodClient_onErrorOccurred;
            hodClient.requestCompletedWithContent += HodClient_requestCompletedWithContent;
            hodClient.requestCompletedWithJobID += HodClient_requestCompletedWithJobID;
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, object e)
        {
            timer.Stop();
            if (hodClient != null && jobID != "")
                hodClient.GetJobStatus(jobID);
        }

        private void HodClient_requestCompletedWithJobID(string response)
        {
            
            HodClient_onErrorOccurred("get job status...");
            jobID = parser.ParseJobID(response);
            if (jobID != "")
                hodClient.GetJobStatus(jobID);
         
        }

        async private void HodClient_requestCompletedWithContent(string response)
        {
            await messageDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                jobID = "";
                outputText.Blocks.Clear();
                RecognizeSpeechResponse resp = (RecognizeSpeechResponse)parser.ParseStandardResponse(StandardResponse.RECOGNIZE_SPEECH, response);
                String text = "";
                if (resp != null)
                {
                    foreach (RecognizeSpeechResponse.Document doc in resp.document)
                    {
                        text += "Paragraph: " + doc.content + "\n";
                        text += "Offset: " + doc.offset.ToString() + "\n";
                    }
                }
                else
                {
                    var errors = parser.GetLastError();
                    foreach (HODErrorObject err in errors)
                    {
                        if (err.error == HODErrorCode.QUEUED)
                        {
                            jobID = err.jobID;
                            text = "In queue. Try again in 2 secs";
                            timer.Interval = TimeSpan.FromSeconds(2);
                            timer.Start();
                            break;
                        }
                        else if (err.error == HODErrorCode.IN_PROGRESS)
                        {
                            jobID = err.jobID;
                            count += 20;
                            text = "In progress. Try again in 20 secs. Total waiting time: " + count.ToString();
                            timer.Interval = TimeSpan.FromSeconds(20);
                            timer.Start();
                            break;
                        }
                        else if (err.error == HODErrorCode.NONSTANDARD_RESPONSE)
                        {
                            // The response is a non-standard response. Define a custom class and use the ParseCustomResponse<T>() function
                            
                            break;
                        }
                        else
                        {
                            text += "Error code: " + err.error.ToString() + "\n";
                            text += "Error reason: " + err.reason + "\n";
                            text += "Error detail: " + err.detail + "\n";
                            text += "job ID: " + err.jobID + "\n";
                        }
                    }
                }
                var p = new Windows.UI.Xaml.Documents.Paragraph();
                p.Inlines.Add(new Windows.UI.Xaml.Documents.Run { Foreground = new SolidColorBrush(Colors.Green), Text = text });

                outputText.Blocks.Add(p);
            });
        }

        async private void HodClient_onErrorOccurred(string errorMessage)
        {
            await messageDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                outputText.Blocks.Clear();
                var p = new Windows.UI.Xaml.Documents.Paragraph();
                p.Inlines.Add(new Windows.UI.Xaml.Documents.Run { Foreground = new SolidColorBrush(Colors.Green), Text = errorMessage });

                outputText.Blocks.Add(p);
            });
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (jobID.Length > 0)
            {
                return;
            }
            
            if (file == null)
            {
                LoadFilePicker(null, null);
                return;
            }
            
            var Params = new Dictionary<string, object>()
            {
                {"file", file },
                {"interval", "20000" }
            };
            HodClient_onErrorOccurred("Submit Speech Recognition request. Please wait.");
            hodClient.PostRequest(ref Params, HODApps.RECOGNIZE_SPEECH, HODClient.REQ_MODE.ASYNC);
        }

        async private void LoadFilePicker(object sender, RoutedEventArgs e)
        {
            FileOpenPicker filePicker = new FileOpenPicker();
            filePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            filePicker.FileTypeFilter.Add(".mp3");

            filePicker.ViewMode = PickerViewMode.List;

            file = await filePicker.PickSingleFileAsync();

            if (file != null)
            {
                SubmitBtn.IsEnabled = true;
            }
            else
            {
                HodClient_onErrorOccurred("No file was picked.");
            }
        }
    }
}
