using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace Box.CMS.Data {


    public class CMSContextInitializer : CreateDatabaseIfNotExists<CMSContext>  {

        protected override void Seed(CMSContext context) {
           
            //context.Database.ExecuteSqlCommand("SELECT * INTO __MigrationHistory_CMS FROM __MigrationHistory");
            context.Database.ExecuteSqlCommand("DROP TABLE __MigrationHistory");

            SeedSample(context);

        }

        private void SeedSample(CMSContext context) {

            if (System.Web.HttpContext.Current == null)
                return;

            try {
                string line = null;
                string path = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/SampleData.sql");
                System.IO.StreamReader file = new System.IO.StreamReader(path);
                
                while ((line = file.ReadLine()) != null) {
                    if (line != "GO")
                        context.Database.ExecuteSqlCommand(line);
                }
                file.Close();
            } catch (Exception ex) {
                string a = ex.Message;
            }
        }
    }
}
