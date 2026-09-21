#include "headers/protocol.H"
enum PARSING_RESULT ProtocolHandler::handleCMSG(char * data, uint32_t packet_length, UserSession * ses_assoc){
	if(!data) return PR_GARBAGE_OURFAULT;
	if(packet_length < sizeof(struct Protocol_Msg)) return PR_GARBAGE;
	struct Protocol_Msg * pmsg = (struct Protocol_Msg*) data;
	char * ww = "WIZARDWEED";
	if(memcmp(ww, &(pmsg->wizardweed_notchecksum), 10)) return PR_GARBAGE; //Someone, non-client, is sending us garbage.
	if(pmsg->message_len != packet_length) return PR_PARTRECV;
	data = data-sizeof(struct Protocol_Msg);
	if((pmsg->rqrspandpvr&0b111)!=PROTOCOL_VERSION) return PR_PROTOMISMATCH; 
	switch(pmsg->type){
		case CS_AUTHRQ:
		case CS_LOGINRQ:
			//No token required. Handle login.
		default:
			User u = ses_assoc->session_user;
			if(!u.user_id) return PR_ILLOGICAL; //Auth required first!
			if(pmsg->tokening){
				if(!verifyToken(pmsg->tokening, ses_assoc))
					return PR_TOKENINVALID;
			}else{
				return PR_ILLOGICAL;
			}
	}
	return PR_SUCCESSFUL;
}
void ProtocolHandler::sendCRQ(enum MESSAGE_TYPE t, ENCVAL_TEMP sctoken, uint32_t message_len, char * data, UserSession * ses_assoc){
	struct Protocol_Msg pmsg;
	char * ww = "WIZARDWEED";
	memcpy(ww,pmsg.wizardweed_notchecksum,10);
	pmsg.message_len=message_len+sizeof(struct Protocol_Msg);
	pmsg.rqrspandpvr=PROTOCOL_VERSION | 0b1000;
	memcpy(sctoken, pmsg.tokening, sizeof(ENCVAL_TEMP));
	pmsg.type = t;
	pmsg.message_id_or_response_to=current_message_id++;
	char * d = (char*)(malloc(pmsg.message_len));
	memcpy(&pmsg, d, sizeof(struct Protocol_Msg));
	memcpy(data, d+sizeof(struct Protocol_Msg), message_len);
	ses_assoc->sendMessageTo(d, pmsg.message_len, k_nSteamNetworkingSend_Reliable); //TODO.
	free(d);
}
void ProtocolHandler::sendCRS(enum MESSAGE_TYPE t, ENCVAL_TEMP sctoken, uint32_t message_len, uint64_t response_to, char * data, UserSession * ses_assoc){
	struct Protocol_Msg pmsg;
	char * ww = "WIZARDWEED";
	memcpy(ww,pmsg.wizardweed_notchecksum,10);
	pmsg.message_len=message_len+sizeof(struct Protocol_Msg);
	pmsg.rqrspandpvr=PROTOCOL_VERSION & 0b111;
	memcpy(sctoken, pmsg.tokening, sizeof(ENCVAL_TEMP));
	pmsg.type = t;
	pmsg.message_id_or_response_to=response_to;
	char * d = (char*)malloc(pmsg.message_len);
	memcpy(&pmsg, d, sizeof(struct Protocol_Msg));
	memcpy(data, d+sizeof(struct Protocol_Msg), message_len);
	ses_assoc->sendMessageTo(d, pmsg.message_len, k_nSteamNetworkingSend_Reliable); //TODO.
	free(d);

}
