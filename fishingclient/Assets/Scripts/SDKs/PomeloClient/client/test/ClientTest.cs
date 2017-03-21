using System;
using SimpleJson;

namespace Pomelo.DotNetClient.Test
{
    public class ClientTest
    {
        public static Connection pomeloClient = null;

        public static void loginTest(string host, int port)
        {
            pomeloClient = new Connection();

            //listen on network state changed event
            pomeloClient.ListenTo(Connection.DisconnectEvent, jsonObj =>
            {
                Console.WriteLine("CurrentState is:" + pomeloClient.netWorkState);
            });

            pomeloClient.Connect(host, port, jsonObj =>
            {
                pomeloClient.Handshake(null, data =>
                {

                    Console.WriteLine("on data back" + data.ToString());
                    JsonObject msg = new JsonObject();
                    msg["uid"] = 111;
                    pomeloClient.Request("gate.gateHandler.queryEntry", msg, OnQuery);
                });
            });
        }

        public static void OnQuery(Message msg)
        {
            var result = msg.jsonObj;

            if (Convert.ToInt32(result["code"]) == 200)
            {
                pomeloClient.Disconnect();

                string host = (string)result["host"];
                int port = Convert.ToInt32(result["port"]);
                pomeloClient = new Connection();

                pomeloClient.ListenTo(Connection.DisconnectEvent, jsonObj =>
                {
                    Console.WriteLine(pomeloClient.netWorkState);
                });

                pomeloClient.Connect(host, port, jsonObj =>
                {
                    pomeloClient.Handshake(null, (data) =>
                    {
                        //JsonObject userMessage = new JsonObject();
                        Console.WriteLine("on connect to connector!");

                        //Login
                        JsonObject enterMsg = new JsonObject();
                        enterMsg["userName"] = "test";
                        enterMsg["rid"] = "pomelo";

                        pomeloClient.Request("connector.entryHandler.enter", enterMsg, OnEnter);
                    });
                });
            }
        }

        public static void OnEnter(Message msg)
        {
            Console.WriteLine("on login " + msg.jsonObj.ToString());
        }

        public static void onDisconnect(JsonObject result)
        {
            Console.WriteLine("on sockect disconnected!");
        }

        public static void Run()
        {
            string host = "192.168.0.156";
            int port = 3014;

            loginTest(host, port);
        }
    }
}