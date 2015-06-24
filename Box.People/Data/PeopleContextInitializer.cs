using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace Box.People.Data {

    public class PeopleContextInitializer : CreateDatabaseIfNotExists<PeopleContext> {

        protected override void Seed(PeopleContext context){
            context.Database.ExecuteSqlCommand("SELECT * INTO __MigrationHistory_People FROM __MigrationHistory");
            context.Database.ExecuteSqlCommand("DROP TABLE __MigrationHistory");
        }
    }
}
