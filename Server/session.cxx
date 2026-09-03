#include <list>
#include <steam/steamnetworkingsockets.h>
#include <steam/isteamnetworkingutils.h>
#include "headers/session.H"
#include <cassert>
bool operator==(const User& a, const User& b){
	return a.user_id==b.user_id;
}
bool operator==(const UserSession& a, const UserSession& b){
	return (a.m_hConn == b.m_hConn && a.session_user == b.session_user);
}
UserSession * SessionsManager::findUserSessionByConnection(HSteamNetConnection m_hConn)	{
	for (UserSession& session: this->sessions)
	{
		if (session.m_hConn == m_hConn)
			return &session;
	}
	return NULL;
}
bool SessionsManager::removeUserSession(UserSession * session){
	//This will NOT always be called as removeUserSession(findUserSessionByConnection(m_hConn)). In many cases, the user will simply click 'leave game' rather than unplugging their ethernet cord.
	if(!session)
		return false;
	
	this->sessions.remove(*session);
		return true;
}
bool SessionsManager::addUserSession(HSteamNetConnection m_hConn){
	 UserSession new_session;
	 new_session.m_hConn = m_hConn;
	 new_session.session_user.user_id = 0;
	 sessions.push_back(new_session);
	 assert(findUserSessionByConnection(m_hConn));
	 return true;
}

