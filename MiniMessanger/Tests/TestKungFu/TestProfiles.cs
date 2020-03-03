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
            bool resultMan = profiles.UpdateGender(user.Profile, "1", ref message);
            bool resultWoman = profiles.UpdateGender(user.Profile, "0", ref message);
            bool withoutGender = profiles.UpdateGender(user.Profile, null, ref message);
            bool wrongGender = profiles.UpdateGender(user.Profile, "2",  ref message);
            Assert.AreEqual(resultMan, true);
            Assert.AreEqual(resultWoman, true);
            Assert.AreEqual(withoutGender, true);
            Assert.AreEqual(wrongGender, false);
        }
        [Test]
        public void UpdateAge()
        {
            User user = CreateMockingUser();
            bool result = profiles.UpdateAge(user.Profile, "28", ref message);
            bool withoutAge = profiles.UpdateAge(user.Profile, null, ref message);
            bool lessAge = profiles.UpdateAge(user.Profile, "-12", ref message);
            bool moreAge = profiles.UpdateAge(user.Profile, "230",  ref message);
            Assert.AreEqual(result, true);
            Assert.AreEqual(withoutAge, true);
            Assert.AreEqual(lessAge, false);
            Assert.AreEqual(moreAge, false);
        }
        [Test]
        public void UpdateCity()
        {
            User user = CreateMockingUser();
            bool result = profiles.UpdateCity(user.Profile, "California", ref message);
            bool withoutCity = profiles.UpdateCity(user.Profile, null, ref message);
            bool lessChars = profiles.UpdateCity(user.Profile, "12", ref message);
            bool moreChars = profiles.UpdateCity(user.Profile, "123456789012345678901234567890123456789012345678901"
            ,  ref message);
            Assert.AreEqual(result, true);
            Assert.AreEqual(withoutCity, true);
            Assert.AreEqual(lessChars, false);
            Assert.AreEqual(moreChars, false);
        }
        [Test]
        public void UpdatePhoto()
        {
            byte[] fileBytes = File.ReadAllBytes("/home/neytchi/Configuration/messanger-configuration/parrot.jpg");
            FormFile file = new FormFile(new MemoryStream(fileBytes), 0, 0, "file", "parrot.jpg");
            file.Headers = new HeaderDictionary();
            file.ContentType = "image/jpeg";
            User user = CreateMockingUser();
            bool result = profiles.UpdatePhoto(file, user.Profile, ref message);
            bool withoutPhoto = profiles.UpdatePhoto(null, user.Profile, ref message);
            file.ContentType = "audio/mp3";
            bool withWrongType = profiles.UpdatePhoto(file, user.Profile, ref message);
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