using Microsoft.VisualStudio.TestTools.UnitTesting;
using Roman.Ambinder.Storage.Impl.EntityFrameworkCore.Tests.SingleKey.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Roman.Ambinder.Storage.Impl.EntityFrameworkCore.Tests.CompositeKey.NonHierarchicalEntityTests
{
    [TestClass]
    public class EFCoreCompositeKeyReadonlyRepositoryForTests
    {
        [TestMethod]
        public async Task ExistingPerson_GetSingle_ReturnedExistingPerson()
        {
            //Arrange
            var repository = await CompsiteKeyRepositoryArranger.TryGetRepositoryAsync().ConfigureAwait(false);
            var person = CompsiteKeyRepositoryArranger.CreatePerson();
            var addOpRes = await repository.TryAddAsync(person)
                .ConfigureAwait(false);
            Assert.IsTrue(addOpRes, addOpRes.ErrorMessage);
            var existingEntityId = new object[] { addOpRes.Value.Key1, addOpRes.Value.Key2, addOpRes.Value.Key3 };

            //Act
            var getOpRes = await repository.TryGetSingleAsync(existingEntityId)
                .ConfigureAwait(false);

            //Assert
            Assert.IsTrue(getOpRes, getOpRes.ErrorMessage);
            Assert.AreEqual(getOpRes.Value, addOpRes.Value);
        }

        [TestMethod]
        public async Task MultpliplePeopleMatchingFilter_GetMultipleByFilter_AllMatchingFilterResultsReturned()
        {
            //Arrange
            const byte numberOfPeople = 10;
            const byte minimalAge = 10;
            var repository = await CompsiteKeyRepositoryArranger.TryGetRepositoryAsync().ConfigureAwait(false);
            for (var i = 0; i < numberOfPeople; i++)
            {
                var postfix = (i + 1).ToString();
                byte age = (byte)(i + minimalAge);
                var person = CompsiteKeyRepositoryArranger.CreatePerson(age, postfix, postfix);
                var addOpRes = await repository.TryAddAsync(person)
                    .ConfigureAwait(false);
                Assert.IsTrue(addOpRes, addOpRes.ErrorMessage);
            }

            //Act
            var getOpRes = await repository.TryGetMultipleAsync(p => p.Age >= minimalAge)
                .ConfigureAwait(false);

            //Assert
            Assert.IsTrue(getOpRes, getOpRes.ErrorMessage);
            Assert.AreEqual(getOpRes.Value.TotalNumberOfItems, numberOfPeople);
        }

        [TestMethod]
        public async Task MultpliplePeopleMatchingFilter_GetMultipleByFilterWithOrderBy_AllMatchingFilterResultsReturnedOrdered()
        {
            //Arrange
            const byte numberOfPeople = 10;
            const byte minimalAge = 10;
            var repository = await CompsiteKeyRepositoryArranger.TryGetRepositoryAsync().ConfigureAwait(false);
            var localRepo = new List<CompsiteKeyPerson>(numberOfPeople);
            for (var i = 0; i < numberOfPeople; i++)
            {
                var postfix = (i + 1).ToString();
                byte age = (byte)(i + minimalAge);
                var person = CompsiteKeyRepositoryArranger.CreatePerson(age, postfix, postfix);
                localRepo.Add(person);
                var addOpRes = await repository.TryAddAsync(person)
                    .ConfigureAwait(false);
                Assert.IsTrue(addOpRes, addOpRes.ErrorMessage);
            }

            //Act
            var getOpRes = await repository.TryGetMultipleAsync(p => p.Age >= minimalAge,
                orderBy: people => people.OrderBy(p => p.Age))
                .ConfigureAwait(false);

            //Assert
            Assert.IsTrue(getOpRes, getOpRes.ErrorMessage);
            Assert.AreEqual(numberOfPeople, getOpRes.Value.TotalNumberOfItems);
            Assert.IsTrue(localRepo.OrderBy(p => p.Age).SequenceEqual(getOpRes.Value.Items));
        }
    }
}