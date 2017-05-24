using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobOverview.Entity;
using System.Collections.ObjectModel;

namespace JobOverview.Model
{
    public class DALTache
    {
        //Liste tâches pour visualiser
        public static List<TacheApercu> RecupererTachesApercu(string codeLogiciel, float version, string codeManager)
        {
            List<TacheApercu> listTache = new List<TacheApercu>();

            var connectString = Properties.Settings.Default.JobOverviewConnectionString;
            //Sélection de toutes les tâches (production et annexes) avec leurs travaux et l'employé affecté
            //Les tâches sans travaux sont incluses ainsi que les employés sans tâches
            //On récupère les 50 dernières modifications
            //Pour la version et le logiciel sélectionnés
            //Pour le manager connecté
            string queryString = @"select top(50) P.Login, P.Prenom+' '+P.Nom NomComplet,P.CodeMetier,P.Manager,T.IdTache,
            T.Libelle,T.CodeActivite,T.Description, T.Annexe,TP.Numero,TP.DureePrevue,TP.DureeRestanteEstimee,
            TP.CodeLogicielVersion,TP.CodeModule,TP.NumeroVersion,TR.DateTravail,TR.Heures,TR.TauxProductivite
            from jo.Personne P
            left outer join jo.Tache T on T.Login=P.Login
            left outer join jo.TacheProd TP on T.IdTache=TP.IdTache
            left outer join jo.Travail TR on  TR.IdTache=T.IdTache 
            where CodeLogicielVersion=@codeLogiciel and NumeroVersion=@numVersion and ( Manager=@manager OR P.Login=@manager )
UNION
select top(50) P.Login, P.Prenom+' '+P.Nom NomComplet,P.CodeMetier,P.Manager,T.IdTache,
            T.Libelle,T.CodeActivite,T.Description, T.Annexe,TP.Numero,TP.DureePrevue,TP.DureeRestanteEstimee,
            TP.CodeLogicielVersion,TP.CodeModule,TP.NumeroVersion,TR.DateTravail,TR.Heures,TR.TauxProductivite
            from jo.Personne P
            left outer join jo.Tache T on T.Login=P.Login
            left outer join jo.TacheProd TP on T.IdTache=TP.IdTache
            left outer join jo.Travail TR on  TR.IdTache=T.IdTache 
            where (Manager=@manager OR P.Login=@manager ) and annexe=1
order by DateTravail";

            //TODO: Epurer la requête et retirer les champs inutiles

            var codeLog = new SqlParameter("@codeLogiciel", DbType.String);
            var numVersion = new SqlParameter("@numVersion", DbType.Double);
            var codeMng = new SqlParameter("@manager", DbType.Double);
            codeLog.Value = codeLogiciel;
            numVersion.Value = version;
            codeMng.Value = codeManager;

            using (var connect = new SqlConnection(connectString))
            {
                //Récupération liste des tâches de production
                var command = new SqlCommand(queryString, connect);
                command.Parameters.Add(codeLog);
                command.Parameters.Add(numVersion);
                command.Parameters.Add(codeMng);
                connect.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        RecupererTachesApercuFromDataReader(listTache, reader);
                    }
                }
            }

            return listTache;
        }

        private static void RecupererTachesApercuFromDataReader(List<TacheApercu> listTache, SqlDataReader reader)
        {
            TacheApercu tache = new TacheApercu();

            if (reader["IdTache"] != DBNull.Value)
                tache.IdTache = (Guid)reader["IdTache"];
            if (reader["Libelle"] != DBNull.Value)
                tache.NomTache = (string)reader["Libelle"];
            if (reader["Annexe"] != DBNull.Value)
                tache.Annexe = (bool)reader["Annexe"];
            if (reader["CodeActivite"] != DBNull.Value)
                tache.CodeActivite = (string)reader["CodeActivite"];
            if (reader["Login"] != DBNull.Value)
                tache.Login = (string)reader["Login"];
            if (reader["Description"] != DBNull.Value)
                tache.Description = (string)reader["Description"];

            if (reader["Numero"] != DBNull.Value)
                tache.NumTache = (int)reader["Numero"];
            if (reader["DureePrevue"] != DBNull.Value)
                tache.DureePrevue = (float)reader["DureePrevue"];
            if (reader["DureeRestanteEstimee"] != DBNull.Value)
                tache.DureeRestante = (float)reader["DureeRestanteEstimee"];
            if (reader["CodeModule"] != DBNull.Value)
                tache.CodeModule = (string)reader["CodeModule"];
            if (reader["NumeroVersion"] != DBNull.Value)
                tache.CodeVersion = (float)reader["NumeroVersion"];
            if (reader["CodeLogicielVersion"] != DBNull.Value)
                tache.CodeLogiciel = (string)reader["CodeLogicielVersion"];
            if (reader["DateTravail"] != DBNull.Value)
                tache.Date = (DateTime)reader["DateTravail"];
            if (reader["Heures"] != DBNull.Value)
                tache.Heures = (float)reader["Heures"];
            if (reader["TauxProductivite"] != DBNull.Value)
                tache.TauxProductivite = (float)reader["TauxProductivite"];

            listTache.Add(tache);
        }

        //Récupération des personnes et leurs tâches
        /// <summary>
        /// Sélection des membres de l'équipe du manager connecté avec la liste des tâches et travaux
        /// pour tous les logiciels et toutes les versions
        /// !!!!!!!!!!!!!!!!!!!!!!! Prend les personnes sans tâches 
        /// tâches prod et tâches annexes incluses
        /// </summary>
        /// <returns></returns>
        public static List<Personne> RecupererPersonnesTaches(/*string codeLogiciel, float version,*/ string codeManager)
        {
            List<Personne> listPers = new List<Entity.Personne>();

            var connectString = Properties.Settings.Default.JobOverviewConnectionString;

            //On récupère d'abord les tâches de production
            string queryStringPersonne = @"select P.Login, P.Prenom+' '+P.Nom NomComplet,P.CodeMetier,P.Manager,
            T.IdTache,T.Libelle,T.CodeActivite,T.Description, T.Annexe,TP.Numero,TP.DureePrevue,TP.DureeRestanteEstimee,
            TP.CodeLogicielVersion,TP.CodeModule,TP.NumeroVersion,TR.DateTravail,TR.Heures,TR.TauxProductivite
            from jo.Personne P
            left outer join jo.Tache T on T.Login=P.Login
            left outer join jo.TacheProd TP on T.IdTache=TP.IdTache
            left outer join jo.Travail TR on  TR.IdTache=T.IdTache 
            where  ( Manager=@manager OR P.Login=@manager )
order by Login,Numero";//CodeLogicielVersion=@codeLogiciel and NumeroVersion=@numVersion and
            //TODO Bien définir quelles tâches on veut exporter afin de peaufiner la requête

            //var codeLog = new SqlParameter("@codeLogiciel", DbType.String);
            //var numVersion = new SqlParameter("@numVersion", DbType.Double);
            var codeMng = new SqlParameter("@manager", DbType.Double);
            //codeLog.Value = codeLogiciel;
            //numVersion.Value = version;
            codeMng.Value = codeManager;
            //TODO: Mettre à jour les paramètres de la méthode

            using (var connect = new SqlConnection(connectString))
            {
                //Récupération liste des tâches de production
                var command = new SqlCommand(queryStringPersonne, connect);
                //command.Parameters.Add(codeLog);
                //command.Parameters.Add(numVersion);
                command.Parameters.Add(codeMng);
                connect.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        RecupererPersonneTachesProdFromDataReader(listPers, reader);
                    }
                }
                //Récupération de la liste des tâches annexes pour toutes les personnes de la liste remplie
                foreach (var p in listPers)
                {
                    p.TachesAnnexes = RecupererTachesAnnexes(p.CodePersonne, connect);
                }
                //Au lieu de garder les listes vides, je les rends nulles
                foreach (var m in listPers)
                {
                    if (m.TachesProd.Count == 0)
                        m.TachesProd = null;
                    if (m.TachesAnnexes.Count == 0)
                        m.TachesAnnexes = null;
                }
            }

            return listPers;
        }

        /// <summary>
        /// Sélection des tâches annexes de la personne entrée en paramètre
        /// </summary>
        /// <param name="codePersonne"></param>
        private static List<Tache> RecupererTachesAnnexes(string codePersonne, SqlConnection connexion)
        {
            List<Tache> listTachesAnnexe = new List<Tache>();
            string queryStringAnnexe = @"select T.IdTache,T.Libelle,T.CodeActivite,T.Description,TR.DateTravail,TR.Heures,
                                        TR.TauxProductivite, T.Annexe
                                        from jo.Tache T
                                        left outer join jo.Travail TR on TR.IdTache=T.IdTache
                                        where Annexe=1 and login=@codePersonne 
                                        order by IdTache";

            var personne = new SqlParameter("@codePersonne", DbType.String);
            personne.Value = codePersonne;

            //Récupération liste des tâches de production
            var command = new SqlCommand(queryStringAnnexe, connexion);
            command.Parameters.Add(personne);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    RecupererTachesAnnexesFromDataReader(listTachesAnnexe, reader);
                }
            }
            return listTachesAnnexe;
        }

        /// <summary>
        /// Lecture du retour de la requête SQL de sélection des tâches annexes de la personne entrée en paramètre
        /// </summary>
        /// <param name="listPers"></param>
        /// <param name="reader"></param>
        private static void RecupererTachesAnnexesFromDataReader(List<Tache> listTache, SqlDataReader reader)
        {
            if (reader["Annexe"] != DBNull.Value)
            {
                Guid idTache = (Guid)reader["IdTache"];
                Tache tache = null;
                if ((listTache.Count == 0) || (listTache.Last().IdTache != idTache))
                {
                    tache = new Tache();
                    tache.IdTache = (Guid)reader["IdTache"];
                    tache.NomTache = (string)reader["Libelle"];
                    tache.Annexe = true;
                    tache.CodeActivite = (string)reader["CodeActivite"];
                    tache.Description = reader["Description"].ToString();

                    listTache.Add(tache);
                }
                else
                {
                    tache = listTache.Last();

                }
                //Récupération des travaux de production
                if (reader["DateTravail"] != DBNull.Value)
                {
                    if (tache.TravauxAnnexes == null)
                        tache.TravauxAnnexes = new List<Travail>();
                    Travail tra = new Travail();

                    tra.Date = (DateTime)reader["DateTravail"];
                    tra.Heures = (float)reader["Heures"];
                    tra.TauxProductivite = (float)reader["TauxProductivite"];

                    tache.TravauxAnnexes.Add(tra);
                }
            }
        }
        //TODO: Dans aperçuliste tâche, retirer les champs version et logiciel de la vue???

        /// <summary>
        /// Lecture du retour de la requête SQL de sélection des personnes et de la liste des tâches de production
        /// </summary>
        /// <param name="listPers"></param>
        /// <param name="reader"></param>
        private static void RecupererPersonneTachesProdFromDataReader(List<Personne> listPers, SqlDataReader reader)
        {
            string login = (string)reader["Login"];
            Personne pers = null;
            if ((listPers.Count == 0) || (listPers[listPers.Count - 1].CodePersonne != login))
            {
                pers = new Personne();
                //Attention aux champs nullable
                pers.CodePersonne = (string)reader["Login"];
                pers.NomPrenom = (string)reader["NomComplet"];
                pers.CodeMetier = (string)reader["CodeMetier"];

                pers.TachesProd = new ObservableCollection<TacheProd>();


                listPers.Add(pers);
            }
            else
            {
                pers = listPers.Last();
            }
            //Récupération de la liste des tâches de production
            if (reader["Numero"] != DBNull.Value)
            {
                var numTache = (int)reader["Numero"];
                TacheProd tache = null;

                if ((pers.TachesProd.Count == 0) || (pers.TachesProd[pers.TachesProd.Count - 1].NumTache != numTache))
                {
                    tache = new TacheProd();
                    tache.IdTache = (Guid)reader["IdTache"];
                    tache.NumTache = (int)reader["Numero"];
                    tache.NomTache = (string)reader["Libelle"];
                    tache.Annexe = false;
                    tache.CodeActivite = (string)reader["CodeActivite"];
                    if (reader["Description"] != DBNull.Value)
                        tache.Description = (string)reader["Description"];
                    tache.DureeRestante = (float)reader["DureeRestanteEstimee"];
                    tache.DureePrevue = (float)reader["DureePrevue"];
                    tache.CodeVersion = (float)reader["NumeroVersion"];
                    tache.CodeLogiciel = (string)reader["CodeLogicielVersion"];
                    tache.CodeModule = (string)reader["CodeModule"];

                    pers.TachesProd.Add(tache);
                }
                else
                {
                    tache = pers.TachesProd.Last();

                }
                //Récupération des travaux de production
                if (reader["DateTravail"] != DBNull.Value)
                {
                    if (tache.TravauxProd == null)
                        tache.TravauxProd = new List<Travail>();
                    Travail tra = new Travail();

                    tra.Date = (DateTime)reader["DateTravail"];
                    tra.Heures = (float)reader["Heures"];
                    tra.TauxProductivite = (float)reader["TauxProductivite"];

                    tache.TravauxProd.Add(tra);
                }
            }
        }
        /// <summary>
        /// Récupère la liste des tâches annexes des memebres de l'équipe du manager connecté
        /// </summary>
        /// <param name="codeManager">Manager connecté</param>
        /// <returns></returns>
        public static List<Personne> RecupererPersonnesTachesAnnexes(string codeManager)
        {
            List<Personne> listPers = new List<Entity.Personne>();

            var connectString = Properties.Settings.Default.JobOverviewConnectionString;

            //On récupère d'abord les membres de l'équipe du manager connecté
            string queryStringPersonne = @"select P.Login, P.Prenom+' '+P.Nom NomComplet,P.CodeMetier,P.Manager
            from jo.Personne P
            where Manager=@manager OR P.Login=@manager";

            var codeMng = new SqlParameter("@manager", DbType.Double);
            codeMng.Value = codeManager;

            using (var connect = new SqlConnection(connectString))
            {
                //Récupération liste des tâches de production
                var command = new SqlCommand(queryStringPersonne, connect);
                command.Parameters.Add(codeMng);
                connect.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        RecupererPersonneTachesAnnexesFromDataReader(listPers, reader);
                    }
                }
                //Récupération de la liste des tâches annexes pour toutes les personnes de la liste remplie
                foreach (var p in listPers)
                {
                    p.TachesAnnexes = RecupererTachesAnnexes(p.CodePersonne, connect);
                }
                //Au lieu de garder les listes vides, je les rends nulles
                foreach (var m in listPers)
                {
                    if (m.TachesAnnexes.Count == 0)
                        m.TachesAnnexes = null;
                }
            }

            return listPers;
        }

        /// <summary>
        /// Chargement de l'objet Personnes à partir du reader envoyé par la méthode de sélection des tâches annexes
        /// </summary>
        /// <param name="listPers"></param>
        /// <param name="reader"></param>
        private static void RecupererPersonneTachesAnnexesFromDataReader(List<Personne> listPers, SqlDataReader reader)
        {

            Personne pers = new Personne();

            pers.CodePersonne = (string)reader["Login"];
            pers.NomPrenom = (string)reader["NomComplet"];
            pers.CodeMetier = (string)reader["CodeMetier"];

            pers.TachesAnnexes = new List<Tache>();

            listPers.Add(pers);
        }

        /// <summary>
        /// Retourne la liste des personnes avec les tâches de production uniquement
        /// </summary>
        /// <param name="codeLogiciel"></param>
        /// <param name="version"></param>
        /// <param name="codeManager"></param>
        /// <returns></returns>
        public static List<Personne> RecupererPersonnesTachesProd(/*string codeLogiciel, float version,*/ string codeManager)
        {
            List<Personne> listPers = new List<Entity.Personne>();

            var connectString = Properties.Settings.Default.JobOverviewConnectionString;

            string queryStringPersonne = @"select P.Login, P.Prenom+' '+P.Nom NomComplet,P.CodeMetier,P.Manager,
            T.IdTache,T.Libelle,T.CodeActivite,T.Description, T.Annexe,TP.Numero,TP.DureePrevue,TP.DureeRestanteEstimee,
            TP.CodeLogicielVersion,TP.CodeModule,TP.NumeroVersion,TR.DateTravail,TR.Heures,TR.TauxProductivite
            from jo.Personne P
            left outer join jo.Tache T on T.Login=P.Login
            left outer join jo.TacheProd TP on T.IdTache=TP.IdTache
            left outer join jo.Travail TR on  TR.IdTache=T.IdTache 
            where  ( Manager=@manager OR P.Login=@manager )
            order by Login,Numero";
            //TODO Bien définir les champs afin de peaufiner la requête
            //CodeLogicielVersion=@codeLogiciel and NumeroVersion=@numVersion and

            //var codeLog = new SqlParameter("@codeLogiciel", DbType.String);
            //var numVersion = new SqlParameter("@numVersion", DbType.Double);
            var codeMng = new SqlParameter("@manager", DbType.Double);
            //codeLog.Value = codeLogiciel;
            //numVersion.Value = version;
            codeMng.Value = codeManager;

            using (var connect = new SqlConnection(connectString))
            {
                var command = new SqlCommand(queryStringPersonne, connect);
                //command.Parameters.Add(codeLog);
                //command.Parameters.Add(numVersion);
                command.Parameters.Add(codeMng);
                connect.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        RecupererPersonneTachesProdFromDataReader(listPers, reader);
                    }
                }

                foreach (var m in listPers)
                {
                    if (m.TachesProd.Count == 0)
                        m.TachesProd = null;
                    //if (m.TachesAnnexes!=null && m.TachesAnnexes.Count == 0)
                    //    m.TachesAnnexes = null;
                }
            }

            return listPers;
        }


        /// <summary>
        /// Permet d'étendre la liste des tâches annexes des employés
        /// </summary>
        /// <param name="listPers"></param>
        public static void RecupererPersonnesTachesAnnexesEtendues(List<Personne> listPers)
        {
            // On récupère la liste des activités annexes
            List<Activite> listAnnexes = RecupererActivitesAnnexes();

            // On complète la liste des tâches annexes de chaque employé
            foreach (var emp in listPers)
            {
                // Il faut gérer le cas où la liste de tâche annexe de l'employé courant est vide.
                if (emp.TachesAnnexes == null)
                    emp.TachesAnnexes = new List<Tache>();

                foreach (var act in listAnnexes)
                {
                    var res = emp.TachesAnnexes.Where(a => a.CodeActivite == act.CodeActivite).FirstOrDefault();

                    if (res == null)
                    {
                        var t = new Tache();

                        t.CodeActivite = act.CodeActivite;
                        t.NomTache = act.NomActivite;
                        t.Annexe = true;
                        t.Assignation = false;

                        emp.TachesAnnexes.Add(t);
                    }
                    else
                    {
                        res.Assignation = true;
                    }
                }

                // Tri par ordre alphabétique selon le nom des tâches annexes
                emp.TachesAnnexes = emp.TachesAnnexes.OrderBy(a => a.NomTache).ToList();
            }
        }

        /// <summary>
        /// Retourne la liste des activités annexes
        /// </summary>
        /// <returns></returns>
        private static List<Activite> RecupererActivitesAnnexes()
        {
            // Liste à retourner
            var listActivites = new List<Activite>();

            // Récupération de la chaîne de connexion
            var connectString = Properties.Settings.Default.JobOverviewConnectionString;

            // Ecriture de la requête
            string req = @"select CodeActivite, Libelle
                            from jo.Activite
                            where Annexe = 1
                            order by Libelle";

            // Création d'une connexion à partir de la chaîne de connexion
            using (var cnx = new SqlConnection(connectString))
            {
                // Création d'une commande à partir de la requête et de la connexion
                var command = new SqlCommand(req, cnx);

                // Ouverture de la connexion (elle sera fermée en sortant de l'instruction using)
                cnx.Open();

                // Exécution de la commande
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    // On parcourt le reader et on remplit la liste.
                    while (reader.Read())
                    {
                        Activite act = new Activite();

                        act.CodeActivite = (string)reader["CodeActivite"];
                        act.NomActivite = (string)reader["Libelle"];

                        listActivites.Add(act);
                    }
                }

                // On retourne la liste obtenue
                return listActivites;
            }
        }

        /// <summary>
        /// Enregistre une tâche annexe dans la base de données
        /// </summary>
        /// <param name="codePersonne"></param>
        /// <param name="tache"></param>
        public static void AjouterTacheAnnexe(string codePersonne, Tache tache)
        {
            // Récupération de la chaîne de connexion
            var connectString = Properties.Settings.Default.JobOverviewConnectionString;

            // Ecriture de la requête
            string req = @"insert jo.Tache(IdTache, Libelle, Annexe, CodeActivite, Login, Description)
                            values (@Id, @nom, 1, @codeAct, @log, @descript)";

            // Paramètres de la requête
            var paramID = new SqlParameter("@Id", DbType.Guid);
            paramID.Value = Guid.NewGuid();
            var paramNom = new SqlParameter("@nom", DbType.String);
            paramNom.Value = tache.NomTache;
            var paramCodeAct = new SqlParameter("@codeAct", DbType.String);
            paramCodeAct.Value = tache.CodeActivite;
            var paramLog = new SqlParameter("@log", DbType.String);
            paramLog.Value = codePersonne;
            var paramDescript = new SqlParameter("@descript", DbType.String);
            // La description de la tâche est facultative
            if (tache.Description != null)
                paramDescript.Value = tache.Description;
            else paramDescript.Value = DBNull.Value;

            // Création d'une connexion à partir de la chaîne de connexion
            using (var cnx = new SqlConnection(connectString))
            {
                // Ouverture de la connection
                cnx.Open();

                // Utilisation d'une transaction
                SqlTransaction tran = cnx.BeginTransaction();

                // Création de la commande et entrée des paramètres.
                var command = new SqlCommand(req, cnx, tran);
                command.Parameters.Add(paramID);
                command.Parameters.Add(paramNom);
                command.Parameters.Add(paramCodeAct);
                command.Parameters.Add(paramLog);
                command.Parameters.Add(paramDescript);

                try
                {
                    command.ExecuteNonQuery();
                    // Validation de la transaction si tout se passe bien.
                    tran.Commit();
                }
                catch (SqlException ex)
                {
                    // Annulation de la transaction
                    tran.Rollback();

                    if (ex.Number == 8152)
                        throw new Exception("Création impossible : le champ description d'une des tâches à créer comporte plus de 1000 caractères.", ex);
                }
            }
        }

        /// <summary>
        /// Supprime une tâche annexe dans la base de données
        /// </summary>
        /// <param name="codePersonne"></param>
        /// <param name="codeActivite"></param>
        public static void SupprimerTacheAnnexe(string codePersonne, string codeActivite)
        {
            // Récupération de la chaîne de connexion
            var connectString = Properties.Settings.Default.JobOverviewConnectionString;

            //Ecriture de la requête
            string req = @"delete jo.Tache where Annexe = 1 and Login=@log and CodeActivite=@codeAct";

            // Paramètres de la requête
            var paramLog = new SqlParameter("@log", DbType.String);
            paramLog.Value = codePersonne;
            var paramCodeAct = new SqlParameter("@codeAct", DbType.String);
            paramCodeAct.Value = codeActivite;

            // Création d'une connexion à partir de la chaîne de connexion
            using (var cnx = new SqlConnection(connectString))
            {
                // Ouverture de la connection
                cnx.Open();

                // Utilisation d'une transaction
                SqlTransaction tran = cnx.BeginTransaction();

                // Création de la commande et entrée des paramètres.
                var command = new SqlCommand(req, cnx, tran);
                command.Parameters.Add(paramLog);
                command.Parameters.Add(paramCodeAct);

                try
                {
                    command.ExecuteNonQuery();
                    // Validation de la transaction si tout se passe bien.
                    tran.Commit();
                }
                catch (SqlException ex)
                {
                    // Annulation de la transaction
                    tran.Rollback();

                    // Erreur liée au cas où du temps a été saisi sur la tâche à supprimer
                    if (ex.Number == 547)
                        throw new Exception("Suppression impossible : du temps est déjà saisi sur une des tâches à supprimer.", ex);
                }
            }
        }

        public static void SupprimerTacheProd(Guid id)
        {
            var connectString = Properties.Settings.Default.JobOverviewConnectionString;
            string queryString1 = @"delete from jo.TacheProd where IdTache=@id ";
            string queryString2 = @"delete from jo.Tache where IdTache=@id ";

            var param1 = new SqlParameter("@id", DbType.Guid);
            param1.Value = id;

            var param2 = new SqlParameter("@id", DbType.Guid);
            param2.Value = id;

            using (var connect = new SqlConnection(connectString))
            {
                connect.Open();
                SqlTransaction tran = connect.BeginTransaction();

                try
                {
                    var command1 = new SqlCommand(queryString1, connect, tran);
                    command1.Parameters.Add(param1);

                    var command2 = new SqlCommand(queryString2, connect, tran);
                    command2.Parameters.Add(param2);

                    command1.ExecuteNonQuery();
                    command2.ExecuteNonQuery();

                    tran.Commit();
                }
                catch (SqlException ex)
                {
                    tran.Rollback();
                    // Erreur liée au cas où du temps a été saisi sur la tâche à supprimer
                    if (ex.Number == 547)
                        throw new Exception("Suppression impossible : du temps est déjà saisi sur cette tâche.", ex);
                    else
                    throw;
                }
            }
        }

        /// <summary>
        /// Enregistre une liste de tâches de production dans la base
        /// </summary>
        /// <param name="listTaches"></param>
        public static void EnregistrerTachesProd(List<TacheApercu> listTaches)
        {
            string sqlConnectionString = Properties.Settings.Default.JobOverviewConnectionString;

            string req = @"Insert jo.Tache(IdTache, Libelle,  CodeActivite, Login, Description)                                                                                                 
                        select Id, Libelle,  Activite, Login, Description
                        from  @table";

            string req2 = @"Insert jo.TacheProd(IdTache, DureePrevue, DureeRestanteEstimee,
                        CodeModule, CodeLogicielModule, NumeroVersion, CodeLogicielVersion)
                                    select Id, DureePrevue, DureeRestante,
                                            Module, LogicielModule, NumeroVersion, Logicielversion

                                            from @table2";

            // Création du paramètre de type table mémoire
            // /!\ Le type TypeTablePersonne doit être créé au préalable dans la base
            var param = new SqlParameter("@table", SqlDbType.Structured);
            param.TypeName = "TypeTableTache";

            var param2 = new SqlParameter("@table2", SqlDbType.Structured);
            param2.TypeName = "TypeTableTacheProd";

            DataTable tableTaches; 
            DataTable tableTachesProd;

            RecupererDataTablePourTache(listTaches, out tableTaches, out tableTachesProd);
            param.Value = tableTaches;
            param2.Value = tableTachesProd;
            using (var cnx = new SqlConnection(sqlConnectionString))
            {
                // Ouverture de la connexion et début de la transaction
                cnx.Open();
                SqlTransaction tran = cnx.BeginTransaction();

                try
                {
                    // Création et exécution des commandes
                    var command = new SqlCommand(req, cnx, tran);
                    command.Parameters.Add(param);
                    command.ExecuteNonQuery();

                    var command2 = new SqlCommand(req2, cnx, tran);
                    command2.Parameters.Add(param2);
                    command2.ExecuteNonQuery();

                    // Validation de la transaction s'il n'y a pas eu d'erreur
                    tran.Commit();
                }
                catch (Exception)
                {
                    tran.Rollback(); // Annulation de la transaction en cas d'erreur
                    throw;   // Remontée de l'erreur à l'appelant
                }
            }
        }

        /// <summary>
        /// Création et remplissage d'une table mémoire à partir d'une liste de tâches de prod
        /// </summary>
        /// <param name="listTachesProd"></param>
        /// <returns></returns>
        private static DataTable RecupererDataTablePourTache(List<TacheApercu> listTachesProd, out DataTable table, out DataTable tableProd)
        {
            // Création de la table et de ses colonnes
             table = new DataTable();
            tableProd = new DataTable();

            var colIdTache = new DataColumn("Id", typeof(Guid));
            colIdTache.AllowDBNull = false;
            table.Columns.Add(colIdTache);

            var colLibelle = new DataColumn("Libelle", typeof(string));
            colLibelle.AllowDBNull = false;
            table.Columns.Add(colLibelle);

            var colCodeActivite = new DataColumn("Activite", typeof(string));
            colCodeActivite.AllowDBNull = false;
            table.Columns.Add(colCodeActivite);

            var colLogin = new DataColumn("Login", typeof(string));
            colLogin.AllowDBNull = false;
            table.Columns.Add(colLogin);

            var colDescription = new DataColumn("Description", typeof(string));
            table.Columns.Add(colDescription);

            //table Tableprod
            var colIdTacheprod = new DataColumn("Id", typeof(Guid));
            colIdTache.AllowDBNull = false;
            tableProd.Columns.Add(colIdTacheprod);

            var colDureePrevue = new DataColumn("DureePrevue", typeof(float));
            colDureePrevue.AllowDBNull = false;
            tableProd.Columns.Add(colDureePrevue);

            var colDureeRestanteEstimee = new DataColumn("DureeRestante", typeof(float));
            colDureeRestanteEstimee.AllowDBNull = false;
            tableProd.Columns.Add(colDureeRestanteEstimee);

            var colCodeModule = new DataColumn("Module", typeof(string));
            colCodeModule.AllowDBNull = false;
            tableProd.Columns.Add(colCodeModule);

            var colCodeLogicieModule = new DataColumn("LogicielModule", typeof(string));
            colCodeLogicieModule.AllowDBNull = false;
            tableProd.Columns.Add(colCodeLogicieModule);

            var colNumeroVersion = new DataColumn("NumeroVersion", typeof(float));
            colNumeroVersion.AllowDBNull = false;
            tableProd.Columns.Add(colNumeroVersion);

            var colCodeLogicielVersion = new DataColumn("Logicielversion", typeof(string));
            colCodeLogicielVersion.AllowDBNull = false;
            tableProd.Columns.Add(colCodeLogicielVersion);

            //var colAnnexe = new DataColumn("Annexe", typeof(bool));
            //colAnnexe.AllowDBNull = false;
            //tableProd.Columns.Add(colAnnexe);

            // Remplissage de la table
            foreach (var p in listTachesProd)
            {
                DataRow ligne = table.NewRow();
                DataRow ligneProd = tableProd.NewRow();
                Guid id = Guid.NewGuid();

                ligne["Id"] = id;
                ligne["Libelle"] = p.NomTache;
                ligne["Activite"] = p.CodeActivite;
                ligne["Login"] = p.Login;
                ligne["Description"] = p.Description;

                ligneProd["Id"] = id;
                ligneProd["DureePrevue"] = p.DureePrevue;
                ligneProd["DureeRestante"] = p.DureeRestante;
                ligneProd["Module"] = p.CodeModule;
                ligneProd["LogicielModule"] = p.CodeLogiciel;
                ligneProd["NumeroVersion"] = p.CodeVersion;
                ligneProd["Logicielversion"] = p.CodeLogiciel;
               // ligneProd["Annexe"] = false;


                table.Rows.Add(ligne);
                tableProd.Rows.Add(ligneProd);

            }
            return table;
        }

        //private static DataTable RecupererDataTablePourTacheProd(List<TacheApercu> listTachesProd)
        //{
        //    // Création de la table et de ses colonnes
        //    DataTable table = new DataTable();

        //    var colIdTache = new DataColumn("Id", typeof(Guid));
        //    colIdTache.AllowDBNull = false;
        //    table.Columns.Add(colIdTache);

        //    var colLibelle = new DataColumn("Libelle", typeof(string));
        //    colLibelle.AllowDBNull = false;
        //    table.Columns.Add(colLibelle);

        //    var colCodeActivite = new DataColumn("Activite", typeof(string));
        //    colCodeActivite.AllowDBNull = false;
        //    table.Columns.Add(colCodeActivite);

        //    var colLogin = new DataColumn("Login", typeof(string));
        //    colLogin.AllowDBNull = false;
        //    table.Columns.Add(colLogin);

        //    var colDescription = new DataColumn("Description", typeof(string));
        //    table.Columns.Add(colDescription);


        //    /*var colDureePrevue = new DataColumn("DureePrevue", typeof(float));
        //    colDureePrevue.AllowDBNull = false;
        //    table.Columns.Add(colDureePrevue);

        //    var colDureeRestanteEstimee = new DataColumn("DureeRestante", typeof(float));
        //    colDureeRestanteEstimee.AllowDBNull = false;
        //    table.Columns.Add(colDureeRestanteEstimee);

        //    var colCodeModule = new DataColumn("Module", typeof(string));
        //    colCodeModule.AllowDBNull = false;
        //    table.Columns.Add(colCodeModule);

        //    var colCodeLogicieModule = new DataColumn("LogicielModule", typeof(string));
        //    colCodeLogicieModule.AllowDBNull = false;
        //    table.Columns.Add(colCodeLogicieModule);

        //    var colNumeroVersion = new DataColumn("NumeroVersion", typeof(float));
        //    colNumeroVersion.AllowDBNull = false;
        //    table.Columns.Add(colNumeroVersion);

        //    var colCodeLogicielVersion = new DataColumn("Logicielversion", typeof(string));
        //    colCodeLogicielVersion.AllowDBNull = false;
        //    table.Columns.Add(colCodeLogicielVersion);

        //    var colAnnexe = new DataColumn("Annexe", typeof(bool));
        //    colAnnexe.AllowDBNull = false;
        //    table.Columns.Add(colAnnexe);
        //    */

        //    // Remplissage de la table
        //    foreach (var p in listTachesProd)
        //    {
        //        DataRow ligne = table.NewRow();
        //        ligne["Id"] = Guid.NewGuid();
        //        ligne["Libelle"] = p.NomTache;
        //        ligne["Activite"] = p.CodeActivite;
        //        ligne["Login"] = p.Login;
        //        ligne["Description"] = p.Description;

        //        /*ligne["DureePrevue"] = p.DureePrevue;
        //        ligne["DureeRestante"] = p.DureeRestante;
        //        ligne["Module"] = p.CodeModule;
        //        ligne["LogicielModule"] = p.CodeLogiciel;
        //        ligne["NumeroVersion"] = p.CodeVersion;
        //        ligne["Logicielversion"] = p.CodeLogiciel;
        //        ligne["Annexe"] = false;*/

        //        table.Rows.Add(ligne);

        //    }
        //    return table;
        //}
    }
}
