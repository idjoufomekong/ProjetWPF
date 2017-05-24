using JobOverview.Entity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Collections.ObjectModel;

namespace JobOverview.Model
{
    public class DALPersonne
    {
        /// <summary>
        /// Récupération de la liste de toutes les personnes de la BDD
        /// avec leurs activités associées
        /// </summary>
        /// <returns>Liste de toutes les personnes</returns>
        public static List<Personne> RecupererToutesPersonne()
        {
            List<Personne> listPersonne = new List<Personne>();

            var connectString = Properties.Settings.Default.JobOverviewConnectionString;

            string queryString = @"select P.Login, P.Prenom+' '+P.Nom NomComplet,P.CodeMetier,A.CodeActivite, A.Libelle
                                 from jo.Personne P 
                                 inner join jo.Metier M on M.CodeMetier=P.CodeMetier
                                 inner join jo.ActiviteMetier AM on AM.MetierCodeMetier=M.CodeMetier
                                 inner join jo.Activite A on A.CodeActivite=AM.ActiviteCodeActivite
                                 order by 2,4";

            using (var connect = new SqlConnection(connectString))
            {
                var command = new SqlCommand(queryString, connect);
                connect.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var log = (string)reader["Login"];
                        Personne p = null;
                        if ((listPersonne.Count == 0) || (listPersonne.Last().CodePersonne != log))
                        {
                            p = new Personne();
                            p.NomPrenom = reader["NomComplet"].ToString();
                            p.CodePersonne = reader["Login"].ToString();
                            p.Activites = new List<Activite>();
                            listPersonne.Add(p);
                        }
                        p = listPersonne.Last();

                        //Chargement de la liste d'activités
                        Activite act = new Activite();
                        act.CodeActivite = reader["CodeActivite"].ToString();
                        act.NomActivite = reader["Libelle"].ToString();
                    }
                }
            }
            return listPersonne;
        }

        /// <summary>
        /// Récupération de la liste d'une personnes ou d'une équipe de la BDD
        /// avec leurs activités associées
        /// </summary>
        /// <returns>Liste des personnes</returns>
        /// <param name="manager">Code personne</param>
        public static List<Personne> RecupererToutesPersonneActivite(string manager)
        {
            List<Personne> listPersonne = new List<Personne>();

            var connectString = Properties.Settings.Default.JobOverviewConnectionString;

            string queryString = @"select P.Login, P.Prenom+' '+P.Nom NomComplet,P.CodeMetier,A.CodeActivite, A.Libelle
                                 from jo.Personne P 
                                 inner join jo.Metier M on M.CodeMetier=P.CodeMetier
                                 inner join jo.ActiviteMetier AM on AM.MetierCodeMetier=M.CodeMetier
                                 inner join jo.Activite A on A.CodeActivite=AM.ActiviteCodeActivite
                                 where Manager=@log and Login=@log and Annexe=0
                                 order by 2,4";
            var log = new SqlParameter("@log", DbType.String);
            log.Value = manager;

            using (var connect = new SqlConnection(connectString))
            {
                var command = new SqlCommand(queryString, connect);
                command.Parameters.Add(log);
                connect.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var login = (string)reader["Login"];
                        Personne p = null;
                        if ((listPersonne.Count == 0) || (listPersonne.Last().CodePersonne != login))
                        {
                            p = new Personne();
                            p.NomPrenom = reader["NomComplet"].ToString();
                            p.CodePersonne = reader["Login"].ToString();
                            p.Activites = new List<Activite>();
                            listPersonne.Add(p);
                        }
                        p = listPersonne.Last();

                        //Chargement de la liste d'activités
                        Activite act = new Activite();
                        act.CodeActivite = reader["CodeActivite"].ToString();
                        act.NomActivite = reader["Libelle"].ToString();
                        p.Activites.Add(act);
                    }
                }
            }
            return listPersonne;
        }

        /// <summary>
        /// Récupération de la liste de personne liée à la personne connecté
        /// si non manager 1 personne récupérée, sinon toute l'équipe
        /// </summary>
        /// <param name="personneConnecte"></param>
        /// <returns>Liste de personne</returns>
        public static List<Personne> RecupererPersonneConnecte(string personneConnecte)
        {
            List<Personne> listPersonne = new List<Personne>();

            var connectString = Properties.Settings.Default.JobOverviewConnectionString;

            string queryString = @"select Login, Prenom+' '+Nom NomComplet from jo.Personne
                                    where login = @log or Manager=@log
                                    order by 2";

            var log = new SqlParameter("@log", DbType.String);
            log.Value = personneConnecte;

            using (var connect = new SqlConnection(connectString))
            {
                var command = new SqlCommand(queryString, connect);
                command.Parameters.Add(log);
                connect.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Personne p = new Personne();
                        p.NomPrenom = reader["NomComplet"].ToString();
                        p.CodePersonne = reader["Login"].ToString();
                        listPersonne.Add(p);
                    }
                }
                if (listPersonne.Count > 1)
                    listPersonne.Where(p => p.CodePersonne == personneConnecte).FirstOrDefault().Manager = true;
            }
            return listPersonne;
        }
    }
}
