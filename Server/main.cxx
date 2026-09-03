#include <steam/steamnetworkingsockets.h>
#include <steam/isteamnetworkingutils.h>
#include <cassert>
#include <cstdio>
#include "headers/session.H"
bool toTerminateServerProcess = false;

class Server{
	public:
		int doLiterallyEverything(uint32_t port){
			m_pInterface = SteamNetworkingSockets();
			SteamNetworkingIPAddr serverAddr;
			serverAddr.Clear();
			serverAddr.m_port = port;
			SteamNetworkingConfigValue_t cfg;
			cfg.SetPtr(k_ESteamNetworkingConfig_Callback_ConnectionStatusChanged, (void*)GNS_SteamNetConnectionStatusChangedCallback);
			m_hListenSock = m_pInterface->CreateListenSocketIP(serverAddr, 1, &cfg);
			assert(m_hListenSock != k_HSteamListenSocket_Invalid);
			m_hPollGroup = m_pInterface->CreatePollGroup();
			assert(m_hPollGroup != k_HSteamNetPollGroup_Invalid);

			while(!toTerminateServerProcess){
				GNS_PollIncoming();
				GNS_PollConnectionStatusChanges();
			}

			//TODO: send close messages and close all connections
			m_pInterface->CloseListenSocket(m_hListenSock);
			assert(m_pInterface->DestroyPollGroup(m_hPollGroup));
			return 1;
		}
	private:
		ISteamNetworkingSockets *m_pInterface;
		HSteamNetPollGroup m_hPollGroup; //According to docs, allows you to poll multiple connections efficiently at the same time - should we assign them per game instance? Something to consider.
		HSteamListenSocket m_hListenSock;
		SessionsManager Sessions;

		void GNS_PollIncoming(){
			while(!toTerminateServerProcess){
				ISteamNetworkingMessage *incoming_msg = NULL;
				int n_msgs = m_pInterface->ReceiveMessagesOnPollGroup(m_hPollGroup, &incoming_msg, 1);
				if(n_msgs == 0) break;
				assert(n_msgs==1 && incoming_msg); 
				//We can now handle the message. Later, this will go through a protocol translator, and will probably be JSON.
				printf("recv: %s\n", incoming_msg->m_pData);

				incoming_msg->Release();
			}
		}
		void GNS_SteamNetConnectionStatusChanged(SteamNetConnectionStatusChangedCallback_t change){
			//From docs: new connection, connection accepted by remote (will not happen), connection closed by remote, problem with connection (localhost closed)
			switch(change.m_info.m_eState){
				case k_ESteamNetworkingConnectionState_None:
				case k_ESteamNetworkingConnectionState_Connected:
					break;
				case k_ESteamNetworkingConnectionState_ClosedByPeer:
				case k_ESteamNetworkingConnectionState_ProblemDetectedLocally:
					//They unplugged their ethernet cable or we unplugged our ethernet cable. Act as a left game if in game.
					if(!Sessions.removeUserSession(Sessions.findUserSessionByConnection(change.m_hConn))){
						assert(false);
					}
					m_pInterface->CloseConnection(change.m_hConn, 0, NULL, false);
					break;
				case k_ESteamNetworkingConnectionState_Connecting:
					//Someone is trying to connect to us. Give them a session, then ask them for a token. If their token is valid, let them in, otherwise kick to login screen.
					if(m_pInterface->AcceptConnection(change.m_hConn) != k_EResultOK){ //Why can this even fail??
						m_pInterface->CloseConnection(change.m_hConn, 0, NULL, false);
						break;
					}
					if(!m_pInterface->SetConnectionPollGroup(change.m_hConn, m_hPollGroup)){
						m_pInterface->CloseConnection(change.m_hConn, 0, NULL, false);
						break;
					}
					Sessions.addUserSession(change.m_hConn);
					break;
				default:
					assert(false);	
			}
		}
		static Server *s_pCallbackInstance;
		static void GNS_SteamNetConnectionStatusChangedCallback(SteamNetConnectionStatusChangedCallback_t change){ //Workaround. 
			s_pCallbackInstance->GNS_SteamNetConnectionStatusChanged(change);
		}
		void GNS_PollConnectionStatusChanges(){
			s_pCallbackInstance = this;
			m_pInterface->RunCallbacks();
		}
};
Server *Server::s_pCallbackInstance=NULL;
int main(int argc, char ** argv){
	SteamDatagramErrMsg errmsg;
	if(!GameNetworkingSockets_Init(NULL,errmsg)){
		return 1;
	}
	Server s;
	s.doLiterallyEverything(1263);

	//TODO: cleanup
	GameNetworkingSockets_Kill();
	return 0;
}
