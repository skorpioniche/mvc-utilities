﻿using System;
using System.Globalization;
using System.Text;
using MVC.Utilities.Encryption;
using NUnit.Framework;

namespace MVC.Utilities.Tests.Encryption
{
    [TestFixture]
    public class SHA1ServiceTests
    {
        private ICryptoService _crypto;

        public SHA1ServiceTests()
        {

        }

        [SetUp]
        public void Initialize()
        {
            const string validationKey = "C7D461D13B86BED2A574C1E57A90DCD613690FA72CF3C9ED37C182222C417C22D886B7620921A3C730626F6BA42637F8B9D974040DB73BFA9CF8D30A14852581";

            _crypto = new HMACSHA1Service(validationKey);
        }

        /// <summary>
        /// Can we hash and check a simple password?
        /// </summary>
        [Test]
        public void CanSha1HashAndCheckSimplePassword()
        {
            //Create a basic password...
            var originalPass = "password";

            //Hash it...
            var hash = _crypto.HashPassword(originalPass);

            //Assert that they're actually different...
            Assert.AreNotEqual(originalPass, hash);

            //And assert that they're not instances of the same object (I'm being anal here)
            Assert.AreNotSame(originalPass, hash);

            //Now use the validation function to check to see if they're equivalent

            Assert.IsTrue(_crypto.CheckPassword(originalPass, hash));
        }

        /// <summary>
        /// Can we hash and check a simple password with a salt provided by the caller?
        /// </summary>
        [Test]
        public void CanSha1HashAndCheckSimplePasswordWithCustomSalt()
        {
            //Create a basic password...
            var originalPass = "password";

            //Create a salt using today's datetime
            var salt = Convert.ToBase64String(Encoding.Unicode.GetBytes(DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)));

            //Hash it...
            var hash = _crypto.HashPassword(originalPass, salt);

            //Assert that they're actually different...
            Assert.AreNotEqual(originalPass, hash);

            //And assert that they're not instances of the same object (I'm being anal here)
            Assert.AreNotSame(originalPass, hash);

            //Now use the validation function to check to see if they're equivalent

            Assert.IsTrue(_crypto.CheckPassword(originalPass, hash, salt));
            
            //Assert that the neckpassword method will fail if a different salt is used (using the default validation key this time)
            Assert.IsFalse(_crypto.CheckPassword(originalPass, hash));
        }

        /// <summary>
        /// Assert that the hashes for our passwords are the same length
        /// </summary>
        [Test]
        public void AreSha1HashesTheSameLength()
        {
            //Create our passwords...
            var pass1 = "password";
            var pass2 = "passwordpassword";
            var pass3 = "passwordpasswordpasswordpassword";
            var pass4 = "passwordpasswordpasswordpasswordpasswordpasswordpasswordpassword";

            //Hash our passwords...
            var hash1 = _crypto.HashPassword(pass1);
            var hash2 = _crypto.HashPassword(pass2);
            var hash3 = _crypto.HashPassword(pass3);
            var hash4 = _crypto.HashPassword(pass4);

            //Assert that the lengths are equivalent
            Assert.AreEqual(hash1.Length, hash2.Length);
            Assert.AreEqual(hash1.Length, hash3.Length);
            Assert.AreEqual(hash1.Length, hash4.Length);
        }

        /// <summary>
        /// Assert that the hashes for our passwords are the same length
        /// </summary>
        [Test]
        public void AreSha1HashesTheSameLengthWithDifferentSalts()
        {
            //Create our passwords...
            var pass1 = "password";
            var pass2 = "passwordpassword";
            var pass3 = "passwordpasswordpasswordpassword";
            var pass4 = "passwordpasswordpasswordpasswordpasswordpasswordpasswordpassword";

            //Create our salts
            var salt1 = "magic";
            var salt2 = "moarmagic";
            var salt3 = "doublemagic";
            var salt4 = "triplemagic";

            //Hash our passwords...
            var hash1 = _crypto.HashPassword(pass1, salt1);
            var hash2 = _crypto.HashPassword(pass2, salt2);
            var hash3 = _crypto.HashPassword(pass3, salt3);
            var hash4 = _crypto.HashPassword(pass4, salt4);

            //Assert that the lengths are equivalent
            Assert.AreEqual(hash1.Length, hash2.Length);
            Assert.AreEqual(hash1.Length, hash3.Length);
            Assert.AreEqual(hash1.Length, hash4.Length);
        }
    }
}
