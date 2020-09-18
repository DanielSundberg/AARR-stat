USE aarrstat;

-- Create table sessiontype
ALTER TABLE `session`
	CHANGE COLUMN `start` `start` DATETIME NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp() AFTER `device_id`,
	CHANGE COLUMN `end` `end` DATETIME NULL DEFAULT NULL AFTER `start`;

ALTER TABLE `session`
	CHANGE COLUMN `start` `start` DATETIME NOT NULL AFTER `device_id`;

