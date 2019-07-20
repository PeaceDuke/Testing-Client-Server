
DROP SCHEMA IF EXISTS `mydb` ;

CREATE SCHEMA IF NOT EXISTS `mydb` DEFAULT CHARACTER SET utf8 ;
USE `mydb` ;

DROP TABLE IF EXISTS `mydb`.`City` ;

CREATE TABLE IF NOT EXISTS `mydb`.`City` (
  `id` INT NOT NULL,
  `Name` VARCHAR(45) NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB;

DROP TABLE IF EXISTS `mydb`.`Univercity` ;

CREATE TABLE IF NOT EXISTS `mydb`.`Univercity` (
  `id` INT NOT NULL,
  `Name` VARCHAR(45) NULL,
  `City_id` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_Univercity_City1_idx` (`City_id` ASC),
  CONSTRAINT `fk_Univercity_City1`
    FOREIGN KEY (`City_id`)
    REFERENCES `mydb`.`City` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

DROP TABLE IF EXISTS `mydb`.`Faculty` ;

CREATE TABLE IF NOT EXISTS `mydb`.`Faculty` (
  `id` INT NOT NULL,
  `Name` VARCHAR(45) NULL,
  `Univercity_id` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_Faculty_Univercity1_idx` (`Univercity_id` ASC),
  CONSTRAINT `fk_Faculty_Univercity1`
    FOREIGN KEY (`Univercity_id`)
    REFERENCES `mydb`.`Univercity` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

DROP TABLE IF EXISTS `mydb`.`Speciality` ;

CREATE TABLE IF NOT EXISTS `mydb`.`Speciality` (
  `id` INT NOT NULL,
  `Name` VARCHAR(45) NULL,
  `Faculty_id` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_Speciality_Faculty1_idx` (`Faculty_id` ASC),
  CONSTRAINT `fk_Speciality_Faculty1`
    FOREIGN KEY (`Faculty_id`)
    REFERENCES `mydb`.`Faculty` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

DROP TABLE IF EXISTS `mydb`.`Student` ;

CREATE TABLE IF NOT EXISTS `mydb`.`Student` (
  `id` INT NOT NULL,
  `FirstName` VARCHAR(45) NULL,
  `SurName` VARCHAR(45) NULL,
  `Speciality_id` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_Student_Speciality_idx` (`Speciality_id` ASC),
  CONSTRAINT `fk_Student_Speciality`
    FOREIGN KEY (`Speciality_id`)
    REFERENCES `mydb`.`Speciality` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;
