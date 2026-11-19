using ClientStructs;
using System;
using System.Text;
using System.Linq;


/*
Notes:
This is the space where all the parsing and contructing functions live. 
I need two functions for each of the possible structs:
    - one to make the struct based on data (this is what mechanics will use; Just pass in the things and everything gets handeled)
    - one to read the struct off of a server msg
YAY

*/


static class parsecontruct
{


    
    public static byte[] constructProtocolMsg(Protocol_Msg msg,byte protoVer,byte[] message)
    {

        

        byte[] rqrspandpvrBytes = new byte[1];
        rqrspandpvrBytes[0] = (byte)(msg.rqrspandpvr|protoVer);
        byte[] messageLengthBytes = BitConverter.GetBytes(msg.message_len);
        byte[] typeBytes = BitConverter.GetBytes((int)msg.type);
        byte[] messageIdOrResponseToBytes = BitConverter.GetBytes(msg.message_id_or_response_to);

        byte[] fullMsg = msg.wizardweed_notchecksum
            .Concat(rqrspandpvrBytes)
            .Concat(messageLengthBytes)
            .Concat(msg.tokening)
            .Concat(typeBytes)
            .Concat(messageIdOrResponseToBytes)
            .Concat(message)
            .ToArray();

        return fullMsg;

    }

    public static Protocol_Msg parseProtocolMsg(byte[] rawbytes)
    {
        Protocol_Msg msg = new Protocol_Msg();

        msg.wizardweed_notchecksum = rawbytes[0..9];
        msg.rqrspandpvr = rawbytes[10];
        msg.message_len = BitConverter.ToUInt32(rawbytes[11..14]);
        msg.tokening = rawbytes[15..270];
        msg.type = (MESSAGE_TYPE)BitConverter.ToUInt32(rawbytes[271..274]);
        msg.message_id_or_response_to = BitConverter.ToUInt64(rawbytes[275..282]);
        return msg;
    }

    public static byte[] constructItem_SkillTree(Item_SkillTree tree)
    {

        //I am entirely aware this function does essentially nothing.
        //However, because all the other structs are going to have
        //thier own funcitons, for the sake of mechanic's sanity,
        //I'll make these so the code seems more cohesive in my scripts.

        Item_SkillTree item_SkillTree = new Item_SkillTree();
        item_SkillTree.unlocked = tree.unlocked;

        byte[] item_SkillTreeBytes = item_SkillTree.unlocked;
        return item_SkillTreeBytes;

    }

    public static Item_SkillTree parseItem_SkillTree(byte[] rawbytes)
    {

        Item_SkillTree item_SkillTree = new Item_SkillTree();
        item_SkillTree.unlocked = rawbytes;
        return item_SkillTree;

    }

    public static byte[] constructItem_WEAPON(Item_WEAPON weapon)
    {

        byte[] typeBytes = BitConverter.GetBytes((uint)weapon.type);
        byte[] Item_SkillTreeBytes = weapon.tree.unlocked;
        byte[] levelBytes = new byte[1];
        levelBytes[0] = weapon.level;
        byte[] xpBytes = BitConverter.GetBytes(weapon.xp);

        byte[] Item_WEAPONBytes = typeBytes
            .Concat(Item_SkillTreeBytes)
            .Concat(levelBytes)
            .Concat(xpBytes)
            .ToArray();

        return Item_WEAPONBytes;
    }

    public static Item_WEAPON parseItem_WEAPON(byte[] rawbytes)
    {
        Item_WEAPON item_weapon = new Item_WEAPON();
        item_weapon.type = (ITEM_WEAPONS)BitConverter.ToUInt32(rawbytes[0..3]);
        item_weapon.tree = parseItem_SkillTree(rawbytes[4..19]);
        item_weapon.level = rawbytes[20];
        item_weapon.xp = BitConverter.ToDouble(rawbytes[21..28]);
        return item_weapon;
    }

    public static byte[] constructGameSessionData_Player_Inventory(GameSessionData_Player_Inventory inventory)
    {
        byte[] weaponBytes = constructItem_WEAPON(inventory.weapon);
        byte[] relicBytes = BitConverter.GetBytes((int)inventory.relic);

        byte[] GameSessionData_Player_Inventory = weaponBytes
            .Concat(relicBytes)
            .ToArray();
        return GameSessionData_Player_Inventory;
    }

    public static GameSessionData_Player_Inventory parseGameSessionData_Player_Inventory(byte[] rawbytes)
    {
        GameSessionData_Player_Inventory gamesessiondata_player_inventory = new GameSessionData_Player_Inventory();
        gamesessiondata_player_inventory.weapon = parseItem_WEAPON(rawbytes[0..28]);
        gamesessiondata_player_inventory.relic = (ITEM_RELICS)BitConverter.ToUInt32(rawbytes[29..36]);
        return gamesessiondata_player_inventory;
    }

    public static byte[] constructProtcol_Msg_AuthRQ(Protocol_Msg_AuthRQ AuthRQ)
    {
        byte[] sptBytes = AuthRQ.spt;
        byte[] skBytes = AuthRQ.sk;

        byte[] Protocol_Msg_AuthRQBytes = sptBytes
            .Concat(skBytes)
            .ToArray();
        
        return Protocol_Msg_AuthRQBytes;
    }

    public static Protocol_Msg_AuthRQ parseProtocol_Msg_AuthRQ(byte[] rawbytes)
    {
        Protocol_Msg_AuthRQ authrq = new Protocol_Msg_AuthRQ();

        authrq.spt = rawbytes[0..511];
        authrq.sk = rawbytes[512..543];

        return authrq;

    }

    public static byte[] constructProtocol_Msg_LoginRQ(Protocol_Msg_LoginRQ loginrq)
    {
        byte[] loginrqbytes = loginrq.username
            .Concat(loginrq.password)
            .Concat(loginrq.sk)
            .ToArray();

        return loginrqbytes;
    }

    public static Protocol_Msg_LoginRQ parseProtocol_Msg_LoginRQ(byte[] rawbytes)
    {
        Protocol_Msg_LoginRQ loginrq = new Protocol_Msg_LoginRQ();
        loginrq.username = rawbytes[0..127];
        loginrq.password = rawbytes[128..255];
        loginrq.sk = rawbytes[256..287];

        return loginrq;
    }

    public static byte[] constructProtocol_Msg_SC_TOKENRSP(Protocol_Msg_SC_TOKENRSP tokenrsp)
    {
        byte[] tokenrspbytes = tokenrsp.st;
        return tokenrspbytes;
    }    

    public static Protocol_Msg_SC_TOKENRSP parseProtocol_Msg_SC_TOKENRSP(byte[] rawbytes)
    {
        Protocol_Msg_SC_TOKENRSP tokenrsp = new Protocol_Msg_SC_TOKENRSP();

        tokenrsp.st = rawbytes;

        return tokenrsp;

    }

    public static byte[] constructProtocol_Msg_SC_TOKENRSPWSP(Protocol_Msg_SC_TOKENRSPWSP tokenrspwsp)
    {
        byte[] tokenrspwspbytes = tokenrspwsp.spt
            .Concat(tokenrspwsp.sk)
            .ToArray();
        
        return tokenrspwspbytes;
    }

    public static Protocol_Msg_SC_TOKENRSPWSP parseProtocol_Msg_SC_TOKENRSPWSP(byte[] rawbytes)
    {
        Protocol_Msg_SC_TOKENRSPWSP tokenrspwsp = new Protocol_Msg_SC_TOKENRSPWSP();

        tokenrspwsp.spt = rawbytes[0..511];
        tokenrspwsp.sk = rawbytes[512..543];

        return tokenrspwsp;
    }

    public static byte[] constructProtocol_Msg_CS_MAPJOINRQ(Protocol_Msg_CS_MAPJOINRQ mapjoinrq)
    {
        byte[] map_idbytes = BitConverter.GetBytes(mapjoinrq.map_id);
        byte[] publicitybytes = BitConverter.GetBytes((uint)mapjoinrq.publicity);
        byte[] difficulty_requestedbytes = new byte[1];
        difficulty_requestedbytes[1] = (byte)mapjoinrq.difficulty_requested;
        byte[] max_playersbytes = new byte[1];
        max_playersbytes[1] = mapjoinrq.max_players;

        byte[] mapjoinrqbytes = map_idbytes
            .Concat(publicitybytes)
            .Concat(difficulty_requestedbytes)
            .Concat(max_playersbytes)
            .ToArray();

        return mapjoinrqbytes;
    }

    public static Protocol_Msg_CS_MAPJOINRQ parseProtocol_Msg_CS_MAPJOINRQ(byte[] rawbytes)
    {
        Protocol_Msg_CS_MAPJOINRQ mapjoinrq = new Protocol_Msg_CS_MAPJOINRQ();

        mapjoinrq.map_id = BitConverter.ToUInt64(rawbytes[0..7]);
        mapjoinrq.publicity = (MAP_PUBLICITY)BitConverter.ToUInt32(rawbytes[8..11]);
        mapjoinrq.difficulty_requested = (MAP_DIFFICULTY)BitConverter.ToUInt32(rawbytes[12..15]);
        mapjoinrq.max_players = rawbytes[16];

        return mapjoinrq;

    }

    public static byte[] constructCastleData_ROOM(CastleData_ROOM room)
    {
        byte[] rotationbytes = BitConverter.GetBytes((UInt32)room.rotation);
        byte[] room_typebytes = new byte[1];
        room_typebytes[1] = (byte)room.room_type;
        byte[] doorsbytes = BitConverter.GetBytes(room.doors);

        byte[] roombytes = rotationbytes
            .Concat(room_typebytes)
            .Concat(doorsbytes)
            .ToArray();

        return roombytes;
    }

    public static CastleData_ROOM parseCastleData_ROOM(byte[] rawbytes)
    {
        CastleData_ROOM room = new CastleData_ROOM();

        room.rotation = (ROTATION)BitConverter.ToUInt32(rawbytes[0..3]);
        room.room_type = (ROOM_TYPE)rawbytes[4];
        room.doors = BitConverter.ToUInt64(rawbytes[5..12]);

        return room;

    }

    public static byte[] constructPositionOrientationVelocity(PositionOrientationVelocity pov)
    {
        byte[] xbytes = BitConverter.GetBytes(pov.x);
        byte[] ybytes = BitConverter.GetBytes(pov.y);  
        byte[] zbytes = BitConverter.GetBytes(pov.z);            
        byte[] yawbytes = BitConverter.GetBytes(pov.yaw);            
        byte[] pitchbytes = BitConverter.GetBytes(pov.pitch);            
        byte[] vxbytes = BitConverter.GetBytes(pov.vx);            
        byte[] vybytes = BitConverter.GetBytes(pov.vy);            
        byte[] vzbytes = BitConverter.GetBytes(pov.vz);

        byte[] povbytes = xbytes
            .Concat(ybytes)
            .Concat(zbytes)
            .Concat(yawbytes)
            .Concat(pitchbytes)
            .Concat(vxbytes)
            .Concat(vybytes)
            .Concat(vzbytes)
            .ToArray();

        return povbytes;
            
    }

    public static PositionOrientationVelocity parsePositionOreitationVelocity(byte[] rawbytes)
    {
        PositionOrientationVelocity pov = new PositionOrientationVelocity();
        pov.x = BitConverter.ToDouble(rawbytes[0..7]);
        pov.y = BitConverter.ToDouble(rawbytes[8..15]);
        pov.z = BitConverter.ToDouble(rawbytes[16..23]);
        pov.yaw = BitConverter.ToDouble(rawbytes[24..31]);
        pov.pitch = BitConverter.ToDouble(rawbytes[32..39]);
        pov.vx = BitConverter.ToDouble(rawbytes[40..47]);
        pov.vy = BitConverter.ToDouble(rawbytes[48..55]);
        pov.vz = BitConverter.ToDouble(rawbytes[56..63]);

        return pov;
    }


    public static byte[] constructCastleData_ROOM(CastleData_CASTLE castle)
    {
        byte[] roomsbytes = new byte[0];

        foreach(CastleData_ROOM[] array in castle.rooms)
        {
            foreach(CastleData_ROOM room in array)
            {
                roomsbytes = roomsbytes.Concat(constructCastleData_ROOM(room)).ToArray();
            }
        }
        byte[] bannerbytes = new byte[1];
        bannerbytes[0] = castle.banner;
        byte[] seedbytes = castle.seed;
        byte[] difficultybytes = new byte[1];
        difficultybytes[0] = castle.difficulties;
        byte[] spawnposbytes = constructPositionOrientationVelocity(castle.spawnpos);

        byte[] castlebytes = roomsbytes
            .Concat(bannerbytes)
            .Concat(seedbytes)
            .Concat(difficultybytes)
            .Concat(spawnposbytes)
            .ToArray();

        return castlebytes;
    }

    public static CastleData_CASTLE parseCastleData_CASTLE(byte[] rawbytes)
    {
        CastleData_CASTLE castle = new CastleData_CASTLE();

        castle.rooms = new CastleData_ROOM[5,5,4];
        CastleData_ROOM[] roomlist = new CastleData_ROOM[14];

        for(int i=0; i<14; i++)
        {
            
        }

    }




}
