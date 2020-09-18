USE aarrstat;

-- Create table sessiontype
CREATE TABLE IF NOT EXISTS `sessiontype` (
	`id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
	`name` VARCHAR(50) NOT NULL,
	`description` VARCHAR(255) NULL,
	PRIMARY KEY (`id`)
)
COLLATE='utf8_general_ci';

-- Populate base data
INSERT INTO sessiontype (id, name, description) VALUES (NULL, 'app', 'Application usage');
INSERT INTO sessiontype (id, name, description) VALUES (NULL, 'blog', 'Reading blog');
INSERT INTO sessiontype (id, name, description) VALUES (NULL, 'post', 'Reading post');

-- Add type column to session table
ALTER TABLE `session` ADD COLUMN `type_id` INT UNSIGNED NOT NULL DEFAULT '1' AFTER `id`;
ALTER TABLE `session` ADD CONSTRAINT `FK_session_sessiontype` FOREIGN KEY (`type_id`) REFERENCES `sessiontype` (`id`);