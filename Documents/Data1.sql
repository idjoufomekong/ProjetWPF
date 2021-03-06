
print 'Table jo.Service'
insert jo.Service (CodeService, Nom) values
('DEV', 'D�veloppement'),
('TEST', 'Test'),
('SL', 'Support Logiciel'),
('MKT', 'Marketing')
go

print 'Table jo.Filiere'
insert jo.Filiere (CodeFiliere, Libelle) values
('BIOV', 'Biologie v�g�tale'),
('BIOH', 'Biologie humaine'),
('BIOA', 'Biologie animale')
go

print 'Table jo.Equipe'
insert jo.Equipe (CodeEquipe, Nom, CodeFiliere, CodeService) values
('BIOH_MKT', 'Marketing Bio humaine', 'BIOH', 'MKT'),
('BIOH_DEV', 'Dev Bio humaine', 'BIOH', 'DEV'),
('BIOH_TEST', 'Test Bio humaine', 'BIOH', 'TEST'),
('BIOH_SL', 'Suppotr Bio humaine', 'BIOH', 'SL')
go

print 'Table jo.Logiciel'
insert jo.Logiciel (CodeLogiciel, CodeFiliere, Nom) values
('GENOMICA', 'BIOH', 'Genomica')
go

print 'Table jo.Module'
insert jo.Module (CodeModule, CodeLogiciel, Libelle, CodeLogicielParent, CodeModuleParent) values
('SEQUENCAGE', 'GENOMICA', 'S�quen�age', null, null),
('MARQUAGE', 'GENOMICA', 'Marquage', 'GENOMICA', 'SEQUENCAGE'),
('SEPARATION', 'GENOMICA', 'S�paration', 'GENOMICA', 'SEQUENCAGE'),
('ANALYSE', 'GENOMICA', 'Analyse', 'GENOMICA', 'SEQUENCAGE'),
('POLYMORPHISME', 'GENOMICA', 'Polymorphisme g�n�tique', null, null),
('VAR_ALLELE', 'GENOMICA', 'Variations all�liques', null, null),
('UTIL_DROITS', 'GENOMICA', 'Utilisateurs et droits', null, null),
('PARAMETRES', 'GENOMICA', 'Param�trage', null, null)
go

print 'Table jo.Version'
insert jo.Version (NumeroVersion, CodeLogiciel, Millesime, DateOuverture, DateSortiePrevue, DateSortieReelle) values
(1.00, 'GENOMICA', '2017', '2016-01-02', '2017-01-08', '2017-01-20'),
(2.00, 'GENOMICA', '2018', '2016-12-28', '2018-02-28', NULL)
go

print 'Table jo.Realease'
insert jo.Release (NumeroRelease, NumeroVersion, CodeLogiciel, DateSetup) values
(1, 1.00, 'GENOMICA', '2016-01-15'),
(2, 1.00, 'GENOMICA', '2016-02-13'),
(3, 1.00, 'GENOMICA', '2016-03-18'),
(4, 1.00, 'GENOMICA', '2016-04-09'),
(5, 1.00, 'GENOMICA', '2016-05-10'),
(6, 1.00, 'GENOMICA', '2016-06-19'),
(7, 1.00, 'GENOMICA', '2016-07-23'),
(8, 1.00, 'GENOMICA', '2016-08-21'),
(9, 1.00, 'GENOMICA', '2016-09-17'),
(10, 1.00, 'GENOMICA', '2016-10-14'),
(11, 1.00, 'GENOMICA', '2016-11-03'),
(12, 1.00, 'GENOMICA', '2016-12-02'),
(1, 2.00, 'GENOMICA', '2017-01-07'),
(2, 2.00, 'GENOMICA', '2017-02-11'),
(3, 2.00, 'GENOMICA', '2017-03-17'),
(4, 2.00, 'GENOMICA', '2017-04-12'),
(5, 2.00, 'GENOMICA', '2017-05-05')
go

print 'Table jo.Metier'
insert jo.Metier (CodeMetier, CodeService, Libelle) values
('ANA', 'MKT', 'Analyste'),
('CDP', 'DEV', 'Chef de Projet'),
('DEV', 'DEV', 'D�veloppeur'),
('DES', 'MKT', 'Designer'),
('TES', 'TEST', 'Testeur')
go

-- Activit�s de production
print 'Table jo.Activite'
insert jo.Activite (CodeActivite, Libelle) values
('DBE', 'D�finition des besoins'),
('ARF', 'Architecture fonctionnelle'),
('ANF', 'Analyse fonctionnelle'),
('DES', 'Design'),
('INF', 'Infographie'),
('ART', 'Architecture technique'),
('ANT', 'Analyse technique'),
('DEV', 'D�veloppement'),
('RPT', 'R�daction de plan de test'),
('TES', 'Test')
go

-- Activit�s annexes
insert jo.Activite(CodeActivite, Libelle, Annexe) values
('APPUI_EQUIPE', 'Appui des personnes de l''�quipe', 1),
('APPUI_AUTRE_SERV', 'Appui aux personnes des autres services', 1),
('FORMATION_RECUE', 'Formation re�ue', 1),
('FORMATION_DISP', 'Formation dispens�e', 1),
('DP', 'Travail de d�l�gu� du personnel', 1),
('SALON', 'D�placement � un salon', 1),
('SEMINAIRE', 'S�minaire entreprise', 1),
('REUNION_EQUIPE', 'R�union d''�quipe', 1),
('REUNION_SERVICE', 'R�union de service', 1)
GO

-- Associations M�tiers - Activit�s
print 'Table jo.ActiviteMetier'
insert jo.ActiviteMetier (MetierCodeMetier, ActiviteCodeActivite) values
('ANA', 'DBE'),('ANA', 'ARF'),('ANA', 'ANF'),
('CDP', 'ARF'),('CDP', 'ANF'),('CDP', 'ART'),('CDP', 'TES'),
('DEV', 'ANF'),('DEV', 'ART'),('DEV', 'ANT'),('DEV', 'DEV'),('DEV', 'TES'),
('DES', 'ANF'),('DES', 'DES'),('DES', 'INF'),
('TES', 'RPT'),('TES', 'TES')
go

insert jo.ActiviteMetier (MetierCodeMetier, ActiviteCodeActivite)
select m.CodeMetier, a.CodeActivite
from jo.Activite a cross join jo.Metier m
where a.Annexe = 1
go

print 'Table jo.Personne'
insert jo.Personne (Login, Prenom, Nom, CodeMetier, CodeEquipe, Manager, TauxProductivite) values
('JROUSSET', 'Joseph', 'ROUSSET', 'ANA', 'BIOH_MKT', null, 1),
('GLECLERCQ', 'Genevi�ve', 'LECLERCQ', 'ANA', 'BIOH_MKT', 'JROUSSET', 1),
('AFERRAND', 'Ang�le', 'FERRAND', 'ANA', 'BIOH_MKT', 'JROUSSET', 1),
('MWEBER', 'Marguerite', 'WEBER', 'DES', 'BIOH_MKT', 'JROUSSET', 1),
('BNORMAND', 'Balthazar', 'NORMAND', 'CDP', 'BIOH_DEV', null, 1),
('RFISHER', 'Raymond', 'FISHER', 'DEV', 'BIOH_DEV', 'BNORMAND', 1),
('LBUTLER', 'Lucien', 'BUTLER', 'DEV', 'BIOH_DEV', 'BNORMAND', 1),
('RBEAUMONT', 'Rosline', 'BEAUMONT', 'DEV', 'BIOH_DEV', 'BNORMAND', 1),
('APARENT', 'Agn�s', 'PARENT', 'TES', 'BIOH_TEST', null, 1),
('HKLEIN', 'Hilaire', 'KLEIN', 'TES', 'BIOH_TEST', 'APARENT', 1),
('NPALMER', 'Nino', 'PALMER', 'TES', 'BIOH_TEST', 'APARENT', 1)
go

----------------------------------------------------------
-- T�ches annexes
print 'Table jo.Tache'
insert jo.Tache(IdTache, CodeActivite, Libelle, Annexe, Login)
select newid(), a.CodeActivite, a.Libelle, 1, p.Login
from jo.Personne p cross join jo.Activite a
where a.Annexe = 1 and a.CodeActivite in ('REUNION_EQUIPE', 'REUNION_SERVICE')

update jo.Tache set Description =
case left(libelle, 3)
	when 'AF ' then replace(libelle, 'AF', 'Analyse fonctionnelle du module') 
	when 'AT ' then replace(libelle, 'AT', 'Analyse technique du module') 
	when 'DEV ' then replace(libelle, 'DEV', 'D�veloppement du module')
	else Libelle
end
go