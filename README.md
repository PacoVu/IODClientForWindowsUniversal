# HODClient Library for Windows Universal 8.1. V2.0

----
## Overview
HODClient library for Windows Universal is a lightweight C# based API, which helps you easily integrate your Windows app with HPE Haven OnDemand Services.

HODClient library supports both Windows and Windows Phone 8.1.

HODClient library v2.0 supports bulk input (source inputs can be an array) where an HOD API is capable of doing so.

HODClient library requires the .NET 4.5.

----
## Integrate HODClient into a Windows/Windows Phone project
1. Click the "Download Zip" button to download the HODClient library for Windows Universal 8.1.
2. Create a new or open an existing Windows project
3. Right click on the project's References folder and select "Add reference...".
>![](/images/importlibrary1.jpg)
4. Browse to the folder where you saved the library and select the HODClient.dll.
>![](/images/importlibrary2.jpg)

----
## API References
**Constructor**

    HODClient(string apiKey, String version="v1")

*Description:* 
* Creates and initializes an HODClient object.

*Parameters:*
* apiKey: your developer apikey.
* version: Haven OnDemand API version. Currently it only supports version 1. Thus, the default value is "v1".

*Example code:*

    using HOD.Client;
    
    HODClient hodClient = new HODClient("your-api-key");

----
**Function GetRequest**

    void GetRequest(ref Dictionary<String, Object> Params, String hodApp, REQ_MODE mode)

*Description:* 
* Sends a HTTP GET request to a Haven OnDemand API.

*Parameters:*
* Params: a Dictionary object containing key/value pair parameters to be sent to a Haven OnDemand API, where the keys are the parameters of that API. 

>Note: 

>For a parameter with its type is an array<>, the parameter must be defined in a List\<object\>. 
>E.g.:


    var entity_type = new List<object>();
    entity_type.Add("people_eng");
    entity_type.Add("places_eng");
    
    var Params = new Dictionary<string, object>()
    {
        {"url", "http://www.cnn.com" },
        {"entity_type", entity_type }
    };



* hodApp: a string to identify a Haven OnDemand API. E.g. "extractentities". Current supported apps are listed in the HODApps class.

* mode [REQ_MODE.ASYNC | REQ_MODE.SYNC]: specifies API call as Asynchronous or Synchronous.

*Response:*
* If the mode is "ASYNC", response will be returned via the requestCompletedWithJobID(String response) callback function.
* If the mode is "SYNC", response will be returned via the requestCompletedWithContent(String response) callback function.
* If there is an error occurred, the error message will be sent via the onErrorOccurred(String errorMessage) callback function.

*Example code:*
    Call the Entity Extraction API to find people and places from CNN and BBC website
```
String hodApp = HODApps.ENTITY_EXTRACTION;

var urls = new List<object>();
urls.Add("http://www.cnn.com");
urls.Add("http://www.bbc.com");

var entity_type = new List<object>();
entity_type.Add("people_eng");
entity_type.Add("places_eng");

var Params = new Dictionary<string, object>()
{
    {"url", urls },
    {"entity_type", entity_type }
};

hodClient.GetRequest(ref Params, hodApp, HODClient.REQ_MODE.SYNC);
```

**Function PostRequest**

    void PostRequest(ref Dictionary<String, Object> Params, String hodApp, REQ_MODE mode)

*Description:* 
* Sends a HTTP POST request to a Haven OnDemand API.

*Parameters:*
* Params: a Dictionary object containing key/value pair parameters to be sent to a Haven OnDemand API, where the keys are the parameters of that API

> Note:

> 1. In the case of the "file" parameter, the value must be a StorageFile object.
> 2. For a parameter with its type is an array<>, the parameter must be defined in a List\<object\>.
> E.g.:

    var entity_type = new List<object>();
    entity_type.Add("people_eng");
    entity_type.Add("places_eng");
    
    StorageFile file1 = await StorageFile.GetFileFromPathAsync("c:\doc1.txt");
    StorageFile file2 = await StorageFile.GetFileFromPathAsync("c:\doc2.txt");

    var files = new List<object>();
    files.Add(file1);
    files.Add(file2);

    var Params = new Dictionary<string, object>()
    {
        {"file", files },
        {"entity_type", entity_type }
    };

 
* hodApp: a string to identify a Haven OnDemand API. E.g. "ocrdocument". Current supported apps are listed in the HODApps class.

* mode [REQ_MODE.SYNC | REQ_MODE.ASYNC]: specifies API call as Asynchronous or Synchronous.

*Response:*
* If the mode is "ASYNC", response will be returned via the requestCompletedWithJobID(String response) callback function.
* If the mode is "SYNC", response will be returned via the requestCompletedWithContent(String response) callback function.
* If there is an error occurred, the error message will be sent via the onErrorOccurred(String errorMessage) callback function.

*Example code:*
    Call the OCR Document API to scan text from an image file
```
String hodApp = HODApps.OCR_DOCUMENT;
StorageFile file = await StorageFile.GetFileFromPathAsync("c:\image.jpg");
var Params =  new Dictionary<String,Object>
{
    {"file", file},
    {"mode", "document_photo"}
};
hodClient.PostRequest(ref Params, hodApp, HODClient.REQ_MODE.ASYNC);
```

**Function GetJobResult**

    void GetJobResult(String jobID)

*Description:*
* Sends a request to Haven OnDemand to retrieve content identified by the jobID.

*Parameter:*
* jobID: the jobID returned from a Haven OnDemand API upon an asynchronous call.

*Response:* 
* Response will be returned via the requestCompletedWithContent(String response)

*Example code:*
    Parse a JSON string contained a jobID and call the function to get the actual content from Haven OnDemand server 
```
void hodClient_requestCompletedWithJobID(string response)
{
    JsonValue root;
    JsonObject jsonObject;
    if (JsonValue.TryParse(response, out root))
    {
        jsonObject = root.GetObject();
        string jobId = jsonObject.GetNamedString("jobID");
        hodClient.GetJobResult(jobId);
    }
}
```

## API callback functions
You will need to implement callback functions to receive responses from Haven OnDemand server
```
hodClient.requestCompletedWithContent += HodClient_requestCompletedWithContent;
hodClient.requestCompletedWithJobID += HodClient_requestCompletedWithJobID;
hodClient.onErrorOccurred += HodClient_onErrorOccurred;
``` 
When you call the GetRequest() or PostRequest() with the ASYNC mode, or call the GetJobResult() function, the response will be returned to this callback function. The response is a JSON string containing the jobID.
```
private void HodClient_requestCompletedWithJobID(string response)
{
    
}
``` 

When you call the GetRequest() or PostRequest() with the SYNC mode, the response will be returned to this callback function. The response is a JSON string containing the actual result of the service.
```
private void HodClient_requestCompletedWithContent(string response)
{
    
}
``` 

If there is an error occurred, the error message will be returned to this callback function.
```
private void HodClient_onErrorOccurred(string errorMessage)
{
    
}
```

## Demo code 1: 

**Call the Entity Extraction API to extract people and places from cnn.com website with a synchronous GET request**
```
using HOD.Client;
    
namespace HODClientDemo
{
    public sealed partial class MainPage : Page
    {
        HODClient hodClient = new HODClient("your-apikey");

        public MainPage()
        {
            this.InitializeComponent();
                
            hodClient.requestCompletedWithContent += HodClient_requestCompletedWithContent;
            hodClient.requestCompletedWithJobID += HodClient_requestCompletedWithJobID;
            hodClient.onErrorOccurred += HodClient_onErrorOccurred;

            useHODClient();
        }

        private void useHODClient()
        {
            String hodApp = HODApps.ENTITY_EXTRACTION;
                
            var entity_type = new List<object>();
            entity_type.Add("people_eng");
            entity_type.Add("places_eng");

            var Params = new Dictionary<string, object>()
            {
                { "url", "http://www.cnn.com" },
                { "entity_type", entity_type },
                { "unique_entities", "true" }
            };

            hodClient.GetRequest(ref Params, hodApp, HODClient.REQ_MODE.SYNC);
        }

        // implement callback functions

        private void HodClient_requestCompletedWithContent(string response)
        {
            JsonValue root;
            JsonObject jsonObj;
            String people = "";
            String places = "";
            bool ret = JsonValue.TryParse(response, out root);
            if (ret)
            {
                jsonObject = root.GetObject();
                if (jsonObject.ContainsKey("entities"))
                {
                    var entityArray = jsonObject.GetNamedArray("entities");
                    foreach (var entity in entityArray)
                    {
                        var entityObj = entity.GetObject();
                        var type = entityObj.GetNamedString("type");
                        if (type == "people_eng")
                        {
                            people += entityObj.GetNamedString("original_text") + System.Environment.NewLine;
                            // parse any other interested information about a people
                        }
                        else if (type == "places_eng")
                        {
                            places += entityObj.GetNamedString("original_text") + System.Environment.NewLine;
                            // parse any other interested information about a place
                        }
                    }
                }
            }
        }
        private void HodClient_onErrorOccurred(string errorMessage)
        {
	    // handle error if any
        }
    }
}
```

## Demo code 2:
 
**Call the OCR Document API to recognize text from an image with an asynchronous POST request**
```
using HOD.Client;
    
namespace HODClientDemo
{
    public sealed partial class MainPage : Page
    {
        HODClient iodClient = new HODClient("your-apikey");
        StorageFile imageFile;
        public MainPage()
        {
            this.InitializeComponent();
                
            hodClient.requestCompletedWithContent += HodClient_requestCompletedWithContent;
            hodClient.requestCompletedWithJobID += HodClient_requestCompletedWithJobID;
            hodClient.onErrorOccurred += HodClient_onErrorOccurred;
        }

        private void useHODClient()
        {
            String hodApp = hODApps.OCR_DOCUMENT;
            StorageFile imgFile = StorageFile.GetFileFromPathAsync("path/and/filename");
            var Params = new Dictionary<string, object>
            {
                { "file", imageFile },
                { "mode", "document_photo" }
            };

            hodClient.PostRequest(ref Params, hodApp, HODClient.REQ_MODE.ASYNC);
        }

        private async void LoadFileButton_Clicked(object sender, RoutedEventArgs e)
        {
            FileOpenPicker filePicker = new FileOpenPicker();
            filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            filePicker.FileTypeFilter.Add(".jpg");
            filePicker.FileTypeFilter.Add(".png");
            filePicker.ViewMode = PickerViewMode.Thumbnail;

            imageFile = await filePicker.PickSingleFileAsync();

            if (imageFile != null)
            {
                useHODClient();
            }
            else
            {
                //Cancel picking file
            }
            
        }            
        
        // implement callback functions
            
        /**************************************************************************************
        * An async request will result in a response with a jobID. We parse the response to get
        * the jobID and send a request for the actual content identified by the jobID.
        **************************************************************************************/ 
        private void HodClient_requestCompletedWithJobID(string response)
        {
            JsonValue root;
            JsonObject jsonObj;
            bool ret = JsonValue.TryParse(response, out root);
            if (ret)
            {
                jsonObj = root.GetObject();
                String jobID = jsonObj.GetNamedString("jobID");
                hodClient.GetJobResult(jobID);
            }
        }

        private void HodClient_requestCompletedWithContent(string response)
        {
            // response is a json string from server
            JsonValue root;
            JsonObject jsonObj;
            String recognizedText = "";
            if (JsonValue.TryParse(response, out root))
            {
                jsonObject = root.GetObject();
                if (jsonObject.ContainsKey("actions"))
                {
                    var actionArray = jsonObject.GetNamedArray("actions");
                    foreach (var action in actionArray)
                    {
                        var actionObj = action.GetObject();
                        if (actionObj.ContainsKey("result"))
                        {
                            var resultObj = actionObj.GetNamedObject("result");
                            if (resultObj.ContainsKey("text_block"))
                            {
                                var textArray = resultObj.GetNamedArray("text_block");
                                foreach (var textBlock in textArray)
                                {
                                    var obj = textBlock.GetObject();
                                    if (obj.ContainsKey("text"))
                                        recognizedText += obj.GetNamedString("text");
                                }
                            }
                        }
                    }
                }
            }
        }

        private void HodClient_onErrorOccurred(string errorMessage)
        {
            // handle error if any
        }
    }
}
```

## License
Licensed under the MIT License.