#ifndef H_PROTOCOL
#define H_PROTOCOL
#include <steam/steamnetworkingsockets.h>
#include <steam/isteamnetworkingutils.h>
#include <list>
#include <string>
#include "session.H"

#define PROTOCOL_VERSION (0b000)
typedef char ENCVAL_TEMP[2048];
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
};
struct Protocol_Msg{
        char wizardweed_notchecksum[10];
        uint16_t rqrspandpvr; //0b0 rq/rsp followed by 0b000 proto ver.
        uint32_t message_len;
        ENCVAL_TEMP tokening;
        enum MESSAGE_TYPE type;
        uint64_t message_id_or_response_to;
        //Sadly, we're not allowed to place variable-length data within a struct. After this struct, the data follows.
};
class ProtocolHandler{
        public:
                enum PARSING_RESULT handleCMSG(char * data, uint32_t packet_length, UserSession * ses_assoc);
		void sendCRQ(enum MESSAGE_TYPE t, ENCVAL_TEMP sctoken, uint32_t message_len, char * data, UserSession * ses_assoc);
                void sendCRS(enum MESSAGE_TYPE t, ENCVAL_TEMP sctoken, uint32_t message_len, uint64_t response_to, char * data, UserSession * ses_assoc);
        private:
                uint64_t current_message_id;    //luckily, we don't actually need to keep track of the CLIENT's message ids, as we either respond to them or we don't. we don't need to backreference them at any point; this saves us TONNES of memory
};


#endif
