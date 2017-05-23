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
                //Récupération liste des tâches de production
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
                        Activite  act = new Activite();
                        act.CodeActivite= reader["CodeActivite"].ToString();
                        act.NomActivite= reader["Libelle"].ToString();
                    }
                }
            }

            return listPersonne;
        }

        public static List<Personne> RecupererToutesPersonnebis()
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
                //Récupération liste des tâches de production
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
                //Récupération liste des tâches de production
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

        public static void SauvegardePropriete(Personne connecte)
        {
            Properties.Settings.Default.CodeDernierUtilisateur = connecte.CodePersonne;
            Properties.Settings.Default.Manager = connecte.Manager;
            Properties.Settings.Default.Save();
        }


    }
}
