
print 'Table jo.Service'
insert jo.Service (CodeService, Nom) values
('DEV', 'Développement'),
('TEST', 'Test'),
('SL', 'Support Logiciel'),
('MKT', 'Marketing')
go

print 'Table jo.Filiere'
insert jo.Filiere (CodeFiliere, Libelle) values
('BIOV', 'Biologie végétale'),
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
('GENOMICA', 'BIOH', 'Genomica'),
('ANATOMIA', 'BIOH', 'Anatomia')
go

print 'Table jo.Module'
insert jo.Module (CodeModule, CodeLogiciel, Libelle, CodeLogicielParent, CodeModuleParent) values
('SEQUENCAGE', 'GENOMICA', 'Séquençage', null, null),
('MARQUAGE', 'GENOMICA', 'Marquage', 'GENOMICA', 'SEQUENCAGE'),
('SEPARATION', 'GENOMICA', 'Séparation', 'GENOMICA', 'SEQUENCAGE'),
('ANALYSE', 'GENOMICA', 'Analyse', 'GENOMICA', 'SEQUENCAGE'),
('POLYMORPHISME', 'GENOMICA', 'Polymorphisme génétique', null, null),
('VAR_ALLELE', 'GENOMICA', 'Variations alléliques', null, null),
('UTIL_DROITS', 'GENOMICA', 'Utilisateurs et droits', null, null),
('PARAMETRES', 'GENOMICA', 'Paramétrage', null, null),
('MICRO', 'ANATOMIA', 'Anatomie microscopique', null, null),
('PATHO', 'ANATOMIA', 'Anatomie pathologique', null, null),
('FONC', 'ANATOMIA', 'Anatomie fonctionnelle', null, null),
('RADIO', 'ANATOMIA', 'Anatomie radiologique', null, null),
('TOPO', 'ANATOMIA', 'Anatomie topographique', null, null)
go

print 'Table jo.Version'
insert jo.Version (NumeroVersion, CodeLogiciel, Millesime, DateOuverture, DateSortiePrevue, DateSortieReelle) values
(1.00, 'GENOMICA', '2017', '2016-01-02', '2017-01-08', '2017-01-20'),
(2.00, 'GENOMICA', '2018', '2016-12-28', '2018-02-28', NULL),
(4.50, 'ANATOMIA', '2015', '2015-09-01', '2016-07-07', '2016-07-20'),
(5.00, 'ANATOMIA', '2016', '2016-08-01', '2017-03-30', '2017-03-25'),
(5.50, 'ANATOMIA', '2017', '2017-03-30', '2017-11-20', NULL)
go

print 'Table jo.Realease'
insert jo.Release (NumeroRelease, NumeroVersion, CodeLogiciel, DateSetup) values
(1, 1.00, 'GENOMICA', '2016-01-15'),
(2, 1.00, 'GENOMICA', '2016-01-25'),
(3, 1.00, 'GENOMICA', '2016-02-05'),
(4, 1.00, 'GENOMICA', '2016-02-13'),
(5, 1.00, 'GENOMICA', '2016-02-23'),
(6, 1.00, 'GENOMICA', '2016-03-09'),
(7, 1.00, 'GENOMICA', '2016-03-18'),
(8, 1.00, 'GENOMICA', '2016-03-25'),
(9, 1.00, 'GENOMICA', '2016-04-02'),
(10, 1.00, 'GENOMICA', '2016-04-09'),
(11, 1.00, 'GENOMICA', '2016-04-16'),
(12, 1.00, 'GENOMICA', '2016-04-28'),
(13, 1.00, 'GENOMICA', '2016-05-10'),
(14, 1.00, 'GENOMICA', '2016-05-17'),
(15, 1.00, 'GENOMICA', '2016-06-10'),
(16, 1.00, 'GENOMICA', '2016-06-19'),
(17, 1.00, 'GENOMICA', '2016-06-27'),
(18, 1.00, 'GENOMICA', '2016-07-04'),
(19, 1.00, 'GENOMICA', '2016-07-13'),
(20, 1.00, 'GENOMICA', '2016-07-23'),
(21, 1.00, 'GENOMICA', '2016-08-01'),
(22, 1.00, 'GENOMICA', '2016-08-09'),
(23, 1.00, 'GENOMICA', '2016-08-21'),
(24, 1.00, 'GENOMICA', '2016-08-27'),
(25, 1.00, 'GENOMICA', '2016-09-02'),
(26, 1.00, 'GENOMICA', '2016-09-14'),
(27, 1.00, 'GENOMICA', '2016-09-22'),
(28, 1.00, 'GENOMICA', '2016-10-14'),
(29, 1.00, 'GENOMICA', '2016-10-23'),
(30, 1.00, 'GENOMICA', '2016-11-03'),
(31, 1.00, 'GENOMICA', '2016-11-15'),
(32, 1.00, 'GENOMICA', '2016-11-26'),
(33, 1.00, 'GENOMICA', '2016-12-02'),
(1, 2.00, 'GENOMICA', '2017-01-07'),
(2, 2.00, 'GENOMICA', '2017-02-11'),
(3, 2.00, 'GENOMICA', '2017-03-17'),
(4, 2.00, 'GENOMICA', '2017-04-12'),
(5, 2.00, 'GENOMICA', '2017-05-05'),
(1, 5.50, 'ANATOMIA', '2017-04-07'),
(2, 5.50, 'ANATOMIA', '2017-04-19'),
(3, 5.50, 'ANATOMIA', '2017-04-28'),
(4, 5.50, 'ANATOMIA', '2017-05-12'),
(5, 5.50, 'ANATOMIA', '2017-05-17')
go

print 'Table jo.Metier'
insert jo.Metier (CodeMetier, CodeService, Libelle) values
('ANA', 'MKT', 'Analyste'),
('CDP', 'DEV', 'Chef de Projet'),
('DEV', 'DEV', 'Développeur'),
('DES', 'MKT', 'Designer'),
('TES', 'TEST', 'Testeur')
go

-- Activités de production
print 'Table jo.Activite'
insert jo.Activite (CodeActivite, Libelle) values
('DBE', 'Définition des besoins'),
('ARF', 'Architecture fonctionnelle'),
('ANF', 'Analyse fonctionnelle'),
('DES', 'Design'),
('INF', 'Infographie'),
('ART', 'Architecture technique'),
('ANT', 'Analyse technique'),
('DEV', 'Développement'),
('RPT', 'Rédaction de plan de test'),
('TES', 'Test')
go

-- Activités annexes
insert jo.Activite(CodeActivite, Libelle, Annexe) values
('APPUI_EQUIPE', 'Appui des personnes de l''équipe', 1),
('APPUI_AUTRE_SERV', 'Appui aux personnes des autres services', 1),
('FORMATION_RECUE', 'Formation reçue', 1),
('FORMATION_DISP', 'Formation dispensée', 1),
('DP', 'Travail de délégué du personnel', 1),
('SALON', 'Déplacement à un salon', 1),
('SEMINAIRE', 'Séminaire entreprise', 1),
('REUNION_EQUIPE', 'Réunion d''équipe', 1),
('REUNION_SERVICE', 'Réunion de service', 1)
GO

-- Associations Métiers - Activités
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
('GLECLERCQ', 'Geneviève', 'LECLERCQ', 'ANA', 'BIOH_MKT', 'JROUSSET', 1),
('AFERRAND', 'Angèle', 'FERRAND', 'ANA', 'BIOH_MKT', 'JROUSSET', 1),
('MWEBER', 'Marguerite', 'WEBER', 'DES', 'BIOH_MKT', 'JROUSSET', 1),
('BNORMAND', 'Balthazar', 'NORMAND', 'CDP', 'BIOH_DEV', null, 1),
('RFISHER', 'Raymond', 'FISHER', 'DEV', 'BIOH_DEV', 'BNORMAND', 1),
('LBUTLER', 'Lucien', 'BUTLER', 'DEV', 'BIOH_DEV', 'BNORMAND', 1),
('RBEAUMONT', 'Rosline', 'BEAUMONT', 'DEV', 'BIOH_DEV', 'BNORMAND', 1),
('APARENT', 'Agnès', 'PARENT', 'TES', 'BIOH_TEST', null, 1),
('HKLEIN', 'Hilaire', 'KLEIN', 'TES', 'BIOH_TEST', 'APARENT', 1),
('NPALMER', 'Nino', 'PALMER', 'TES', 'BIOH_TEST', 'APARENT', 1)
go

----------------------------------------------------------
-- Tâches annexes
print 'Table jo.Tache'
insert jo.Tache(IdTache, CodeActivite, Libelle, Annexe, Login)
select newid(), a.CodeActivite, a.Libelle, 1, p.Login
from jo.Personne p cross join jo.Activite a
where a.Annexe = 1 and a.CodeActivite in ('REUNION_EQUIPE', 'REUNION_SERVICE')

update jo.Tache set Description =
case left(libelle, 3)
	when 'AF ' then replace(libelle, 'AF', 'Analyse fonctionnelle du module') 
	when 'AT ' then replace(libelle, 'AT', 'Analyse technique du module') 
	when 'DEV ' then replace(libelle, 'DEV', 'Développement du module')
	else Libelle
end
go