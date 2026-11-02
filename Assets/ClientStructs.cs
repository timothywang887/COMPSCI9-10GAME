using UnityEngine;
using Valve.Sockets;
using System;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;

namespace ClientStructs
{


    enum ITEM_WEAPONS
    {
        
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    struct Item_SkillTree
    {
        byte[] unlocked;// size 16
    }

    enum ITEM_RELICS
    {
        
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    struct Item_WEAPON
    {
        ITEM_WEAPONS type;
        Item_SkillTree tree;
        byte level;//uint8
        double xp;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    struct GameSessionData_Player_Inventory
    {//Temporary to session
        Item_WEAPON weapon;
        ITEM_RELICS relic;
        //Also 3 consumables in the loadout which appear somewhere in the map.
    }
    


    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    struct Protocol_Msg_AuthRQ
    {
        byte[] spt;//C_SEMIPERMA size tbd
        byte[] sk; // ENCVAL SYMETRIC TEMP size tbd
    }
    
    
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    struct Protocol_Msg_LoginRQ
    {
        byte[] username;//size 128
        byte[] password;//size 128
        byte[] sk;// ENCVAL SYMETRIC TEMP size tbd
        //We need to use bitconverter for these, bc c# and c++ store chars differently. bitconverter UTF8 conversion will take care of it.
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    struct Protocol_Msg_SC_TOKENRSP
    {
        byte[] st; //C_SEESIONTOK size tbd
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    struct Protocol_Msg_SC_TOKENRSPWSP
    {
        byte[] spt; // C_SEMIPERMA size tbd
        byte[] sk; // ENCVAL TEMP SYMMETRIC size tbd
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
        ulong map_id;
        MAP_PUBLICITY publicity;
        MAP_DIFFICULTY difficulty_requested;
        byte max_players;//This is a uint8. Gonna have to be careful how I assign this
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
        ROTATION rotation;
        ROOM_TYPE room_type;
        ulong doors;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    struct PositionOrientationVelocity
    {
        double x;
        double y;
        double z;

        double yaw;
        double pitch;

        double vx;
        double vy;
        double vz;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    struct CastleData_CASTLE
    {
        CastleData_ROOM[] rooms; // this is a [5][5][4]. 5x5x4
        byte banner;//Unused use this later
        byte[] seed;// Size 9, seed for world gen.
        byte difficulties; //bitmap of difficulties
        PositionOrientationVelocity spawnpos;
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




    
