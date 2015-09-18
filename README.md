# IODClient Library for Windows Universal 8.1. V1.0

----
## Overview
IODClient library for Windows Universal is a lightweight C# based API, which helps you easily integrate your Windows app with HP IDOL OnDemand Services.

IODClient library supports both Windows and Windows Phone 8.1.

IODClient library requires the .NET 4.5.

----
## Integrate IODClient into a Windows/Windows Phone project
1. Download the IODClient.dll library for Windows Universal 8.1.
2. Create a new or open an existing Windows project
3. Add the IODClient.dll to the project. Browse to the folder where you saved the library and select the IODClient.dll.

----
## API References
**IODClient(string apiKey, String version="v1")**

*Description:* 
>Constructor. Creates and initializes an IODClient object.

*Parameters:*
>apiKey: your developer apikey.
>version: IDOL OnDemand API version. Currently it only supports version 1. Thus, the default value is “v1”.

**GetRequest(ref Dictionary<String, Object> Params, String iodApp, REQ_MODE mode)**
*Description:* 
Sends a GET request to an IDOL OnDemand API.
*Parameters:*
>params: a Dictionary object containing key/value pair parameters to be sent to an IDOL OnDemand API, where the keys are the parameters of an IDOL OnDemand API. 
>Note: 
>In the case of a parameter type is an array<>, the key must be defined as "arrays" and the value must be a Dictionary<String,String> object with the key is the parameter name and the values separated by commas ","
>E.g.:
    var entity_array = new Dictionary<String, String>
    {
        {"entity_type", "people_eng,places_eng"}
    }

>iodApp: a string to identify an IDOL OnDemand API. E.g. “extractentities”. Current supported apps are listed in the IODApps class.
>mode [REQ_MODE.ASYNC | REQ_MODE.SYNC]: specifies API call as Asynchronous or Synchronous.
*Return: void.*
**Response:**
* If the mode is "ASYNC", response will be returned via the requestCompletedWithJobID(String response) callback function.
* If the mode is "SYNC", response will be returned via the requestCompletedWithContent(String response) callback function.
* If there is an error occurred, the error message will be sent via the onErrorOccurred(String errorMessage) callback function.

**PostRequest(ref Dictionary<String, Object> Params, String iodApp, REQ_MODE mode)**

*Description:* 
>Sends a POST request to an IDOL OnDemand API.
*Parameters:*
>params: a Dictionary object containing key/value pair parameters to be sent to an IDOL OnDemand API, where the keys are the parameters of an IDOL OnDemand API
>Note: 
>1. In the case of the "file" parameter, the value must be a StorageFile object.
>2. In the case of a parameter type is an array<>, the key must be defined as "arrays" and the value must be a Dictionary<String,String> object with the key is the parameter name and the values separated by commas “,”. E.g.:
>E.g.:
    var entity_array = new Dictionary<String, String>
    {
        {"entity_type", "people_eng,places_eng"}
    }
>iodApp: a string to identify an IDOL OnDemand API. E.g. “ocrdocument”. Current supported apps are listed in the IODApps class.
>mode [REQ_MODE.SYNC | REQ_MODE.ASYNC]: specifies API call as Asynchronous or Synchronous.
*Return: void.*
**Response:**
* If the mode is “ASYNC”, response will be returned via the requestCompletedWithJobID(String response) callback function.
* If the mode is “SYNC”, response will be returned via the requestCompletedWithContent(String response) callback function.
* If there is an error occurred, the error message will be sent via the onErrorOccurred(String errorMessage) callback function.

**GetJobResult(String jobID)**
*Description:*
Sends a request to IDOL OnDemand to retrieve the content identified by the jobID.
**Parameter:**
>jobID: the jobID returned from an IDOL OnDemand API upon an asynchronous call.
**Response:** 
>Response will be returned via the requestCompletedWithContent(String response)

---
## Demo code 1: 

**Use the Entity Extraction API to extract people and places from cnn.com website with a synchronous GET request**

    using IOD.Client;
    
    namespace IODClientDemo
    {
        public sealed partial class MainPage : Page
        {
            IODClient iodClient = new IODClient("your-apikey");

            public MainPage()
            {
                this.InitializeComponent();
                
                iodClient.requestCompletedWithContent += IodClient_requestCompletedWithContent;
                iodClient.requestCompletedWithJobID += IodClient_requestCompletedWithJobID;
                iodClient.onErrorOccurred += IodClient_onErrorOccurred;

                useIODClient();
            }

            private void useIODClient()
            {
                String iodApp = IODApps.ENTITY_EXTRACTION;
                
                var arrays = new Dictionary<String, String>
                {
                    {"entity_type", "people_eng,places_eng"}
                };

                var Params = new Dictionary<string, object>
                {
                    { "url", "http://www.cnn.com" },
                    { "arrays", arrays },
                    { "unique_entities", "true" }
                };

                iodClient.GetRequest(ref Params, iodApp, IODClient.REQ_MODE.SYNC);
            }

            // implement callback functions

            private void IodClient_requestCompletedWithContent(string response)
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

	    private void IodClient_onErrorOccurred(string errorMessage)
            {
                // handle error if any
            }
        }
    }

----

## Demo code 2:
 
**Use the OCR Document API to recognize text from an image with an asynchronous POST request**

    using IOD.Client;
    
    namespace IODClientDemo
    {
        public sealed partial class MainPage : Page
        {
            IODClient iodClient = new IODClient("your-apikey");

            public MainPage()
            {
                this.InitializeComponent();
                
                iodClient.requestCompletedWithContent += IodClient_requestCompletedWithContent;
                iodClient.requestCompletedWithJobID += IodClient_requestCompletedWithJobID;
                iodClient.onErrorOccurred += IodClient_onErrorOccurred;

                useIODClient();
            }

            private void useIODClient()
            {
                String iodApp = IODApps.OCR_DOCUMENT;
                StorageFile imgFile = StorageFile.GetFileFromPathAsync("path/and/filename");
                var Params = new Dictionary<string, object>
                {
                    { "file", imgFile },
                    { "mode", "document_photo" }
                };

                iodClient.PostRequest(ref Params, iodApp, IODClient.REQ_MODE.ASYNC);
            }
            
            // implement callback functions
            
            /**************************************************************************************
            * An async request will result in a response with a jobID. We parse the response to get
            * the jobID and send a request for the actual content identified by the jobID.
            **************************************************************************************/ 
            private void IodClient_requestCompletedWithJobID(string response)
            {
                JsonValue root;
                JsonObject jsonObj;
                bool ret = JsonValue.TryParse(response, out root);
                if (ret)
                {
                    jsonObj = root.GetObject();
                    String jobID = jsonObj.GetNamedString("jobID");
                    iodClient.GetJobResult(jobID);
                }
            }

            private void IodClient_requestCompletedWithContent(string response)
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

            private void IodClient_onErrorOccurred(string errorMessage)
            {
                // handle error if any
            }
        }
    }

----
## License
Licensed under the MIT License.