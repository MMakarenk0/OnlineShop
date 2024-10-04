using AutoMapper;
using DataLayer.Data.Infrastructure;
using OnlineShop.BLL.Dtos.Create;
using OnlineShop.BLL.Dtos.Read;
using OnlineShop.BLL.Dtos.Update;
using OnlineShop.BLL.Services.Interfaces;
using OnlineShop.DataLayer.Entities;

namespace OnlineShop.BLL.Services.Classes;

public class TraitService : ITraitService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TraitService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<Guid> AddAsync(CreateTraitDto model)
    {
        var traitRepository = _unitOfWork.TraitRepository;

        var trait = _mapper.Map<Trait>(model);

        await traitRepository.Create(trait);

        await _unitOfWork.SaveChangesAsync();

        return trait.Id;
    }

    public async Task DeleteAsync(Guid id)
    {
        var traitRepository = _unitOfWork.TraitRepository;

        await traitRepository.Delete(id);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<TraitDto>> GetAllAsync()
    {
        var traitRepository = _unitOfWork.TraitRepository;

        return _mapper.Map<IEnumerable<TraitDto>>(traitRepository.GetAll());
    }

    public async Task<TraitDto> GetByIdAsync(Guid id)
    {
        var traitRepository = _unitOfWork.TraitRepository;

        var trait = await traitRepository.Find(id);

        if (trait == null)
            throw new KeyNotFoundException($"Trait with Id {id} not found.");

        var traitDto = _mapper.Map<TraitDto>(trait);

        return traitDto;
    }

    public async Task<Guid> UpdateAsync(UpdateTraitDto model)
    {
        var traitRepository = _unitOfWork.TraitRepository;

        var trait = await traitRepository.Find(model.Id);

        if (trait == null)
            throw new KeyNotFoundException($"Trait with Id {model.Id} not found.");

        _mapper.Map(model, trait);

        var result = await traitRepository.Update(trait);

        await _unitOfWork.SaveChangesAsync();

        return result.Id;
    }
}
