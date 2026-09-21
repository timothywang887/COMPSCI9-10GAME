#ifndef H_COMM
#define H_COMM
#include <steam/steamnetworkingsockets.h>
#include <steam/isteamnetworkingutils.h>
#include "protocol.H"
#include "session.H"
class Server{
	public: 
		int doLiterallyEverything(uint32_t port);
		ISteamNetworkingSockets *m_pInterface;
	private:
		HSteamNetPollGroup m_hPollGroup;
		HSteamListenSocket m_hListenSock;
		SessionsManager Sessions;
		ProtocolHandler PH;
		void GNS_PollIncoming();
		void GNS_SteamNetConnectionStatusChanged(SteamNetConnectionStatusChangedCallback_t change);
		static Server *s_pCallbackInstance;
		static void GNS_SteamNetConnectionStatusChangedCallback(SteamNetConnectionStatusChangedCallback_t change);
		void GNS_PollConnectionStatusChanges();
};
extern Server SERVER_INSTANCE;
#endif
