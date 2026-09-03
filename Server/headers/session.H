#ifndef H_SESSION
#define H_SESSION
#include <steam/steamnetworkingsockets.h>
#include <steam/isteamnetworkingutils.h>
#include <list>
class User{
	public:
		uint64_t user_id; //(0 is invalid)
		std::string username;
		//Some datatype for Yescrypt hash and some type of salt
		//bool verifyLogin(std::string password, ? hash, ? salt) - zero out the used memory afterwards!
};
class UserSession{
	public:
		HSteamNetConnection m_hConn; //connection handle
		User session_user;
};
bool operator==(const User& a, const User& b);
bool operator==(const UserSession& a, const UserSession& b);
class SessionsManager{
	public: 
		UserSession * findUserSessionByConnection(HSteamNetConnection m_hConn);
		bool removeUserSession(UserSession * session);
		bool addUserSession(HSteamNetConnection m_hConn);
	private:
		std::list<UserSession> sessions;

};

#endif
