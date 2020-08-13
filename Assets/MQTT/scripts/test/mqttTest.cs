using System;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//Used parts of the M2MQTT library
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

//Not used yet - To discover parts of the library
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;





public class mqttTest : MonoBehaviour {

    //Text Fields
    public Text messageReceived; //Text to show message received from the broker
	public Text mqttClientID; //Client ID 

    // Variables 
	string messageMQTT; //Stores message received from the broker
	string brokerAdress = " "; //Adress of the broker
	string topic = "/"; //Name of the subscribed topic 
	int port = 1883; //Port of the broker
	
    //Input Fields
	public InputField brokerAdressInput; //User inputs the broker adress
	public InputField portBrokerInput; //User input for the broker port
	public InputField topicInput; //User input for the topic subscribed

	//Controller
	Boolean run = false;

    //Button
	public Button btnClick; //Button to send user input data

    // New MQTT client 
	private MqttClient client; //New MQTT client


	void Start () {

		//Button to create a new client
		btnClick.onClick.AddListener(GetInputOnClick);

        // Create client instance - Broker Adress , Port , Secure? , Certificate 
			client = new MqttClient("localhost", 1883, false, null);

			// Register to message received 
			client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

			//Client ID - This program is a client connected to the broker receiving data from another client
			string clientId = Guid.NewGuid().ToString();
			client.Connect(clientId);
			mqttClientID.text = "Client ID: " + clientId;


			// Subscribe to the TOPIC with QoS 2 
			client.Subscribe(new string[] { "SomeTopic/" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });



    }


	public void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e){

		Debug.Log("Received: " + System.Text.Encoding.UTF8.GetString(e.Message));
        messageMQTT = System.Text.Encoding.UTF8.GetString(e.Message);

	}

    // Button to get values from input field on click 
	public void GetInputOnClick()
	{

		run = true;
		Debug.Log(run);
        //Broker Adress
		brokerAdress = brokerAdressInput.text;

        //Topic to be subscribed assigned to variable topic
		topic = topicInput.text;

        
        //Port input conversion to int and assigned to variable port
        port = Convert.ToInt32(portBrokerInput.text);
        


		Debug.Log("Broker Adress : " + brokerAdress);
        Debug.Log("Port: " + port);
		Debug.Log("Topic: " + topic);

	}


	// Update
	void Update()
    {
        //Checking for messages received by the broker 
		messageReceived.text = "Received from " + topic + " " + messageMQTT;

        //Checking state of run
		Debug.Log(run);

        /* Creating a new client using user data input - run is set as false untill the button is clicked it then returns
         * all the variables assigned with user input and initiates a new client - The difference from the first one is the assigned variables */
		switch (run)
		{
			case true:
                //Testing purposes
				Debug.Log("New client created");

				// Create client instance // args = (Broker Adress , Port , Secure? , Certificate?) // ? = Still not know what it means inside this context 
				client = new MqttClient(IPAddress.Parse(brokerAdress), port, false, null);

				// Register to message received 
				client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

				//Client ID - This creates a new client ID
				string clientId = Guid.NewGuid().ToString();
				client.Connect(clientId);
				mqttClientID.text = "Client ID: " + clientId;


				// Subscribe to the TOPIC input by the user
				client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

                //Assiging run to false to break
				run = false;
				break;

			default:
				break;
		}
	}
}



