using UnityEngine;
using Valve.Sockets;
using System;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;
namespace ClientStructs


/*
That these are structs is a bit of a misnomer, but writing clientstructsenums is clunky and i think it gets along well enough.
Notes:
When sending a message to the server, you must send a protocol_Msg struct followed by a different struct that
corresponds to what you want to send to the server, eg and AuthRQ, LoginRQ
Then, tack on that struct to the back of the message. 
Things you have to do:
Ensure the right parts are encrypted
Read the message length properly (usually after encryption occurs, as this will usually change byte length)
Parse messages coming in.
make sure to encode everything properly.
use what is there (sendMessagetoServer) to help make the new things that I must do. 
Probably call it with the message arguement set to a conversion of a struct.
Should make a helper function to help convert structs cleanly as not to do it manually. It's way to annoying to make cases for each struct and they may change over time.

*/
{

    

    enum MESSAGE_TYPE{
            CS_AUTHRQ,
            CS_LOGINRQ,
            CS_MAPJOINRQ,
            CS_GSSJOINRQ,
            CS_HSYNCRQ,
            CS_INVSYNCRQ,
            CS_ANIMATIONSYNCRQ,
        CS_POSITIONUPDATE,
        CS_ACK,

            SC_HSYNCRQ,
            SC_TOKENRSP,
        SC_TOKENRSPWSP,
            SC_LOGINRQ,
            SC_GSSJOINRSP,
        SC_WORLDUPDATE,
        SC_MAPJOINRSP
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    struct Protocol_Msg
    {
        public byte[] wizardweed_notchecksum;//length 10
        public byte rqrspandpvr; //uint8_t
        public uint message_len;
        public byte[] tokening;//ENCVALTEMP size 256
        public MESSAGE_TYPE type;
        public ulong message_id_or_response_to;
    }



    enum ITEM_WEAPONS
    {
        
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    struct Item_SkillTree
    {
        public byte[] unlocked;// size 16
    }

    enum ITEM_RELICS
    {
        
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    struct Item_WEAPON
    {
        public ITEM_WEAPONS type;
        public Item_SkillTree tree;
        public byte level;//uint8
        public double xp;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    struct GameSessionData_Player_Inventory
    {//Temporary to session
        public Item_WEAPON weapon;
        public ITEM_RELICS relic;
        //Also 3 consumables in the loadout which appear somewhere in the map.
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    struct Protocol_Msg_AuthRQ
    {
        public byte[] spt;//C_SEMIPERMA 512
        public byte[] sk; // ENCVAL SYMETRIC TEMP 32
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    struct Protocol_Msg_LoginRQ
    {
        public byte[] username;//size 128
        public byte[] password;//size 128
        public byte[] sk;// ENCVAL SYMETRIC TEMP 32
        //We need to use bitconverter for these, bc c# and c++ store chars differently. bitconverter UTF8 conversion will take care of it.
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    struct Protocol_Msg_SC_TOKENRSP
    {
        public byte[] st; //C_SEESIONTOK size 32
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    struct Protocol_Msg_SC_TOKENRSPWSP
    {
        public byte[] spt; // C_SEMIPERMA 512
        public byte[] sk; // ENCVAL TEMP SYMMETRIC 32
    }

    enum MAP_DIFFICULTY:byte
    {
        DIFFICULTY_TRIVIAL = 0x1,
        DIFFICULTY_EASY = 0x2,
        DIFFICULTY_MEDIUM = 0x4,
        DIFFICULTY_HARD = 0x8,
        DIFFICULTY_STONED = 0x10
    }

    enum MAP_PUBLICITY
    {
	PUBLICITY_PRIVATE, //Only people with join code.
	PUBLICITY_FRIENDS, //Only friends.
	PUBLICITY_PUBLIC   //Everyone.
    };


    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    struct Protocol_Msg_CS_MAPJOINRQ
    {
        public ulong map_id;
        public MAP_PUBLICITY publicity;
        public MAP_DIFFICULTY difficulty_requested;
        public byte max_players;//This is a uint8. Gonna have to be careful how I assign this
    }

    enum ROTATION
    {//to the left
        ROTATION_0,
        ROTATION_90,
        ROTATION_180,
        ROTATION_270
    }

    enum ROOM_TYPE
    {
        ROOM_1x1_INT_OCCUPIED  = 0x0,
        ROOM_1x1_INT_AIR = 0x1,

        ROOM_1x1_HALLWAY,
        ROOM_1x1_Stair_MIDDLE,
        ROOM_1x1_Stair_UP,
        ROOM_1x1_STAIR_DOWN,
        ROOM_1x1_COURTYARD
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    struct CastleData_ROOM
    {//There is a lot that needs to be done here, Katie has notes on it in the ServerFork
        public ROTATION rotation;
        public ROOM_TYPE room_type;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    struct PositionOrientationVelocity
    {
        public double x;
        public double y;
        public double z;

        public double yaw;
        public double pitch;

        public double vx;
        public double vy;
        public double vz;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    struct CastleData_CASTLE//here
    {
        public CastleData_ROOM[,,] rooms; // this is a [5][5][4]. 5x5x4
        ulong[] BasementToThreeDoors;//use the first 40 bits per floor. Vertical 20 and then horizontal 20.
        ulong GNDDoors;//30+30 vertical 30 and then horizontal 30.
        public byte banner;//Unused use this later
        public byte[] seed;// Size 9, seed for world gen.
        public byte difficulties; //bitmap of difficulties
        public PositionOrientationVelocity spawnpos;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    struct GameSessionData_PLAYER_INIT
    {
        PositionOrientationVelocity pos;
        GameSessionData_Player_Inventory inv;
        byte player_no;//uint8
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    struct Protocol_Msg_SC_MAPJOINRSP
    {
        CastleData_CASTLE castle;
        MAP_DIFFICULTY difficulty;
        ulong join_code;
        GameSessionData_PLAYER_INIT you;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    struct Protocol_Msg_SC_GSSJOINRSP
    {
        
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    struct playerPositionUpdate
    {
        PositionOrientationVelocity pos;
        byte player_no;//uint8
    }


    enum WORLDUPDATE_TYPE
    {
        WORLDUPDATE_TYPE_playerPositionUpdate,
	    WORLDUPDATE_TYPE_playerHealthUpdate,
	    WORLDUPDATE_TYPE_playerJoinUpdate,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    struct playerHealthUpdate
    {
        double hp;
        bool hathshuffledoffthismortalcoil;
        byte player_no;//uint8
    }



//this set of structs are standing in for the union switcher. It does require some logic so send the right on, but it shouldn't be terrible.
//The more worrisome bit is that they may not be the right size. We may pad some junk on it or something, need to talk to Katie about it.
//Follow up - unions in c++ leave the junk on the back of the union. I will have to calculate some sizes, and then add
//Byte[] junk to each of them, leaving notes on their respective sizes.
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    struct GWU_PLAYERPOSU
    {
        WORLDUPDATE_TYPE type;//always set to playerPositionUpdate
        playerPositionUpdate u;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    struct GWU_PLAYERHEALTHU
    {
        WORLDUPDATE_TYPE type;//always set to playerHealthUpdate
        playerHealthUpdate u;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    struct GWU_PLAYERJOINU
    {
        WORLDUPDATE_TYPE type;//always set to playerJoinUpdate
        GameSessionData_PLAYER_INIT u;
    }





};




    
