using AutoMapper;
using DataLayer.Data.Infrastructure;
using DataLayer.Data.Repositories.Interfaces;
using MockQueryable;
using NSubstitute;
using OnlineShop.BLL.Dtos.Create;
using OnlineShop.BLL.Dtos.Read;
using OnlineShop.BLL.Dtos.Update;
using OnlineShop.BLL.Services.Classes;
using OnlineShop.BLL.Services.Interfaces;
using OnlineShop.DataLayer.Data.Repositories.Interfaces;
using OnlineShop.DataLayer.Entities;

namespace UnitTests.BLLTests;

public class TraitServiceTests
{
    private readonly ITraitService _service;
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly ITraitRepository _traitRepository = Substitute.For<ITraitRepository>();

    public TraitServiceTests()
    {
        _service = new TraitService(_unitOfWork, _mapper);
        _unitOfWork.TraitRepository.Returns(_traitRepository);
    }

    [Fact]
    public async Task AddAsync_Should_Add_Trait_And_Return_Id()
    {
        // Arrange
        var createDto = new CreateTraitDto { Name = "New Trait" };
        var trait = new Trait { Id = Guid.NewGuid(), Name = "New Trait" };

        _mapper.Map<Trait>(createDto).Returns(trait);
        _traitRepository.Create(trait).Returns(trait);

        // Act
        var result = await _service.AddAsync(createDto);

        // Assert
        Assert.Equal(trait.Id, result);
        await _unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task DeleteAsync_Should_Delete_Trait()
    {
        // Arrange
        var traitId = Guid.NewGuid();

        _traitRepository.Delete(Arg.Any<Guid>()).Returns(Task.CompletedTask);
        // Act
        await _service.DeleteAsync(traitId);

        // Assert
        await _traitRepository.Received(1).Delete(Arg.Any<Guid>());
        await _unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_Traits()
    {
        // Arrange
        var traits = new List<Trait>
        {
            new Trait { Id = Guid.NewGuid(), Name = "Trait 1" },
            new Trait { Id = Guid.NewGuid(), Name = "Trait 2" }
        };
        var traitDtos = new List<TraitDto>
        {
            new TraitDto { Id = traits[0].Id, Name = "Trait 1" },
            new TraitDto { Id = traits[1].Id, Name = "Trait 2" }
        };

        _traitRepository.GetAll().Returns(traits.AsQueryable());
        _mapper.Map<IEnumerable<TraitDto>>(Arg.Any<IQueryable<Trait>>()).Returns(traitDtos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Equal(traitDtos, result);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_TraitDto_If_Found()
    {
        // Arrange
        var traitId = Guid.NewGuid();
        var trait = new Trait { Id = traitId, Name = "Trait" };
        var traitDto = new TraitDto { Id = traitId, Name = "Trait" };

        _traitRepository.Find(Arg.Any<Guid>()).Returns(trait);
        _mapper.Map<TraitDto>(trait).Returns(traitDto);

        // Act
        var result = await _service.GetByIdAsync(traitId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(traitDto, result);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Throw_KeyNotFoundException_If_Not_Found()
    {
        // Arrange
        var traitId = Guid.NewGuid();
        _traitRepository.Find(traitId).Returns((Trait)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetByIdAsync(traitId));
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Trait_And_Return_Id()
    {
        // Arrange
        var updateDto = new UpdateTraitDto { Id = Guid.NewGuid(), Name = "Updated Trait" };
        var trait = new Trait { Id = updateDto.Id, Name = "Old Trait" };

        _traitRepository.Find(Arg.Any<Guid>()).Returns(trait);
        _mapper.Map(updateDto, trait);
        _traitRepository.Update(trait).Returns(Task.FromResult(trait));

        // Act
        var result = await _service.UpdateAsync(updateDto);

        // Assert
        Assert.Equal(updateDto.Id, result);
        await _unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateAsync_Should_Throw_KeyNotFoundException_If_Trait_Not_Found()
    {
        // Arrange
        var updateDto = new UpdateTraitDto { Id = Guid.NewGuid(), Name = "Updated Trait" };
        _traitRepository.Find(updateDto.Id).Returns((Trait)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(updateDto));
    }
}