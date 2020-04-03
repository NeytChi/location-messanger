using Common;
using System.IO;
using System.Linq;
using NUnit.Framework;
using miniMessanger.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace miniMessanger.Test
{
    [TestFixture]
    public class TestProfiles
    {
        public TestProfiles()
        {
            this.context = new Context(true, true);
            this.profiles = new Profiles(context);
            this.authentication = new Authentication(context, new Validator());
        }
        public Context context;
        public Profiles profiles;
        public Authentication authentication;
        public string message;
        public FileSaver system = new FileSaver();
        
        [Test]
        public void UpdateProfile()
        {
            User user = CreateMockingUser();
            Profile result = profiles.UpdateProfile(user.UserId, ref message);
            Assert.AreEqual(result.UserId, user.UserId);
        }
        [Test]
        public void UpdateGender()
        {
            User user = CreateMockingUser();
            Profile profile = user.Profile;
            bool resultMan = profiles.UpdateGender(ref profile, "1", ref message);
            bool resultWoman = profiles.UpdateGender(ref profile, "0", ref message);
            bool withoutGender = profiles.UpdateGender(ref profile, null, ref message);
            bool wrongGender = profiles.UpdateGender(ref profile, "2",  ref message);
            Assert.AreEqual(resultMan, true);
            Assert.AreEqual(resultWoman, true);
            Assert.AreEqual(withoutGender, true);
            Assert.AreEqual(wrongGender, false);
        }
        [Test]
        public void UpdateAge()
        {
            User user = CreateMockingUser();
            Profile profile = user.Profile;
            bool result = profiles.UpdateAge(ref profile, "28", ref message);
            bool withoutAge = profiles.UpdateAge(ref profile, null, ref message);
            bool lessAge = profiles.UpdateAge(ref profile, "-12", ref message);
            bool moreAge = profiles.UpdateAge(ref profile, "230",  ref message);
            Assert.AreEqual(result, true);
            Assert.AreEqual(withoutAge, true);
            Assert.AreEqual(lessAge, false);
            Assert.AreEqual(moreAge, false);
        }
        [Test]
        public void UpdateCity()
        {
            User user = CreateMockingUser();
            Profile profile = user.Profile;
            bool result = profiles.UpdateCity(ref profile, "California", ref message);
            bool withoutCity = profiles.UpdateCity(ref profile, null, ref message);
            bool lessChars = profiles.UpdateCity(ref profile, "12", ref message);
            bool moreChars = profiles.UpdateCity(ref profile, "123456789012345678901234567890123456789012345678901"
            ,  ref message);
            Assert.AreEqual(result, true);
            Assert.AreEqual(withoutCity, true);
            Assert.AreEqual(lessChars, false);
            Assert.AreEqual(moreChars, false);
        }
        [Test]
        public void UpdatePhoto()
        {
            Profile profile ;
            byte[] fileBytes = File.ReadAllBytes("/home/neytchi/Configuration/messanger-configuration/parrot.jpg");
            FormFile file = new FormFile(new MemoryStream(fileBytes), 0, 0, "file", "parrot.jpg");
            file.Headers = new HeaderDictionary();
            file.ContentType = "image/jpeg";
            User user = CreateMockingUser();
            profile = user.Profile;
            bool result = profiles.UpdatePhoto(file, ref profile, ref message);
            bool withoutPhoto = profiles.UpdatePhoto(null, ref profile, ref message);
            file.ContentType = "audio/mp3";
            bool withWrongType = profiles.UpdatePhoto(file, ref profile, ref message);
            Assert.AreEqual(result, true);
            Assert.AreEqual(withoutPhoto, true);
            Assert.AreEqual(withWrongType, false);
            system.DeleteFile(user.Profile.UrlPhoto);
        }
        
        [Test]
        public void CreateIfNotExistProfile()
        {
            User user = CreateMockingUser();
            user.Profile = profiles.CreateIfNotExistProfile(user.UserId);
            Assert.AreEqual(user.Profile.UserId, user.UserId);
        }
        public User CreateMockingUser()
        {
            string UserEmail = "test@gmail.com";
            string UserPassword = "Test1234";
            string UserLogin = "Test";
            string UserToken = "Test";
            DeleteUser();
            User user = authentication.CreateUser(UserEmail, UserLogin, UserPassword);
            UserToken = user.UserToken;
            user.Activate = 1;
            context.User.Update(user);
            context.SaveChanges();
            return user;
        }
        public void DeleteUser()
        {
            string UserEmail = "test@gmail.com";
            System.Collections.Generic.List<User> users = context.User.Where(u => u.UserEmail == UserEmail).ToList();
            context.User.RemoveRange(users);
            context.SaveChanges();
        }
        
    }
}