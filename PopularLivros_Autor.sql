-- Procedures para povoar as tabelas.
DROP PROCEDURE IF EXISTS povoar_Livros_has_Autor;

DELIMITER $$
CREATE PROCEDURE povoar_Livros_has_Autor(IN QntdLinhas INT)
BEGIN
	DECLARE qtde INT;
    
    SET qtde = 1;
    
    WHILE qtde < QntdLinhas DO
        INSERT INTO Livros_has_Autor VALUES (qtde, qtde);
        SET qtde = qtde + 1;
	END WHILE;
END $$
DELIMITER ;

CALL povoar_Livros_has_Autor(1000);