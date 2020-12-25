using Microsoft.VisualStudio.TestTools.UnitTesting;
using Roman.Ambinder.Storage.Impl.EntityFrameworkCore.Tests.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Roman.Ambinder.Storage.Impl.EntityFrameworkCore.Tests.NonHierarchicalEntityTests
{
    [TestClass]
    public class EFCoreSingleKeyReadonlyRepositoryForTests
    {
        [TestMethod]
        public async Task ExistingPerson_GetSingle_ReturnedExistingPerson()
        {
            //Arrange
            var repository = await Arranger.TryGetRepositoryAsync().ConfigureAwait(false);
            var person = Arranger.CreatePerson();
            var addOpRes = await repository.TryAddAsync(person)
                .ConfigureAwait(false);
            Assert.IsTrue(addOpRes, addOpRes.ErrorMessage);

            //Act
            var getOpRes = await repository.TryGetSingleAsync(addOpRes.Value.Id)
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
            var repository = await Arranger.TryGetRepositoryAsync().ConfigureAwait(false);
            for (var i = 0; i < numberOfPeople; i++)
            {
                var postfix = (i + 1).ToString();
                byte age = (byte)(i + minimalAge);
                var person = Arranger.CreatePerson(age, postfix, postfix);
                var addOpRes = await repository.TryAddAsync(person)
                    .ConfigureAwait(false);
                Assert.IsTrue(addOpRes, addOpRes.ErrorMessage);
            }

            //Act
            var getOpRes = await repository.TryGetMultipleAsync(p => p.Age >= minimalAge)
                .ConfigureAwait(false);

            //Assert
            Assert.IsTrue(getOpRes, getOpRes.ErrorMessage);
            Assert.AreEqual(getOpRes.Value.Count, numberOfPeople);
        }


        [TestMethod]
        public async Task MultpliplePeopleMatchingFilter_GetMultipleByFilterWithOrderBy_AllMatchingFilterResultsReturnedOrdered()
        {
            //Arrange
            const byte numberOfPeople = 10;
            const byte minimalAge = 10;
            var repository = await Arranger.TryGetRepositoryAsync().ConfigureAwait(false);
            var localRepo = new List<Person>(numberOfPeople);
            for (var i = 0; i < numberOfPeople; i++)
            {
                var postfix = (i + 1).ToString();
                byte age = (byte)(i + minimalAge);
                var person = Arranger.CreatePerson(age, postfix, postfix);
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
            Assert.AreEqual(numberOfPeople, getOpRes.Value.Count);
            Assert.IsTrue(localRepo.OrderBy(p => p.Age).SequenceEqual(getOpRes.Value));
        }

    }
}