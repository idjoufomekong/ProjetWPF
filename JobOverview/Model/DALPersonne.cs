﻿using JobOverview.Entity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace JobOverview.Model
{
    public class DALPersonne
    {
        //Récupération des personnes et leurs tâches
        /// <summary>
        /// Sélection des membres de l'équipe du manager connecté avec la liste des tâches et travaux
        /// pour tous les logiciels et toutes les versions
        /// !!!!!!!!!!!!!!!!!!!!!!! Prend les personnes sans tâches 
        /// </summary>
        /// <returns></returns>
        public static List<Personne> RecupererPersonnesTaches(string codeLogiciel, float version, string codeManager)
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
            where Manager=@manager OR P.Login=@manager";
            //TODO Bien définir quelles tâches on veut exporter afin de peaufiner la requête

            //var codeLog = new SqlParameter("@codeLogiciel", DbType.String);
            //var numVersion = new SqlParameter("@numVersion", DbType.Double);
            //codeLog.Value = codeLogiciel;
            //numVersion.Value = version;
            var codeMng = new SqlParameter("@manager", DbType.Double);
            codeMng.Value = codeManager;

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
                    tache.Description = (string)reader["Description"];

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

                pers.TachesProd = new List<TacheProd>();
                pers.TachesAnnexes = new List<Tache>();

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

        
    }
}
