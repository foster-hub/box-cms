using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Box.Core.Services;


namespace Box.People.Services {

    [Export]
    public class PeopleService {

        public IEnumerable<Person> GetPeople(string filter, int skip, int top, string optin, string group) {

            using (var context = new Data.PeopleContext()) {
                IQueryable<Person> people = null;
                people = context.Person;

                if (!String.IsNullOrEmpty(filter)) {
                    string[] tags = filter.ToLower().Split(new char[] { ' ' });
                    people = people.Where(p => tags.All(t => p.Email.ToLower().Contains(t) || p.PersonalId1.Contains(t) || p.PersonalId2.Contains(t) || p.Name.Contains(t)));
                }

                people = FilterOptin(people, optin).OrderBy(p => p.Name);

                if (group != "All" && group != null)
                    people = people.Where(p => p.GroupName == group);

                if (skip != 0)
                    people = people.Skip(skip);

                if (top != 0)
                    people = people.Take(top);


                return people.ToArray();
            }
        }

        public Person GetPersonByEmail(string email) {
            using (var context = new Data.PeopleContext()) {
                return context.Person.Where(p => p.Email == email).SingleOrDefault();
            }
        }

        public IEnumerable<string> GetGroups(int skip, int top) {

            using (var context = new Data.PeopleContext()) {
                IQueryable<string> groups = context.Person.Where(p => p.GroupName != null).GroupBy(p => p.GroupName).OrderBy(p => p.FirstOrDefault().GroupName).Select(p => p.FirstOrDefault().GroupName);

                if (skip != 0)
                    groups = groups.Skip(skip);

                if (top != 0)
                    groups = groups.Take(top);

                return groups.ToArray();
            }
        }

        private IQueryable<Person> FilterOptin(IQueryable<Person> people, string optin) {
            if (optin == "All")
                return people;
            if (optin == "Email")
                return people.Where(p => p.EmailOptIn == true);
            if (optin == "Sms")
                return people.Where(p => p.SMSOptIn == true);
            else
                return people.Where(p => p.MailOptIn == true);
        }

        public void SavePerson(Person person) {
            using (Data.PeopleContext context = new Data.PeopleContext()) {

                Person oldPerson = context.Person.SingleOrDefault(p => p.PersonUId == person.PersonUId);

                if (oldPerson != null) {
                    context.Person.Attach(oldPerson);
                    context.Entry<Person>(oldPerson).CurrentValues.SetValues(person);
                }
                else
                    context.Person.Add(person);

                context.SaveChanges();
            }
        }

        public Person GetPerson(string personUId) {
            using (Data.PeopleContext context = new Data.PeopleContext()) {
                return context.Person.SingleOrDefault(p => p.PersonUId == personUId);
            }
        }

        private short CalcPct(int i, int total) {
            if (total == 0)
                return 0;
            return Convert.ToInt16(((double)i / total) * 100);
        }

        public void Export(AsyncTaskService taskService, string id, string filter, string optin = "All", string group = "All") {
            // get people
            if (taskService.UpdateTask(id, "Getting people...", 0) == null)
                return;

            var people = GetPeople(filter, 0, 0, optin, group);

            // zip it
            int i = 0;
            int total = people.Count();
           
            taskService.Write(id, "Exporting people...",  CalcPct(i, total), GetCSVHeader() + "\n");

            foreach (var person in people) {
                taskService.Write(id, "Exporting people...",  CalcPct(i, total), person.ToString() + "\n");
                i++;
            }

            taskService.UpdateTask(id, "Ready for download", 100);
            taskService.FinishTask(id);
  
        }

        public bool IsEmailAlreadyRegistred(string email) {
            using (Data.PeopleContext context = new Data.PeopleContext()) {
                if (context.Person.Any(p => p.Email == email))
                    return true;
                return false;
            }
        }

        private string GetCSVHeader() {
            return
                "\"" + "PersonUId" + "\";" +
                "\"" + "Name" + "\";" +
                "\"" + "Surename" + "\";" +
                "\"" + "Title" + "\";" +
                "\"" + "Email" + "\";" +
                "\"" + "GroupName" + "\";" +
                "\"" + "Birthday" + "\";" +
                "\"" + "Gender" + "\";" +
                "\"" + "Job" + "\";" +
                "\"" + "PersonalId1" + "\";" +
                "\"" + "PersonalId2" + "\";" +
                "\"" + "Phone1" + "\";" +
                "\"" + "Phone2" + "\";" +
                "\"" + "Phone3" + "\";" +
                "\"" + "Address1" + "\";" +
                "\"" + "Address2" + "\";" +
                "\"" + "Company" + "\";" +
                "\"" + "Aceita receber email" + "\";" +
                "\"" + "EmailOptOutDate" + "\";" +
                "\"" + "Aceita receber correspondencia" + "\";" +
                "\"" + "MailOptOutDate" + "\";" +
                "\"" + "MarketField" + "\";" +
                "\"" + "RegisterDate" + "\";" +
                "\"" + "RegisterSource" + "\";" +
                "\"" + "Role" + "\";" +
                "\"" + "Aceita receber SMS" + "\";" +
                "\"" + "SMSOptOutDate" + "\";" +
                "\"" + "State" + "\";" +
                "\"" + "ZipCode" + "\";" +
                "\"" + "Country" + "\";" +
                "\"" + "City" + "\";";
        }
    }
}
