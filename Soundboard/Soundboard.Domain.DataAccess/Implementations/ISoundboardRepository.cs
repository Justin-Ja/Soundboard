using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soundboard.Domain.DataAccess.Implementations;

public interface ISoundboardRepository
{
    Task<List<SoundButton>> GetAllSoundButtonsAsync();
    Task<List<SoundButtonGridLayout>> GetAllButtonGridsAsync();
    Task<SoundButton> AddSoundButtonAsync(SoundButton soundButton);
    Task<SoundButtonGridLayout> AddButtonGridAsync(SoundButtonGridLayout category);
    Task UpdateSoundButtonAsync(SoundButton soundButton);
    Task DeleteSoundButtonAsync(Guid id);
    Task<SoundButton> GetSoundButtonByIdAsync(Guid id);
    Task<SoundButtonGridLayout> AddButtonGridWithSoundButtonsAsync(SoundButtonGridLayout grid);
    Task<SoundButtonGridLayout> UpdateButtonGridAsync(SoundButtonGridLayout grid);
}

