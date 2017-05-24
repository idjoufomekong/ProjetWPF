using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JobOverview.Entity;
using System.Collections.Generic;
using JobOverview.Model;
using System.Linq;

namespace UnitTestJobOverView
{
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// On veut s'assurer qu'on a bien récupéré toutes les tâches de production de RBEAUMONT
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {
            List<Personne> _personnes = DALTache.RecupererPersonnesTaches("GENOMICA");
            var p = _personnes.Where(x => x.CodePersonne == "RBEAUMONT").FirstOrDefault();
            Assert.AreEqual(9, p.TachesProd.Count());

//            Récupérer la valeur du test avec la requête suivante
//                select*
//from jo.Tache T
//inner join jo.TacheProd TP on TP.IdTache = T.IdTache
//where Annexe = 0 and Login = 'RBEAUMONT' and NumeroVersion = 1
        }
    }
}
