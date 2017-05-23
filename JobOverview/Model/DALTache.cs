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
            foreach(var emp in listPers)
            {
                foreach(var act in listAnnexes)
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
    }
}
