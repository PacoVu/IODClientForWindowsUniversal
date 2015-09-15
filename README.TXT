#####################################################################################################
IODClient library for Windows Universal is a lightweight C# based API, which helps you easily integrate 
your Windows app with HP IDOL OnDemand Services.

IODClient library supports both Windows and Windows Phone 8.1.

IODClient library requires .NET 4.5.

************************************ IODClient class ************************************************
1. Integrate IODClient into Windows project

a) Create a new or open an existing Windows project
b) Add the IODClient.dll to the project.
c) Browse to the folder where you saved the library and select the IODClient.dll.

2. Code Example

using IOD.Client;

public class MyAppClass 
    {
        IODClient iodClient;

	protected override void OnNavigatedTo(NavigationEventArgs e) {
		String apiKey = “your-api-key”;
		iodClient = new IODClient(apiKey);

		// implement delegated function
		iodClient.requestCompletedWithJobID += iodClient_requestCompletedWithJobID;
		iodClient.requestCompletedWithContent += iodClient_requestCompletedWithContent;
		iodClient.onErrorOccurred += iodClient_onErrorOccurred;
	}

	void useIODClientAPI() {
		String iodApp = IODApps.ENTITY_EXTRACTION;
		var arrays = new Dictionary<String, String>
		{
			{“entity_type”, “people_eng,places_eng”}
		}
		var Params = new Dictionary<String, Object> 
		{ 
			{“url”, “http://www.cnn.com”},
			{“arrays”, arrays}
		};
		iodClient.GetRequest(ref Params, iodApp, IODClient.REQ_MODE.SYNC);

	}
	void iodClient_requestCompletedWithJobID(string response) {  
	
	}
	void iodClient_ requestCompletedWithContent(string response) { 
	
	}
	void iodClient_onErrorOccurred(string errorMessage) { 
	
	}
}