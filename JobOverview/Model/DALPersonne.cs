using JobOverview.Entity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobOverview.Model
{
    public class DALPersonne
    {
        //Récupération des personnes et leurs tâches
        /// <summary>
        /// Sélection des memebres de l'équipe du manager connecté avec la liste des tâches et travaux
        /// pour tous les logiciels et toutes les versions!!!!!!!!!!!!!!!!!!!!!!! Manager: BNORMAND 
        /// </summary>
        /// <returns></returns>
        public static List<Personne> RecupererPersonnesTaches()
        {
            List<Personne> listPers = new List<Entity.Personne>();

            var connectString = Properties.Settings.Default.JobOverviewConnectionString;
            string queryStringPersonne = @"select P.Login, P.Prenom+' '+P.Nom NomComplet,P.CodeMetier,P.Manager,T.IdTache,T.Libelle,T.CodeActivite,
T.Description,
TP.Numero,TP.DureePrevue,TP.DureeRestanteEstimee,TP.CodeLogicielVersion,TP.CodeModule,TP.NumeroVersion,TR.DateTravail,TR.Heures,
TR.TauxProductivite
from jo.Personne P
left outer join jo.Tache T on T.Login=P.Login
left outer join jo.TacheProd TP on T.IdTache=TP.IdTache
left outer join jo.Travail TR on  TR.IdTache=T.IdTache
where Manager='BNORMAND' and Annexe=1
order by Numero";

            using (var connect = new SqlConnection(connectString))
            {
                //Récupération liste des tâches de production
                var command = new SqlCommand(queryStringPersonne, connect);
                connect.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        RecupererPersonneTachesProdFromDataReader(listPers, reader);
                    }
                }
                //Récupération de la liste des tâches annexes pour toutes les personnes de la liste remplie
                foreach(var p in listPers)
                {
                    RecupererTachesAnnexes(p.CodePersonne);
                }
            }

            return listPers;
        }

        private static void RecupererTachesAnnexes(string codePersonne)
        {
            throw new NotImplementedException();
        }

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

                if (!string.IsNullOrEmpty(reader["Numero"].ToString()))
                    pers.TachesProd = new List<TacheProd>();

                listPers.Add(pers);
            }
            else
            {
                pers = listPers.Last();
            }
            if (!string.IsNullOrEmpty(reader["Numero"].ToString()))
            {
                TacheProd tache = new TacheProd();
                tache.NumTache = (int)reader["Numero"];
                tache.NomTache = (string)reader["Libelle"];
                if (reader["Description"] != DBNull.Value)
                    tache.Description = (string)reader["Description"];
                tache.DureeRestante = (float)reader["DureeRestanteEstimee"];
                tache.DureePrevue = (float)reader["DureePrevue"];
                tache.CodeVersion = (float)reader["NumeroVersion"];
                tache.CodeLogiciel = (string)reader["CodeLogicielVersion"];

                pers.TachesProd.Add(tache);
            }
        }
    }
}
