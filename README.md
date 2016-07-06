# HavenOnDemand library for Windows Universal 8.1. V2.0

----
## Overview
HODClient library for Windows Universal is a lightweight C# based API, which helps you easily access over 60 APIs from HPE HavenOnDemand platform.

The library contains 2 packages:

HODClient package for sending HTTP GET/POST requests to HavenOnDemand APIs.

HODResponseParser package for parsing JSON responses from HavenOnDemand APIs.

HODClient library requires the .NET 4.5.

----
## Integrate HODClient into a Windows/Windows Phone project
1. Right click on the project's References folder and select "Manage Nuget Packages...".
>![](/images/managenuget.jpg)

2. Select Browse and choose nuget.org for Package source then type in the search field "HavenOnDemand"
>![](/images/installhodnuget.jpg)

3. Select a package and click Install.

----
## HODClient API References
**Constructor**

    HODClient(string apiKey, String version="v1")

*Description:* 
* Creates and initializes an HODClient object.

*Parameters:*
* apiKey: your developer apikey.
* version: Haven OnDemand API version. Currently it only supports version 1. Thus, the default value is "v1".

*Example code:*

    using HOD.Client;
    using HOD.Response.Parser;

    HODClient hodClient = new HODClient("your-api-key");

    HODResponseParser parser = new HODResponseParser; 

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

**Function GetJobStatus**
```
GetJobStatus(String jobID)
```
*Description:*
* Sends a request to Haven OnDemand to retrieve status of a job identified by a job ID. If the job is completed, the response will be the result of that job. Otherwise, the response will be None and the current status of the job will be held in the error object. 

*Parameter:*
* jobID: the job ID returned from an Haven OnDemand API upon an asynchronous call.

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
        hodClient.GetJobStatus(jobId);
    }
}

private void HodClient_requestCompletedWithContent(string response)
{

}
``` 

## HODClient API callback functions
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

## HODResponseParser API References
**Constructor**

    HODResponseParser()

*Description:* 
* Creates and initializes an HODResponseParser object.

*Parameters:*
* None.

*Example code:*

    using HOD.Response.Parser;

    HODResponseParser parser = new HODResponseParser; 

----
**Function ParseJobID**

    string ParseJobID(string response)

*Description:* 
* Parses a jobID from a json string returned from an asynchronous API call.

*Parameters:*
* response: a json string returned from an asynchronous API call.

*Return value:*
* The jobID or an empty string if not found.

*Example code:*

```
void hodClient_requestCompletedWithJobID(string response)
{
    string jobID = parser.ParseJobID(response);
    if (jobID != "")
        hodClient.GetJobResult(jobID);
}
```
---
**Function ParseServerResponse**

    object ParseServerResponse(string hodApp, string jsonStr)

*Description:* 
* Parses a json string and returns an object type based on the API name (defined by hodApp).
>Note: Only APIs which return standard responses can be parsed by using this function. A list of supported APIs can be found from the SupportedApps class.

*Parameters:*
* hodApp: a string identify an HOD API. Detect supported APIs' responses from SupportedApps.
* jsonStr: a json string returned from a synchronous API call or from the GetJobResult function.

*Return value:*
* An object containing API's response values.

*Example code:*

```
// 
void hodClient_requestCompletedWithContent(string response)
{
    OCRDocumentResponse resp = parser.ParseOCRDocumentResponse(ref response);
    if (resp != null)
    {
        string text = "";
        foreach (OCRDocumentResponse.TextBlock obj in resp.text_block)
        {
            text += String.Format("Recognized text: {0}\n", obj.text);
            text += String.Format("Top/Left corner: {0}/{1}\n", obj.left, obj.top);
            text += String.Format("Width/Height: {0}/{1}\n", obj.width, obj.height);
        }
	outputText.Text = text;
    }
    else
    {
        var errors = parser.GetLastError();
        foreach (HODErrorObject err in errors)
        {
            if (err.error == HODErrorCode.QUEUED)
            {
                // Task is in queue. Let's wait for a few second then call GetJobStatus() again
                hodClient.GetJobStatus(err.jobID);
                break;
            }
            else if (err.error == HODErrorCode.IN_PROGRESS)
            {
                // Task is In Progress. Let's wait for some time then call GetJobStatus() gain
                hodClient.GetJobStatus(err.jobID);
                break;
            }
            else // It is an error. Let's print out the error code, reason and detail
            {
                outputText.Text += err.error.ToString() + "\n";
                outputText.Text += err.reason + "\n";
                outputText.Text += err.detail + "\n";
            }
	}
    }
}
```

---
**Function ParseCustomResponse**

    object ParseCustomResponse<T>(jsonStr)

*Description:* 
* Parses a json string and returns a custom object type based on the T class.
>Note: .

*Parameters:*
* <T>: a custom class object.
* jsonStr: a json string returned from a synchronous API call or from the GetJobResult function.

*Return value:*
* An object containing API's response values.

*Example code:*

```
// define a custom class for Query Text Index API response
public class QueryIndexResponse
{
    public List<Documents> documents;
    public int totalhits { get; set; }
    public class Documents
    {
        public string reference { get; set; } 
        public string index { get; set; }
        public double weight { get; set; } 
        public List<string> from { get; set; } 
        public List<string> to { get; set; } 
        public List<string> sent { get; set; } 
        public List<string> subject { get; set; } 
        public List<string> attachment { get; set; }
        public List<string> hasattachments { get; set; }
        public List<string> content_type { get; set; }
        public string content { get; set; } 
    }
}
void hodClient_requestCompletedWithContent(string response)
{
    QueryIndexResponse resp = (QueryIndexResponse)hodParser.ParseCustomResponse<QueryIndexResponse>(ref response);
    if (resp != null)
    {
        foreach (QueryIndexResponse.Documents doc in resp.documents)
        {
	    // walk thru documents array
            var reference = doc.reference;
            var index = doc.index;
            var weight = doc.weight;
            if (doc.from != null)
            {
                var from = doc.from[0];
            }
            if (doc.to != null)
                var to = doc.to[0];
            
            // parse any other values
        }
    }
    else
    {
        var errors = parser.GetLastError();
        foreach (HODErrorObject err in errors)
        {
            if (err.error == HODErrorCode.QUEUED)
            {
                // Task is in queue. Let's wait for a few second then call GetJobStatus() again
                hodClient.GetJobStatus(err.jobID);
                break;
            }
            else if (err.error == HODErrorCode.IN_PROGRESS)
            {
                // Task is In Progress. Let's wait for some time then call GetJobStatus() gain
                hodClient.GetJobStatus(err.jobID);
                break;
            }
            else // It is an error. Let's print out the error code, reason and detail
            {
                outputText.Text += err.error.ToString() + "\n";
                outputText.Text += err.reason + "\n";
                outputText.Text += err.detail + "\n";
            }
	}
    }
}
```

---
## Demo code 1: 

**Call the Entity Extraction API to extract people and places from cnn.com website with a synchronous GET request**
```
using HOD.Client;
using HOD.Response.Parser;

namespace HODClientDemo
{
    // define Entity Extraction response object
    public class EntityExtractionResponse
    {
        public List<Entity> entities { get; set; }

        public class Entity
        {
            public string normalized_text { get; set; }
            public string original_text { get; set; }
            public string type { get; set; }
            public int normalized_length { get; set; }
            public int original_length { get; set; }
            public double score { get; set; }
            public string normalized_date { get; set; }
            public EntityAdditionalInformation additional_information { get; set; }
            public List<object> components { get; set; }
        }
        public class EntityAdditionalInformation
        {
            public List<string> person_profession { get; set; }
            public string person_date_of_birth { get; set; }
            public int wikidata_id { get; set; }
            public string wikipedia_eng { get; set; }
            public string image { get; set; }
            public string person_date_of_death { get; set; }

            public double lon { get; set; } 
            public double lat { get; set; }
            public int place_population { get; set; } 
            public string place_country_code { get; set; } 
            public string place_region1 { get; set; } 
            public string place_region2 { get; set; } 
            public string url_homepage { get; set; }
        }
    }    
    
    public sealed partial class MainPage : Page
    {
        HODClient hodClient = new HODClient("your-apikey");
        HODResponseParser parser = new HODResponseParser();

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
            EntityExtractionResponse resp = (EntityExtractionResponse)parser.ParseCustomResponse<EntityExtractionResponse>(ref response);
            if (resp != null)
            {
                String people = "";
                String places = "";
                foreach (EntityExtractionResponse.Entity entity in resp.entities)
                {
                    if (entity.type == "people_eng")
                    {
                        people += entity.original_text + System.Environment.NewLine;
                        // parse any other interested information about a people
                    }
                    else if (entity.type == "places_eng")
                    {
                        places += entity.original_text + System.Environment.NewLine;
                        // parse any other interested information about a place
                    }
                }
            } 
	    else
            {
                var errors = parser.GetLastError();
                foreach (HODErrorObject err in errors)
                {
                    if (err.error == HODErrorCode.QUEUED)
                    {
                        // Task is in queue. Let's wait for a few second then call GetJobStatus() again
                        hodClient.GetJobStatus(err.jobID);
                        break;
                    }
                    else if (err.error == HODErrorCode.IN_PROGRESS)
                    {
                        // Task is In Progress. Let's wait for some time then call GetJobStatus() gain
                        hodClient.GetJobStatus(err.jobID);
                        break;
                    }
                    else 
                    {
                        // It is an error. Check error info and handle error accordingly
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
using HOD.Response.Parser;

namespace HODClientDemo
{
    public sealed partial class MainPage : Page
    {
        HODClient iodClient = new HODClient("your-apikey");
	HODResponseParser parser = new HODResponseParser();
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
                // Cancel picking file
            }
            
        }            
        
	// implement callback functions
            
        /**************************************************************************************
        * An async request will result in a response with a jobID. We parse the response to get
        * the jobID and send a request for the actual content identified by the jobID.
        **************************************************************************************/ 
        private void HodClient_requestCompletedWithJobID(string response)
        {
            string jobID = parser.ParseJobID(response);
            if (jobID != "")
                client.GetJobResult(jobID);
        }

        private void HodClient_requestCompletedWithContent(string response)
        {
            OCRDocumentResponse resp = parser.ParseOCRDocumentResponse(ref response);
	    if (resp != null)
	    {
                var text = "";
                foreach (OCRDocumentResponse.TextBlock obj in resp.text_block)
                {
                    text += String.Format("Recognized text: {0}\n", obj.text);
                    text += String.Format("Top/Left corner: {0}/{1}\n", obj.left, obj.top);
                    text += String.Format("Width/Height: {0}/{1}\n", obj.width, obj.height);
                }
            }
            else
            {
                var errors = parser.GetLastError();
                foreach (HODErrorObject err in errors)
                {
                    if (err.error == HODErrorCode.QUEUED)
                    {
                        // Task is in queue. Let's wait for a few second then call GetJobStatus() again
                        hodClient.GetJobStatus(err.jobID);
                        break;
                    }
                    else if (err.error == HODErrorCode.IN_PROGRESS)
                    {
                        // Task is In Progress. Let's wait for some time then call GetJobStatus() gain
                        hodClient.GetJobStatus(err.jobID);
                        break;
                    }
                    else 
                    {
                        // It is an error. Check error info and handle error accordingly
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