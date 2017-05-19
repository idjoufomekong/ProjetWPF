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
        public static List<Personne> RecupererPersonnesTaches()
        {
            List<Personne> listPers = new List<Entity.Personne>();

            var connectString = Properties.Settings.Default.JobOverviewConnectionString;
            string queryString = @"select Distinct P.Login, P.Prenom +' '+P.Nom NomComplet, T.IdTache, T.Libelle, 
                T.Description, TP.Numero, TP.DureeRestanteEstimee,TP.CodeLogicielVersion,TP.NumeroVersion,
                P.CodeMetier  
                from jo.Tache T
                inner join jo.TacheProd TP on TP.IdTache = T.IdTache
                right outer join jo.Personne P on T.Login = P.Login 
                where CodeEquipe = 'BIOH_DEV'
                order by 1";

            using (var connect = new SqlConnection(connectString))
            {
                var command = new SqlCommand(queryString, connect);
                connect.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        GetpersonneFromDataReader(listPersonne, reader);
                    }
                }
            }

            return listPers;
        }
    }
}
