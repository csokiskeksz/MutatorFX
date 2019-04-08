﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryMutator.Core;

namespace QueryMutator.Tests
{
    [TestClass]
    public class GeneralTests
    {
        [TestMethod]
        public void DefaultMapping()
        {
            var options = DatabaseHelper.GetDatabaseOptions(nameof(DefaultMapping));

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMapping<ParentEntity, ParentEntityDto>();
            });
            var mapper = config.CreateMapper();
            var dogMapping = mapper.GetMapping<ParentEntity, ParentEntityDto>();

            using (var context = new DatabaseContext(options))
            {
                var dogs = context.ParentEntities.Select(dogMapping).ToList();

                Assert.AreEqual(1, dogs.Count);

                var result = dogs.FirstOrDefault();

                var expected = new ParentEntityDto
                {
                    Id = 1,
                    Name = "Entity1",
                    DtoProperty = 0,
                    Ignored = "Ignore this property!",
                    Parameterized = 0,
                    NestedEntity = new NestedEntityDto
                    {
                        Id = 1,
                        Name ="NestedEntity1",
                        NestedNestedEntity = new NestedNestedEntityDto
                        {
                            Id = 1,
                            Name = "NestedNestedEntity1"
                        }
                    }
                };

                Assert.AreEqual(true, expected.Equals(result));
            }
        }

        [TestMethod]
        public void ExplicitMapping()
        {
            var options = DatabaseHelper.GetDatabaseOptions(nameof(ExplicitMapping));

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMapping<ParentEntity, ParentEntityDto>(mapping => mapping
                    .MapMember(d => d.DtoProperty, dd => dd.EntityProperty)
                );
            });
            var mapper = config.CreateMapper();
            var dogMapping = mapper.GetMapping<ParentEntity, ParentEntityDto>();

            using (var context = new DatabaseContext(options))
            {
                var dogs = context.ParentEntities.Select(dogMapping).ToList();

                Assert.AreEqual(1, dogs.Count);

                var result = dogs.FirstOrDefault();

                var expected = new ParentEntityDto
                {
                    Id = 1,
                    Name = "Entity1",
                    DtoProperty = 5,
                    Ignored = "Ignore this property!",
                    Parameterized = 0,
                    NestedEntity = new NestedEntityDto
                    {
                        Id = 1,
                        Name ="NestedEntity1",
                        NestedNestedEntity = new NestedNestedEntityDto
                        {
                            Id = 1,
                            Name = "NestedNestedEntity1"
                        }
                    }
                };

                Assert.AreEqual(true, expected.Equals(result));
            }
        }

        [TestMethod]
        public void IgnoreMember()
        {
            var options = DatabaseHelper.GetDatabaseOptions(nameof(IgnoreMember));

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMapping<ParentEntity, ParentEntityDto>(mapping => mapping
                    .IgnoreMember(d => d.Ignored)
                    .IgnoreMember(d => d.NestedEntity)
                );
            });
            var mapper = config.CreateMapper();
            var dogMapping = mapper.GetMapping<ParentEntity, ParentEntityDto>();

            using (var context = new DatabaseContext(options))
            {
                var dogs = context.ParentEntities.Select(dogMapping).ToList();

                Assert.AreEqual(1, dogs.Count);

                var result = dogs.FirstOrDefault();

                var expected = new ParentEntityDto
                {
                    Id = 1,
                    Name = "Entity1",
                    DtoProperty = 0,
                    Ignored = null,
                    Parameterized = 0,
                    NestedEntity = null
                };

                Assert.AreEqual(true, expected.Equals(result));
            }
        }

        [TestMethod]
        public void ConstantMapping()
        {
            var options = DatabaseHelper.GetDatabaseOptions(nameof(ConstantMapping));

            var constant = 15;
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMapping<ParentEntity, ParentEntityDto>(mapping => mapping
                    .MapMember(d => d.DtoProperty, constant)
                );
            });
            var mapper = config.CreateMapper();
            var dogMapping = mapper.GetMapping<ParentEntity, ParentEntityDto>();

            using (var context = new DatabaseContext(options))
            {
                var dogs = context.ParentEntities.Select(dogMapping).ToList();

                Assert.AreEqual(1, dogs.Count);

                var result = dogs.FirstOrDefault();

                var expected = new ParentEntityDto
                {
                    Id = 1,
                    Name = "Entity1",
                    DtoProperty = constant,
                    Ignored = "Ignore this property!",
                    Parameterized = 0,
                    NestedEntity = new NestedEntityDto
                    {
                        Id = 1,
                        Name ="NestedEntity1",
                        NestedNestedEntity = new NestedNestedEntityDto
                        {
                            Id = 1,
                            Name = "NestedNestedEntity1"
                        }
                    }
                };

                Assert.AreEqual(true, expected.Equals(result));
            }
        }

        [TestMethod]
        public void NestedMapping()
        {
            var options = DatabaseHelper.GetDatabaseOptions(nameof(NestedMapping));

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMapping<ParentEntity, ParentEntityDto>(mapping => mapping
                    .MapMember(d => d.NestedEntity, dd => new NestedEntityDto { Id = dd.NestedEntity.Id })
                );
            });
            var mapper = config.CreateMapper();
            var dogMapping = mapper.GetMapping<ParentEntity, ParentEntityDto>();

            using (var context = new DatabaseContext(options))
            {
                var dogs = context.ParentEntities.Select(dogMapping).ToList();

                Assert.AreEqual(1, dogs.Count);

                var result = dogs.FirstOrDefault();

                var expected = new ParentEntityDto
                {
                    Id = 1,
                    Name = "Entity1",
                    DtoProperty = 0,
                    Ignored = "Ignore this property!",
                    Parameterized = 0,
                    NestedEntity = new NestedEntityDto
                    {
                        Id = 1,
                        Name = null
                    }
                };

                Assert.AreEqual(true, expected.Equals(result));
            }

            config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMapping<ParentEntity, ParentEntityDto>(mapping => mapping
                    .MapMember(d => d.NestedEntity, dd => new NestedEntityDto
                    {
                        Id = dd.NestedEntity.Id,
                        Name = dd.NestedEntity.Name,
                        NestedNestedEntity = new NestedNestedEntityDto
                        {
                            Id = dd.NestedEntity.NestedNestedEntity.Id,
                            Name = dd.NestedEntity.NestedNestedEntity.Name,
                        }
                    })
                );
            });
            mapper = config.CreateMapper();
            dogMapping = mapper.GetMapping<ParentEntity, ParentEntityDto>();

            using (var context = new DatabaseContext(options))
            {
                var dogs = context.ParentEntities.Select(dogMapping).ToList();

                Assert.AreEqual(1, dogs.Count);

                var result = dogs.FirstOrDefault();

                var expected = new ParentEntityDto
                {
                    Id = 1,
                    Name = "Entity1",
                    DtoProperty = 0,
                    Ignored = "Ignore this property!",
                    Parameterized = 0,
                    NestedEntity = new NestedEntityDto
                    {
                        Id = 1,
                        Name ="NestedEntity1",
                        NestedNestedEntity = new NestedNestedEntityDto
                        {
                            Id = 1,
                            Name = "NestedNestedEntity1"
                        }
                    }
                };

                Assert.AreEqual(true, expected.Equals(result));
            }
        }

        [TestMethod]
        public void CollectionMapping()
        {
            var options = DatabaseHelper.GetDatabaseOptions(nameof(CollectionMapping));

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMapping<CollectionItem, CollectionItemDto>();

                cfg.CreateMapping<Collection, CollectionDto>(mapping => mapping
                    .MapMemberList(d => d.CollectionItems, dd => dd.CollectionItems)
                    .MapMemberList(d => d.CollectionItemDtos, dd => dd.CollectionItems)
                );
            });
            var mapper = config.CreateMapper();
            var collectionMapping = mapper.GetMapping<Collection, CollectionDto>();

            using (var context = new DatabaseContext(options))
            {
                var collections = context.Collections.Select(collectionMapping).ToList();

                var result = collections.FirstOrDefault();

                var expected = new CollectionDto
                {
                    Id = 1,
                    CollectionItemDtos = new List<CollectionItemDto>
                    {
                        new CollectionItemDto { Id = 1 },
                        new CollectionItemDto { Id = 2 },
                    },
                    CollectionItems = new List<CollectionItem>
                    {
                        new CollectionItem { Id = 1, CollectionId = 1 },
                        new CollectionItem { Id = 2, CollectionId = 1 },
                    }
                };

                Assert.AreEqual(true, expected.Equals(result));
            }
        }

        [TestMethod]
        public void MappingWithParameter()
        {
            var options = DatabaseHelper.GetDatabaseOptions(nameof(MappingWithParameter));

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMapping<ParentEntity, ParentEntityDto, int>(mapping => mapping
                    .MapMemberWithParameter(d => d.Parameterized, p => dd => dd.Id * p)
                    .IgnoreMember(d => d.Ignored)
                );

                cfg.CreateMapping<ParentEntity, ParentEntityDto, ParentEntityParamaters>(mapping => mapping
                    .MapMemberWithParameter(d => d.Parameterized, p => dd => dd.Id * p.IntProperty)
                    .MapMemberWithParameter(d => d.Name, p => dd => dd.Name + p.StringProperty)
                );
            });
            var mapper = config.CreateMapper();
            var dogMappingWithParameter = mapper.GetMapping<ParentEntity, ParentEntityDto, int>();
            var dogMappingWithParameters = mapper.GetMapping<ParentEntity, ParentEntityDto, ParentEntityParamaters>();

            using (var context = new DatabaseContext(options))
            {
                var param = 5;
                var dogs = context.ParentEntities.Select(dogMappingWithParameter, param).ToList();

                Assert.AreEqual(1, dogs.Count);

                var result = dogs.FirstOrDefault();

                var expected = new ParentEntityDto
                {
                    Id = 1,
                    Name = "Entity1",
                    DtoProperty = 0,
                    Ignored = null,
                    Parameterized = 1 * param,
                    NestedEntity = new NestedEntityDto
                    {
                        Id = 1,
                        Name ="NestedEntity1",
                        NestedNestedEntity = new NestedNestedEntityDto
                        {
                            Id = 1,
                            Name = "NestedNestedEntity1"
                        }
                    }
                };

                Assert.AreEqual(true, expected.Equals(result));

                var @params = new ParentEntityParamaters
                {
                    IntProperty = 10,
                    StringProperty = "_suffix"
                };
                dogs = context.ParentEntities.Select(dogMappingWithParameters, @params).ToList();

                Assert.AreEqual(1, dogs.Count);

                result = dogs.FirstOrDefault();

                expected = new ParentEntityDto
                {
                    Id = 1,
                    Name = "Entity1" + @params.StringProperty,
                    DtoProperty = 0,
                    Ignored = "Ignore this property!",
                    Parameterized = 1 * @params.IntProperty,
                    NestedEntity = new NestedEntityDto
                    {
                        Id = 1,
                        Name ="NestedEntity1",
                        NestedNestedEntity = new NestedNestedEntityDto
                        {
                            Id = 1,
                            Name = "NestedNestedEntity1"
                        }
                    }
                };

                Assert.AreEqual(true, expected.Equals(result));
            }
        }

        [TestMethod]
        public void JoinResultMapping()
        {
            var options = DatabaseHelper.GetDatabaseOptions(nameof(JoinResultMapping));

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMapping<ParentEntity, ParentEntityDto>();
            });
            var mapper = config.CreateMapper();
            var dogMapping = mapper.GetMapping<ParentEntity, ParentEntityDto>();

            using (var context = new DatabaseContext(options))
            {
                var joined = context.ParentEntities
                    .Join(context.NestedEntities,
                        d => d.NestedEntityId,
                        s => s.Id,
                        (d, s) => new ParentEntity
                        {
                            Id = d.Id,
                            Name = d.Name,
                            EntityProperty = d.EntityProperty,
                            Ignored = d.Ignored,
                            NestedEntity = s
                        })
                    .Select(dogMapping).ToList();

                Assert.AreEqual(1, joined.Count);

                var result = joined.FirstOrDefault();

                var expected = new ParentEntityDto
                {
                    Id = 1,
                    Name = "Entity1",
                    DtoProperty = 0,
                    Ignored = "Ignore this property!",
                    Parameterized = 0,
                    NestedEntity = new NestedEntityDto
                    {
                        Id = 1,
                        Name ="NestedEntity1",
                        NestedNestedEntity = new NestedNestedEntityDto
                        {
                            Id = 1,
                            Name = "NestedNestedEntity1"
                        }
                    }
                };

                Assert.AreEqual(true, expected.Equals(result));
            }
        }

        [TestMethod]
        public void NullableMapping()
        {
            var options = DatabaseHelper.GetDatabaseOptions(nameof(NullableMapping));

            var constant = 15;
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMapping<NullableEntity, NullableEntityDto>(mapping => mapping
                    .MapMember(d => d.NullableProperty, dd => dd.NotNullableProperty)
                    .MapMember(d => d.NullableProperty2, dd => dd.NullableProperty)
                    .MapMember(d => d.NotNullableProperty, dd => dd.NullableProperty)
                    .MapMember(d => d.NotNullableProperty2, dd => dd.NullablePropertyWithValue)
                    .MapMember(d => d.NullableProperty3, constant)
                );
            });
            var mapper = config.CreateMapper();
            var nullableMapping = mapper.GetMapping<NullableEntity, NullableEntityDto>();

            using (var context = new DatabaseContext(options))
            {
                var nullableDtos = context.NullableEntities.Select(nullableMapping).ToList();

                Assert.AreEqual(1, nullableDtos.Count);

                var result = nullableDtos.FirstOrDefault();

                var expected = new NullableEntityDto
                {
                    Id = 1,
                    NullableProperty = 10,
                    NullableProperty2 = null,
                    NullableProperty3 = constant,
                    NotNullableProperty = 0,
                    NotNullableProperty2 = 20
                };

                Assert.AreEqual(true, expected.Equals(result));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(MappingNotFoundException))]
        public void MappingNotFound()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMapping<NestedEntity, NestedEntityDto>();
            });
            var mapper = config.CreateMapper();

            var dogMapping = mapper.GetMapping<ParentEntity, ParentEntityDto>();
        }

        [TestMethod]
        [ExpectedException(typeof(MappingNotFoundException))]
        public void MappingWithParameterNotFound()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMapping<ParentEntity, ParentEntityDto, int>(mapping => mapping
                    .MapMemberWithParameter(d => d.Parameterized, p => dd => dd.Id * p)
                );
            });
            var mapper = config.CreateMapper();

            var dogMapping = mapper.GetMapping<ParentEntity, ParentEntityDto, string>();
        }

        [TestMethod]
        public void SourceValidationSuccess()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMapping<NestedNestedEntity, NestedNestedEntityDto>(mapping => mapping
                    .ValidateMapping(ValidationMode.Source)
                );
            });
            config.CreateMapper();
        }

        [TestMethod]
        [ExpectedException(typeof(MappingValidationException))]
        public void SourceValidationFailure()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMapping<ParentEntity, ParentEntityDto>(mapping => mapping
                    .ValidateMapping(ValidationMode.Source)
                );
            });
            config.CreateMapper();
        }

        [TestMethod]
        [ExpectedException(typeof(MappingValidationException))]
        public void GlobalSourceValidationFailure()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMapping<ParentEntity, ParentEntityDto>();
            }, new MapperConfigurationOptions { ValidationMode = ValidationMode.Source });
            config.CreateMapper();
        }

        [TestMethod]
        public void DestinationValidationSuccess()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMapping<NestedNestedEntity, NestedNestedEntityDto>(mapping => mapping
                    .ValidateMapping(ValidationMode.Destination)
                );
            });
            config.CreateMapper();
        }

        [TestMethod]
        [ExpectedException(typeof(MappingValidationException))]
        public void DestinationValidationFailure()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMapping<ParentEntity, ParentEntityDto>(mapping => mapping
                    .ValidateMapping(ValidationMode.Destination)
                );
            });
            config.CreateMapper();
        }

        [TestMethod]
        [ExpectedException(typeof(MappingValidationException))]
        public void GlobalDestinationValidationFailure()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMapping<ParentEntity, ParentEntityDto>();
            }, new MapperConfigurationOptions { ValidationMode = ValidationMode.Destination });
            config.CreateMapper();
        }

        [TestMethod]
        [ExpectedException(typeof(MappingAlreadyExistsException))]
        public void MappingAlreadyExists()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMapping<ParentEntity, ParentEntityDto>();

                cfg.CreateMapping<ParentEntity, ParentEntityDto>();
            });
            config.CreateMapper();
        }

        [TestMethod]
        [ExpectedException(typeof(MappingAlreadyExistsException))]
        public void MappingWithActionAlreadyExists()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMapping<ParentEntity, ParentEntityDto>();

                cfg.CreateMapping<ParentEntity, ParentEntityDto>(mapping => mapping
                    .MapMember(d => d.DtoProperty, dd => dd.EntityProperty)
                );
            });
            config.CreateMapper();
        }

        [TestMethod]
        [ExpectedException(typeof(MappingAlreadyExistsException))]
        public void MappingWithParameterAlreadyExists()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMapping<ParentEntity, ParentEntityDto, int>(mapping => mapping
                    .MapMemberWithParameter(d => d.Parameterized, p => dd => dd.Id * p)
                );

                cfg.CreateMapping<ParentEntity, ParentEntityDto, int>(mapping => mapping
                    .MapMemberWithParameter(d => d.Parameterized, p => dd => dd.Id * p)
                );
            });
            config.CreateMapper();
        }

        [TestMethod]
        public void MappingWithParameterDoesNotExists()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMapping<ParentEntity, ParentEntityDto, int>(mapping => mapping
                    .MapMemberWithParameter(d => d.Parameterized, p => dd => dd.Id * p)
                );

                cfg.CreateMapping<ParentEntity, ParentEntityDto, string>(mapping => mapping
                    .MapMemberWithParameter(d => d.Name, p => dd => dd.Name + p)
                );
            });
            config.CreateMapper();
        }
    }
}