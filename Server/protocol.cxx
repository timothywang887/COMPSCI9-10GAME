#define PROTOCOL_VERSION (0b000)
typedef ENCVAL_TEMP ?
enum PARSING_RESULT{
	PR_GARBAGE,
	PR_GARBAGE_OURFAULT,
	PR_ILLOGICAL,
	PR_PROTOMISMATCH,
	PR_PARTRECV,
	PR_TOKENINVALID,
	PR_SUCCESSFUL
};
enum MESSAGE_TYPE{
	CS_AUTHRQ,
	CS_LOGINRQ,
	CS_MAPJOINRQ,
	CS_GSSJOINRQ,
	CS_HSYNCRQ,
	CS_INVSYNCRQ,
	CS_ANIMATIONSYNCRQ,

	SC_HSYNCRQ,
	SC_TOKENRSP,
}
struct Protocol_Msg{
	char[10] wizardweed_notchecksum;
	uint16_t rqrspandpvr; //0b0 rq/rsp followed by 0b000 proto ver.
	uint32_t message_len;
	ENCVAL_TEMP tokening;
	enum MESSAGE_TYPE type;
	uint64_t message_id_or_response_to;
	//Sadly, we're not allowed to place variable-length data within a struct. After this struct, the data follows.
}
class ProtocolHandler{
	public:
		enum PARSING_RESULT handleCMSG(char * data, uint32_t packet_length, UserSession * ses_assoc){
			if(!data) return PR_GARBAGE_OURFAULT;
			if(packet_length < sizeof(struct Protocol_Msg)) return PR_GARBAGE;
			struct Protocol_Msg * pmsg = (struct Protocol_Msg*) data;
			if(memcmp("WIZARDWEED", &(pmsg.wizardweed_notchecksum), 10)) return PR_GARBAGE; //Someone, non-client, is sending us garbage.
			if(pmsg->message_len != packet_length) return PR_PARTRECV;
			data = data-sizeof(struct Protocol_Msg);
			if((pmsg->rqrspandpvr&0b111)!=PROTOCOL_VERSION) return PR_PROTOMISMATCH; 
			switch(pmsg->type){
				case CS_AUTHRQ:
				case CS_LOGINRQ:
					//No token required. Handle login.
				default:
					User u = ses_assoc->session_user;
					if(!u) return PR_ILLOGICAL; //Auth required first!
					if(pmsg->tokening){
						if(!verifyToken(pmsg->tokening, ses_assoc))
							return PR_TOKENINVALID;
					}else{
						return PR_ILLOGICAL;
					}
			}
			
		}
		void sendCRQ(enum MESSAGE_TYPE t, ENCVAL_TEMP sctoken, uint32_t message_len, char * data, UserSession * ses_assoc){
			struct Protocol_Msg pmsg;
			memcpy("WIZARDWEED",pmsg.wizardweed_notchecksum,10);
			pmsg.message_len=message_len+sizeof(struct Protocol_Msg);
			pmsg.rqrsporpvr=PROTOCOL_VERSION | 0b1000;
			memcpy(sctoken, pmsg.tokening, sizeof(ENCVAL_TEMP));
			pmsg.type = t;
			pmsg.message_id_or_response_to=current_message_id++;
			char * d = malloc(pmsg.message_len);
			memcpy(pmsg, d, sizeof(struct Protocol_Msg));
			memcpy(data, d+sizeof(struct Protocol_Msg), message_len);
			ses_assoc.sendMessageTo(d, pmsg.message_len, k_nSteamNetworkingSend_Reliable); //TODO.
			free(d);
		}
		void sendCRS(enum MESSAGE_TYPE t, ENCVAL_TEMP sctoken, uint32_t message_len, uint64_t response_to, char * data, UserSession * ses_assoc){
			struct Protocol_Msg pmsg;
			memcpy("WIZARDWEED",pmsg.wizardweed_notchecksum,10);
			pmsg.message_len=message_len+sizeof(struct Protocol_Msg);
			pmsg.rqrsporpvr=PROTOCOL_VERSION & 0b111;
			memcpy(sctoken, pmsg.tokening, sizeof(ENCVAL_TEMP));
			pmsg.type = t;
			pmsg.message_id_or_response_to=response_to;
			char * d = malloc(pmsg.message_len);
			memcpy(pmsg, d, sizeof(struct Protocol_Msg));
			memcpy(data, d+sizeof(struct Protocol_Msg), message_len);
			ses_assoc.sendMessageTo(d, pmsg.message_len, k_nSteamNetworkingSend_Reliable); //TODO.
			free(d);

		}
	private:
		uint64_t current_message_id;	//luckily, we don't actually need to keep track of the CLIENT's message ids, as we either respond to them or we don't. we don't need to backreference them at any point; this saves us TONNES of memory
};
