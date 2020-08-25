using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Collections.Generic;
using System.Linq;

public class mqttTest : MonoBehaviour {

	//Broker hard coded info
	string brokerAdress = "prevu3d.intelligenceindustrielle.com";
	string topic = "from_pump1";
	int port = 1883;
	//string username = "prevu3d";
	//string password = "WJb3zbfzmM3*5^";

	//Input, Text, Game Objects 
	public Text messageReceived;
	public Text mqttClientID;
	public Button btnClick;
	public Button btnGetData;
	public InputField brokerAdressInput;
	public InputField portBrokerInput;
	public InputField topicInput;
	private MqttClient client;
	public GameObject verticalLayout;
	public GameObject templateVerticalElement;
	public GameObject[] newFieldCreated;

	//Controllers : Time and Loops
	Boolean run;
	public float timeRemaining = 10;
	public bool timerIsRunning = false;
	string messageMQTT;


	void Start ()
    {
		timerIsRunning = true;
		btnClick.onClick.AddListener(GetInputOnClick);
		
	}


	void Update()
    {

		//Timer update
		if (timeRemaining > 0)
		{
			timeRemaining -= Time.deltaTime;
		}

		else
		{
			//Raw message field
			messageReceived.text = "Message received from " + topic + ": " +  messageMQTT;

			//Json to Dictionary using Json Newton Library
			var parsedMessages = JsonConvert.DeserializeObject<Dictionary<string, string>>(messageMQTT);

			//Getting all objects created with NewField tag - DRY purposes
			newFieldCreated = GameObject.FindGameObjectsWithTag("NewField");

			//Creating new UI fields for each row inside the Library
			foreach (KeyValuePair<string, string> kvp in parsedMessages)
			{
				//If there are more fields into the Dictionary add a new UI
				int fieldsCounter = newFieldCreated.Length;
				if (fieldsCounter <= parsedMessages.Count)
                {
					GameObject fields = Instantiate(templateVerticalElement, verticalLayout.transform);
					fields.GetComponent<Text>().text = " " + kvp.Key + " : " + kvp.Value;
				}
			};


			
			//Updating each UI with new info.
			foreach (GameObject field in newFieldCreated)
			{
				//Find each row of Dictionary
				for (int index = 0; index < parsedMessages.Count; index++)
				{
					var item = parsedMessages.ElementAt(index);
					field.GetComponent<Text>().text = " " + item.Key + " : " + item.Value;
					Debug.Log(item.Key + " : " + item.Value);
				}
				
			}

			timeRemaining = 5;
		}
		

		CreateNewClient();
	}

	public void CreateTextFields()
    {
		GameObject go = Instantiate(templateVerticalElement, verticalLayout.transform);
		go.GetComponent<Text>().text = "Token: Value: ";
	}


	public void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
	{
	//	Debug.Log("Received: " + System.Text.Encoding.UTF8.GetString(e.Message));
		messageMQTT = System.Text.Encoding.UTF8.GetString(e.Message);
		

	}



	public void CreateNewClient()
	{
		switch (run)
		{
			case true:
				client = new MqttClient(brokerAdress, port, false, null);
				client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
				string clientId = Guid.NewGuid().ToString();
				client.Connect(clientId, "prevu3d", "WJb3zbfzmM3*5^");
				mqttClientID.text = "Client ID: " + clientId;
				client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

				run = false;
				break;

			default:
				break;
		}
	}


	public void GetInputOnClick()
	{
		run = true;

	}

}

/*foreach (var item in parsedMessages.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value))
				{
					field.GetComponent<Text>().text = item;
				}
				*/


/* fieldsCreated = GameObject.FindGameObjectsWithTag("Respawn");
			if (fieldsCreated == null)
				Debug.Log("No fields created.");

			foreach (GameObject field in fieldsCreated)
			{
				foreach (KeyValuePair<string, string> kvp in messages)
                {
					field.GetComponent<Text>().text = " " + kvp.Key + " : " + kvp.Value;
					Debug.Log("Key = {0}" + kvp.Key + "/ Value = {1}" + kvp.Value);

					Debug.Log("");
				}
			}
*/

/*
Debug.Log("Token: " + reader.TokenType + "Value: " + reader.Value);
messageReceived.text = "Received from " + topic + ": " + messageMQTT;
*/



//It goes inside GetInputOnClick
/*
brokerAdress = brokerAdressInput.text;
topic = topicInput.text;
port = Convert.ToInt32(portBrokerInput.text);

Debug.Log(brokerAdress);
Debug.Log(topic);
Debug.Log(port);
*/


/* JsonTextReader reader = new JsonTextReader(new StringReader(messageMQTT));
			reader.Read();

			while (reader.Read())
			{
				if (reader.Value != null)
				{
					GameObject go = Instantiate(templateVerticalElement, verticalLayout.transform);
					go.GetComponent<Text>().text = "Token: " + reader.TokenType + "Value: " + reader.Value;

					timeRemaining = 10;
					timerIsRunning = false;
				}
			}

*/

/*
 *
 * foreach (KeyValuePair<string, string> kvp in jsonParsed)
			{
			    Debug.Log("Key = " + 
					kvp.Key + " " +  kvp.Value);
			};
*/

/*
 * tokentype = reader.TokenType.ToString();
						value = reader.Value.ToString();
						Debug.Log("Create Text Fiels method!");
						CreateTextFields();
						Debug.Log("From get Data - Token: " + reader.TokenType + "Value: " + reader.Value);
						runGetData = false;
*/

/*			using (var reader = new JsonTextReader(new StringReader(messageMQTT)))
			{
				while (reader.Read())
				{
					jsonParsed.Add(reader.TokenType.ToString(), reader.Value.ToString());
				}
			}
*/

/*public void getData()
	{
        switch (runGetData)
        {
			case true:
				reader = new JsonTextReader(new StringReader(messageMQTT));
				while (reader.Read())
				{
                    if(reader != null)
                    {
                        CreateTextFields();

                        tokentype = reader.TokenType.ToString();
                        value = reader.Value.ToString();
                        jsonParsed.Add(tokentype, value);

                        Debug.Log("Create Text Fiels method!");
                        Debug.Log("From get Data - Token: " + reader.TokenType + "Value: " + reader.Value);

                        runGetData = false;
                    }
					else
                    {
						runGetData = false;
                    }
				}
				break;

			default:
                break;
		}
		

	}

    */


/*
 *
 * 

		foreach (KeyValuePair<string, string> kvp in jsonParsed){
			CreateTextFields();
			Debug.Log("Key = " + kvp.Key + " " + kvp.Value);
		};
*/

/*
    JsonTextReader reader = new JsonTextReader(new StringReader(messageMQTT));
    while (reader.Read())
    {
    Debug.Log("Token: " + reader.TokenType + " / " + " Value: " + reader.Value);
    dictFromJson.Add("{}", reader.TokenType.ToString());
    dictFromJson.Add("{1}", reader.Value.ToString());
    }
*/