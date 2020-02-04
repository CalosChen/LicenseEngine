﻿using System;
using KeyCommon;
using KeyGenerate;
using KeyVerify;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeyTest
{
    [TestClass]
    public class KeyTests
    {
        [TestMethod]
        public void Test_pkv_licence_key_generation_and_verification()
        {
            var pkvLicenceKey = new KeyGenerator();

            var pkvKeyCheck = new KeyCheck();

            string key = String.Empty;

            KeyByteSet[] keyByteSets = new[]
                                               {
                                                   new KeyByteSet(1, 58, 6, 97),
                                                   new KeyByteSet(2, 96, 254, 23),
                                                   new KeyByteSet(3, 11, 185, 69),
                                                   new KeyByteSet(4, 2, 93, 41),
                                                   new KeyByteSet(5, 62, 4, 234),
                                                   new KeyByteSet(6, 200, 56, 49),
                                                   new KeyByteSet(7, 89, 45,142),
                                                   new KeyByteSet(8, 6, 88, 32)
                                               };

            // Change these to a random key byte set from the above array to test key verification with

            KeyByteSet kbs1 = keyByteSets[3];
            KeyByteSet kbs2 = keyByteSets[7];
            KeyByteSet kbs3 = keyByteSets[4];

            // The check project also uses a class called KeyByteSet, but with
            // separate name spacing to achieve single self contained dll

            KeyByteSet keyByteSet1 = new KeyByteSet(kbs1.KeyByteNo, kbs1.KeyByteA, kbs1.KeyByteB, kbs1.KeyByteC); // Change no to test others
            KeyByteSet keyByteSet2 = new KeyByteSet(kbs2.KeyByteNo, kbs2.KeyByteA, kbs2.KeyByteB, kbs2.KeyByteC);
            KeyByteSet keyByteSet3 = new KeyByteSet(kbs3.KeyByteNo, kbs3.KeyByteA, kbs3.KeyByteB, kbs3.KeyByteC);

            for (int i = 0; i < 10000; i++)
            {
                int seed = new Random().Next(0, Int32.MaxValue);

                key = pkvLicenceKey.MakeKey(seed, keyByteSets);

                // Check that check sum validation passes
                Assert.IsTrue(pkvKeyCheck.CheckKeyChecksum(key, keyByteSets.Length));

                // Check using full check method
                Assert.IsTrue(pkvKeyCheck.CheckKey(
                                  key,
                                  new[] { keyByteSet1, keyByteSet2, keyByteSet3 },
                                  keyByteSets.Length,
                                  null
                              ) == LicenceKeyResult.KeyGood, "Failed on iteration " + i
                            );

                // Check that erroneous check sum validation fails
                Assert.IsFalse(pkvKeyCheck.CheckKeyChecksum(key.Remove(23, 1) + "A", keyByteSets.Length)); // Change key by replacing 17th char
            }

            // Check a few random inputs
            Assert.IsFalse(pkvKeyCheck.CheckKey("adcsadrewf",
                               new[] { keyByteSet1, keyByteSet2 },
                               keyByteSets.Length,
                               null) == LicenceKeyResult.KeyGood
                        );
            Assert.IsFalse(pkvKeyCheck.CheckKey("",
                                            new[] { keyByteSet1, keyByteSet2 },
                                            keyByteSets.Length,
                                            null) == LicenceKeyResult.KeyGood
                        );
            Assert.IsFalse(pkvKeyCheck.CheckKey("123",
                                            new[] { keyByteSet1, keyByteSet2 },
                                            keyByteSets.Length,
                                            null) == LicenceKeyResult.KeyGood
                        );
            Assert.IsFalse(pkvKeyCheck.CheckKey("*()",
                                            new[] { keyByteSet1, keyByteSet2 },
                                            keyByteSets.Length,
                                            null) == LicenceKeyResult.KeyGood
                        );
            Assert.IsFalse(pkvKeyCheck.CheckKey("dasdasdasgdjwqidqiwd21887127eqwdaishxckjsabcxjkabskdcbq2e81y12e8712",
                                            new[] { keyByteSet1, keyByteSet2 },
                                            keyByteSets.Length,
                                            null) == LicenceKeyResult.KeyGood
                        );
        }

        [TestMethod]
        public void Test_PKV_licence_key_generation_and_verification_with_random_key_bytes_key_byte_qty_and_verification_key_byte_selection()
        {
            var pkvLicenceKey = new KeyGenerator();

            var pkvKeyCheck = new KeyCheck();

            for (int i = 0; i < 10000; i++)
            {
                int randomKeyByteSetsLength = new Random().Next(2, 400);

                KeyByteSet[] keyByteSets = new KeyByteSet[randomKeyByteSetsLength];

                for (int j = 0; j < randomKeyByteSetsLength; j++)
                {
                    var random = new Random();

                    var kbs = new KeyByteSet
                                  (
                                      j + 1,
                                      (byte)random.Next(0, 256),
                                      (byte)random.Next(0, 256),
                                      (byte)random.Next(0, 256)
                                  );

                    keyByteSets[j] = kbs;
                }

                // Select a random key byte set to test key verification with

                KeyByteSet kbs1 = keyByteSets[new Random().Next(0, randomKeyByteSetsLength)];
                KeyByteSet kbs2 = keyByteSets[new Random().Next(0, randomKeyByteSetsLength)];

                // The check project also uses a class called KeyByteSet, but with
                // separate name spacing to achieve single self contained dll

                KeyByteSet keyByteSet1 = new KeyByteSet(kbs1.KeyByteNo, kbs1.KeyByteA, kbs1.KeyByteB, kbs1.KeyByteC); // Change no to test others
                KeyByteSet keyByteSet2 = new KeyByteSet(kbs2.KeyByteNo, kbs2.KeyByteA, kbs2.KeyByteB, kbs2.KeyByteC);

                int seed = new Random().Next(0, Int32.MaxValue);

                string key = pkvLicenceKey.MakeKey(seed, keyByteSets);

                // Check that check sum validation passes

                Assert.IsTrue(pkvKeyCheck.CheckKeyChecksum(key, keyByteSets.Length));

                // Check using full check method

                Assert.IsTrue(pkvKeyCheck.CheckKey(
                                            key,
                                            new[] { keyByteSet1, keyByteSet2 },
                                            keyByteSets.Length,
                                            null
                                        ) == LicenceKeyResult.KeyGood, "Failed on iteration " + i
                            );

            }
        }
    }
}