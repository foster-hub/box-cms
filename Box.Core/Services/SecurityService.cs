using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box.Composition;

using System.ComponentModel.Composition;
using System.Threading;
using System.Web.Http;
using System.Configuration;
using System.Web.Mvc;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.Configuration;
using System.Text.RegularExpressions;

namespace Box.Core.Services {

    [Export]
    [Export(typeof(Box.Composition.Services.IAuthorizationService))]
    public class SecurityService : Box.Composition.Services.IAuthorizationService {

        [ImportMany]
        private IUserGroup[] allGroups { get; set; }

        public static string ADMIN_GROUP_ID = "ADMIN----167e-42a3-abb2-9e3f7ba2074d";

        [Import]
        private LogService log { get; set; }

        public static bool IsDebug
        {
            get
            {
                //CompilationSection compilationSection = System.Configuration.ConfigurationManager.GetSection(@"system.web/compilation") as CompilationSection;
                //if (compilationSection == null)
                //    return false;
                //return compilationSection.Debug;
                object isOn = ConfigurationManager.AppSettings["BOX_DEBUG_ON"];
                if (isOn == null)
                    return false;

                bool isOnBool = false;
                Boolean.TryParse(isOn.ToString(), out isOnBool);

                return isOnBool;
            }
        }

        private bool IsRazorEngineOn {
            get {
                object isOn = ConfigurationManager.AppSettings["RAZOR_ENGINE_ON"];
                if (isOn == null)
                    return true;

                bool isOnBool = true;
                Boolean.TryParse(isOn.ToString(), out isOnBool);

                return isOnBool;
            }
        }

        public bool IsFormsAuthEnable {
            get {
                object isOn = ConfigurationManager.AppSettings["FORMS_AUTH_ENABLE"];
                if (isOn == null)
                    return true;

                bool isOnBool = true;
                Boolean.TryParse(isOn.ToString(), out isOnBool);

                return isOnBool;
            }
        }

        public bool IsWindowsAuthEnable {
            get {
                object isOn = ConfigurationManager.AppSettings["WINDOWS_AUTH_ENABLE"];
                if (isOn == null)
                    return false;

                bool isOnBool = true;
                Boolean.TryParse(isOn.ToString(), out isOnBool);

                return isOnBool;
            }
        }

        public string WindowsAuthUrl {
            get {
                var url = ConfigurationManager.AppSettings["WINDOWS_AUTH_URL"];
                if (url == null)
                    return "/adm-nt";


                return url;
            }
        }

        public int MaxErrorCount {
            get {
                object maxError = ConfigurationManager.AppSettings["MAX_ERROR_COUNT"];

                int maxErrorCount = 0;

                if (maxError == null)
                    return maxErrorCount;

                int.TryParse(maxError.ToString(), out maxErrorCount);

                return maxErrorCount;
            }
        }


        /// <summary>
        /// Returns all users the have a give role.
        /// </summary>
        /// <param name="role">Role</param>
        /// <returns>Users that have a given role</returns>
        public IEnumerable<User> GetUsersWithRole(string role)
        {
            return GetUsersWithRoles(new string[] { role });
        }

        /// <summary>
        /// Returns all users the have at least one of the given roles.
        /// </summary>
        /// <param name="roles">Roles</param>
        /// <returns>Users that have at least one of the given roles</returns>
        public IEnumerable<User> GetUsersWithRoles(string[] roles)
        {
            using (var context = new Data.CoreContext())
            {
                return context.Users.Where(u => u.Memberships.Any(m => roles.Contains(m.UserGroupUId))
                || u.GroupCollectionMemberships.Any(g => g.Collection.CollectionGroups.Any(g2 => roles.Contains(g2.UserGroupUId)))).ToArray();
            }
        }

        public IEnumerable<User> GetUsers(ref int totalRecords, string filter = null, int skip = 0, int top = 0)
        {

            using (var context = new Data.CoreContext())
            {
                IQueryable<User> users = context.Users;
                if (!String.IsNullOrEmpty(filter))
                {
                    string[] tags = filter.ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    string[] groupIds = GetMatchedUserGroupIds(tags);

                    users = users.Where(u => tags.All(t => u.Email.ToLower().Contains(t) || u.Name.ToLower().Contains(t) || u.Memberships.Any(m => groupIds.Contains(m.UserGroupUId))));

                }


                users = users.OrderBy(u => u.Name);

                // total records for pagination
                totalRecords = users.Count();

                if (skip != 0)
                    users = users.Skip(skip);

                if (top != 0)
                    users = users.Take(top);

                return users.ToArray();
            }
        }

        public IEnumerable<User> GetUsers(string filter = null, int skip = 0, int top = 0) {

            using (var context = new Data.CoreContext()) {
                IQueryable<User> users = context.Users;
                if (!String.IsNullOrEmpty(filter)) {
                    string[] tags = filter.ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    string[] groupIds = GetMatchedUserGroupIds(tags);

                    users = users.Where(u => tags.All(t => u.Email.ToLower().Contains(t) || u.Name.ToLower().Contains(t) || u.Memberships.Any(m => groupIds.Contains(m.UserGroupUId))));

                }


                users = users.OrderBy(u => u.Name);

                if (skip != 0)
                    users = users.Skip(skip);

                if (top != 0)
                    users = users.Take(top);

                return users.ToArray();
            }
        }

        public GroupCollection GetGroupCollection(string groupCollectionUId) {
            using (var context = new Data.CoreContext())
                return context.GroupCollections.Include("CollectionGroups").SingleOrDefault(g => g.GroupCollectionUId == groupCollectionUId);
        }

        public GroupCollection SaveGroupCollection(GroupCollection groupCollection) {
            using (var context = new Data.CoreContext()) {
                GroupCollection oldGroupCollection = GetGroupCollection(groupCollection.GroupCollectionUId);

                List<GroupCollection_Group> removedGroups = new List<GroupCollection_Group>();
                List<GroupCollection_Group> addedGroups = groupCollection.CollectionGroups.ToList();

                if (oldGroupCollection == null)
                    context.GroupCollections.Add(groupCollection);
                else {
                    removedGroups = oldGroupCollection.CollectionGroups.Where(o => !groupCollection.CollectionGroups.Any(u => u.UserGroupUId == o.UserGroupUId)).ToList();
                    addedGroups = groupCollection.CollectionGroups.Where(u => !oldGroupCollection.CollectionGroups.Any(o => o.UserGroupUId == u.UserGroupUId)).ToList();

                    context.GroupCollections.Attach(oldGroupCollection);
                    context.Entry<GroupCollection>(oldGroupCollection).CurrentValues.SetValues(groupCollection);
                }

                foreach (GroupCollection_Group m in removedGroups)
                    context.GroupCollection_Groups.Remove(m);

                foreach (GroupCollection_Group m in addedGroups) {
                    m.GroupCollectionUId = groupCollection.GroupCollectionUId;
                    context.GroupCollection_Groups.Add(m);
                }

                context.SaveChanges();

                return groupCollection;
            }
        }

        private string[] GetMatchedUserGroupIds(string[] tags) {
            return allGroups.Where(g => tags.Any(t => g.Name.ToLower().Contains(t))).Select(g => g.UserGroupUId).ToArray<string>();
        }

        public User GetUser(string email) {
            using (var context = new Data.CoreContext()) {
                return context.Users.Include("Memberships").Include("GroupCollectionMemberships.Collection.CollectionGroups").Include("Password").SingleOrDefault(u => u.Email.ToLower() == email.ToLower());
            }
        }

        public User GetUserByLoginNT(string loginNt) {
            using (var context = new Data.CoreContext()) {
                if (loginNt == null)
                    return null;
                return context.Users.SingleOrDefault(u => u.LoginNT.ToLower() == loginNt.ToLower());
            }
        }

        public User SaveUser(User user) {

            using (var context = new Data.CoreContext()) {

                CheckUniqueLoginNT(user);

                // gets the old version of user                
                User oldUser = GetUser(user.Email);

                List<GroupMembership> removedGroups = new List<GroupMembership>();
                List<GroupMembership> addedGroups = user.Memberships.ToList();

                List<GroupCollectionMembership> removedGroupCollection = new List<GroupCollectionMembership>();
                List<GroupCollectionMembership> addedGroupCollection = user.GroupCollectionMemberships.ToList();

                if (user.Password != null) {
                    user.Password.Password = Crypt(user.Password.Password);
                    user.Password.Email = user.Email;
                }

                if (String.IsNullOrWhiteSpace(user.LoginNT))
                    user.LoginNT = null;

                user.LoginErrorsCount = 0;

                if (oldUser != null) {

                    //Groups
                    removedGroups = oldUser.Memberships.Where(o => !user.Memberships.Any(u => u.UserGroupUId == o.UserGroupUId)).ToList();
                    addedGroups = user.Memberships.Where(u => !oldUser.Memberships.Any(o => o.UserGroupUId == u.UserGroupUId)).ToList();

                    //GroupsCollection
                    removedGroupCollection = oldUser.GroupCollectionMemberships.Where(o => !user.GroupCollectionMemberships.Any(u => u.GroupCollectionUId == o.GroupCollectionUId)).ToList();
                    addedGroupCollection = user.GroupCollectionMemberships.Where(u => !oldUser.GroupCollectionMemberships.Any(o => o.GroupCollectionUId == u.GroupCollectionUId)).ToList();

                    context.Users.Attach(oldUser);
                    context.Entry<User>(oldUser).CurrentValues.SetValues(user);

                    // if the password was changed
                    if (oldUser.Password != null && user.Password != null)
                        oldUser.Password.Password = user.Password.Password;

                    // ig it has no password before, and now it was added
                    if (oldUser.Password == null && user.Password != null) {
                        context.UserPasswords.Add(user.Password);
                    }
                } else {
                    context.Users.Add(user);
                }

                foreach (GroupMembership m in removedGroups)
                    context.GroupMemberships.Remove(m);

                foreach (GroupMembership m in addedGroups) {
                    m.Email = user.Email;
                    context.GroupMemberships.Add(m);
                }

                foreach (GroupCollectionMembership m in removedGroupCollection)
                    context.GroupCollectionMemberships.Remove(m);

                foreach (GroupCollectionMembership m in addedGroupCollection) {
                    m.Email = user.Email;
                    context.GroupCollectionMemberships.Add(m);
                }


                context.SaveChanges();
            }
            return user;
        }


        public void RemoveGroupCollection(string groupCollectionUId) {
            using (var context = new Data.CoreContext()) {
                GroupCollection groupCollection = GetGroupCollection(groupCollectionUId);
                if (groupCollection == null)
                    return;

                context.GroupCollections.Attach(groupCollection);
                context.GroupCollections.Remove(groupCollection);
                context.SaveChanges();
            }
        }

        public void RemoveUser(string email) {
            using (var context = new Data.CoreContext()) {
                User user = GetUser(email);
                if (user == null)
                    return;
                context.Users.Attach(user);
                context.Users.Remove(user);
                context.SaveChanges();
            }
        }

        public IEnumerable<IUserGroup> GetAllGroups() {
            return allGroups;
        }

        public IEnumerable<GroupCollection> GetAllGroupCollection(ref int totalRecords, string filter)
        {
            using (var context = new Data.CoreContext())
            {

                IQueryable<GroupCollection> groupCollection = context.GroupCollections.Include("CollectionGroups");

                if (!String.IsNullOrEmpty(filter))
                {
                    string[] tags = filter.ToLower().Split(new char[] { ' ' });
                    string[] groupIds = GetMatchedUserGroupIds(tags);
                    groupCollection = groupCollection.Where(g => tags.All(t => g.Name.ToLower().Contains(t) || g.CollectionGroups.Any(m => groupIds.Contains(m.UserGroupUId))));
                }

                // total records for pagination
                totalRecords = groupCollection.Count();

                return groupCollection.ToArray();
            }
        }

        public IEnumerable<GroupCollection> GetAllGroupCollection(string filter) {
            using (var context = new Data.CoreContext()) {

                IQueryable<GroupCollection> groupCollection = context.GroupCollections.Include("CollectionGroups");

                if (!String.IsNullOrEmpty(filter)) {
                    string[] tags = filter.ToLower().Split(new char[] { ' ' });
                    string[] groupIds = GetMatchedUserGroupIds(tags);
                    groupCollection = groupCollection.Where(g => tags.All(t => g.Name.ToLower().Contains(t) || g.CollectionGroups.Any(m => groupIds.Contains(m.UserGroupUId))));
                }

                return groupCollection.ToArray();
            }
        }

        public IEnumerable<IUserGroup> GetGroupsFromUser(string fromUser) {
            GroupMembership[] memberships;

            using (var context = new Data.CoreContext())
                memberships = context.GroupMemberships.Where(m => m.Email == fromUser).ToArray();

            return allGroups.Where(g => memberships.Any(m => m.UserGroupUId == g.UserGroupUId));
        }

        public IEnumerable<IUserGroup> GetGroupsFromGroupCollection(string fromGroupCollection) {
            GroupCollection_Group[] memberships;

            using (var context = new Data.CoreContext())
                memberships = context.GroupCollection_Groups.Where(m => m.GroupCollectionUId == fromGroupCollection).ToArray();

            return allGroups.Where(g => memberships.Any(m => m.UserGroupUId == g.UserGroupUId));
        }

        public IEnumerable<GroupCollection> GetGroupCollectionFromUser(string fromUser) {
            List<GroupCollectionMembership> groupCollectionMemberships;
            using (var context = new Data.CoreContext()) {
                groupCollectionMemberships = context.GroupCollectionMemberships.Where(m => m.Email == fromUser).ToList();
                List<GroupCollection> groupCollection = context.GroupCollections.ToList();
                return groupCollection.Where(g => groupCollectionMemberships.Any(m => m.GroupCollectionUId == g.GroupCollectionUId));
            }
        }

        private void CreateBoxToken(string email) {

            UserToken token = null;
            using (var context = new Data.CoreContext()) {
                token = context.UserTokens.Add(new UserToken());
                token.Email = email;
                token.Token = Guid.NewGuid().ToString();
                token.IssueDate = DateTime.Now;
                context.SaveChanges();
            }

            CleanOldTokens();

            // create encryption cookie
            //System.Web.Security.FormsAuthenticationTicket authTicket = new System.Web.Security.FormsAuthenticationTicket(
            //        1,
            //        token.Token,
            //        DateTime.Now,
            //        DateTime.Now.AddDays(1),
            //        true,
            //        "");

            //// add cookie to response stream
            //string encryptedTicket = System.Web.Security.FormsAuthentication.Encrypt(authTicket);

            //System.Web.HttpCookie authCookie = new System.Web.HttpCookie(System.Web.Security.FormsAuthentication.FormsCookieName, encryptedTicket);
            //System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);


            System.Web.Security.FormsAuthentication.SetAuthCookie(token.Token, true);

        }

        public string Crypt(string text) {

            if (String.IsNullOrEmpty(text))
                return text;

            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(text);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs) {
                s.Append(b.ToString("x2").ToLower());
            }
            return s.ToString();
        }

        public int SignInUser(string email) {

            User user = GetUser(email);

            if (user == null)
                return 0;

            if (user.Blocked)
                return -1;

            CreateBoxToken(email);

            return 1;
        }

        public int SignInUserNT(string loginNt) {

            if (String.IsNullOrWhiteSpace(loginNt))
                return 0;

            User user = GetUserByLoginNT(loginNt);

            if (user == null)
                return 0;

            if (user.Blocked)
                return -1;

            CreateBoxToken(user.Email);

            if(log==null)
            {
                log = new LogService();
            }
            log.Log(loginNt + " verified");
            
            return 1;
        }


        private void CleanOldTokens() {
            using (var context = new Data.CoreContext()) {
                DateTime oneWeekAgo = DateTime.Today.AddDays(-7);
                UserToken[] oldTokens = context.UserTokens.Where(t => t.IssueDate < oneWeekAgo).ToArray();
                foreach (UserToken token in oldTokens)
                    context.UserTokens.Remove(token);
                context.SaveChanges();
            }
        }

        private void UpdateLoginErrorsCount(string email) {
            using (var context = new Data.CoreContext()) {
                User user = context.Users.SingleOrDefault(u => u.Email == email);
                if (user == null)
                    return;
                user.LoginErrorsCount++;
                if (user.LoginErrorsCount > MaxErrorCount && MaxErrorCount > 0 && MaxErrorCount != null)
                    user.Blocked = true;

                context.SaveChanges();
            }
        }

        private void ResetLoginErrorsCount(string email) {
            using (var context = new Data.CoreContext()) {
                User user = context.Users.SingleOrDefault(u => u.Email == email);
                if (user == null)
                    return;
                user.LoginErrorsCount = 0;
                user.Blocked = false;
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Sign in user using forms auhtentication.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public int SignInUser(string email, string password) {

            // if the form authenticaton is not enable, cant sign in with email + password
            if (!IsFormsAuthEnable)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            User user = GetUser(email);

            // NO USER
            if (user == null || user.Password == null || String.IsNullOrEmpty(user.Password.Password))
                return 0;

            // WAS BLOCKED
            if (user.Blocked)
                return -1;

            // INVALID PASSWORD
            if (user.Password.Password != Crypt(password)) {
                UpdateLoginErrorsCount(email);
                return 0;
            }

            // SUCCESS
            ResetLoginErrorsCount(email);
            CreateBoxToken(email);

            return 1;
        }

        public void SignOutUser() {

            string token = System.Threading.Thread.CurrentPrincipal.Identity.Name;
            if (String.IsNullOrEmpty(token))
                return;

            using (var context = new Data.CoreContext()) {
                UserToken oldToken = context.UserTokens.SingleOrDefault(t => t.Token == token);
                if (oldToken != null) {
                    context.UserTokens.Remove(oldToken);
                    context.SaveChanges();
                }
            }

            System.Web.Security.FormsAuthentication.SignOut();
        }
        public string GetSignedUserToken() {
            return System.Web.HttpContext.Current.User.Identity.Name;
        }

        public User GetSignedUser() {
            return GetUserByAuthToken(System.Web.HttpContext.Current.User.Identity.Name);
        }
        
        public bool ValidatePassword(string password) {
            // Password must be equal or more than 8 characters, and must include at least one upper case letter, one lower case letter, and one numeric digit."
            string patternPassword = @"^(?=.*?[A-Z])(?=(.*[a-z]){1,})(?=(.*[\d]){1,})(?=(.*[\W]){1,})(?!.*\s).{8,}$";
            if (!string.IsNullOrEmpty(password)) {
                if (!Regex.IsMatch(password, patternPassword)) {
                    return false;
                }

            }
            return true;
        }

        public User GetUserByAuthToken(string token) {
            UserToken authToken = null;

            using (var context = new Data.CoreContext()) {
                DateTime lastDay = DateTime.Today.AddDays(-1);
                authToken = context.UserTokens.SingleOrDefault(t => t.Token == token && t.IssueDate >= lastDay);
            }

            if (authToken == null)
                return null;

            return GetUser(authToken.Email);
        }

        public UserToken GetAuthTokenByLoginNT(string loginNT) {
            UserToken authToken = null;

            using (var context = new Data.CoreContext()) {
                DateTime lastDay = DateTime.Today.AddDays(-1);
                string email = GetUserByLoginNT(loginNT).Email;
                authToken = context.UserTokens.OrderByDescending(d => d.IssueDate).FirstOrDefault(t => t.Email.ToLower() == email.ToLower() && t.IssueDate >= lastDay);
            }

            if (authToken == null)
                return null;

            return authToken;
        }

        private string[] RequestTypes = { ".js", ".css",  ".png", ".gif", ".jpg" };
                

        public void AuthenticateRequestPrincipal() {

            
            HttpContext context = HttpContext.Current;

            if (context == null)
            {                                
                return;
            }

            // skip those types
            string requestType = context.Request.CurrentExecutionFilePathExtension.ToLower();
            
            if (RequestTypes.Contains(requestType) || !context.Request.IsAuthenticated || !(context.User.Identity is FormsIdentity))
                return;

            string token = context.User.Identity.Name;

            User user = GetUserByAuthToken(token);

            if (user == null) {            
                SignOutUser();
                return;
            }

            string[] rolesList = GetUserRoles(user);
            
            Thread.CurrentPrincipal = new BoxPrincipal(Thread.CurrentPrincipal.Identity, rolesList, user.Name, user.Email);
            context.User = Thread.CurrentPrincipal;
            
        }


        public string[] GetUserRoles(User user) {

            List<string> rolesList = new List<string>();
            if (user.Memberships != null)
                rolesList.AddRange(user.Memberships.Select(m => m.UserGroupUId).ToArray<string>());

            if (user.GroupCollectionMemberships != null) {
                foreach (GroupCollectionMembership membership in user.GroupCollectionMemberships)
                    if (membership.Collection != null && membership.Collection.CollectionGroups != null)
                        rolesList.AddRange(membership.Collection.CollectionGroups.Select(g => g.UserGroupUId));
            }

            return rolesList.Distinct().ToArray();
        }

        public bool ResetPassword(string email) {

            if (System.Web.HttpContext.Current == null)
                return false;

            string path = System.Web.HttpContext.Current.Request.MapPath("~/Views/Core_Signin/ResetPassword_Email.cshtml");
            string root = System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority;

            if (System.Web.HttpContext.Current.Request.ApplicationPath != "/")
                root = root + System.Web.HttpContext.Current.Request.ApplicationPath;


            return ResetPassword(email, path, root);
        }

        public bool ResetPassword(string email, string templatePath, string siteRoot) {

            User user = GetUser(email);
            if (user == null)
                return false;

            string pass = CreateRandomPassword();
            PasswordReset reset = CreatePasswordReset(user.Email, pass);

            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage(AdminEmailAccount, email);
            msg.Subject = SystemName + " - " + SharedStrings.Change_password;
            //msg.SubjectEncoding = System.Text.UnicodeEncoding.Unicode;
            //msg.BodyEncoding = System.Text.UnicodeEncoding.Unicode;
            msg.IsBodyHtml = true;

            string template = System.IO.File.ReadAllText(templatePath, Encoding.Default);

            string body = "";
            if (IsRazorEngineOn)
                body = RazorEngine.Razor.Parse(template, new { NewPassword = pass, SiteRoot = siteRoot, ChangeKey = reset.Key, SystemName = SystemName, Name = user.Name });
            else {
                body = Box.Composition.EmailParser.FakeRazorParser(template, "@Model.", new { NewPassword = pass, SiteRoot = siteRoot, ChangeKey = reset.Key, SystemName = SystemName });
            }

            msg.Body = body;

            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();

            ThreadPool.QueueUserWorkItem(t => {
                smtp.Send(msg);
            });

            return true;
        }

        public bool ApplyResetPassword(string key) {
            PasswordReset reset = null;
            using (var context = new Data.CoreContext()) {
                DateTime limitDate = DateTime.Now.AddHours(-2);
                reset = context.PasswordResets.SingleOrDefault(p => p.Key == key && p.IssueDate > limitDate);
                if (reset == null)
                    return false;

                User user = GetUser(reset.Email);
                if (user == null)
                    return false;

                context.Users.Attach(user);

                if (user.Password != null) {
                    user.Password.Password = reset.NewPassword;
                    user.Blocked = false;
                    user.LoginErrorsCount = 0;
                } else
                    user.Password = new UserPassword() { Email = user.Email, Password = reset.NewPassword };

                context.PasswordResets.Remove(reset);

                context.SaveChanges();
            }

            return true;
        }

        public void CheckUniqueLoginNT(User user) {

            if (String.IsNullOrWhiteSpace(user.LoginNT))
                return;

            using (Core.Data.CoreContext context = new Data.CoreContext()) {

                // uses try/catch 'cuz some old versions may not have the LoginNT field at the database
                bool otherUser = false;
                try {
                    otherUser = context.Users.Any(x => x.LoginNT.ToLower() == user.LoginNT.ToLower() && x.Email != user.Email);
                } catch (Exception) { }

                if (otherUser)
                    throw new HttpResponseException(System.Net.HttpStatusCode.Conflict);
            }
        }

        private PasswordReset CreatePasswordReset(string email, string pass) {
            PasswordReset reset = null;
            using (var context = new Data.CoreContext()) {
                PasswordReset oldReset = context.PasswordResets.SingleOrDefault(p => p.Email.ToLower() == email.ToLower());
                if (oldReset != null)
                    context.PasswordResets.Remove(oldReset);
                reset = new PasswordReset() { Email = email, IssueDate = DateTime.Now, Key = Guid.NewGuid().ToString(), NewPassword = Crypt(pass) };
                context.PasswordResets.Add(reset);
                context.SaveChanges();
            }

            return reset;

        }

        private string CreateRandomPassword() {

            string letters = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm12345678!@#$%&*";
            string pass = "";

            Random rand = new Random();
            while (pass.Length < 7) {
                pass = pass + letters[rand.Next(letters.Length - 1)];
            }

            return pass;
        }

        private string AdminEmailAccount {
            get {
                string email = System.Configuration.ConfigurationManager.AppSettings["ADMIN_EMAIL"] as String;
                if (email == null)
                    email = "admin@localhost";
                return email;
            }
        }

        private string SystemName {
            get {
                string name = System.Configuration.ConfigurationManager.AppSettings["SYSTEM_NAME"] as String;
                if (String.IsNullOrEmpty(name)) {
                    if (System.Web.HttpContext.Current != null)
                        name = System.Web.HttpContext.Current.Request.Url.Authority;
                    else
                        name = "";
                }
                return name;
            }
        }

        public void UpdateAdminGroup() {

            try {

                GroupCollection group = GetGroupCollection(ADMIN_GROUP_ID);
                if (group == null)
                    return;

                var notAddedYet = allGroups.Where(a => !group.CollectionGroups.Any(g => g.UserGroupUId == a.UserGroupUId));

                if (notAddedYet.Count() == 0)
                    return;

                foreach (var g in notAddedYet)
                    group.CollectionGroups.Add(new GroupCollection_Group() { GroupCollectionUId = ADMIN_GROUP_ID, UserGroupUId = g.UserGroupUId });

                SaveGroupCollection(group);

            } catch (Exception) { }

        }

        public static System.Data.SqlClient.SqlException GetSqlException(Exception ex)
        {
            var sql = ex as System.Data.SqlClient.SqlException;
            if (sql != null)
                return sql;
            if (ex.InnerException == null)
                return null;
            return GetSqlException(ex.InnerException);
        }
      
    }
}
