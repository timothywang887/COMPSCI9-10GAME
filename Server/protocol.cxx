#define PROTOCOL_VERSION (0b000)
typedef ENCVAL_TEMP ?
enum PARSING_RESULT{
	PR_GARBAGE,
	PR_GARBAGE_OURFAULT,
	PR_ILLOGICAL,
	PR_PROTOMISMATCH,
	PR_PARTRECV,
	PR_SUCCESSFUL
};
enum MESSAGE_TYPE{
	
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
		enum PARSING_RESULT handleCMSG(char * data, uint32_t packet_length){
			if(!data) return PR_GARBAGE_OURFAULT;
			if(packet_length < sizeof(struct Protocol_Msg)) return PR_GARBAGE;
			struct Protocol_Msg pmsg = (struct Protocol_Msg) data;
			if(strcmp("WIZARDWEED", &(pmsg.wizardweed_notchecksum))) return PR_GARBAGE; //Someone, non-client, is sending us garbage.
			if(pmsg->message_len != packet_length) return PR_PARTRECV;
			data = data-sizeof(struct Protocol_Msg);
			switch(pmsg->type){}
			
		}
		void sendCRQ(enum MESSAGE_TYPE t, ENCVAL_TEMP sctoken, uint32_t message_len, char * data){
			
		}
		void sendCRS(enum MESSAGE_TYPE t, ENCVAL_TEMP sctoken, uint32_t message_len, uint64_t response_to, char * data){
			
		}
	private:
		uint64_t current_message_id;	//luckily, we don't actually need to keep track of the CLIENT's message ids, as we either respond to them or we don't. we don't need to backreference them at any point; this saves us TONNES of memory
};
