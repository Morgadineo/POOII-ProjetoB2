DROP DATABASE IF EXISTS Biblioteca;
DROP SCHEMA IF EXISTS Biblioteca;
CREATE DATABASE Biblioteca;
USE Biblioteca;

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

CREATE SCHEMA IF NOT EXISTS `Biblioteca` ;
USE `Biblioteca` ;

-- -----------------------------------------------------
-- Table `Biblioteca`.`Livros`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `Biblioteca`.`Livros` (
  `idLivro` INT NOT NULL AUTO_INCREMENT,
  `Titulo` VARCHAR(100) NOT NULL,
  `AnoPublicacao` INT NOT NULL,
  `Genero` VARCHAR(80) NULL,
  PRIMARY KEY (`idLivro`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Biblioteca`.`Autor`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `Biblioteca`.`Autor` (
  `idAutor` INT NOT NULL AUTO_INCREMENT,
  `Nome` VARCHAR(45) NOT NULL,
  `Sobrenome` VARCHAR(100) NOT NULL,
  `Nacionalidade` VARCHAR(45) NULL,
  PRIMARY KEY (`idAutor`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Biblioteca`.`Livros_has_Autor`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `Biblioteca`.`Livros_has_Autor` (
  `Livros_idLivro` INT NOT NULL,
  `Autor_idAutor` INT NOT NULL,
  PRIMARY KEY (`Livros_idLivro`, `Autor_idAutor`),
  INDEX `fk_Livros_has_Autor_Autor1_idx` (`Autor_idAutor` ASC) VISIBLE,
  INDEX `fk_Livros_has_Autor_Livros_idx` (`Livros_idLivro` ASC) VISIBLE,
  CONSTRAINT `fk_Livros_has_Autor_Livros`
    FOREIGN KEY (`Livros_idLivro`)
    REFERENCES `Biblioteca`.`Livros` (`idLivro`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_Livros_has_Autor_Autor1`
    FOREIGN KEY (`Autor_idAutor`)
    REFERENCES `Biblioteca`.`Autor` (`idAutor`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
