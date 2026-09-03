CREATE TABLE `playerRank` (
  `rankID` int PRIMARY KEY,
  `rankName` varchar(20)
);

CREATE TABLE `castleRanks` (
  `rankID` int PRIMARY KEY,
  `rankName` varchar(20)
);

CREATE TABLE `tileTypes` (
  `tileTypeID` int PRIMARY KEY,
  `tileName` varchar(50) UNIQUE
);

CREATE TABLE `player` (
  `playerID` int PRIMARY KEY,
  `username` varchar(25) UNIQUE NOT NULL,
  `pinHash` varchar(128) NOT NULL,
  `pinSalt` varcahr(128) NOT NULL,
  `rank` int NOT NULL
);

CREATE TABLE `castle` (
  `castleID` int PRIMARY KEY,
  `playerID` int UNIQUE NOT NULL,
  `rank` int NOT NULL
);

CREATE TABLE `tile` (
  `tileID` int PRIMARY KEY,
  `type` int NOT NULL,
  `castleID` int NOT NULL
);

ALTER TABLE `player` ADD FOREIGN KEY (`rank`) REFERENCES `playerRank` (`rankID`);

ALTER TABLE `castle` ADD FOREIGN KEY (`playerID`) REFERENCES `player` (`playerID`);

ALTER TABLE `castle` ADD FOREIGN KEY (`rank`) REFERENCES `castleRanks` (`rankID`);

ALTER TABLE `tile` ADD FOREIGN KEY (`type`) REFERENCES `tileTypes` (`tileTypeID`);

ALTER TABLE `tile` ADD FOREIGN KEY (`castleID`) REFERENCES `castle` (`castleID`);
