create database library

USE library
GO

CREATE USER breno FOR LOGIN breno;
GO

ALTER ROLE db_owner ADD MEMBER breno;
GO
 -- 1. Categoria 
 CREATE TABLE Categorias ( 
 IdCategoria INT PRIMARY KEY IDENTITY(1,1), 
 Nome VARCHAR(100) NOT NULL, 
 Slug VARCHAR(100) NOT NULL UNIQUE -- Para URLs amigáveis (ex: /categoria/ficcao) 
 );
-- 2. Livro 
CREATE TABLE Livros ( 
IdLivro INT PRIMARY KEY IDENTITY(1,1), 
Titulo VARCHAR(150) NOT NULL, 
Autor VARCHAR(100) NOT NULL, 
AnoPublicacao INT, 
CapaUrl VARCHAR(255), -- Link para a imagem da capa 
Descricao VARCHAR(MAX), -- Sinopse do livro 
IdCategoria INT NOT NULL, FOREIGN KEY (IdCategoria) REFERENCES Categorias(IdCategoria) 
); 

CREATE LOGIN breno WITH 
PASSWORD=N'LltEr032007.',
	DEFAULT_DATABASE=library,
	CHECK_EXPIRATION=OFF,
	CHECK_POLICY=ON;
