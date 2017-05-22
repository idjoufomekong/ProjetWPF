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
        public static ObservableCollection<Personne> RecupererToutesPersonne()
        {
            ObservableCollection<Personne> listPersonne = new ObservableCollection<Personne>();

            var connectString = Properties.Settings.Default.JobOverviewConnectionString;

            string queryString = @"select Login, Prenom+' '+Nom NomComplet from jo.Personne";

            using (var connect = new SqlConnection(connectString))
            {
                //Récupération liste des tâches de production
                var command = new SqlCommand(queryString, connect);
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
            }

            return listPersonne;
        }

        public static ObservableCollection<Personne> RecupererPersonneConnecte(string personneConnecte)
        {
            ObservableCollection<Personne> listPersonne = new ObservableCollection<Personne>();

            var connectString = Properties.Settings.Default.JobOverviewConnectionString;

            string queryString = @"select Login, Prenom+' '+Nom NomComplet from jo.Personne
                                    where login = @log or Manager=@log";
            var log = new SqlParameter("@log", DbType.String);

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
            SauvegardePropriete(listPersonne.Where(c => c.CodePersonne == personneConnecte).FirstOrDefault());
            return listPersonne;
        }

        private static void SauvegardePropriete(Personne connecte)
        {
            Properties.Settings.Default.CodeDernierUtilisateur = connecte.CodePersonne;
            Properties.Settings.Default.Manager = connecte.Manager;
            Properties.Settings.Default.NomDernierUtilisateur = connecte.NomPrenom;
        }


    }
}
