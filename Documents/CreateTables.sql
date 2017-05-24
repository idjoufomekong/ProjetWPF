---------------------------------------------------------------------------
-- Création d'une procédure de suppression des tables et contraintes de clés étrangères d'un schéma

if exists(select 1 from INFORMATION_SCHEMA.ROUTINES
   where SPECIFIC_SCHEMA = 'dbo' and SPECIFIC_NAME = 'usp_DropTablesAndFKInSchema')
drop procedure dbo.usp_DropTablesAndFKInSchema
go
create procedure usp_DropTablesAndFKInSchema @nomSchema nvarchar(50)
as
begin
DECLARE @req AS NVARCHAR(max)
set @req = ''

select @req = @req + 'alter table ' + @nomSchema + '.' + TABLE_NAME + 
         ' drop constraint ' + CONSTRAINT_NAME + CHAR(13)
from INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
where CONSTRAINT_SCHEMA = @nomSchema and CONSTRAINT_TYPE = 'FOREIGN KEY'

EXEC(@req)
print @req

set @req = ''
select @req = @req + 'drop table '  + @nomSchema + '.' + TABLE_NAME + CHAR(13) 
from INFORMATION_SCHEMA.TABLES
where TABLE_SCHEMA = @nomSchema

EXEC(@req)
print @req
end
go

---------------------------------------------------------------------------
-- Suppression des tables et contraintes FK du schéma jo à l'aide de la procédure

exec usp_DropTablesAndFKInSchema 'jo' 

-----------------------------------------------------------------------
--- Création du schéma s'il n'existe pas

if exists (select 1 from INFORMATION_SCHEMA.SCHEMATA where SCHEMA_NAME= 'jo')
DROP SCHEMA jo
go

CREATE SCHEMA jo
go


---------------------------------------------------------------------------
-- Création des tables

CREATE TABLE jo.Activite 
(
CodeActivite VARCHAR (20) NOT NULL , 
Libelle NVARCHAR (40) NOT NULL , 
Annexe BIT NOT NULL DEFAULT 0 )
GO 


ALTER TABLE jo.Activite ADD CONSTRAINT Activite_PK PRIMARY KEY CLUSTERED (CodeActivite)
GO

CREATE TABLE jo.ActiviteMetier 
(
MetierCodeMetier VARCHAR (20) NOT NULL , 
ActiviteCodeActivite VARCHAR (20) NOT NULL )
GO 


ALTER TABLE jo.ActiviteMetier ADD CONSTRAINT ActiviteMetier_PK PRIMARY KEY CLUSTERED (MetierCodeMetier, ActiviteCodeActivite)
GO

CREATE TABLE jo.Equipe 
(
CodeEquipe VARCHAR (20) NOT NULL , 
CodeService VARCHAR (20) NOT NULL , 
CodeFiliere VARCHAR (20) NOT NULL , 
Nom NVARCHAR (40) NOT NULL )
GO 

ALTER TABLE jo.Equipe ADD CONSTRAINT Equipe_PK PRIMARY KEY CLUSTERED (CodeEquipe)
GO

CREATE TABLE jo.Filiere 
(
CodeFiliere VARCHAR (20) NOT NULL , 
Libelle NVARCHAR (40) NOT NULL )
GO 

ALTER TABLE jo.Filiere ADD CONSTRAINT Filiere_PK PRIMARY KEY CLUSTERED (CodeFiliere)
GO

CREATE TABLE jo.Logiciel 
(
CodeLogiciel VARCHAR (20) NOT NULL , 
Nom NVARCHAR (40) NOT NULL , 
CodeFiliere VARCHAR (20) NOT NULL )
GO 

ALTER TABLE jo.Logiciel ADD CONSTRAINT Logiciel_PK PRIMARY KEY CLUSTERED (CodeLogiciel)
GO

CREATE TABLE jo.Metier 
(
CodeMetier VARCHAR (20) NOT NULL , 
CodeService VARCHAR (20) NOT NULL , 
Libelle NVARCHAR (40) NOT NULL )
GO 

ALTER TABLE jo.Metier ADD CONSTRAINT Metier_PK PRIMARY KEY CLUSTERED (CodeMetier)
GO

CREATE TABLE jo.Module 
(
CodeModule VARCHAR (20) NOT NULL , 
CodeLogiciel VARCHAR (20) NOT NULL , 
Libelle NVARCHAR (40) NOT NULL , 
CodeModuleParent VARCHAR (20) , 
CodeLogicielParent VARCHAR (20) )
GO 


ALTER TABLE jo.Module ADD CONSTRAINT Module_PK PRIMARY KEY CLUSTERED (CodeModule, CodeLogiciel)
GO

CREATE TABLE jo.Personne 
(
Login VARCHAR (20) NOT NULL , 
Nom NVARCHAR (40) NOT NULL , 
Prenom NVARCHAR (40) NOT NULL , 
CodeEquipe VARCHAR (20) NOT NULL , 
CodeMetier VARCHAR (20) NOT NULL , 
Manager VARCHAR (20) , 
TauxProductivite FLOAT (3) NOT NULL )
GO 

ALTER TABLE jo.Personne ADD CONSTRAINT Personne_PK PRIMARY KEY CLUSTERED (Login)
GO

CREATE TABLE jo.Release 
(
NumeroRelease SMALLINT NOT NULL , 
NumeroVersion FLOAT (4) NOT NULL , 
CodeLogiciel VARCHAR (20) NOT NULL , 
DateSetup DATE NOT NULL )
GO 


ALTER TABLE jo.Release ADD CONSTRAINT Release_PK PRIMARY KEY CLUSTERED (NumeroVersion, CodeLogiciel, NumeroRelease)
GO

CREATE TABLE jo.Service 
(
CodeService VARCHAR (20) NOT NULL , 
Nom NVARCHAR (40) NOT NULL )
GO 

ALTER TABLE jo.Service ADD CONSTRAINT Service_PK PRIMARY KEY CLUSTERED (CodeService)
GO

CREATE TABLE jo.Tache 
(
IdTache UNIQUEIDENTIFIER NOT NULL , 
Libelle NVARCHAR (40) NOT NULL , 
Annexe BIT NOT NULL DEFAULT 0 , 
CodeActivite VARCHAR (20) NOT NULL , 
Login VARCHAR (20) NOT NULL , 
Description NVARCHAR (1000) )
GO 

ALTER TABLE jo.Tache ADD CONSTRAINT Tache_PK PRIMARY KEY CLUSTERED (IdTache)
GO

CREATE TABLE jo.TacheProd 
(
IdTache UNIQUEIDENTIFIER NOT NULL , 
Numero INTEGER NOT NULL IDENTITY, 
DureePrevue FLOAT (5) NOT NULL DEFAULT 0 , 
DureeRestanteEstimee FLOAT (5) NOT NULL DEFAULT 0 , 
CodeModule VARCHAR (20) NOT NULL , 
CodeLogicielModule VARCHAR (20) NOT NULL , 
NumeroVersion FLOAT (4) NOT NULL , 
CodeLogicielVersion VARCHAR (20) NOT NULL )
GO 

ALTER TABLE jo.TacheProd ADD CONSTRAINT TacheProd_PK PRIMARY KEY CLUSTERED (IdTache)
GO

CREATE TABLE jo.Travail 
(
IdTache UNIQUEIDENTIFIER NOT NULL , 
DateTravail DATE NOT NULL , 
Heures FLOAT (5) NOT NULL DEFAULT 0 , 
TauxProductivite FLOAT (3) NOT NULL DEFAULT 1 )
GO 

ALTER TABLE jo.Travail ADD CONSTRAINT Travail_PK PRIMARY KEY CLUSTERED (IdTache, DateTravail)
GO

CREATE TABLE jo.Version 
(
NumeroVersion FLOAT (4) NOT NULL , 
CodeLogiciel VARCHAR (20) NOT NULL , 
Millesime SMALLINT NOT NULL , 
DateOuverture DATE NOT NULL , 
DateSortiePrevue DATE NOT NULL , 
DateSortieReelle DATE )
GO 

-------------------------------------------------------------------------
-- Création des contraintes de clés étrangères

ALTER TABLE jo.Version ADD CONSTRAINT Version_PK PRIMARY KEY CLUSTERED (NumeroVersion, CodeLogiciel)
GO

ALTER TABLE jo.Equipe 
ADD CONSTRAINT Equipe_Filiere_FK FOREIGN KEY (CodeFiliere) 
REFERENCES jo.Filiere (CodeFiliere ) 
GO 

ALTER TABLE jo.Equipe 
ADD CONSTRAINT Equipe_Service_FK FOREIGN KEY (CodeService) 
REFERENCES jo.Service (CodeService ) 
GO 

ALTER TABLE jo.ActiviteMetier 
ADD CONSTRAINT FK_ASS_22 FOREIGN KEY (MetierCodeMetier) 
REFERENCES jo.Metier (CodeMetier ) 
GO 

ALTER TABLE jo.ActiviteMetier 
ADD CONSTRAINT FK_ASS_23 FOREIGN KEY (ActiviteCodeActivite) 
REFERENCES jo.Activite (CodeActivite ) 
GO 

ALTER TABLE jo.Logiciel 
ADD CONSTRAINT Logiciel_Filiere_FK FOREIGN KEY (CodeFiliere) 
REFERENCES jo.Filiere (CodeFiliere ) 
GO 

ALTER TABLE jo.Metier 
ADD CONSTRAINT Metier_Service_FK FOREIGN KEY (CodeService) 
REFERENCES jo.Service (CodeService ) 
GO 

ALTER TABLE jo.Module 
ADD CONSTRAINT Module_Logiciel_FK FOREIGN KEY (CodeLogiciel) 
REFERENCES jo.Logiciel (CodeLogiciel ) 
GO 

ALTER TABLE jo.Module 
ADD CONSTRAINT Module_Module_FK FOREIGN KEY (CodeModuleParent, 
CodeLogicielParent) 
REFERENCES jo.Module (CodeModule , CodeLogiciel) 
GO 

ALTER TABLE jo.Personne 
ADD CONSTRAINT Personne_Equipe_FK FOREIGN KEY (CodeEquipe) 
REFERENCES jo.Equipe (CodeEquipe ) 
GO 

ALTER TABLE jo.Personne 
ADD CONSTRAINT Personne_Metier_FK FOREIGN KEY (CodeMetier) 
REFERENCES jo.Metier (CodeMetier ) 
GO 

ALTER TABLE jo.Personne 
ADD CONSTRAINT Personne_Personne_FK FOREIGN KEY (Manager) 
REFERENCES jo.Personne (Login ) 
GO 

ALTER TABLE jo.Release 
ADD CONSTRAINT Release_Version_FK FOREIGN KEY (NumeroVersion, 
CodeLogiciel) 
REFERENCES jo.Version (NumeroVersion , CodeLogiciel ) 
GO 

ALTER TABLE jo.TacheProd 
ADD CONSTRAINT TacheProd_Module_FK FOREIGN KEY (CodeModule, 
CodeLogicielModule) 
REFERENCES jo.Module (CodeModule , CodeLogiciel ) 
GO 

ALTER TABLE jo.TacheProd 
ADD CONSTRAINT TacheProd_Tache_FK FOREIGN KEY (IdTache) 
REFERENCES jo.Tache (IdTache ) 
GO 

ALTER TABLE jo.TacheProd 
ADD CONSTRAINT TacheProd_Version_FK FOREIGN KEY (NumeroVersion, 
CodeLogicielVersion) 
REFERENCES jo.Version (NumeroVersion , CodeLogiciel ) 
GO 

ALTER TABLE jo.Tache 
ADD CONSTRAINT Tache_Activite_FK FOREIGN KEY (CodeActivite) 
REFERENCES jo.Activite (CodeActivite ) 
GO 

ALTER TABLE jo.Tache 
ADD CONSTRAINT Tache_Personne_FK FOREIGN KEY (Login) 
REFERENCES jo.Personne (Login ) 
GO 

ALTER TABLE jo.Travail 
ADD CONSTRAINT Travail_Tache_FK FOREIGN KEY (IdTache) 
REFERENCES jo.Tache (IdTache ) 
GO 

ALTER TABLE jo.Version 
ADD CONSTRAINT Version_Logiciel_FK FOREIGN KEY (CodeLogiciel) 
REFERENCES jo.Logiciel (CodeLogiciel ) 
GO 