CREATE TABLE `playerRank` (
  `rankId` int PRIMARY KEY,
  `rankName` varchar(20)
);

CREATE TABLE `castleRanks` (
  `rankId` int PRIMARY KEY,
  `rankName` varchar(20)
);

CREATE TABLE `tileTypes` (
  `tileTypeId` int PRIMARY KEY,
  `tileName` varchar(50) UNIQUE
);

CREATE TABLE `player` (
  `playerId` int PRIMARY KEY,
  `username` varchar(25) UNIQUE NOT NULL,
  `pinHash` varchar(128) NOT NULL,
  `pinSalt` varcahr(128) NOT NULL,
  `rank` int NOT NULL
);

CREATE TABLE `castle` (
  `castleId` int PRIMARY KEY,
  `playerId` int UNIQUE NOT NULL,
  `rank` int NOT NULL
);

CREATE TABLE `tile` (
  `tileId` int PRIMARY KEY,
  `type` int NOT NULL,
  `castleId` int NOT NULL
);

ALTER TABLE `player` ADD FOREIGN KEY (`rank`) REFERENCES `playerRank` (`rankId`);

ALTER TABLE `castle` ADD FOREIGN KEY (`playerId`) REFERENCES `player` (`playerId`);

ALTER TABLE `castle` ADD FOREIGN KEY (`rank`) REFERENCES `castleRanks` (`rankId`);

ALTER TABLE `tile` ADD FOREIGN KEY (`type`) REFERENCES `tileTypes` (`tileTypeId`);

ALTER TABLE `tile` ADD FOREIGN KEY (`castleId`) REFERENCES `castle` (`castleId`);
