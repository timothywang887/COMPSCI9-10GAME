//Timmy: LEARN WHAT A POINTER IS!!!

#include <steam/steamnetworkingsockets.h>
#include <steam/isteamnetworkingutils.h>
#include <cassert>
#include <cstdio>
#include <list>
bool toTerminateServerProcess = false;
class User{
	public:
		uint64_t user_id; //(0 is invalid)
};
class UserSession{
	public:
		HSteamNetConnection m_hConn; //connection handle
		User session_user;
};
bool operator==(const User& a, const User& b){
	return a.user_id==b.user_id;
}
bool operator==(const UserSession& a, const UserSession& b){
	return (a.m_hConn == b.m_hConn && a.session_user == b.session_user);
}
class SessionsManager{
	public: 
		UserSession * findUserSessionByConnection(HSteamNetConnection m_hConn)
		{
			for (UserSession& session: this->sessions)
			{
				if (session.m_hConn == m_hConn)
				{
					return &session;
				}
			}
			return NULL;
		}
		bool removeUserSession(UserSession * session)
		{
			//This will NOT always be called as removeUserSession(findUserSessionByConnection(m_hConn)). In many cases, the user will simply click 'leave game' rather than unplugging their ethernet cord.
			if(!session)
				return false;
			
			this->sessions.remove(*session);
			return true;
		}
		bool addUserSession(HSteamNetConnection m_hConn)
		{
			UserSession new_session;
			new_session.m_hConn = m_hConn;
			new_session.session_user.user_id = 0;
			sessions.push_back(new_session);
			assert(findUserSessionByConnection(m_hConn));
			return true;
			
		}
	private:
		std::list<UserSession> sessions;

};
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
