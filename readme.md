# Rcon for .NET
[Valve RCON Protocol](https://developer.valvesoftware.com/wiki/Source_RCON_Protocol) implementation library.  
**This contains:**
* RCON Client Interface
* RCON Server Interface
* RCON Client
* RCON Server

# Usage example
## Client
```cs
using System;
using System.Threading.Tasks;
using Rcon;

namespace RconClient
{
	static class Program
    {
    	static async Task Main(string[] args)
        {
        	RconClient client = new RconClient();
            await client.ConnectAsync("127.0.0.1", 25575);
            await client.AuthenticateAsync("super_secret_password");
            if(!client.Authenticated) {
            	Console.WriteLine("Invalid password!!");
                return;
            }
            
            for(;;) {
            	Console.Write("> ");
                string response = await client.SendCommandAsync(Console.ReadLine());
                if(!String.IsNullOrEmpty(response)) {
                	Console.WriteLine("] {0}", response);
                }
          	}
        }
    }
}
```

## Server
```cs
using System;
using Rcon;
using Rcon.Events;

namespace RconServer
{
    class Program
    {
        static void Main(string[] args)
        {
            using(RconServer server = new RconServer("super_secret_password", 25575)) {
                server.OnClientCommandReceived += Server_OnClientCommandReceived;
                server.OnClientConnected += Server_OnClientConnected;
                server.OnClientAuthenticated += Server_OnClientAuthenticated;
                server.OnClientDisconnected += Server_OnClientDisconnected;
                server.Start();
            }
        }

        static void Server_OnClientAuthenticated(object sender, ClientAuthenticatedEventArgs e)
        {
            Console.WriteLine("{0} authenticated", e.Client.Client.LocalEndPoint);
        }

        static void Server_OnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            Console.WriteLine("{0} disconnected", e.EndPoint);
        }

        static void Server_OnClientConnected(object sender, ClientConnectedEventArgs e)
        {
            Console.WriteLine("{0} connected", e.Client.Client.LocalEndPoint);
        }

        static string Server_OnClientCommandReceived(object sender, ClientSentCommandEventArgs e)
        {
            Console.WriteLine("{0}: {1}", e.Client.Client.LocalEndPoint, e.Command);
            return e.Command;
        }
    }
}

```

# Documentation
Still not, but there are some external docs:  
* [Valve Developer Community](https://developer.valvesoftware.com/wiki/Source_RCON_Protocol)
* [Minecraft(?) wiki](https://wiki.vg/RCON)

