using System;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Collections.Generic;
using System.Linq;

public class mqttTest : MonoBehaviour {

	//Broker hard coded info
	string messageMQTT, brokerAdress = "prevu3d.intelligenceindustrielle.com", topic = "from_pump1", username="prevu3d", password="WJb3zbfzmM3*5^"; 
	int port = 1883;
	public string itemKey, itemValue;
	//Controllers : Time and Loops
	public float timeRemaining = 10;
	public bool run;
	//Input, Text, Game Objects 
	public Text messageReceived, mqttClientID, TimeRemaining;
	public Button btnClick;
	public InputField brokerAdressInput, brokerPortInput, topicInput;
	private MqttClient client;
	public GameObject verticalLayout, templateVerticalElement;
	public List<Text> newFieldCreated;

	void Start()
	{
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
            RawMessage();

			ParseJson();

			timeRemaining = 5;
		}

		CreateNewClient();
		TimeRemaining.text = timeRemaining.ToString();
	}

	public void ParseJson()
    {
		//Json to Dictionary using Json Newton 
		var parsedMessages = JsonConvert.DeserializeObject<Dictionary<string, object>>(messageMQTT);


		//Creating new UI fields for each pair key/value inside the Dictionary 
		foreach (KeyValuePair<string, object> item in parsedMessages)
		{
			//Counting List<newFieldCreated> and creating new fields according to kvp inside the dictionary
			int fieldsCounter = newFieldCreated.Count;
			if (fieldsCounter < parsedMessages.Count)
			{
				//Instatiating new Text elements (cloning)
				GameObject fields = Instantiate(templateVerticalElement, verticalLayout.transform);
				fields.GetComponent<Text>().text = item.Key + " : " + item.Value;
				newFieldCreated.Add(fields.GetComponent<Text>());
			}
		};

		//Updating each new UI created with the new info.
		for (int i = 0; i < newFieldCreated.Count; i++)
		{
			Text field = newFieldCreated[i];
			itemKey = parsedMessages.ElementAt(i).Key;
			itemValue = parsedMessages.ElementAt(i).Value.ToString();
			field.text = i + " - " + itemKey + " : " + itemValue;
		}
	}
	//Create new subscriber / Also new client ID
	public void CreateNewClient()
	{
		switch (run)
		{
			case true:
				client = new MqttClient(brokerAdress, port, false, null);
				client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
				string clientId = Guid.NewGuid().ToString();
				client.Connect(clientId, username, password);
				mqttClientID.text = "Client ID: " + clientId;
				client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

				run = false;
				break;

			default:
				break;
		}
	}

	public void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
	{
		messageMQTT = System.Text.Encoding.UTF8.GetString(e.Message);

	}


	//Subscribe Button - creates new client connection / getting input data and assigning to variables. 
	public void GetInputOnClick()
	{
		run = true;
		//brokerAdress = brokerAdressInput.text;
		//topic = topicInput.text;
		//port = Convert.ToInt32(portBrokerInput.text);

	}

	//Display not parsed message
	public void RawMessage()
	{
		messageReceived.text = "Message received from " + topic + ": " + messageMQTT;
	}
}

