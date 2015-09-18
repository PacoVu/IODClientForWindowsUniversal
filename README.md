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

## Demo code 1: 

**Use the Entity Extraction API to extract people and places from cnn.com website with a GET request**

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

                iodClient.GetRequest(ref Params, iodApp, IODClient.REQ_MODE.ASYNC);
            }

            
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
 
**Use the OCR Document API to recognize text from an image with a POST request**

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
		StorageFile imgFile = 
                var Params = new Dictionary<string, object>
                {
                    {"file", imgFile},
		    {"mode", "document_photo"}
                };
                iodClient.PostRequest(ref Params, iodApp, IODClient.REQ_MODE.ASYNC);
            }

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

	    private void IodClient_onErrorOccurred(string errorMessage)
            {
                // handle error if any
            }
        }
    }

----
## License
Licensed under the MIT License.